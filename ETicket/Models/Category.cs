using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETicket.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ValidateNever]
        public List<Movie> Movies { get; set; }
    }
}
