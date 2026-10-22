using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;
using static CrustyAPI.DTOs.PaymentResponse;

namespace CrustyAPI.Services;

public class PaymentService
{
    private readonly AppDbContext _context;

    public PaymentService(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // PAYMENT METHODS
    // =========================================================

    public async Task<List<PaymentMethodResponse>> GetPaymentMethods()
    {
        return await _context.PaymentMethods
            .Select(pm => new PaymentMethodResponse
            {
                PaymentMethodID = pm.PaymentMethodID,
                MethodName = pm.MethodName
            })
            .ToListAsync();
    }

    public async Task<PaymentMethodResponse?> GetPaymentMethodById(int id)
    {
        return await _context.PaymentMethods
            .Where(pm => pm.PaymentMethodID == id)
            .Select(pm => new PaymentMethodResponse
            {
                PaymentMethodID = pm.PaymentMethodID,
                MethodName = pm.MethodName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PaymentMethodResponse> CreatePaymentMethod(
        CreatePaymentMethodRequest request)
    {
        var paymentMethod = new PaymentMethod
        {
            MethodName = request.MethodName
        };

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        return new PaymentMethodResponse
        {
            PaymentMethodID = paymentMethod.PaymentMethodID,
            MethodName = paymentMethod.MethodName
        };
    }

    public async Task<bool> DeletePaymentMethod(int id)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.PaymentMethodID == id);

        if (paymentMethod == null)
        {
            return false;
        }

        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // SAVED CUSTOMER PAYMENT METHODS
    // =========================================================

    public async Task<List<CustomerPaymentMethodResponse>>
        GetCustomerPaymentMethods(int customerId)
    {
        return await _context.CustomerPaymentMethods
            .Where(cpm => cpm.CustomerID == customerId)
            .Select(cpm => new CustomerPaymentMethodResponse
            {
                CustomerPaymentMethodID = cpm.CustomerPaymentMethodID,
                CustomerID = cpm.CustomerID,
                PaymentMethodID = cpm.PaymentMethodID,
                MethodName = cpm.PaymentMethod.MethodName,
                CardholderName = cpm.CardholderName,
                CardLast4 = cpm.CardLast4,
                ExpiryMonth = cpm.ExpiryMonth,
                ExpiryYear = cpm.ExpiryYear
            })
            .ToListAsync();
    }

    public async Task<CustomerPaymentMethodResponse?>
        SaveCustomerPaymentMethod(CreateSavedCardRequest request)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerID == request.CustomerID);

        if (!customerExists)
        {
            return null;
        }

        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm =>
                pm.PaymentMethodID == request.PaymentMethodID);

        if (paymentMethod == null)
        {
            return null;
        }

        // Remove spaces from the card number
        var cardNumber = request.CardNumber.Replace(" ", "");

        // Only store the final four digits
        var last4 = cardNumber.Length >= 4
            ? cardNumber[^4..]
            : cardNumber;

        var savedMethod = new CustomerPaymentMethod
        {
            CustomerID = request.CustomerID,
            PaymentMethodID = request.PaymentMethodID,
            CardholderName = request.CardholderName,
            CardLast4 = last4,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear
        };

        _context.CustomerPaymentMethods.Add(savedMethod);
        await _context.SaveChangesAsync();

        return new CustomerPaymentMethodResponse
        {
            CustomerPaymentMethodID =
                savedMethod.CustomerPaymentMethodID,

            CustomerID = savedMethod.CustomerID,

            PaymentMethodID = savedMethod.PaymentMethodID,

            MethodName = paymentMethod.MethodName,

            CardholderName = savedMethod.CardholderName,

            CardLast4 = savedMethod.CardLast4,

            ExpiryMonth = savedMethod.ExpiryMonth,

            ExpiryYear = savedMethod.ExpiryYear
        };
    }

    public async Task<bool> DeleteCustomerPaymentMethod(
        int customerPaymentMethodId,
        int customerId)
    {
        var savedMethod = await _context.CustomerPaymentMethods
            .FirstOrDefaultAsync(cpm =>
                cpm.CustomerPaymentMethodID == customerPaymentMethodId &&
                cpm.CustomerID == customerId);

        if (savedMethod == null)
        {
            return false;
        }

        _context.CustomerPaymentMethods.Remove(savedMethod);
        await _context.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // PAYMENTS
    // =========================================================

    public async Task<PaymentResponse?> CreatePayment(
        CreatePaymentRequest request)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderID == request.OrderID);

        if (order == null)
        {
            return null;
        }

        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm =>
                pm.PaymentMethodID == request.PaymentMethodID);

        if (paymentMethod == null)
        {
            return null;
        }

        var payment = new Payment
        {
            OrderID = request.OrderID,
            PaymentMethodID = request.PaymentMethodID,
            Amount = request.Amount,
            PaymentStatus = "Pending",
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return await BuildPaymentResponse(payment);
    }

    public async Task<PaymentResponse?> GetPaymentById(int id)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentID == id);

        if (payment == null)
        {
            return null;
        }

        return await BuildPaymentResponse(payment);
    }

    public async Task<List<PaymentResponse>> GetPaymentsByOrder(int orderId)
    {
        var payments = await _context.Payments
            .Where(p => p.OrderID == orderId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        var responses = new List<PaymentResponse>();

        foreach (var payment in payments)
        {
            responses.Add(await BuildPaymentResponse(payment));
        }

        return responses;
    }

    public async Task<PaymentResponse?> UpdatePaymentStatus(
        int paymentId,
        UpdatePaymentStatusRequest request)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentID == paymentId);

        if (payment == null)
        {
            return null;
        }

        payment.PaymentStatus = request.PaymentStatus;

        await _context.SaveChangesAsync();

        return await BuildPaymentResponse(payment);
    }

    private async Task<PaymentResponse> BuildPaymentResponse(
        Payment payment)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstAsync(pm =>
                pm.PaymentMethodID == payment.PaymentMethodID);

        return new PaymentResponse
        {
            PaymentID = payment.PaymentID,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethodID = payment.PaymentMethodID,
            MethodName = paymentMethod.MethodName,
            OrderID = payment.OrderID
        };
    }
}