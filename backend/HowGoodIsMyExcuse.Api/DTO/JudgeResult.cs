namespace HowGoodIsMyExcuse.Api.DTOs;

public class JudgeResult
{
    public int Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public string Roast { get; set; } = string.Empty;
}