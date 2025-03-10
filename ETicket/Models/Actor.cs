using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETicket.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public string News { get; set; }

        [ValidateNever]
        public List<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
        [ValidateNever]
        public List<Movie> Movies { get; set; }
    }
}
