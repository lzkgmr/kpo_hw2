using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repos;
using FileStorageService.Infrastructure.Repositories;
using FileStorageService.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Shared.Clients;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);
var app = builder.Build();

ConfigureMiddleware(app);
InitializeDatabase(app);
ConfigureEndpoints(app);

app.Run();
return;

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
    webBuilder.Services.AddDbContext<FileStorageDbContext>(options =>
        options.UseNpgsql(webBuilder.Configuration.GetConnectionString("FileStorageDatabase")));
    
    webBuilder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

void ConfigureApplicationServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddScoped<IFileStorageRepository, FileStorageRepository>();
    webBuilder.Services.AddScoped<IFileStorageService, FileStorageService.Application.Services.FileStorageService>();
}

void ConfigureHttpClients(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddHttpClient<IFileAnalysisClient, FileAnalysisClient>(client =>
        client.BaseAddress = new Uri(webBuilder.Configuration["FileAnalysisService:Url"]!));
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
        application.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorageService v1"));
    }

    application.UseHttpsRedirection();
    application.UseRouting();
}

void InitializeDatabase(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    InitializeDbContext(services);
    CreateUploadsDirectory(application);
}

void InitializeDbContext(IServiceProvider services)
{
    var dbContext = services.GetRequiredService<FileStorageDbContext>();
    dbContext.Database.Migrate();
}

void CreateUploadsDirectory(WebApplication application)
{
    var uploadsPath = Path.Combine(application.Environment.ContentRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);
}

void ConfigureEndpoints(WebApplication application)
{
    application.MapGroup("/api")
        .MapFileStorageApi()
        .WithTags("File Storage API");
    
    application.MapControllers();
}