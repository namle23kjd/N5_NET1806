using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using System;
using System.Collections.Generic;

namespace PetSpa.Data
{
    public partial class PetSpaContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public PetSpaContext(DbContextOptions<PetSpaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<Combo> Combos { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<Images> Images { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                //optionsBuilder.UseSqlServer("Server=DESKTOP-S41VFN3\\PIEDTEAM;Database=Pet__SPAManagement;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True");
                // optionsBuilder.UseSqlServer("Server = HOANGNE\\SQLEXPRESS; Database = Pet_Spa; User Id = SA; Password = 1; Trusted_Connection = True; TrustServerCertificate = True");

                //optionsBuilder.UseSqlServer("Server=DESKTOP-S41VFN3\\PIEDTEAM;Database=Pet__SPAManagement;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True");
                optionsBuilder.UseSqlServer("Server=DESKTOP-LJMA02H\\PIEDTEAM;Database=Pet_Spa;User Id=SA;Password=12345;Trusted_Connection=True;TrustServerCertificate=True");

                //Server = HOANGNE\\SQLEXPRESS; Database = Pet_Spa; User Id = SA; Password = 12345; Trusted_Connection = True; TrustServerCertificate = True
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var StaffRoleId = Guid.NewGuid();
            var CustomerRoleId = Guid.NewGuid();
            var AdminRoleId = Guid.NewGuid();
            var ManagerRoleId = Guid.NewGuid();

            var roles = new List<IdentityRole<Guid>> {
        new IdentityRole<Guid>{
            Id = CustomerRoleId,
            ConcurrencyStamp = CustomerRoleId.ToString(),
            Name = "Customer",
            NormalizedName = "CUSTOMER"
        },
        new IdentityRole<Guid>
        {
            Id = StaffRoleId,
            ConcurrencyStamp = StaffRoleId.ToString(),
            Name = "Staff",
            NormalizedName = "STAFF"
        },
        new IdentityRole<Guid>
        {
            Id = AdminRoleId,
            ConcurrencyStamp = AdminRoleId.ToString(),
            Name = "Admin",
            NormalizedName = "ADMIN"
        },
        new IdentityRole<Guid>
        {
            Id = ManagerRoleId,
            ConcurrencyStamp = ManagerRoleId.ToString(),
            Name = "Manager",
            NormalizedName = "MANAGER"
        }
    };
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId).HasName("PK__Admin__AD050086168743E5");

                entity.ToTable("Admin");

                entity.HasIndex(e => e.Id, "UQ__Admin__A471AFFBB3371801").IsUnique();

                entity.Property(e => e.AdminId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("adminID");
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Id");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Admin>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Admin__accID__398D8EEE");
                entity.HasMany(d => d.Managers)
                    .WithOne(p => p.Admins)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Manager_Admin");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId).HasName("PK__Booking__C6D03BEDDB4CC5ED");

                entity.ToTable("Booking");

                entity.Property(e => e.BookingId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("bookingID");

                entity.Property(e => e.BookingSchedule)
                    .HasColumnType("datetime")
                    .HasColumnName("bookingSchedule");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime2")
                    .HasColumnName("StartDate");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime2")
                    .HasColumnName("EndDate");

                entity.Property(e => e.CusId)
                    .IsRequired()
                    .HasColumnName("cusID");

                entity.Property(e => e.CheckAccept)
                    .HasColumnName("checkAccept")
                    .HasDefaultValue(false);

                entity.Property(e => e.ManaId)
                    .IsRequired()
                    .HasColumnName("manaID");

                entity.Property(e => e.Feedback)
                    .HasColumnType("text")
                    .HasColumnName("feedback");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<int?>() // Store the enum as an integer in the database
                    .HasDefaultValue(BookingStatus.NotStarted);


                entity.Property(e => e.PaymentStatus).HasColumnName("paymentstatus");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalAmount");

                entity.Property(e => e.PaymentId)
                    .HasColumnName("paymentID")
                    .IsRequired(false);

                entity.Property(e => e.InvoiceId)
                    .HasColumnName("invoiceID")
                    .IsRequired(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking__cusID__4AB81AF0");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Manager");

                entity.HasOne(d => d.Payments)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade) // Change to Cascade
                    .HasConstraintName("FK_Booking_Payment");

                entity.HasOne(d => d.Invoice)
                    .WithOne(i => i.Bookings)
                    .HasForeignKey<Booking>(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Booking_Invoice");
            });

            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(e => e.BookingDetailId).HasName("PK__Booking___942CA05E22BB22C8");

                entity.ToTable("Booking_Detail");

                entity.Property(e => e.BookingDetailId)
                    .ValueGeneratedNever()
                    .HasColumnName("bookingDetailID");

                entity.Property(e => e.BookingId)
                    .HasColumnName("bookingID");

                entity.Property(e => e.PetId)
                    .HasColumnName("petID");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("serviceID")
                    .IsRequired(false);

                entity.Property(e => e.StaffId)
                    .HasColumnName("staffID").IsRequired(false);

                entity.Property(e => e.ComboId)
                    .HasColumnName("comboID")
                    .IsRequired(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ComboType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("comboType");
                entity.Property(e => e.Duration)
                    .HasColumnType("time(7)")
                    .HasColumnName("duration");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.Cascade) // Change to Cascade
                    .HasConstraintName("FK__Booking_D__booki__571DF1D5");

                entity.HasOne(d => d.Combo)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.ComboId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Booking_D__combo__5535A963");

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.PetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking_D__petID__5441852A");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Booking_D__servi__534D60F1");
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
                entity.Property(e => e.Duration)
                    .HasColumnType("time(7)")
                    .HasColumnName("duration");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CusId).HasName("PK__Customer__BA9897D361CDA2E7");

                entity.ToTable("Customer");

                entity.HasIndex(e => e.Id).IsUnique().HasDatabaseName("UQ__Customer__A471AFFBD525B8F4");

                entity.Property(e => e.CusId)
                    .HasColumnName("cusID")
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .IsRequired();

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("fullName").IsRequired(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender").IsRequired(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phoneNumber").IsRequired(false);

                entity.Property(e => e.CusRank)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cusRank");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Customer>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_AspNetUsers");

                entity.HasMany(d => d.Payments)
                    .WithOne(p => p.Customer)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Payment");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__1252410C1622F1EA");

                entity.ToTable("Invoice");
                entity.HasIndex(e => e.BookingId, "UQ__Invoice__C6D03BECA66EC071").IsUnique();

                entity.Property(e => e.InvoiceId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("invoiceID");

                entity.Property(e => e.BookingId)
                    .IsRequired()
                    .HasColumnName("bookingID");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.PaymentId)
                    .IsRequired()
                    .HasColumnName("paymentID");

                entity.HasOne(d => d.Bookings)
                    .WithOne(b => b.Invoice)
                    .HasForeignKey<Invoice>(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__booking__5AEE82B9");

                entity.HasOne(e => e.Payment)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_Payment");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(e => e.ManaId).HasName("PK__Manager__22DAE4264DC65004");

                entity.ToTable("Manager");

                entity.HasIndex(e => e.Id, "UQ__Manager__A471AFFBEADB9F39").IsUnique();

                entity.Property(e => e.ManaId)
                    .ValueGeneratedNever()
                    .HasColumnName("manaID");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Id");

                entity.Property(e => e.AdminId)
                    .HasColumnName("adminID");

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

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Manager>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Manager_AspNetUsers");

                entity.HasOne(d => d.Admins)
                    .WithMany(p => p.Managers)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Manager_Admin");

                entity.HasMany(d => d.Staffs)
                    .WithOne(p => p.Manager)
                    .HasForeignKey(d => d.ManagerManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_Manager_ManagerManaId");
                entity.HasMany(d => d.Vouchers)
                    .WithOne(p => p.Managers)
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Voucher_Manager");

                entity.HasMany(d => d.Bookings)
                    .WithOne(p => p.Manager)
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Manager");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PK__Payment__123123123");

                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("paymentID");

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("transactionId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate");
                entity.Property(e => e.ExpirationTime)
                    .HasColumnName("expirationTime");

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("paymentMethod");

                entity.Property(e => e.CusId)
                  .IsRequired()
                  .HasColumnName("cusID");

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Payments)
                    .HasForeignKey(e => e.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Customer");

                entity.HasMany(e => e.Invoices)
                    .WithOne(e => e.Payment)
                    .HasForeignKey(i => i.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Invoice_Payment");
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
                entity.Property(e => e.Status).HasColumnName("status").HasDefaultValue(true);

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
                    .HasColumnName("comboID")
                    .IsRequired(false);
                entity.Property(e => e.ServiceDescription)
                    .HasColumnType("text")
                    .HasColumnName("serviceDescription");
                entity.Property(e => e.ServiceImage).HasColumnName("serviceImage");
                entity.Property(e => e.ServiceName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("serviceName");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Duration)
                    .HasColumnType("time(7)")
                    .HasColumnName("duration");

                entity.Property(e => e.Points).HasColumnName("points");
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.HasOne(d => d.Combo).WithMany(p => p.Services)
                    .HasForeignKey(d => d.ComboId)
                    .HasConstraintName("FK_Service_Combo")
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__Staff__6465E19E05D526E9");

                entity.ToTable("Staff");

                entity.HasIndex(e => e.Id, "UQ__Staff__A471AFFB206680B8").IsUnique();

                entity.Property(e => e.StaffId)
                    .ValueGeneratedNever()
                    .HasColumnName("staffID");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Id");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("fullName");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender");

                entity.Property(e => e.ManagerManaId)
                    .HasColumnName("ManagerManaId");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Staff>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_AspNetUsers");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Staffs)
                    .HasForeignKey(d => d.ManagerManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_Manager_ManagerManaId");
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

                entity.HasOne(d => d.Bookings).WithOne(p => p.Voucher)
                    .HasForeignKey<Voucher>(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Voucher__booking__6A30C649");

                entity.HasOne(d => d.Managers).WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Voucher__manaID__693CA210");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}