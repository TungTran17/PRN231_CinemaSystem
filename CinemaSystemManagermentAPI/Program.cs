using BussinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OData;
using CinemaSystemManagermentAPI.Filters;

namespace CinemaSystemManagermentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure Swagger and OData
            builder.Services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ODataOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cinema System API", Version = "v1" });
                c.CustomSchemaIds(type => type.FullName); // Tránh xung đột tên
            });

            // Register DbContext
            builder.Services.AddDbContext<CinemaSystemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register OData
            builder.Services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(100)
                .AddRouteComponents("odata", GetEdmModel()));

            builder.Services.AddScoped<AdminAuthorizationFilter>();

            var app = builder.Build();

            app.UseCookiePolicy();

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

        // OData GetEdmModel
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Category>("Category");
            builder.EntitySet<Film>("Home");
            builder.EntitySet<Film>("Film");
            builder.EntitySet<User>("User");
            builder.EntitySet<Show>("Show");
            return builder.GetEdmModel();
        }
    }
}
