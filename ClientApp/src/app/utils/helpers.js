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
    BLOG: `${url}/blog`,
    USERS: `${url}/users`,
    PROFILE: `${url}/profile`,
    NOTIFICATIONS: `${url}/notifications`,
    LINEUPS_HISTORY: `${url}/history`,
    LOGIN: `${url}/login`,
    REGISTER: `${url}/register`,
    ACHIEVEMENTS: `${url}/achievements`,
    TOURNAMENTS: `${url}/tournaments`,
    TOURNAMENTS_CREATE: `${url}/tournaments/create`
};

export const camelCaseToSentenceCase = (text) => {
    const result = text.replace(/([A-Z])/g, " $1");
    return result.charAt(0).toUpperCase() + result.slice(1);
};

export const getTournamentType = (value) => {
    switch (value) {
        case 1:
            return 'One For All';
        case 2:
            return 'Matchups';
        default:
            return "";
    }
};

export const TOURNAMENT_DATE_FORMAT = 'MMMM Do YYYY, h:mm:ss a';
