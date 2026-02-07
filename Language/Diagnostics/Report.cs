using System.Diagnostics.CodeAnalysis;

namespace Turbo.Language.Diagnostics;

public static class Report
{
    public static bool PreferThrownErrors = false;
    
    [DoesNotReturn]
    public static Exception Error(string message, Location? location = null)
    {
        location ??= Location.None;

        Console.ForegroundColor = ConsoleColor.Red;
        
        if (location.SourceFile is { } sourceFile)
        {
            var line = sourceFile.GetLineSpan(location.Line);

            var (lineStart, lineEnd) = sourceFile.GetLineStartAndEnd(location.Line);
            var column = Math.Min(location.Start, lineEnd) - lineStart;
            
            Console.WriteLine(sourceFile.FileInfo is not null
                ? $"Error {sourceFile.FileInfo?.FullName}:{location.Line}:{column}\n|   {message}"
                : $"Error ?:{location.Line}:{column}\n|   {message}");

            Console.WriteLine("|");
            Console.WriteLine($"| {line.ToString()}");
            Console.Write("| ");

            for (var i = 0; i < column; i++)
            {
                Console.Write(' ');
            }
            
            Console.WriteLine('^');
        }
        else
        {
            Console.WriteLine($"Error ?:{location.Line}\n|   {message}");
        }
        
        Console.ResetColor();

        if (PreferThrownErrors)
        {
            throw new Exception(message);
        }
        
        Environment.Exit(1);
        return new Exception();
    }
}