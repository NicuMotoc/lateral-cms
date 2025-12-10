using FluentValidation;
using FluentValidation.AspNetCore;
using LateralCMS.Application.Commands;
using LateralCMS.Application.Queries;
using LateralCMS.Auth;
using LateralCMS.Infrastructure.Persistence.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LateralCMS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CmsEventDtoValidator>());
        builder.Services.AddOpenApi();

        builder.Services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

        builder.Services.AddDbContext<CmsDbContext>(opt =>
            opt.UseInMemoryDatabase("CmsDb"));
        builder.Services.AddScoped<EfEntityRepository>();
        builder.Services.AddScoped<ProcessCmsEventsCommand>();
        builder.Services.AddScoped<DisableEntityCommand>();
        builder.Services.AddScoped<EntityQueryService>();

        var app = builder.Build();

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
