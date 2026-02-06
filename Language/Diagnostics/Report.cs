using System.Diagnostics.CodeAnalysis;
using Turbo.Language.Diagnostics.Reports;
using Turbo.Language.Parsing.Nodes.Classifications;

namespace Turbo.Language.Diagnostics;

public static class Report
{
    public static bool PreferThrownErrors = false;

    [DoesNotReturn]
    public static Exception Error(ReportMessage reportMessage, Location? location = null)
    {
        return Error(reportMessage.Message, location);
    }
    
    [DoesNotReturn]
    public static Exception Error(string message, Location? location = null)
    {
        location ??= Location.None;
        
        var column = location.End - location.Start;

        Console.ForegroundColor = ConsoleColor.Red;
        
        if (location.SourceFile is { } sourceFile)
        {
            var line = sourceFile.GetLineSpan(location.Line);

            Console.WriteLine(sourceFile.FileInfo is not null
                ? $"Error {sourceFile.FileInfo?.FullName}:{location.Line}:{column}:\n|   {message}"
                : $"Error ?:{location.Line}:{column}\n|   {message}");

            Console.WriteLine("|");
            Console.WriteLine($"| {line.ToString()}");
            Console.WriteLine("|");
        }
        else
        {
            Console.WriteLine($"Error ?:{location.Line}:{column}\n|   {message}");
        }
        
        Console.ResetColor();

        if (PreferThrownErrors)
        {
            throw new Exception(message);
        }
        
        Environment.Exit(1);
        return new Exception();
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static void AssertArgumentCount(IEnumerable<IParameterNode> parameters, int realCount, Location? location = null)
    {
        if (parameters.Count() != realCount)
        {
            throw Error(new WrongArgumentCountReportMessage(parameters, realCount), location);
        }
    }
}