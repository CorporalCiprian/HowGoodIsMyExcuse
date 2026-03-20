using HowGoodIsMyExcuse.Api.DTOs;

namespace HowGoodIsMyExcuse.Api.Services;

public interface IExcuseService
{
    Task<ExcuseResponse> SubmitExcuse(SubmitExcuseRequest request, Guid userId);
    Task<List<ExcuseResponse>> GetLeaderboard(int page, int pageSize, Guid? currentUserId);
    Task<ExcuseResponse> GetExcuseById(Guid id, Guid? currentUserId);
    Task<ExcuseResponse> VoteOnExcuse(Guid excuseId, Guid userId);
}