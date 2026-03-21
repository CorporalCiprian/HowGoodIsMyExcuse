namespace HowGoodIsMyExcuse.Api.Services;

public class GroqServiceException : Exception
{
    public GroqServiceException(string message) : base(message) { }
}