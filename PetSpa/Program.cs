
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Mappings;
using PetSpa.Repositories;
using PetSpa.Repositories.Customer;
using PetSpa.Repositories.Pet;

namespace PetSpa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddControllers(options =>
            //{
            //    options.Filters.Add(new ValidateModeAtrribute());
            //});
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle




            builder.Services.AddDbContext<PetSpaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<ICusRepository, CusRepository>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; }); 
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
