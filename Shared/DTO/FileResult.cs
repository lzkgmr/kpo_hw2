namespace Shared.DTOs;

public class FileResult
{
    public int Hash { get; init; }   
    public int ParagraphCount { get; init; }
    public int WordCount { get; init; }    
    public int SymbolCount { get; init; } 
}