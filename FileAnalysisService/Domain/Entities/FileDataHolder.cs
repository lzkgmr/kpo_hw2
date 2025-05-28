namespace FileAnalysisService.Domain.Entities;

public class FileDataHolder(Guid fileId, int hash, int wordCount, int paragraphCount, int symbolCount)
{
    public Guid FileId { get; init; } = fileId;
    public int Hash { get; init; } = hash;
    public int WordCount { get; init; } = wordCount;
    public int ParagraphCount { get; init; } = paragraphCount;
    public int SymbolCount { get; init; } = symbolCount;
}