using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PetSpa.Models.Domain;

namespace PetSpa.Data;

public partial class PetSpaContext : DbContext
{
    public PetSpaContext()
    {
    }

    public PetSpaContext(DbContextOptions<PetSpaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-S41VFN3\\PIEDTEAM;Database=Pet_SPA;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccId).HasName("PK__Account__A471AFFAB1118508");

            entity.ToTable("Account");

            entity.Property(e => e.AccId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("accID");
            entity.Property(e => e.ForgotPasswordToken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("forgot_password_token");
            entity.Property(e => e.PassWord)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("passWord");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__AD050086168743E5");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.AccId, "UQ__Admin__A471AFFBB3371801").IsUnique();

            entity.Property(e => e.AdminId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("adminID");
            entity.Property(e => e.AccId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("accID");

            entity.HasOne(d => d.Acc).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__accID__398D8EEE");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BEDDB4CC5ED");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.BookingSchedule)
                .HasColumnType("datetime")
                .HasColumnName("bookingSchedule");
            entity.Property(e => e.CusId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cusID");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.Feedback)
                .HasColumnType("text")
                .HasColumnName("feedback");
            entity.Property(e => e.StaffId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("staffID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalAmount");

            entity.HasOne(d => d.Cus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__cusID__4AB81AF0");

            entity.HasOne(d => d.Staff).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__staffID__4BAC3F29");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId).HasName("PK__Booking___942CA05E22BB22C8");

            entity.ToTable("Booking_Detail");

            entity.Property(e => e.BookingDetailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingDetailID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.ComboId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comboID");
            entity.Property(e => e.ComboType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comboType");
            entity.Property(e => e.PetId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("petID");
            entity.Property(e => e.ServiceId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("serviceID");
            entity.Property(e => e.StaffId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("staffID");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking_D__booki__571DF1D5");

            entity.HasOne(d => d.Combo).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__Booking_D__combo__5535A963");

            entity.HasOne(d => d.Pet).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking_D__petID__5441852A");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Booking_D__servi__534D60F1");

            entity.HasOne(d => d.Staff).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__Booking_D__staff__5629CD9C");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combo__3C30C369B0BC9225");

            entity.ToTable("Combo");

            entity.Property(e => e.ComboId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comboID");
            entity.Property(e => e.ComboType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comboType");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CusId).HasName("PK__Customer__BA9897D361CDA2E7");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.AccId, "UQ__Customer__A471AFFBD525B8F4").IsUnique();

            entity.Property(e => e.CusId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cusID");
            entity.Property(e => e.AccId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("accID");
            entity.Property(e => e.CusRank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cusRank");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");

            entity.HasOne(d => d.Acc).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customer__accID__44FF419A");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__1252410C1622F1EA");

            entity.ToTable("Invoice");

            entity.HasIndex(e => e.BookingId, "UQ__Invoice__C6D03BECA66EC071").IsUnique();

            entity.Property(e => e.InvoiceId)
                .ValueGeneratedNever()
                .HasColumnName("invoiceID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Booking).WithOne(p => p.Invoice)
                .HasForeignKey<Invoice>(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoice__booking__5AEE82B9");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__Job__164AA1884F5D3624");

            entity.ToTable("Job");

            entity.HasIndex(e => e.BookingDetailId, "UQ__Job__942CA05F6B2E2507").IsUnique();

            entity.Property(e => e.JobId)
                .ValueGeneratedNever()
                .HasColumnName("jobID");
            entity.Property(e => e.BookingDetailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingDetailID");
            entity.Property(e => e.ManaId).HasColumnName("manaID");
            entity.Property(e => e.StaffId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("staffID");

            entity.HasOne(d => d.BookingDetail).WithOne(p => p.Job)
                .HasForeignKey<Job>(d => d.BookingDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Job__bookingDeta__6383C8BA");

            entity.HasOne(d => d.Mana).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.ManaId)
                .HasConstraintName("FK__Job__manaID__6477ECF3");

            entity.HasOne(d => d.Staff).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Job__staffID__628FA481");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManaId).HasName("PK__Manager__22DAE4264DC65004");

            entity.ToTable("Manager");

            entity.HasIndex(e => e.AccId, "UQ__Manager__A471AFFBEADB9F39").IsUnique();

            entity.Property(e => e.ManaId)
                .ValueGeneratedNever()
                .HasColumnName("manaID");
            entity.Property(e => e.AccId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("accID");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");

            entity.HasOne(d => d.Acc).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Manager__accID__3D5E1FD2");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PayId).HasName("PK__Payment__082E8AE3B771F3B3");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.InvoiceId, "UQ__Payment__1252410D691653AC").IsUnique();

            entity.Property(e => e.PayId)
                .ValueGeneratedNever()
                .HasColumnName("payID");
            entity.Property(e => e.InvoiceId).HasColumnName("invoiceID");

            entity.HasOne(d => d.Invoice).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__invoice__5EBF139D");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("PK__Pet__DDF85059445442CA");

            entity.ToTable("Pet");

            entity.Property(e => e.PetId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("petID");
            entity.Property(e => e.CusId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cusID");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.PetBirthday).HasColumnName("petBirthday");
            entity.Property(e => e.PetHeight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("petHeight");
            entity.Property(e => e.PetName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("petName");
            entity.Property(e => e.PetType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("petType");
            entity.Property(e => e.PetWeight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("petWeight");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Cus).WithMany(p => p.Pets)
                .HasForeignKey(d => d.CusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pet__cusID__47DBAE45");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__4550733F4C227307");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("serviceID");
            entity.Property(e => e.ComboId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comboID");
            entity.Property(e => e.ServiceDescription)
                .HasColumnType("text")
                .HasColumnName("serviceDescription");
            entity.Property(e => e.ServiceImage).HasColumnName("serviceImage");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("serviceName");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Combo).WithMany(p => p.Services)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK_Service_Combo");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__6465E19E05D526E9");

            entity.HasIndex(e => e.AccId, "UQ__Staff__A471AFFB206680B8").IsUnique();

            entity.Property(e => e.StaffId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("staffID");
            entity.Property(e => e.AccId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("accID");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");

            entity.HasOne(d => d.Acc).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__accID__412EB0B6");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__F53389899EBEA773");

            entity.ToTable("Voucher");

            entity.HasIndex(e => e.BookingId, "UQ__Voucher__C6D03BEC32216A74").IsUnique();

            entity.Property(e => e.VoucherId)
                .ValueGeneratedNever()
                .HasColumnName("voucherID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bookingID");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CusId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cusID");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            entity.Property(e => e.IssueDate).HasColumnName("issueDate");
            entity.Property(e => e.ManaId).HasColumnName("manaID");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Booking).WithOne(p => p.Voucher)
                .HasForeignKey<Voucher>(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Voucher__booking__6A30C649");

            entity.HasOne(d => d.Cus).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.CusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Voucher__cusID__68487DD7");

            entity.HasOne(d => d.Mana).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.ManaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Voucher__manaID__693CA210");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}