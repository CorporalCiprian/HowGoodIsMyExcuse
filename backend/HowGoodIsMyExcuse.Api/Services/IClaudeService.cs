using HowGoodIsMyExcuse.Api.DTOs;

namespace HowGoodIsMyExcuse.Api.Services;

public interface IClaudeService
{
    Task<JudgeResult> EvaluateExcuse(string excuseText, string judgePersonality);
}