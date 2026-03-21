using HowGoodIsMyExcuse.Api.Data;
using HowGoodIsMyExcuse.Api.DTOs;
using HowGoodIsMyExcuse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HowGoodIsMyExcuse.Api.Services;

public class ExcuseService : IExcuseService
{
    private readonly AppDbContext _db;
    private readonly IGroqService _groqService;

public ExcuseService(AppDbContext db, IGroqService groqService)
{
    _db = db;
    _groqService = groqService;
}

    public async Task<ExcuseResponse> SubmitExcuse(SubmitExcuseRequest request, Guid userId)
    {
        var judgeResult = await _groqService.EvaluateExcuse(request.Text, request.JudgePersonality);

        var excuse = new Excuse
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Score = judgeResult.Score,
            Verdict = judgeResult.Verdict,
            Roast = judgeResult.Roast,
            JudgePersonality = request.JudgePersonality,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Excuses.Add(excuse);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);

        return MapToResponse(excuse, user!.Username, false);
    }

    public async Task<List<ExcuseResponse>> GetLeaderboard(int page, int pageSize, Guid? currentUserId)
    {
        var excuses = await _db.Excuses
            .Include(e => e.User)
            .OrderByDescending(e => e.Votes)
            .ThenByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new List<ExcuseResponse>();

        foreach (var excuse in excuses)
        {
            var hasVoted = currentUserId.HasValue && await _db.Votes
                .AnyAsync(v => v.ExcuseId == excuse.Id && v.UserId == currentUserId.Value);

            result.Add(MapToResponse(excuse, excuse.User.Username, hasVoted));
        }

        return result;
    }

    public async Task<ExcuseResponse> GetExcuseById(Guid id, Guid? currentUserId)
    {
        var excuse = await _db.Excuses
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new KeyNotFoundException("Excuse not found.");

        var hasVoted = currentUserId.HasValue && await _db.Votes
            .AnyAsync(v => v.ExcuseId == id && v.UserId == currentUserId.Value);

        return MapToResponse(excuse, excuse.User.Username, hasVoted);
    }

    public async Task<ExcuseResponse> VoteOnExcuse(Guid excuseId, Guid userId)
    {
        var alreadyVoted = await _db.Votes
            .AnyAsync(v => v.ExcuseId == excuseId && v.UserId == userId);

        if (alreadyVoted)
            throw new InvalidOperationException("Already voted.");

        var excuse = await _db.Excuses
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == excuseId)
            ?? throw new KeyNotFoundException("Excuse not found.");

        _db.Votes.Add(new Vote
        {
            Id = Guid.NewGuid(),
            ExcuseId = excuseId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        excuse.Votes++;
        await _db.SaveChangesAsync();

        return MapToResponse(excuse, excuse.User.Username, true);
    }

    private static ExcuseResponse MapToResponse(Excuse excuse, string username, bool hasVoted) => new()
    {
        Id = excuse.Id,
        Text = excuse.Text,
        Score = excuse.Score,
        Verdict = excuse.Verdict,
        Roast = excuse.Roast,
        JudgePersonality = excuse.JudgePersonality,
        Votes = excuse.Votes,
        CreatedAt = excuse.CreatedAt,
        Username = username,
        HasVoted = hasVoted
    };
}