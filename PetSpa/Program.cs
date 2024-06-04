
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Mappings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Serilog;
using PetSpa.Middlewares;
using PetSpa.CustomActionFilter;
using PetSpa.Repositories.ComboRepository;
using PetSpa.Repositories.ImageRepository;
using PetSpa.Repositories.ManagerRepository;
using PetSpa.Repositories.ServiceRepository;
using PetSpa.Repositories.StaffRepository;
using PetSpa.Repositories.BookingRepository;
using PetSpa.Repositories.BookingDetailRepository;
using PetSpa.Repositories.AdminRepository;
using Microsoft.AspNetCore.Identity;

namespace PetSpa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("Logs/Pet_Spa.txt", rollingInterval: RollingInterval.Minute).MinimumLevel.Information()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IStaffRepository, SQLStaffRepository>();
            builder.Services.AddScoped<IImagesRepository, LocalImageRepository>();
            builder.Services.AddScoped<IManagerRepository, SQLManagerRepositorycs>();
            builder.Services.AddScoped<ApiResponseService>();
            builder.Services.AddScoped<IServiceRepository, SQLServiceRepository>();
            builder.Services.AddScoped<IComboRespository, SQLComboRepository>();
            builder.Services.AddScoped<IBookingRepository, SQLBookingRepository>();
            builder.Services.AddScoped<IBookingDetailsRepository, SQLBookingDetailRepository>();
            builder.Services.AddScoped<IAdminRepository, SQLAdminRepository>();

            //Login User 
            //var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailSettings>();

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        builder =>
            //        {
            //            builder.AllowAnyOrigin()
            //                   .AllowAnyHeader()
            //                   .AllowAnyMethod();
            //        });
            //});
            //builder.Services.AddSingleton(emailConfig);
            //builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Thêm các tùy chọn khác nếu cần thiết
            }).AddEntityFrameworkStores<PetSpaContext>().AddDefaultTokenProviders();


            builder.Services.AddIdentityCore<IdentityUser>().AddRoles<IdentityRole>().AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Default").AddEntityFrameworkStores<PetSpaContext>().AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            });


            //Login User 


            builder.Services.AddDbContext<PetSpaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();


            app.UseAuthentication();

            app.UseAuthorization();

            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });

            app.MapControllers();

            app.Run();
        }
    }
}
