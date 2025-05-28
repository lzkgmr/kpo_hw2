using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Presentation;

public static class FileAnalysisApiExtensions
{
    public static RouteGroupBuilder MapFileAnalysisApi(this RouteGroupBuilder group)
    {
        group.MapGet("analysis/{id}", HandleGetAnalysis)
            .Produces<FileDataHolder>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
            
        group.MapGet("analysis/GetHash/{content}", HandleGetHash)
            .Produces<int>(StatusCodes.Status200OK);
        
        return group;
    }

    public static async Task<IResult> HandleGetAnalysis(
        Guid id,
        [FromServices] IFileAnalysisService service)
    {
        var stats = await service.GetFileStatistics(id);
        return stats == null ? Results.BadRequest($"File {id} not found in storage or analysis DB") : Results.Ok(stats);
    }

    public static async Task<IResult> HandleGetHash(
        string content,
        [FromServices] IFileAnalysisService service)
    {
        var result = service.AnalyzeFile(content);
        return Results.Content(result.Hash.ToString());
    }
}