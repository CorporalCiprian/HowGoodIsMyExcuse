namespace HowGoodIsMyExcuse.Api.Services;

public class ClaudeServiceException : Exception
{
    public ClaudeServiceException(string message) : base(message) { }
}