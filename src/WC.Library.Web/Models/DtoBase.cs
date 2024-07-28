using System.ComponentModel.DataAnnotations;

namespace WC.Library.Web.Models;

public class DtoBase : IDto
{
    [Required]
    public required Guid Id { get; set; }
}
