using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Mappings;
using PetSpa.Repositories;
using PetSpa.Repositories.Token;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using PetSpa.Models.Domain;
using PetSpa.Repositories.SendingEmail;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FluentAssertions.Common;
using Serilog;
using PetSpa.Middlewares;
using PetSpa.Repositories.AdminRepository;
using PetSpa.Repositories.BookingRepository;
using PetSpa.Repositories.BookingDetailRepository;
using PetSpa.Repositories.ComboRepository;
using PetSpa.Repositories.ImageRepository;
using PetSpa.Repositories.ManagerRepository;
using PetSpa.Repositories.ServiceRepository;
using PetSpa.Repositories.StaffRepository;
using PetSpa.Repositories.CustomerRepository;
using Hangfire;
using System.Reflection;
using PetSpa.Repositories.PaymentRepository;

namespace PetSpa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/Pet_Spa.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            // Identity and Authentication configuration
            builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(3); // Token lifespan 15 minutes
            });
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Smtp"));

            builder.Services.AddDbContext<PetSpaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<PetSpaContext>().AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            // Other services
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
            builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();
            builder.Services.AddLogging();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            // Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Test Api", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "Oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }

            });
            });

            // Thêm In-Memory Cache
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register repositories and services
            builder.Services.AddScoped<ApiResponseService>();
            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            builder.Services.AddScoped<IAdminRepository, SQLAdminRepository>();
            builder.Services.AddScoped<IBookingRepository, SQLBookingRepository>();
            builder.Services.AddScoped<IBookingDetailsRepository, SQLBookingDetailRepository>();
            builder.Services.AddScoped<IComboRespository, SQLComboRepository>();
            builder.Services.AddScoped<IImagesRepository, LocalImageRepository>();
            builder.Services.AddScoped<IManagerRepository, SQLManagerRepositorycs>();
            builder.Services.AddScoped<IServiceRepository, SQLServiceRepository>();
            builder.Services.AddScoped<IStaffRepository, SQLStaffRepository>();
            builder.Services.AddScoped<IVnPayService, VnpayService>();
            builder.Services.AddScoped<ICustomerRepository, SQLCustomerRepository>();
            builder.Services.AddScoped<BookingStatusChecker>();

            // Add email config
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailSettings>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Api v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Payment service api v1");
                });
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                using var scope = app.Services.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                BookingStatusChecker.ConfigureHangfireJobs(serviceProvider);
            });

            app.MapControllers();

            app.Run();
        }
    }
}
