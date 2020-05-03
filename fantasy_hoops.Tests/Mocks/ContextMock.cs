using System;
using System.Collections.Generic;
using fantasy_hoops.Database;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.Tournaments;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

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
            private readonly List<User> _users = new List<User>();
            private readonly List<Player> _players = new List<Player>();
            private readonly List<Achievement> _achievements = new List<Achievement>();
            private readonly List<UserAchievement> _userAchievements = new List<UserAchievement>();
            private readonly List<Post> _posts = new List<Post>();
            private readonly List<UserLineup> _userLineups = new List<UserLineup>();
            private readonly List<PushSubscription> _pushSubscriptions = new List<PushSubscription>();
            private readonly List<Stats> _stats = new List<Stats>();
            private readonly List<Tournament> _tournaments = new List<Tournament>();
            private readonly List<TournamentUser> _tournamentsUsers = new List<TournamentUser>();
            private readonly List<Game> _games = new List<Game>();
            private readonly List<Contest> _contests = fantasy_hoops.Mocks.Tournaments.MockedContests;

            public Builder SetAchievements(List<Achievement> achievements = null)
            {
                _achievements.AddRange(achievements ?? fantasy_hoops.Mocks.Achievements.MockedAchievements);
                return this;
            }
            public Builder SetBlogPosts(List<Post> blogPosts = null)
            {
                _posts.AddRange(blogPosts ?? fantasy_hoops.Mocks.Blog.MockedPosts);
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

            public Builder SetUserLineups(List<UserLineup> userLineups = null)
            {
                _userLineups.AddRange(userLineups ?? fantasy_hoops.Mocks.UserLineups.MockedUserLineups);
                return this;
            }

            public Builder SetPushSubscriptions(List<PushSubscription> pushSubscriptions = null)
            {
                _pushSubscriptions.AddRange(pushSubscriptions ?? fantasy_hoops.Mocks.Push.MockedSubscriptions);
                return this;
            }
            
            public Builder SetStats(List<Stats> stats = null)
            {
                _stats.AddRange(stats ?? fantasy_hoops.Mocks.Stats.MockedStats);
                return this;
            }

            public Builder SetTournaments(List<Tournament> tournaments = null)
            {
                _tournaments.AddRange(tournaments ?? fantasy_hoops.Mocks.Tournaments.MockedTournaments);
                return this;
            }
            
            public Builder SetTournamentsUsers(List<TournamentUser> tournamentsUsers = null)
            {
                _tournamentsUsers.AddRange(tournamentsUsers ?? fantasy_hoops.Mocks.Tournaments.MockedTournamentsUsers);
                return this;
            }

            public Builder SetGames(List<Game> games = null)
            {
                _games.AddRange(games ?? fantasy_hoops.Mocks.Games.MockedGames);
                return this;
            }

            public GameContext Build()
            {
                var options = new DbContextOptionsBuilder<GameContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseMemoryCache(null)
                    .Options;
                var context = new GameContext(options);
                context.Database.EnsureCreated();

                context.Players.AddRange(_players);
                context.Users.AddRange(_users);
                context.Achievements.AddRange(_achievements);
                context.UserAchievements.AddRange(_userAchievements);
                context.Posts.AddRange(_posts);
                context.UserLineups.AddRange(_userLineups);
                context.PushSubscriptions.AddRange(_pushSubscriptions);
                context.Stats.AddRange(_stats);
                context.Tournaments.AddRange(_tournaments);
                context.Games.AddRange(_games);
                context.Contests.AddRange(_contests);
                context.TournamentUsers.AddRange(_tournamentsUsers);

                context.SaveChanges();
                return context;
            }
        }
    }
}