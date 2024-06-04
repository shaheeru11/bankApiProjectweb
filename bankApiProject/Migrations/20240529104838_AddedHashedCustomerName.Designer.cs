﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using bankApiProject.Models;

#nullable disable

namespace bankApiProject.Migrations
{
    [DbContext(typeof(BankSystemContext))]
    [Migration("20240529104838_AddedHashedCustomerName")]
    partial class AddedHashedCustomerName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("bankApiProject.Models.Admin", b =>
                {
                    b.Property<int>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AdminID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdminId"));

                    b.Property<string>("AdminPassword")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("AdminUsername")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("BankId")
                        .HasColumnType("int")
                        .HasColumnName("BankID");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("CustomerID");

                    b.HasKey("AdminId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Admin", (string)null);
                });

            modelBuilder.Entity("bankApiProject.Models.Bank", b =>
                {
                    b.Property<int>("BankId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BankID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BankId"));

                    b.Property<string>("BankAccountNumber")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("BankBalance")
                        .HasColumnType("int");

                    b.Property<string>("BankName")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("CustomerID");

                    b.Property<int?>("DepositAmount")
                        .HasColumnType("int");

                    b.Property<bool?>("IsBankCreated")
                        .HasColumnType("bit")
                        .HasColumnName("Is_Bank_Created");

                    b.Property<int?>("WithdrawalAmount")
                        .HasColumnType("int");

                    b.HasKey("BankId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Bank", (string)null);
                });

            modelBuilder.Entity("bankApiProject.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CustomerID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("CustomerEmail")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CustomerName")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CustomerPassword")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("HashedCustomerName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("bankApiProject.Models.Admin", b =>
                {
                    b.HasOne("bankApiProject.Models.Customer", "Customer")
                        .WithMany("Admins")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_Admin_Customer");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("bankApiProject.Models.Bank", b =>
                {
                    b.HasOne("bankApiProject.Models.Customer", "Customer")
                        .WithMany("Banks")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_Bank_Bank");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("bankApiProject.Models.Customer", b =>
                {
                    b.Navigation("Admins");

                    b.Navigation("Banks");
                });
#pragma warning restore 612, 618
        }
    }
}