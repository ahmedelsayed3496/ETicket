using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETicket.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? CinemaLogo { get; set; }
        public string Address { get; set; }
        [ValidateNever]
        public List<Movie> Movies { get; set; }
    }
}
