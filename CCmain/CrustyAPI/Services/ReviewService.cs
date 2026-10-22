
using CrustyAPI.Data;
using CrustyAPI.DTOs;
using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Services;

public class ReviewService
{
    private readonly AppDbContext _context;

    public ReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerReviewResponse> CreateReview(
        int customerId,
        CreateReviewRequest request)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerID == customerId);

        if (!customerExists)
            throw new KeyNotFoundException("Customer not found.");

        var comment = request.Comment.Trim();

        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty.");

        var review = new CustomerReview
        {
            CustomerID = customerId,
            Rating = request.Rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.CustomerReviews.Add(review);
        await _context.SaveChangesAsync();

        return await GetReviewById(review.ReviewID)
            ?? throw new InvalidOperationException("Review could not be retrieved.");
    }

    public async Task<List<CustomerReviewResponse>> GetAllReviews()
    {
        return await _context.CustomerReviews
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CustomerReviewResponse
            {
                ReviewID = r.ReviewID,
                CustomerID = r.CustomerID,
                CustomerName = _context.Customers
                    .Where(c => c.CustomerID == r.CustomerID)
                    .Select(c => c.FirstName + " " + c.LastName)
                    .FirstOrDefault() ?? "Unknown customer",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                AdminResponse = r.AdminResponse,
                AdminID = r.AdminID,
                AdminUsername = _context.AdminUsers
                    .Where(a => a.AdminID == r.AdminID)
                    .Select(a => a.Username)
                    .FirstOrDefault(),
                RespondedAt = r.RespondedAt
            })
            .ToListAsync();
    }
    public async Task<List<CustomerReviewResponse>> GetCustomerReviews(int customerId)
    {
        return await _context.CustomerReviews
            .AsNoTracking()
            .Where(r => r.CustomerID == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CustomerReviewResponse
            {
                ReviewID = r.ReviewID,
                CustomerID = r.CustomerID,
                CustomerName = _context.Customers
                    .Where(c => c.CustomerID == r.CustomerID)
                    .Select(c => c.FirstName + " " + c.LastName)
                    .FirstOrDefault() ?? "Unknown customer",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                AdminResponse = r.AdminResponse,
                AdminID = r.AdminID,
                AdminUsername = _context.AdminUsers
                    .Where(a => a.AdminID == r.AdminID)
                    .Select(a => a.Username)
                    .FirstOrDefault(),
                RespondedAt = r.RespondedAt
            })
            .ToListAsync();
    }
    public async Task<CustomerReviewResponse?> GetReviewById(int id)
    {
        return await _context.CustomerReviews
            .AsNoTracking()
            .Where(r => r.ReviewID == id)
            .Select(r => new CustomerReviewResponse
            {
                ReviewID = r.ReviewID,
                CustomerID = r.CustomerID,
                CustomerName = _context.Customers
                    .Where(c => c.CustomerID == r.CustomerID)
                    .Select(c => c.FirstName + " " + c.LastName)
                    .FirstOrDefault() ?? "Unknown customer",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                AdminResponse = r.AdminResponse,
                AdminID = r.AdminID,
                AdminUsername = _context.AdminUsers
                    .Where(a => a.AdminID == r.AdminID)
                    .Select(a => a.Username)
                    .FirstOrDefault(),
                RespondedAt = r.RespondedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerReviewResponse?> RespondToReview(
        int reviewId,
        int adminId,
        AdminReviewResponseRequest request)
    {
        var review = await _context.CustomerReviews
            .FirstOrDefaultAsync(r => r.ReviewID == reviewId);

        if (review == null)
            return null;

        var adminExists = await _context.AdminUsers
            .AnyAsync(a => a.AdminID == adminId);

        if (!adminExists)
            throw new KeyNotFoundException("Admin not found.");

        var response = request.AdminResponse.Trim();

        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("Response cannot be empty.");

        review.AdminResponse = response;
        review.AdminID = adminId;
        review.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetReviewById(reviewId);
    }
}
