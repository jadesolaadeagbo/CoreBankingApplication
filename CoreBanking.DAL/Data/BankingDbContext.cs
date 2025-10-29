using CoreBanking.Core.Entities;
using CoreBanking.Core.Enums;
using CoreBanking.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Data
{
    public class BankingDbContext:DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
        {}
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Account> Accounts => Set<Account>();   
        public DbSet<Transaction> Transactions => Set<Transaction>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.CustomerId)
                .HasConversion(customerId => customerId.Value, value => new CustomerId(value));

                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(255);
                entity.Property(c => c.PhoneNumber).HasMaxLength(20);

                entity.HasMany(c => c.Accounts)
                      .WithOne(a => a.Customer)
                      .HasForeignKey(a => a.CustomerId);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountId);
                entity.OwnsOne(a => a.AccountNumber, an =>
                {
                    an.Property(a => a.Value)
                    .HasColumnName("AccountNumber")
                    .IsRequired()
                    .HasMaxLength(10);
                });

                entity.OwnsOne(a => a.Balance, money =>
                {
                    money.Property(m => m.Amount)
                    .HasColumnName("Balance")
                    .HasPrecision(18, 2);
                    money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("NGN");
                });

                entity.Property(a => a.AccountType)
                .HasConversion<string>()
                .IsRequired();

                entity.HasMany(a => a.Transactions)
                      .WithOne(t => t.Account)
                      .HasForeignKey(t => t.AccountId);

                entity.Navigation(a => a.Transactions).AutoInclude(false);

            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);
                entity.OwnsOne(t => t.Amount, money =>
                {

                    money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2);
                    money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3);

                });
                entity.Property(t => t.Type)
                .HasConversion<string>()
                .IsRequired();

                entity.Property(t => t.Description).HasMaxLength(500);
                entity.Property(t => t.Reference).HasMaxLength(50);
                entity.Property(t => t.Timestamp).IsRequired();
            });

            modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Account>().HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(a => a.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            });

            // Seed the DB
            modelBuilder.Entity<Customer>().HasData(new
            {
                CustomerId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789abc"),
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@email.com",
                PhoneNumber = "555-0101",
                DateCreated = DateTime.UtcNow.AddDays(-30),
                IsActive = true,
                IsDeleted = false
            }
            );

            modelBuilder.Entity<Account>().HasData(new
            {
                AccountId = Guid.Parse("c3d4e5f6-3456-7890-cde1-345678901cde"),
                AccountNumber = "1000000001", // maps to AccountNumber.Value
                AccountType = AccountType.Checking, // EF handles enum conversion
                CustomerId = Guid.Parse("a1b2c3d4-1234-5678-9abc-123456789abc"),
                BalanceAmount = 1500.00m, // maps to Money.Amount
                Currency = "NGN",
                DateOpened = DateTime.UtcNow.AddDays(-20),
                IsActive = true,
                IsDeleted = false
            }
            );

        }


    }
}
