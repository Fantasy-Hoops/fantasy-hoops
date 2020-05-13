import moment from "moment";

const url = "https://fantasyhoops.org";

export const Meta = {
    TITLE: "Fantasy Hoops | NBA Fantasy Basketball Game",
    DESCRIPTION: "Build your daily NBA lineup and compete with others. Prices, injuries, news, leaderboards are based on real stats."
};

export const Canonicals = {
    MAIN: `${url}/`,
    LINEUP: `${url}/lineup`,
    INJURIES: `${url}/injuries`,
    NEWS: `${url}/news`,
    LEADERBOARDS: `${url}/leaderboards`,
    USERS_LEADERBOARD: `${url}/leaderboard/users`,
    PLAYERS_LEADERBOARD: `${url}/leaderboard/players`,
    SEASON_LEADERBOARD: `${url}/leaderboard/season`,
    SELECTED_PLAYERS_LEADERBOARD: `${url}/leaderboard/selected/players`,
    BEST_LINEUPS_LEADERBOARD: `${url}/leaderboard/best-lineups`,
    BLOG: `${url}/blog`,
    USERS: `${url}/users`,
    PROFILE: `${url}/profile`,
    NOTIFICATIONS: `${url}/notifications`,
    LINEUPS_HISTORY: `${url}/history`,
    LOGIN: `${url}/login`,
    REGISTER: `${url}/register`,
    ACHIEVEMENTS: `${url}/achievements`,
    TOURNAMENTS: `${url}/tournaments`,
    TOURNAMENTS_CREATE: `${url}/tournaments/create`,
    TOURNAMENTS_SUMMARY: `${url}/tournaments/summary`,
    TOURNAMENTS_INVITATIONS: `${url}/tournaments/invitations`,
};

export const camelCaseToSentenceCase = (text) => {
    const result = text.replace(/([A-Z])/g, " $1");
    return result.charAt(0).toUpperCase() + result.slice(1);
};

export const getTournamentType = (value) => {
    switch (value) {
        case 0:
            return 'One For All';
        case 1:
            return 'Matchups';
        default:
            return "";
    }
};

export const TOURNAMENT_DATE_FORMAT = 'MMMM Do YYYY, h:mm:ss a';

export const TournamentStatus = {
    CREATED: 0,
    ACTIVE: 1,
    FINISHED: 2,
    CANCELLED: 3
};

export const ContestState = {
    FINISHED: -1,
    ACTIVE: 0,
    NOT_STARTED: 1
};

export function getContestState(contest) {
    if (contest.isFinished) {
        return ContestState.FINISHED;
    }
    
    const isStarted = moment(contest.contestStart).isBefore();
    const isFinished = !moment(contest.contestEnd).isAfter();
    
    if (isStarted && isFinished) {
        return ContestState.FINISHED;
    }
    
    if (isStarted && !isFinished) {
        return ContestState.ACTIVE;
    }
    
    if (!isStarted && !isFinished) {
        return ContestState.NOT_STARTED;
    }
}
