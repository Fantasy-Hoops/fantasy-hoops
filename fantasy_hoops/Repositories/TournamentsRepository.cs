using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Database;
using fantasy_hoops.Dtos;
using fantasy_hoops.Helpers;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Tournaments;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories
{
    public class TournamentsRepository : ITournamentsRepository
    {
        private readonly GameContext _context;
        private readonly ILineupRepository _lineupRepository;

        public TournamentsRepository(GameContext gameContext = null, ILineupRepository lineupRepository = null)
        {
            _context = gameContext ?? new GameContext();
            _lineupRepository = lineupRepository ?? new LineupRepository();
        }

        public List<TournamentTypeDto> GetTournamentTypes()
        {
            return Tournament.TournamentType.Values()
                .Select(type => new TournamentTypeDto
                {
                    Id = type.GetId(),
                    Name = type.ToString()
                })
                .ToList();
        }

        public Tournament.TournamentType GetTournamentTypeById(int id)
        {
            return Tournament.TournamentType.Values().FirstOrDefault(type => type.GetId() == id);
        }

        public List<DateTime> GetUpcomingStartDates()
        {
            if (bool.Parse(Startup.Configuration["UseMock"]))
            {
                return Mocks.Tournaments.StartDates;
            }

            return _context.Games
                .AsEnumerable()
                .Where(game => game.Date.HasValue && game.Date.Value.DayOfWeek != DayOfWeek.Sunday)
                .ToList()
                .GroupBy(game => CommonFunctions.Instance.GetIso8601WeekOfYear(game.Date.Value))
                .Select(group => group.Min(game => game.Date.Value))
                .Where(date => date > CommonFunctions.Instance.EtcNow())
                .OrderBy(date => date)
                .ToList();
        }

        public DateTime GetLastEndDate()
        {
            if (bool.Parse(Startup.Configuration["UseMock"]))
            {
                return Mocks.Tournaments.StartDates.Max();
            }

            return _context.Games
                .Where(game => game.Date.HasValue)
                .Max(tournament => tournament.Date.Value);
        }

        public Tournament GetTournamentById(string tournamentId)
        {
            return _context.Tournaments
                .FirstOrDefault(tournament => tournament.Id.Equals(tournamentId));
        }

        public TournamentDetailsDto GetTournamentDetails(string tournamentId)
        {
            return GetTournamentDetails(null, tournamentId);
        }

        public TournamentDetailsDto GetTournamentDetails(string userId, string tournamentId)
        {
            TournamentDetailsDto tournamentDetails = new TournamentDetailsDto();

            Tournament tournament = GetTournamentById(tournamentId);
            UserDto tournamentCreator = _context.Users
                .Where(user => user.Id.Equals(tournament.CreatorID))
                .Select(user => new UserDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    AvatarUrl = user.AvatarURL
                }).FirstOrDefault();
            tournamentDetails.Id = tournamentId;
            tournamentDetails.Status = tournament.Status;
            tournamentDetails.StartDate = tournament.StartDate;
            tournamentDetails.EndDate = tournament.EndDate;
            tournamentDetails.Creator = tournamentCreator;
            tournamentDetails.Type = tournament.Type;
            tournamentDetails.TypeName = ((Tournament.TournamentType) tournament.Type).ToString();
            tournamentDetails.ImageURL = tournament.ImageURL;
            tournamentDetails.Title = tournament.Title;
            tournamentDetails.Description = tournament.Description;
            tournamentDetails.Contests = _context.Contests
                .Where(contest => contest.TournamentId.Equals(tournamentId))
                .Include(contest => contest.Matchups)
                .Include(contest => contest.Winner)
                .Select(contest => new ContestDto
                {
                    Id = contest.Id,
                    TournamentId = tournamentId,
                    ContestNumber = contest.ContestNumber,
                    ContestStart = contest.ContestStart,
                    ContestEnd = contest.ContestEnd,
                    Winner = contest.Winner != null
                        ? new TournamentUserDto
                        {
                            UserId = contest.WinnerId,
                            Username = contest.Winner.UserName,
                            AvatarUrl = contest.Winner.AvatarURL,
                            TournamentId = contest.TournamentId,
                        }
                        : null,
                    IsFinished = contest.IsFinished,
                    Matchups = contest.Matchups
                        .Select(pair => new MatchupPairDto
                        {
                            FirstUser = new TournamentUserDto
                            {
                                UserId = pair.FirstUserID,
                                Username = pair.FirstUser.UserName,
                                AvatarUrl = pair.FirstUser.AvatarURL,
                            },
                            FirstUserScore = pair.FirstUserScore,
                            SecondUser = new TournamentUserDto
                            {
                                UserId = pair.SecondUserID,
                                Username = pair.SecondUser.UserName,
                                AvatarUrl = pair.SecondUser.AvatarURL
                            },
                            SecondUserScore = pair.SecondUserScore
                        })
                        .OrderByDescending(matchup => matchup.FirstUserScore)
                        .ToList()
                })
                .OrderBy(contest => contest.ContestStart)
                .ToList();
            tournamentDetails.Standings = _context.TournamentUsers
                .Where(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId))
                .Include(tournamentUser => tournamentUser.User)
                .Select(tournamentUser => new TournamentUserDto
                {
                    TournamentId = tournamentUser.TournamentID,
                    UserId = tournamentUser.UserID,
                    Username = tournamentUser.User.UserName,
                    AvatarUrl = tournamentUser.User.AvatarURL,
                    W = tournamentUser.Wins,
                    L = tournamentUser.Losses,
                    Points = tournamentUser.Points,
                    IsEliminated = tournamentUser.IsEliminated
                }).OrderByDescending(tournamentUser =>
                    (Tournament.TournamentType) tournament.Type == Tournament.TournamentType.MATCHUPS
                        ? tournamentUser.W - tournamentUser.L
                        : tournamentUser.Points)
                .ThenBy(tournamentUser => tournamentUser.IsEliminated)
                .ToList()
                .Select((tournamentUser, index) => new KeyValuePair<int, TournamentUserDto>(index, tournamentUser))
                .Select(record => new TournamentUserDto
                {
                    TournamentId = record.Value.TournamentId,
                    UserId = record.Value.UserId,
                    Position = record.Key + 1,
                    Username = record.Value.Username,
                    AvatarUrl = record.Value.AvatarUrl,
                    W = record.Value.W,
                    L = record.Value.L,
                    Points = record.Value.Points,
                    IsEliminated = record.Value.IsEliminated
                })
                .OrderBy(tournamentUser => tournamentUser.Position)
                .ToList();

            if (userId != null)
            {
                tournamentDetails.IsCreator =
                    _context.Tournaments.Any(t => t.CreatorID.Equals(userId) && t.Id.Equals(tournament.Id));
                tournamentDetails.CurrentLineup = _lineupRepository.GetUserCurrentLineup(userId);
                tournamentDetails.AcceptedInvite =
                    IsUserInvited(userId, tournamentId) && IsUserInTournament(userId, tournamentId);
            }

            tournamentDetails.Winner = _context.Users
                .Select(user => new UserDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    AvatarUrl = user.AvatarURL
                })
                .FirstOrDefault(user => user.UserId.Equals(tournament.WinnerID));

            return tournamentDetails;
        }

        public List<TournamentDetailsDto> GetAllTournamentsDetails()
        {
            return _context.Tournaments
                .ToList()
                .Select(tournament => GetTournamentDetails(tournament.CreatorID, tournament.Id))
                .ToList();
        }

        public Dictionary<string, List<TournamentDto>> GetUserTournaments(string userId)
        {
            List<TournamentDto> createdTournaments =
                _context.Tournaments
                    .Where(tournament => tournament.CreatorID.Equals(userId))
                    .Select(tournament => new TournamentDto
                    {
                        Id = tournament.Id,
                        Status = tournament.Status,
                        Type = tournament.Type,
                        TypeName = ((Tournament.TournamentType) tournament.Type).ToString(),
                        StartDate = tournament.StartDate,
                        EndDate = tournament.EndDate,
                        Title = tournament.Title,
                        Description = tournament.Description,
                        ImageURL = tournament.ImageURL,
                        Entrants = tournament.Entrants,
                        Contests = tournament.Contests,
                        DroppedContests = tournament.DroppedContests,
                        Winner = tournament.WinnerID != null
                            ? _context.Users
                                .Where(user => user.Id.Equals(tournament.WinnerID))
                                .Select(user => new UserDto
                                {
                                    UserId = user.Id,
                                    Username = user.UserName,
                                    AvatarUrl = user.AvatarURL
                                })
                                .FirstOrDefault()
                            : null
                    })
                    .ToList();
            List<TournamentDto> joinedTournaments =
                _context.TournamentUsers
                    .Include(tournamentUser => tournamentUser.Tournament)
                    .Where(tournamentUser => tournamentUser.UserID.Equals(userId)
                                             && !tournamentUser.Tournament.CreatorID.Equals(userId))
                    .Select(tournamentUser => tournamentUser.Tournament)
                    .Select(tournament => new TournamentDto
                    {
                        Id = tournament.Id,
                        Status = tournament.Status,
                        Type = tournament.Type,
                        TypeName = ((Tournament.TournamentType) tournament.Type).ToString(),
                        StartDate = tournament.StartDate,
                        EndDate = tournament.EndDate,
                        Title = tournament.Title,
                        Description = tournament.Description,
                        ImageURL = tournament.ImageURL,
                        Entrants = tournament.Entrants,
                        Contests = tournament.Contests,
                        DroppedContests = tournament.DroppedContests,
                        Winner = tournament.WinnerID != null
                            ? _context.Users
                                .Where(user => user.Id.Equals(tournament.WinnerID))
                                .Select(user => new UserDto
                                {
                                    UserId = user.Id,
                                    Username = user.UserName,
                                    AvatarUrl = user.AvatarURL
                                })
                                .FirstOrDefault()
                            : null
                    })
                    .ToList();

            return new Dictionary<string, List<TournamentDto>>
            {
                {"created", createdTournaments},
                {"joined", joinedTournaments}
            };
        }

        public bool CreateTournament(Tournament tournament)
        {
            _context.Tournaments.Add(tournament);

            return _context.SaveChanges() > 0;
        }

        public bool TournamentExists(Tournament tournament)
        {
            return TournamentExists(tournament.Id);
        }

        public bool TournamentExists(string id)
        {
            return _context.Tournaments.Any(tournament => tournament.Id.Equals(id));
        }

        public bool TournamentNameExists(string name)
        {
            return _context.Tournaments.Any(tournament => tournament.Title.ToUpper().Equals(name.ToUpper()));
        }

        public bool IsUserInTournament(User user, string tournamentId)
        {
            return IsUserInTournament(user.Id, tournamentId);
        }

        public bool IsUserInTournament(string userId, Tournament tournament)
        {
            return IsUserInTournament(userId, tournament.Id);
        }

        public bool IsUserInTournament(User user, Tournament tournament)
        {
            return IsUserInTournament(user.Id, tournament.Id);
        }

        public List<Tournament> GetTournamentsForStartDate(DateTime startDate)
        {
            return _context.Tournaments
                .Where(tournament => tournament.StartDate.Date == startDate.Date)
                .ToList();
        }

        public List<Contest> GetTournamentContests(string tournamentId)
        {
            return _context.Contests
                .Where(contest => contest.TournamentId.Equals(tournamentId))
                .ToList();
        }

        public List<ContestDto> GetAllCurrentContests()
        {
            return _context.Contests
                .Where(contest => !contest.IsFinished
                                  && contest.ContestStart < CommonFunctions.Instance.EtcNow()
                                  && contest.ContestEnd > CommonFunctions.Instance.EtcNow())
                .Include(contest => contest.Matchups)
                .Include(contest => contest.Winner)
                .Select(contest => new ContestDto
                {
                    Id = contest.Id,
                    TournamentId = contest.TournamentId,
                    ContestNumber = contest.ContestNumber,
                    ContestStart = contest.ContestStart,
                    ContestEnd = contest.ContestEnd,
                    IsFinished = contest.IsFinished,
                    Winner = new TournamentUserDto
                    {
                        UserId = contest.WinnerId,
                        Username = contest.Winner.UserName,
                        AvatarUrl = contest.Winner.AvatarURL
                    },
                    Matchups = contest.Matchups.Select(contestPair => new MatchupPairDto
                    {
                        FirstUser = new TournamentUserDto
                        {
                            UserId = contestPair.FirstUserID,
                            Username = contestPair.FirstUser.UserName,
                            AvatarUrl = contestPair.FirstUser.AvatarURL
                        },
                        FirstUserScore = contestPair.FirstUserScore,
                        SecondUser = new TournamentUserDto
                        {
                            UserId = contestPair.SecondUserID,
                            Username = contestPair.SecondUser.UserName,
                            AvatarUrl = contestPair.SecondUser.AvatarURL
                        },
                        SecondUserScore = contestPair.SecondUserScore
                    }).ToList()
                }).ToList();
        }

        public bool DeleteTournament(Tournament tournament)
        {
            if (!DeleteTournamentResources(tournament))
            {
                return false;
            }

            _context.Tournaments.Remove(tournament);
            return _context.SaveChanges() != 0;
        }

        public bool DeleteTournament(string tournamentId)
        {
            Tournament tournamentToDelete = _context.Tournaments.Find(tournamentId);
            if (!DeleteTournamentResources(tournamentToDelete))
            {
                return false;
            }

            _context.Tournaments.Remove(tournamentToDelete);
            return _context.SaveChanges() != 0;
        }

        public List<TournamentDto> GetTournamentInvitations(string userId)
        {
            return _context.TournamentInvites
                .Include(invite => invite.Tournament)
                .Where(invite => invite.Status == RequestStatus.PENDING && invite.InvitedUserID.Equals(userId))
                .Select(invite => invite.Tournament)
                .Select(tournament => new TournamentDto
                {
                    Id = tournament.Id,
                    Status = tournament.Status,
                    Type = tournament.Type,
                    TypeName = ((Tournament.TournamentType) tournament.Type).ToString(),
                    StartDate = tournament.StartDate,
                    EndDate = tournament.EndDate,
                    Title = tournament.Title,
                    Description = tournament.Description,
                    ImageURL = tournament.ImageURL,
                    Entrants = tournament.Entrants,
                    Contests = tournament.Contests,
                    DroppedContests = tournament.DroppedContests,
                    Winner = tournament.WinnerID != null
                        ? _context.Users
                            .Where(user => user.Id.Equals(tournament.WinnerID))
                            .Select(user => new UserDto
                            {
                                UserId = user.Id,
                                Username = user.UserName,
                                AvatarUrl = user.AvatarURL
                            })
                            .FirstOrDefault()
                        : null
                })
                .ToList();
        }

        public bool ChangeInvitationStatus(string tournamentId, string userId,
            RequestStatus status = RequestStatus.NO_REQUEST)
        {
            TournamentInvite invitation = _context.TournamentInvites
                .FirstOrDefault(invite =>
                    invite.TournamentID.Equals(tournamentId) && invite.InvitedUserID.Equals(userId));
            if (invitation == null)
            {
                _context.TournamentInvites.Add(new TournamentInvite
                {
                    InvitedUserID = userId,
                    TournamentID = tournamentId,
                    Status = status
                });
                return _context.SaveChanges() != 0;
            }

            invitation.Status = status;
            return _context.SaveChanges() != 0;
        }

        public User SetTournamentUserEliminated(TournamentUserDto tournamentUser)
        {
            TournamentUser dbTournamentUser = GetTournamentUser(tournamentUser.TournamentId, tournamentUser.UserId);
            if (dbTournamentUser == null)
            {
                return null;
            }

            dbTournamentUser.IsEliminated = true;
            _context.SaveChanges();
            return _context.Users.Find(dbTournamentUser.UserID);
        }

        public Contest GetContestById(int contestId)
        {
            return _context.Contests.Find(contestId);
        }

        public void UpdateTournamentUserStats(TournamentUser tournamentUser, int wins, int losses, int points)
        {
            if (tournamentUser == null)
            {
                return;
            }

            tournamentUser.Wins = wins;
            tournamentUser.Losses = losses;
            tournamentUser.Points = points;

            _context.SaveChanges();
        }

        public void RemoveUserMatchup(string userId, int contestId)
        {
            MatchupPair matchupToRemove = _context.TournamentMatchups
                .FirstOrDefault(matchup => matchup.ContestId == contestId && matchup.FirstUserID.Equals(userId)
                                                                          && matchup.SecondUserID.Equals(userId));
            if (matchupToRemove == null)
            {
                return;
            }
            _context.TournamentMatchups.Remove(matchupToRemove);
            _context.SaveChanges();
        }

        public bool UpdateTournament(Tournament tournament, CreateTournamentViewModel model)
        {
            if (model.TournamentTitle != null && model.TournamentDescription != null)
            {
                tournament.Title = model.TournamentTitle;
                tournament.Description = model.TournamentDescription;
                return _context.SaveChanges() != 0;
            }

            return false;
        }

        public List<MatchupPair> GetContestMatchups(int contestId)
        {
            return _context.TournamentMatchups
                .Where(matchup => matchup.ContestId == contestId)
                .ToList();
        }

        private bool DeleteTournamentResources(Tournament tournamentToDelete)
        {
            List<MatchupPair> matchupsToDelete = _context.TournamentMatchups
                .Where(matchup => tournamentToDelete.Id.Equals(matchup.TournamentID)).ToList();
            List<Contest> contestsToDelete = _context.Contests
                .Where(contest => tournamentToDelete.Id.Equals(contest.TournamentId)).ToList();
            List<TournamentInvite> invitesToDelete = _context.TournamentInvites
                .Where(invite => tournamentToDelete.Id.Equals(invite.TournamentID)).ToList();
            List<RequestNotification> requestNotificationsToDelete = _context.RequestNotifications
                .Where(notification => tournamentToDelete.Id.Equals(notification.TournamentId)).ToList();
            List<TournamentUser> tournamentUsersToDelete = _context.TournamentUsers
                .Where(tournamentUser => tournamentToDelete.Id.Equals(tournamentUser.TournamentID)).ToList();


            _context.TournamentMatchups.RemoveRange(matchupsToDelete);
            _context.Contests.RemoveRange(contestsToDelete);
            _context.TournamentInvites.RemoveRange(invitesToDelete);
            _context.Notifications.RemoveRange(requestNotificationsToDelete);
            _context.TournamentUsers.RemoveRange(tournamentUsersToDelete);

            return _context.SaveChanges() != 0;
        }

        public List<String> GetTournamentUsersIds(string tournamentId)
        {
            return _context.TournamentUsers
                .Include(tournamentUser => tournamentUser.User)
                .Where(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId))
                .Select(tournamentUser => tournamentUser.UserID)
                .ToList();
        }

        public TournamentUser GetTournamentUser(string tournamentId, string userId)
        {
            return _context.TournamentUsers.Find(tournamentId, userId);
        }

        public void AddCreatorToTournament(Tournament tournament)
        {
            if (!_context.TournamentUsers.Any(tournamentUser => tournamentUser.UserID.Equals(tournament.CreatorID)
                                                                && tournamentUser.TournamentID.Equals(tournament.Id)))
            {
                _context.TournamentUsers.Add(new TournamentUser
                {
                    UserID = tournament.CreatorID,
                    TournamentID = tournament.Id
                });
                _context.SaveChanges();
            }
        }

        public bool AddUserToTournament(string userId, string tournamentId)
        {
            Tournament tournament = _context.Tournaments.Find(tournamentId);
            int tournamentUsersCount =
                _context.TournamentUsers.Count(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId));
            if (tournament.Entrants == tournamentUsersCount)
            {
                return false;
            }

            _context.TournamentUsers.Add(new TournamentUser
            {
                UserID = userId,
                TournamentID = tournamentId
            });
            return _context.SaveChanges() != 0;
        }

        public bool IsUserInTournament(string userId, string tournamentId)
        {
            bool isTournamentPlayer = _context.TournamentUsers
                .Any(tournamentUser => tournamentUser.TournamentID.Equals(tournamentId)
                                       && tournamentUser.UserID.Equals(userId));
            bool isCreator = _context.Tournaments.Any(tournament => tournament.Id.Equals(tournamentId)
                                                                    && tournament.CreatorID.Equals(userId));

            return isTournamentPlayer || isCreator;
        }

        public bool IsUserInvited(string userId, string tournamentId)
        {
            return _context.TournamentInvites.Any(invite =>
                invite.TournamentID.Equals(tournamentId) && invite.InvitedUserID.Equals(userId));
        }
    }
}