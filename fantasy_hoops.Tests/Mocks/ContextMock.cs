using System;
using System.Collections.Generic;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Tests.Mocks
{
    public class ContextMock
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
        public DbSet<RequestNotification> RequestNotifications { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<BestLineup> BestLineups { get; set; }
        public DbSet<PlayersBestLineups> PlayersBestLineups { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentUser> TournamentUsers { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<MatchupPair> TournamentMatchups { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<TournamentInvite> TournamentInvites { get; set; }

        public class Builder
        {
            private readonly List<Team> _teams  = new List<Team>();
            private readonly List<Player> _players = new List<Player>();
            private readonly List<Achievement> _achievements = new List<Achievement>();
            private readonly List<UserAchievement> _userAchievements = new List<UserAchievement>();

            // public DbSet<Game> Games { get; set; }
            // public DbSet<Stats> Stats { get; set; }
            // public DbSet<Injury> Injuries { get; set; }
            // public DbSet<Paragraph> Paragraphs { get; set; }
            // public DbSet<News> News { get; set; }
            // public DbSet<UserLineup> UserLineups { get; set; }
            // public DbSet<FriendRequest> FriendRequests { get; set; }
            // public DbSet<Notification> Notifications { get; set; }
            // public DbSet<GameScoreNotification> GameScoreNotifications { get; set; }
            // public DbSet<InjuryNotification> InjuryNotifications { get; set; }
            // public DbSet<RequestNotification> RequestNotifications { get; set; }
            // public DbSet<PushSubscription> PushSubscriptions { get; set; }
            // public DbSet<Post> Posts { get; set; }
            // public DbSet<UserAchievement> UserAchievements { get; set; }
            // public DbSet<BestLineup> BestLineups { get; set; }
            // public DbSet<PlayersBestLineups> PlayersBestLineups { get; set; }
            // public DbSet<Tournament> Tournaments { get; set; }
            // public DbSet<TournamentUser> TournamentUsers { get; set; }
            // public DbSet<Contest> Contests { get; set; }
            // public DbSet<MatchupPair> TournamentMatchups { get; set; }
            // public DbSet<Season> Seasons { get; set; }
            // public DbSet<TournamentInvite> TournamentInvites { get; set; }
            private readonly List<User> _users = new List<User>();

            public Builder SetAchievements(List<Achievement> achievements = null)
            {
                _achievements.AddRange(achievements ?? fantasy_hoops.Mocks.Achievements.MockedAchievements);
                return this;
            }
            
            public Builder SetUserAchievements(List<UserAchievement> userAchievements = null)
            {
                _userAchievements.AddRange(userAchievements ?? fantasy_hoops.Mocks.Achievements.MockedUserAchievements);
                return this;
            }

            public Builder SetUsers(List<User> users = null)
            {
                _users.AddRange(users ?? fantasy_hoops.Mocks.Users.MockedUsers);
                return this;
            }

            public Builder SetPlayers(List<Player> players = null)
            {
                _players.AddRange(players ?? fantasy_hoops.Mocks.Players.MockedPlayers);
                return this;
            }

            public GameContext Build()
            {
                var options = new DbContextOptionsBuilder<GameContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                var context = new GameContext(options);
                context.Database.EnsureCreated();

                context.Players.AddRange(_players);
                context.Users.AddRange(_users);
                context.Achievements.AddRange(_achievements);
                context.UserAchievements.AddRange(_userAchievements);

                context.SaveChanges();
                return context;
            }
        }
    }
}