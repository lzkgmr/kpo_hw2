using FileAnalysisService.Domain.Interfaces;
using FileAnalysisService.Infrastructure.Repositories;
using FileAnalysisService.Presentation;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);
var app = builder.Build();

ConfigureMiddleware(app);
ConfigureDatabase(app);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddAuthorization();

    ConfigureDatabaseServices(webBuilder);
    ConfigureApplicationServices(webBuilder);
    ConfigureHttpClients(webBuilder);
    ConfigureApiServices(webBuilder);
}

void ConfigureDatabaseServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddDbContext<FileAnalysisDbContext>(options =>
        options.UseNpgsql(webBuilder.Configuration.GetConnectionString("FileAnalysisDatabase")));

    webBuilder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

void ConfigureApplicationServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddScoped<IFileAnalysisRepository, FileAnalysisRepository>();
    webBuilder.Services.AddScoped<IFileAnalysisService, FileAnalysisService.Application.Services.FileAnalysisService>();
}

void ConfigureHttpClients(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddHttpClient<IFileStorageClient, FileStorageClient>(client =>
        client.BaseAddress = new Uri(webBuilder.Configuration["FileStorageService:Url"]!));
}

void ConfigureApiServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddControllers();
    webBuilder.Services.AddEndpointsApiExplorer();
    webBuilder.Services.AddSwaggerGen();
}

void ConfigureMiddleware(WebApplication application)
{
    if (application.Environment.IsDevelopment())
    {
        application.UseSwagger();
        application.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileAnalysisService v1"));
    }

    application.UseHttpsRedirection();
    application.UseRouting();
}

void ConfigureDatabase(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FileAnalysisDbContext>();
    dbContext.Database.Migrate();
}

void ConfigureEndpoints(WebApplication application)
{
    application.MapGroup("/api")
        .MapFileAnalysisApi()
        .WithTags("File Analysis API");

    application.MapControllers();
}