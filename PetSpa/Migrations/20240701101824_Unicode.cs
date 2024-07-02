using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetSpa.Migrations
{
    /// <inheritdoc />
    public partial class Unicode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    refreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    comboID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    comboType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    duration = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Combo__3C30C369B0BC9225", x => x.comboID);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    adminID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 100, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__AD050086168743E5", x => x.adminID);
                    table.ForeignKey(
                        name: "FK__Admin__accID__398D8EEE",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    serviceID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    serviceName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    serviceDescription = table.Column<string>(type: "text", nullable: true),
                    serviceImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    duration = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    comboID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: true),
                    points = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__4550733F4C227307", x => x.serviceID);
                    table.ForeignKey(
                        name: "FK_Service_Combo",
                        column: x => x.comboID,
                        principalTable: "Combo",
                        principalColumn: "comboID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    cusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    phoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    cusRank = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    totalSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__BA9897D361CDA2E7", x => x.cusID);
                    table.ForeignKey(
                        name: "FK_Customer_Admin_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admin",
                        principalColumn: "adminID");
                    table.ForeignKey(
                        name: "FK_Customer_AspNetUsers",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    manaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 100, nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    phoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Manager__22DAE4264DC65004", x => x.manaID);
                    table.ForeignKey(
                        name: "FK_Manager_Admin_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admin",
                        principalColumn: "adminID");
                    table.ForeignKey(
                        name: "FK_Manager_AspNetUsers",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    paymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    transactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expirationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__123123123", x => x.paymentID);
                    table.ForeignKey(
                        name: "FK_Payment_Customer",
                        column: x => x.cusID,
                        principalTable: "Customer",
                        principalColumn: "cusID");
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    petID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    cusID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    petType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    petName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    petBirthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    petWeight = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    petHeight = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pet__DDF85059445442CA", x => x.petID);
                    table.ForeignKey(
                        name: "FK__Pet__cusID__47DBAE45",
                        column: x => x.cusID,
                        principalTable: "Customer",
                        principalColumn: "cusID");
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    staffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 100, nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    ManagerManaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Staff__6465E19E05D526E9", x => x.staffID);
                    table.ForeignKey(
                        name: "FK_Staff_AspNetUsers",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Staff_Manager_ManagerManaId",
                        column: x => x.ManagerManaId,
                        principalTable: "Manager",
                        principalColumn: "manaID");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    bookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    manaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true, defaultValue: -1),
                    totalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    bookingSchedule = table.Column<DateTime>(type: "datetime", nullable: false),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    checkAccept = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    paymentID = table.Column<int>(type: "int", nullable: true),
                    invoiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__C6D03BEDDB4CC5ED", x => x.bookingID);
                    table.ForeignKey(
                        name: "FK_Booking_Manager",
                        column: x => x.manaID,
                        principalTable: "Manager",
                        principalColumn: "manaID");
                    table.ForeignKey(
                        name: "FK_Booking_Payment",
                        column: x => x.paymentID,
                        principalTable: "Payment",
                        principalColumn: "paymentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Booking__cusID__4AB81AF0",
                        column: x => x.cusID,
                        principalTable: "Customer",
                        principalColumn: "cusID");
                });

            migrationBuilder.CreateTable(
                name: "Booking_Detail",
                columns: table => new
                {
                    bookingDetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    petID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    serviceID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    staffID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    comboID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    duration = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    comboType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking___942CA05E22BB22C8", x => x.bookingDetailID);
                    table.ForeignKey(
                        name: "FK_Booking_Detail_Staff_staffID",
                        column: x => x.staffID,
                        principalTable: "Staff",
                        principalColumn: "staffID");
                    table.ForeignKey(
                        name: "FK__Booking_D__booki__571DF1D5",
                        column: x => x.bookingID,
                        principalTable: "Booking",
                        principalColumn: "bookingID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Booking_D__combo__5535A963",
                        column: x => x.comboID,
                        principalTable: "Combo",
                        principalColumn: "comboID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Booking_D__petID__5441852A",
                        column: x => x.petID,
                        principalTable: "Pet",
                        principalColumn: "petID");
                    table.ForeignKey(
                        name: "FK__Booking_D__servi__534D60F1",
                        column: x => x.serviceID,
                        principalTable: "Service",
                        principalColumn: "serviceID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    invoiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    paymentID = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoice__1252410C1622F1EA", x => x.invoiceID);
                    table.ForeignKey(
                        name: "FK_Invoice_Payment",
                        column: x => x.paymentID,
                        principalTable: "Payment",
                        principalColumn: "paymentID");
                    table.ForeignKey(
                        name: "FK__Invoice__booking__5AEE82B9",
                        column: x => x.bookingID,
                        principalTable: "Booking",
                        principalColumn: "bookingID");
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    voucherID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    discount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    issueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    cusID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    manaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bookingID = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    CustomersCusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagersManaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Voucher__F53389899EBEA773", x => x.voucherID);
                    table.ForeignKey(
                        name: "FK_Voucher_Customer_CustomersCusId",
                        column: x => x.CustomersCusId,
                        principalTable: "Customer",
                        principalColumn: "cusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Voucher_Manager_ManagersManaId",
                        column: x => x.ManagersManaId,
                        principalTable: "Manager",
                        principalColumn: "manaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Voucher__booking__6A30C649",
                        column: x => x.bookingID,
                        principalTable: "Booking",
                        principalColumn: "bookingID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0c0d4537-00c0-4291-a87a-658636d685f0"), "0c0d4537-00c0-4291-a87a-658636d685f0", "Customer", "CUSTOMER" },
                    { new Guid("363dda9c-581b-4a57-8c39-2f49e4b954b0"), "363dda9c-581b-4a57-8c39-2f49e4b954b0", "Staff", "STAFF" },
                    { new Guid("5f3d9592-0a8c-461e-9091-eda793302a1e"), "5f3d9592-0a8c-461e-9091-eda793302a1e", "Manager", "MANAGER" },
                    { new Guid("b776ec51-1854-4452-b17b-1fe5ad84da59"), "b776ec51-1854-4452-b17b-1fe5ad84da59", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Admin__A471AFFBB3371801",
                table: "Admin",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_cusID",
                table: "Booking",
                column: "cusID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_manaID",
                table: "Booking",
                column: "manaID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_paymentID",
                table: "Booking",
                column: "paymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Detail_bookingID",
                table: "Booking_Detail",
                column: "bookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Detail_comboID",
                table: "Booking_Detail",
                column: "comboID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Detail_petID",
                table: "Booking_Detail",
                column: "petID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Detail_serviceID",
                table: "Booking_Detail",
                column: "serviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Detail_staffID",
                table: "Booking_Detail",
                column: "staffID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_AdminId",
                table: "Customer",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__A471AFFBD525B8F4",
                table: "Customer",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_paymentID",
                table: "Invoice",
                column: "paymentID");

            migrationBuilder.CreateIndex(
                name: "UQ__Invoice__C6D03BECA66EC071",
                table: "Invoice",
                column: "bookingID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manager_AdminId",
                table: "Manager",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "UQ__Manager__A471AFFBEADB9F39",
                table: "Manager",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_cusID",
                table: "Payment",
                column: "cusID");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_cusID",
                table: "Pet",
                column: "cusID");

            migrationBuilder.CreateIndex(
                name: "IX_Service_comboID",
                table: "Service",
                column: "comboID");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_ManagerManaId",
                table: "Staff",
                column: "ManagerManaId");

            migrationBuilder.CreateIndex(
                name: "UQ__Staff__A471AFFB206680B8",
                table: "Staff",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_CustomersCusId",
                table: "Voucher",
                column: "CustomersCusId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_ManagersManaId",
                table: "Voucher",
                column: "ManagersManaId");

            migrationBuilder.CreateIndex(
                name: "UQ__Voucher__C6D03BEC32216A74",
                table: "Voucher",
                column: "bookingID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Booking_Detail");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
