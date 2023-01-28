using FixxAPI.Helper;
using FixxAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Twilio.Clients;

namespace FixxAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {        
        services.AddControllers();
        //MINE
        services.AddHttpContextAccessor();
        services.AddHttpClient<ITwilioRestClient, TwilioRestClient>();
        services.AddDbContext<Data_Context>(options => options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));
        services.AddScoped<Ibusiness_auth_service, business_auth_service>();

        //jwt
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        //enable cors
        var cors_policy = "fixx_cors";
        services.AddCors
            (
                options =>
                {
                    options.AddPolicy(cors_policy, policy =>
                    {
                        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                    });
                }
            );
        //enable use of jwt in controllers
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard authorization header using the bearer scheme",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Fixx247 API", Version = "v1" });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        //MINE
        app.UseAuthentication();
        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fixx247 API V1");
        });

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}