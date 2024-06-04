using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bankApiProject.Models;

public partial class BankSystemContext : DbContext
{
    public BankSystemContext()
    {
    }

    public BankSystemContext(DbContextOptions<BankSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerView> CustomerViews { get; set; }

    public virtual DbSet<CustomerVieww> CustomerViewws { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=DESKTOP-M8FNJU8;database=BankSystem;TrustServerCertificate=True;trusted_connection=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.AdminPassword)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AdminUsername)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankId).HasColumnName("BankID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Admins)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Admin_Customer");
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.ToTable("Bank");

            entity.Property(e => e.BankId).HasColumnName("BankID");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.IsBankCreated).HasColumnName("Is_Bank_Created");

            entity.HasOne(d => d.Customer).WithMany(p => p.Banks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Bank_Bank");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CustomerPassword)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HashedCustomerName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HashedTokens)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CustomerView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CustomerView");

            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            entity.Property(e => e.CustomerPassword)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HashedCustomerName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CustomerVieww>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CustomerVieww");

            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            entity.Property(e => e.CustomerPassword)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HashedCustomerName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
