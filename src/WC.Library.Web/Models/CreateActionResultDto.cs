using System.ComponentModel.DataAnnotations;

namespace WC.Library.Web.Models;

public class CreateActionResultDto : IDto
{
    [Required]
    public Guid Id { get; init; }
}
