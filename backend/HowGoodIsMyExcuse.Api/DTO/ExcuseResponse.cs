namespace HowGoodIsMyExcuse.Api.DTOs;

public class ExcuseResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public string Roast { get; set; } = string.Empty;
    public string JudgePersonality { get; set; } = string.Empty;
    public int Votes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool HasVoted { get; set; }
}