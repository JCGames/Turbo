using Turbo.Language.Parsing;

namespace Turbo.Language.Diagnostics;

public class Location
{
    public static Location None => new()
    {
        SourceFile = null,
        Line = -1,
        Start = -1,
        End = -1
    };

    public static Location New(SourceFile sourceFile)
    {
        return new Location
        {
            SourceFile = sourceFile,
            Line = sourceFile.CurrentLine,
            Start = sourceFile.CurrentIndex,
            End = sourceFile.CurrentIndex
        };
    }
    
    public required SourceFile? SourceFile { get; set; }
    public required int Line { get; set; }
    public required int Start { get; set; }
    public required int End { get; set; }
}