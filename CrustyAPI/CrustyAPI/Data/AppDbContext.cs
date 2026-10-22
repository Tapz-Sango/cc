using CrustyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrustyAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<AdminUser> AdminUsers { get; set; }

    public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Staff> Staff { get; set; }

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }
    public DbSet<CustomerReview> CustomerReviews { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .ToTable("Customer");

        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerID);

        modelBuilder.Entity<AdminUser>()
            .ToTable("Admin_User");

        modelBuilder.Entity<AdminUser>()
            .HasKey(a => a.AdminID);

        modelBuilder.Entity<DeliveryAddress>()
            .ToTable("Delivery_Address");

        modelBuilder.Entity<DeliveryAddress>()
            .HasKey(a => a.DeliveryAddressID);

        modelBuilder.Entity<DeliveryAddress>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(a => a.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItem>()
    .ToTable("Menu_Item");

        modelBuilder.Entity<MenuItem>()
            .HasKey(m => m.MenuItemID);

        modelBuilder.Entity<Cart>()
    .ToTable("Cart");

        modelBuilder.Entity<Cart>()
            .HasKey(c => c.CartID);

        modelBuilder.Entity<Cart>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .ToTable("Cart_Item");

        modelBuilder.Entity<CartItem>()
            .HasKey(ci => ci.CartItemID);

        modelBuilder.Entity<CartItem>()
            .HasOne<Cart>()
            .WithMany()
            .HasForeignKey(ci => ci.CartID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne<MenuItem>()
            .WithMany()
            .HasForeignKey(ci => ci.MenuItemID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
    .ToTable("Order");

        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderID);

        modelBuilder.Entity<Order>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne<DeliveryAddress>()
            .WithMany()
            .HasForeignKey(o => o.DeliveryAddressID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne<Staff>()
            .WithMany()
            .HasForeignKey(o => o.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderItem>()
            .ToTable("Order_Item");

        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => oi.OrderItemID);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<MenuItem>()
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Staff>()
    .ToTable("Staff");

        modelBuilder.Entity<Staff>()
            .HasKey(s => s.StaffId);

        modelBuilder.Entity<PaymentMethod>()
    .ToTable("Payment_method");

        modelBuilder.Entity<PaymentMethod>()
            .HasKey(pm => pm.PaymentMethodID);

        modelBuilder.Entity<Payment>()
            .ToTable("Payment");

        modelBuilder.Entity<Payment>()
            .HasKey(p => p.PaymentID);

        modelBuilder.Entity<Payment>()
            .HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(p => p.PaymentMethodID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(p => p.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomerPaymentMethod>(entity =>
        {
            entity.ToTable("Customer_Payment_Method");

            entity.HasKey(e => e.CustomerPaymentMethodID);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PaymentMethod)
                .WithMany()
                .HasForeignKey(e => e.PaymentMethodID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerReview>(entity =>
            {
                entity.ToTable("Customer_Review");

                entity.HasKey(r => r.ReviewID);

                entity.Property(r => r.Comment)
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(r => r.AdminResponse)
                    .HasColumnType("text");

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(r => r.CustomerID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<AdminUser>()
                    .WithMany()
                    .HasForeignKey(r => r.AdminID)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        });
    }

}