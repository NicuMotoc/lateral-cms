using FluentValidation;
using FluentValidation.AspNetCore;
using LateralCMS.Application.Commands;
using LateralCMS.Application.DTOs.Validators;
using LateralCMS.Application.Queries;
using LateralCMS.Auth;
using LateralCMS.Infrastructure.Persistence.EF;
using LateralCMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LateralCMS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<CmsEventDtoValidator>();

        builder.Services.AddOpenApi();

        builder.Services.AddAuthentication("Basic")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

        builder.Services.AddDbContext<CmsDbContext>(opt =>
            opt.UseInMemoryDatabase("CmsDb"));
        builder.Services.AddScoped<EfEntityRepository>();
        builder.Services.AddScoped<EfReadOnlyEntityRepository>();
        builder.Services.AddScoped<ProcessCmsEventsCommand>();
        builder.Services.AddScoped<DisableEntityCommand>();
        builder.Services.AddScoped<EntityQueryService>();
        builder.Services.AddScoped<SanitizationService>();

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
