using System;
using System.Collections.Generic;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.Tournaments;

namespace fantasy_hoops
{
    public static class Mocks
    {
        public static class Tournaments
        {
            public static readonly List<DateTime> StartDates = new List<DateTime>
            {
                new DateTime(2020, 06, 15, 19, 0, 0),
                new DateTime(2020, 06, 22, 19, 0, 0),
                new DateTime(2020, 06, 29, 19, 0, 0),
                new DateTime(2020, 07, 06, 19, 0, 0),
                new DateTime(2020, 07, 13, 19, 0, 0),
                new DateTime(2020, 07, 20, 19, 0, 0),
                new DateTime(2021, 04, 02, 19, 0, 0)
            };

            public static readonly DateTime MockedStartDate = new DateTime(2021, 04, 02, 19, 0, 0);
            
            public static readonly List<Tournament> MockedTournaments = new List<Tournament>
            {
                new Tournament
                {
                    Id = "tournament2",
                    StartDate = new DateTime(2020, 04, 20, 19, 0, 0),
                },
                new Tournament
                {
                    Id = "tournament3",
                    StartDate = new DateTime(2020, 05, 04, 19, 0, 0)
                },
                new Tournament
                {
                    Id = "tournament1",
                    Title = "Tournament 1 Title",
                    Description = "Tournament 1 Description",
                    StartDate = new DateTime(2020, 04, 02, 19, 0, 0),
                    EndDate = new DateTime(2021, 04, 02, 19, 0, 0),
                    Creator = Users.MockedUsers[0],
                    CreatorID = "xxx"
                }
            };
            
            public static readonly List<Contest> MockedContests = new List<Contest>
            {
                new Contest
                {
                    Id = 9999,
                    TournamentId = MockedTournaments[2].Id
                }
            };
            
            public static readonly List<TournamentUser> MockedTournamentsUsers = new List<TournamentUser>
            {
                new TournamentUser
                {
                    User = Users.MockedUsers[0],
                    UserID = Users.MockedUsers[0].Id,
                    Tournament = MockedTournaments[0],
                    TournamentID = MockedTournaments[0].Id,
                    Wins = 15,
                    Losses = 10,
                    Points = 100
                }
            };
        }

        public static class Players
        {
            public static readonly List<int> PlayerPool = new List<int>
            {
                1385, 1946, 1307, 1304, 1297, 1483, 1915, 1913, 1302, 1353, 1301, 1914, 1306, 1639, 1388, 1918, 1544,
                1390, 1485, 1810, 1378, 1377, 1386, 1948, 1308, 1249, 1452, 1449, 1453, 1870, 1505, 1282, 1871, 1455,
                1719, 1536, 1263, 1240, 1251, 1389, 1257, 1933, 1264, 1348, 1586, 1492, 1324, 1444, 1869, 1922, 1374,
                1318, 1334, 1337, 1332, 1338, 1421, 1328, 1329, 1878, 1331, 1591, 1920, 1394, 1407, 1343, 1580, 1281,
                1404, 1560, 1945, 1944, 1721, 1330, 1743, 1432, 1427, 1433, 1425, 1771, 1429, 1428, 1424, 1615, 1437,
                1847, 1285, 1447, 1290, 1293, 1287, 1565, 1265, 1361, 1563, 1705, 1436, 1258, 1571, 1506, 1821, 1271,
                1273, 1572, 1402, 1267, 1266, 1275, 1380, 1466, 1898, 1901, 1820, 1899, 1715, 1519, 1517, 1669, 1671,
                1456, 1675, 1668, 1670, 1691, 1662, 1423, 1674, 1672, 1514, 1289, 1604, 1600, 1498, 1603, 1925, 1500,
                1238, 1683, 1924, 1632, 1607, 1495, 1496, 1488, 1687, 1295, 1497, 1494, 1490, 1272, 1891, 1879, 1319,
                1605, 1387, 1905, 1684, 1692, 1794, 1516, 1602, 1502, 1597, 1535, 1904, 1906, 1957, 1228, 1234, 1553,
                1482, 1558, 1554, 1551, 1557, 1349, 1552, 1887, 1690, 1472
            };
            
            public static readonly List<Player> MockedPlayers = new List<Player>
            {
                new Player
                {
                    Position = "PG",
                    PlayerID = 123,
                    NbaID = 123123,
                    IsPlaying = false,
                    Price = 60
                },
                new Player
                {
                    Position = "SG",
                    PlayerID = 456,
                    NbaID = 456456,
                    IsPlaying = false,
                    Price = 60
                },
                new Player
                {
                    Position = "SF",
                    PlayerID = 321,
                    NbaID = 321321,
                    IsPlaying = false,
                    Price = 60
                },
                new Player
                {
                    Position = "PF",
                    PlayerID = 654,
                    NbaID = 654654,
                    IsPlaying = false,
                    Price = 60
                },
                new Player
                {
                    Position = "C",
                    PlayerID = 789,
                    NbaID = 789789,
                    IsPlaying = false,
                    Price = 60
                },
                new Player
                {
                    Position = "C",
                    PlayerID = 987,
                    NbaID = 987987,
                    IsPlaying = false,
                    Price = 60
                }
            };
        }

