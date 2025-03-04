using ETicket.Models;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ActorMovie>()
        .HasKey(e => new { e.ActorId, e.MovieId }); 

            modelBuilder.Entity<ActorMovie>()
                .HasOne(e => e.Actor)
                .WithMany(e => e.ActorMovies) 
                .HasForeignKey(e => e.ActorId);

            modelBuilder.Entity<ActorMovie>()
                .HasOne(e => e.Movie)
                .WithMany(e => e.ActorMovies) 
                .HasForeignKey(e => e.MovieId);

            
            modelBuilder.Entity<Movie>()
                .HasOne(e => e.Cinema)
                .WithMany(e => e.Movies)
                .HasForeignKey(e => e.CinemaId);

           
            modelBuilder.Entity<Movie>()
                .HasOne(e => e.Category)
                .WithMany(e => e.Movies)
                .HasForeignKey(e => e.CategoryId);
        }
    }
}

