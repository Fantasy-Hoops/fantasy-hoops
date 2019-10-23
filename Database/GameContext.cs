using fantasy_hoops.Models;
using fantasy_hoops.Models.Notifications;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace fantasy_hoops.Database
{
    public class GameContext : IdentityDbContext<User>
    {

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Injury> Injuries { get; set; }
        public DbSet<Paragraph> Paragraphs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<UserLineup> UserLineups { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GameScoreNotification> GameScoreNotifications { get; set; }
        public DbSet<InjuryNotification> InjuryNotifications { get; set; }
        public DbSet<FriendRequestNotification> FriendRequestNotifications { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }

        public GameContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Player>()
                .HasOne(p => p.Injury)
                .WithOne(i => i.Player)
                .HasForeignKey<Injury>(i => i.PlayerID);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        }
    }
}
