using System.ComponentModel.DataAnnotations;

namespace WC.Library.Web.Models;

public interface IDto
{
    [Required]
    Guid Id { get; set; }
}