        public static class Users
        {
            public static readonly List<User> MockedUsers = new List<User>
            {
                new User
                {
                    Id = "xxx",
                    UserName = "xUser",
                    Email = "xxx@test.com"
                },
                new User
                {
                    Id = "yyy",
                    UserName = "yUser",
                    Email = "yyy@test.com"
                }
            };
        }
        
        public static class Achievements
        {
            public static readonly List<Achievement> MockedAchievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = 123,
                    Type = AchievementType.SINGLE_LEVEL,
                    Title = "Achievement 1"
                },
                new Achievement
                {
                    Id = 456,
                    Type = AchievementType.MULTI_LEVEL,
                    Title = "Achievement 2"
                }
            };
            
            public static readonly List<UserAchievement> MockedUserAchievements = new List<UserAchievement>
            {
                new UserAchievement
                {
                    Progress = 100,
                    Level = 1,
                    LevelUpGoal = 100,
                    UserID = Users.MockedUsers[0].Id,
                    User = Users.MockedUsers[0],
                    AchievementID = MockedAchievements[0].Id,
                    Achievement = MockedAchievements[0]
                },
                new UserAchievement
                {
                    Progress = 200,
                    Level = 2,
                    LevelUpGoal = 100,
                    UserID = Users.MockedUsers[1].Id,
                    User = Users.MockedUsers[1],
                    AchievementID = MockedAchievements[1].Id,
                    Achievement = MockedAchievements[1]
                }
            };
        }

        public static class Blog
        {
            public static readonly List<Post> MockedPosts = new List<Post>
            {
                new Post
                {
                    PostID = 0,
                    Title = "Title 0",
                    Body = "Body 0",
                    Status = PostStatus.APPROVED,
                    AuthorID = Users.MockedUsers[0].Id,
                    Author = Users.MockedUsers[0]
                },
                new Post
                {
                    PostID = 200,
                    Title = "Title 200",
                    Body = "Body 200",
                    Status = PostStatus.PENDING,
                    AuthorID = Users.MockedUsers[1].Id,
                    Author = Users.MockedUsers[1]
                }
            };
        }

        public static class UserLineups
        {
            public static readonly List<UserLineup> MockedUserLineups = new List<UserLineup>
            {
                new UserLineup
                {
                    ID = 0,
                    UserID = Users.MockedUsers[0].Id,
                    User = Users.MockedUsers[0],
                    Date = new DateTime(2020, 04, 20),
                    PgID = Players.MockedPlayers[0].PlayerID,
                    Pg = Players.MockedPlayers[0],
                    SgID = Players.MockedPlayers[1].PlayerID,
                    Sg = Players.MockedPlayers[1],
                    SfID = Players.MockedPlayers[2].PlayerID,
                    Sf = Players.MockedPlayers[2],
                    PfID = Players.MockedPlayers[3].PlayerID,
                    Pf = Players.MockedPlayers[3],
                    CID = Players.MockedPlayers[4].PlayerID,
                    C = Players.MockedPlayers[4]
                },
                new UserLineup
                {
                    ID = 200,
                    UserID = Users.MockedUsers[1].Id,
                    User = Users.MockedUsers[1],
                    Date = new DateTime(2020, 04, 20),
                    PgID = Players.MockedPlayers[0].PlayerID,
                    Pg = Players.MockedPlayers[0],
                    SgID = Players.MockedPlayers[1].PlayerID,
                    Sg = Players.MockedPlayers[1],
                    SfID = Players.MockedPlayers[2].PlayerID,
                    Sf = Players.MockedPlayers[2],
                    PfID = Players.MockedPlayers[3].PlayerID,
                    Pf = Players.MockedPlayers[3],
                    CID = Players.MockedPlayers[5].PlayerID,
                    C = Players.MockedPlayers[5]
                }
            };
        }
        
        public static class Push
        {
            public static readonly List<PushSubscription> MockedSubscriptions = new List<PushSubscription>
            {
                new PushSubscription
                {
                    Auth = "123",
                    Endpoint = "yyy",
                    P256Dh = "iii",
                    UserID = "xxx"
                },
                new PushSubscription
                {
                    Auth = "456",
                    Endpoint = "yyy",
                    P256Dh = "ppp",
                    UserID = "yyy"
                }
            };
        }
        
        public static class Stats
        {
            public static readonly List<Models.Stats> MockedStats = new List<Models.Stats>
            {
                new Models.Stats
                {
                    StatsID = 1991,
                    PlayerID = 123,
                    Player = Players.MockedPlayers[0],
                    GS = 20,
                    Score = "120-110"
                },
                new Models.Stats
                {
                    StatsID = 2882,
                    PlayerID = 123,
                    Player = Players.MockedPlayers[0],
                    GS = 40,
                    Score = "120-110"
                },
                new Models.Stats
                {
                    StatsID = 3773,
                    PlayerID = 123,
                    Player = Players.MockedPlayers[0],
                    GS = 30,
                    Score = "120-110"
                },
            };
        }
        
        public static class Games
        {
            public static List<Game> MockedGames = new List<Game>
            {
                new Game
                {
                    Id = 1001,
                    Date = new DateTime(2020, 05, 05)
                },
                new Game
                {
                    Id = 1991,
                    Date = new DateTime(2020, 05, 06)
                },
                new Game
                {
                    Id = 1881,
                    Date = new DateTime(2020, 05, 07)
                }
            };
        }
    }
}