using System.ComponentModel.DataAnnotations;

namespace HowGoodIsMyExcuse.Api.DTOs;

public class SubmitExcuseRequest
{
    [Required]
    [MinLength(10)]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string JudgePersonality { get; set; } = string.Empty;
}