namespace HowGoodIsMyExcuse.Api.Models;

public class Excuse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public string Roast { get; set; } = string.Empty;
    public string JudgePersonality { get; set; } = string.Empty;
    public int Votes { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}