using LateralCMS.Auth;
using Microsoft.AspNetCore.Authentication;
using LateralCMS.Infrastructure.Persistence.EF;
using LateralCMS.Application.Commands;
using LateralCMS.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CmsOnly", policy =>
                    policy.RequireAssertion(ctx => ctx.User.Identity != null && ctx.User.Identity.Name == "cms_webhook_user"));
            });

            builder.Services.AddDbContext<CmsDbContext>(opt =>
                opt.UseInMemoryDatabase("CmsDb"));
            builder.Services.AddScoped<EfEntityRepository>();
            builder.Services.AddScoped<ProcessCmsEventsCommand>();
            builder.Services.AddScoped<DisableEntityCommand>();
            builder.Services.AddScoped<EntityQueryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
