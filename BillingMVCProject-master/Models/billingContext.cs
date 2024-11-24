using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BillingMVCProject.Models
{
    public partial class billingContext : DbContext
    {
        public billingContext()
        {
        }

        public billingContext(DbContextOptions<billingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CustomersDetail> CustomersDetails { get; set; } = null!;
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; } = null!;
        public virtual DbSet<Invoiceitemdetail> Invoiceitemdetails { get; set; } = null!;
        public virtual DbSet<Invoicesgenerate> Invoicesgenerates { get; set; } = null!;
        public virtual DbSet<Jobcarddetail> Jobcarddetails { get; set; } = null!;
        public virtual DbSet<Jobcardgenerate> Jobcardgenerates { get; set; } = null!;
        public virtual DbSet<Joblineitem> Joblineitems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;initial catalog=billing;persist security info=False;user=root;password=root@123;connection timeout=30", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.39-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<CustomersDetail>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PRIMARY");

                entity.ToTable("customers_details");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("Customer_Name")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(100)
                    .HasColumnName("Email_id")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.GstNumber)
                    .HasMaxLength(50)
                    .HasColumnName("GST_Number");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.InvoiceId)
                    .HasName("PRIMARY");

                entity.ToTable("invoice_details");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeliveryCharge).HasPrecision(10, 2);

                entity.Property(e => e.GrandTotal).HasPrecision(10, 2);

                entity.Property(e => e.GstAmount).HasPrecision(10, 2);

                entity.Property(e => e.InvoiceOutStandingAmount).HasPrecision(10, 2);

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.PaymentMode).HasMaxLength(100);

                entity.Property(e => e.PaymentStatus).HasMaxLength(255);
            });

            modelBuilder.Entity<Invoiceitemdetail>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PRIMARY");

                entity.ToTable("invoiceitemdetails");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Gsm).HasMaxLength(50);

                entity.Property(e => e.InvoiceId).HasColumnType("text");

                entity.Property(e => e.Side).HasMaxLength(50);

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.Property(e => e.Total)
                    .HasPrecision(10, 2)
                    .HasComputedColumnSql("`Quantity` * `UnitPrice`", true);

                entity.Property(e => e.TotalValues).HasPrecision(18, 2);

                entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Invoicesgenerate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("invoicesgenerate");

                entity.Property(e => e.Invoiceid)
                    .HasMaxLength(100)
                    .HasColumnName("invoiceid")
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<Jobcarddetail>(entity =>
            {
                entity.HasKey(e => e.JobCardId)
                    .HasName("PRIMARY");

                entity.ToTable("jobcarddetails");

                entity.Property(e => e.JobCardId)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<Jobcardgenerate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("jobcardgenerate");

                entity.Property(e => e.JobCardId)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");
            });

            modelBuilder.Entity<Joblineitem>(entity =>
            {
                entity.HasKey(e => e.JobItemId)
                    .HasName("PRIMARY");

                entity.ToTable("joblineitem");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.EndMachineReading).HasMaxLength(50);

                entity.Property(e => e.EndTime).HasMaxLength(50);

                entity.Property(e => e.Gsm)
                    .HasMaxLength(100)
                    .HasColumnName("GSM");

                entity.Property(e => e.Imp)
                    .HasMaxLength(100)
                    .HasColumnName("IMP");

                entity.Property(e => e.JobCardId)
                    .HasMaxLength(500)
                    .UseCollation("utf8mb3_general_ci")
                    .HasCharSet("utf8mb3");

                entity.Property(e => e.MachineReading).HasMaxLength(50);

                entity.Property(e => e.PlateNum).HasMaxLength(50);

                entity.Property(e => e.Side).HasMaxLength(100);

                entity.Property(e => e.Size).HasMaxLength(100);

                entity.Property(e => e.StartTime).HasMaxLength(50);

                entity.Property(e => e.TotImp)
                    .HasMaxLength(50)
                    .HasColumnName("TotIMP");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
