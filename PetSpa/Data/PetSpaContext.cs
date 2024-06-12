using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetSpa.Models.Domain;
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
        public virtual DbSet<PaymentT> Payments { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; } // Thêm bảng Merchant

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-LJMA02H\\PIEDTEAM;Database=Pet_Spa;User Id=SA;Password=12345;Trusted_Connection=True;TrustServerCertificate=True;");
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
                entity.HasMany(d => d.Managers) // Quan hệ với bảng Manager
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
                    .HasMaxLength(50)
                    .IsUnicode(false)
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
                    .HasMaxLength(50)
                    .IsUnicode(false)

                    .HasColumnName("cusID");
                entity.Property(e => e.CheckAccept)
                    .HasColumnName("checkAccept") // Thêm cột CheckAccept
                    .HasDefaultValue(false);

                entity.Property(e => e.ManaId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("manaID"); // Thêm cột ManaId

                entity.Property(e => e.Feedback)
                    .HasColumnType("text")
                    .HasColumnName("feedback");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalAmount");

                entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking__cusID__4AB81AF0");

                entity.HasOne(d => d.Manager).WithMany(p => p.Bookings) // Quan hệ với bảng Manager
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Manager");
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
                    .HasColumnName("staffID");

                entity.Property(e => e.ComboId)
                    .HasColumnName("comboID")
                    .IsRequired(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ComboType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("comboType");
                entity.Property(e => e.Duration)
                    .HasColumnType("time(7)") // Định nghĩa kiểu dữ liệu time(7) cho Duration
                    .HasColumnName("duration");

                entity.HasOne(d => d.Booking) // Quan hệ với bảng Booking
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking_D__booki__571DF1D5");

                entity.HasOne(d => d.Combo) // Quan hệ với bảng Combo
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.ComboId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Booking_D__combo__5535A963");

                entity.HasOne(d => d.Pet) // Quan hệ với bảng Pet
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.PetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Booking_D__petID__5441852A");

                entity.HasOne(d => d.Service) // Quan hệ với bảng Service
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
                    .HasColumnType("time(7)") // Định nghĩa kiểu dữ liệu time(7) cho Duration
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
                    .HasColumnName("fullName");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phoneNumber");

                entity.Property(e => e.CusRank)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cusRank");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Customer>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_AspNetUsers");
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

                entity.Property(e => e.AdminId) // Thêm cột AdminId
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

                entity.HasOne(d => d.User) // Quan hệ với bảng AspNetUsers
                    .WithOne()
                    .HasForeignKey<Manager>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Manager_AspNetUsers");

                entity.HasOne(d => d.Admins) // Quan hệ với bảng Admin
                    .WithMany(p => p.Managers)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Manager_Admin");

                entity.HasMany(d => d.Staffs) // Quan hệ với bảng Staff
                    .WithOne(p => p.Manager)
                    .HasForeignKey(d => d.ManagerManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_Manager_ManagerManaId");

                entity.HasMany(d => d.Bookings) // Quan hệ với bảng Booking
                    .WithOne(p => p.Manager)
                    .HasForeignKey(d => d.ManaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Manager");
            });

            modelBuilder.Entity<PaymentT>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RequiredAmount)
                    .HasColumnType("decimal(19, 2)");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("decimal(19, 2)");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime");

                entity.Property(e => e.InvoiceId)
                    .HasColumnName("invoiceID")
                    .IsRequired();

                entity.Property(e => e.MerchantId)
                    .HasColumnName("merchantID")
                    .IsRequired();

                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Merchant)
                    .WithMany(m => m.Payments)
                    .HasForeignKey(e => e.MerchantId)
                    .OnDelete(DeleteBehavior.Cascade);
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
                    .HasColumnType("time(7)") // Định nghĩa kiểu dữ liệu time(7) cho Duration
                    .HasColumnName("duration");
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

                entity.HasOne(d => d.User) // Quan hệ với bảng AspNetUsers
                    .WithOne()
                    .HasForeignKey<Staff>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_AspNetUsers");

                entity.HasOne(d => d.Manager) // Quan hệ với bảng Manager
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
                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Bookings).WithOne(p => p.Voucher)
                    .HasForeignKey<Voucher>(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Voucher__booking__6A30C649");

                entity.HasOne(d => d.Customers).WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Voucher__cusID__68487DD7");
            });


            modelBuilder.Entity<Merchant>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ContactInfo)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
