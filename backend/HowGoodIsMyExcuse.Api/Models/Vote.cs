namespace HowGoodIsMyExcuse.Api.Models;

public class Vote
{
    public Guid Id { get; set; }
    public Guid ExcuseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Excuse Excuse { get; set; } = null!;
    public User User { get; set; } = null!;
}