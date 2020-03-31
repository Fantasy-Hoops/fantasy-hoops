import {isAuth} from "./auth";

const isLoggedIn = isAuth();
export const getLocationSlug = pathname => {
    let slug = pathname.substring(1);
    const parsedSlug = slug.substring(0, slug.indexOf("/"));
        
     return parsedSlug.length === 0
    ? slug
    : parsedSlug;
};

export const getLastLocationSlug = pathname => {
    return pathname.substring(pathname.lastIndexOf('/') + 1);
};

export const getLocationEnumValue = pathname => {
    const slug = getLocationSlug(pathname);
    if (Route[slug] == null) {
        return false;
    }
    
    return Route[slug];
};

export const Route = {
    lineup: 0,
    leaderboards: isLoggedIn ? 1 : 0,
    leaderboard: isLoggedIn ? 1 : 0,
    tournaments: isLoggedIn ? 2 : 1,
    achievements: isLoggedIn ? 3 : 2,
    injuries: isLoggedIn ? 4 : 3,
    news: isLoggedIn ? 5 : 4,
    users: isLoggedIn ? 6 : 5,
    blog: isLoggedIn ? 7 : 6
};
