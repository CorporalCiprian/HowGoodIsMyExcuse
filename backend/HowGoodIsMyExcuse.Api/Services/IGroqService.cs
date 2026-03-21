using HowGoodIsMyExcuse.Api.DTOs;

namespace HowGoodIsMyExcuse.Api.Services;

public interface IGroqService
{
    Task<JudgeResult> EvaluateExcuse(string excuseText, string judgePersonality);
}