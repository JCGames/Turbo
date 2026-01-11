using System.Diagnostics.CodeAnalysis;
using Turbo.Language.Parsing.Nodes.Classifications;

namespace Turbo.Language.Diagnostics.Reports;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class WrongArgumentCountReportMessage : ReportMessage
{
    public WrongArgumentCountReportMessage(IEnumerable<IParameterNode> expectedArguments, int passedCount, int? requiredCount = null)
        : base($"{requiredCount ?? expectedArguments.Count()} {(requiredCount > 1 ? "arguments were" : "argument was")} required ({string.Join(", ", expectedArguments.Select(x => x.PublicParameterName))}), but {passedCount} {(passedCount > 1 ? "were" : "was")} passed.")
    { }
}