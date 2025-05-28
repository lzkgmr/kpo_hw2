using Microsoft.AspNetCore.Mvc;
using Shared.Clients;
using Shared.DTOs;
using FileResult = Shared.DTOs.FileResult;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

var app = builder.Build();
ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();
return;

void ConfigureServices(WebApplicationBuilder webBuilder)
{
    webBuilder.Services.AddEndpointsApiExplorer();
    webBuilder.Services.AddSwaggerGen();

    webBuilder.Services.AddHttpClient<FileStorageClient>(c =>
        c.BaseAddress = new Uri(webBuilder.Configuration["FileStorageService:Url"]!));

    webBuilder.Services.AddHttpClient<FileAnalysisClient>(c =>
        c.BaseAddress = new Uri(webBuilder.Configuration["FileAnalysisService:Url"]!));
}

void ConfigureMiddleware(WebApplication application)
{
    if (!application.Environment.IsDevelopment()) return;
    
    application.UseSwagger();
    application.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = "";
    });
}

void ConfigureEndpoints(WebApplication application)
{
    MapFileUploadEndpoint(application);
    MapFileContentEndpoint(application);
    MapFileAnalysisEndpoint(application);
}

void MapFileUploadEndpoint(WebApplication webApplication)
{
    webApplication.MapPost("/api/files", async (IFormFile file, [FromServices] FileStorageClient storage) =>
        {
            if (file.Length == 0)
                return Results.BadRequest("File is required");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var id = await storage.Upload(ms, file.FileName);
            return Results.Created($"/api/files/{id}", id);
        })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<Guid>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("UploadFile")
        .WithTags("Files");
}

void MapFileContentEndpoint(WebApplication webApplication)
{
    webApplication.MapGet("/api/files/{id}", async (Guid id, [FromServices] FileStorageClient storage) =>
        {
            try
            {
                var content = await storage.GetFileContent(id);
                return Results.Ok(content);
            }
            catch (HttpRequestException)
            {
                return Results.NotFound();
            }
        })
        .Produces<string>()
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetFileContent")
        .WithTags("Files");
}

void MapFileAnalysisEndpoint(WebApplication webApplication)
{
    webApplication.MapGet("/api/files/{id}/analysis", async (Guid id, [FromServices] FileAnalysisClient analysis) =>
        {
            var stats = await analysis.GetStatistics(id);
            if (stats == null)
                return Results.NotFound();
            return Results.Ok(stats);
        })
        .Produces<FileResult>()
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetFileAnalysis")
        .WithTags("Analysis");
}