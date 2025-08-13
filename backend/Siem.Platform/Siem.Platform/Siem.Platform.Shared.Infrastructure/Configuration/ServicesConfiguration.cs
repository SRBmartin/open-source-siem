using System.ComponentModel.DataAnnotations;

namespace Siem.Platform.Shared.Infrastructure.Configuration;

public class ServicesConfiguration
{
    [Required, Url]
    public string IamPlatform { get; set; } = default!;
    [Required, Url]
    public string MailService { get; set; } = default!;
}
