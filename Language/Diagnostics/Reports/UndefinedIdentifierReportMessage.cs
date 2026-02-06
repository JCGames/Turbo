namespace Turbo.Language.Diagnostics.Reports;

public class UndefinedIdentifierReportMessage : ReportMessage
{
    public UndefinedIdentifierReportMessage(string identifier) : base($"`{identifier}` is not defined.")
    { }
}