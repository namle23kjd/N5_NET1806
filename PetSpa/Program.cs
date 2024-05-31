
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
using PetSpa.Repositories.AccountRepository;
using PetSpa.Repositories.ComboRepository;
using PetSpa.Repositories.ImageRepository;
using PetSpa.Repositories.JobRepository;
using PetSpa.Repositories.ManagerRepository;
using PetSpa.Repositories.ServiceRepository;
using PetSpa.Repositories.StaffRepository;
using PetSpa.Repositories.BookingRepository;

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


           
            builder.Services.AddScoped<IAccountRepository, SQLAccountRepository>();
            builder.Services.AddScoped<IStaffRepository, SQLStaffRepository>();
            builder.Services.AddScoped<IImagesRepository, LocalImageRepository>();
            builder.Services.AddScoped<IManagerRepository, SQLManagerRepositorycs>();
            builder.Services.AddScoped<IJobRepository, SQLJobRepository>();
            builder.Services.AddScoped<ApiResponseService>();
            builder.Services.AddScoped<IServiceRepository, SQLServiceRepository>();
            builder.Services.AddScoped<IComboRespository, SQLComboRepository>();
            builder.Services.AddScoped<IBookingRepository, SQLBookingRepository>();
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
