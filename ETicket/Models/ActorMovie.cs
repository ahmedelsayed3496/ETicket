using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ETicket.Models
{
    public class ActorMovie
    {
        public int ActorId { get; set; }
        public int MovieId { get; set; }
        [ValidateNever]
        public Actor Actor { get; set; }
        [ValidateNever]
        public Movie Movie { get; set; }
    }
}
