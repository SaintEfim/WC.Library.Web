using System.ComponentModel.DataAnnotations;

namespace WC.Library.Web.Models;

public class CreateActionResultDto : IDto
{
    [Required]
    public required Guid Id { get; set; }
}
