import {isAuth} from "./auth";

const isLoggedIn = isAuth();
export const getLocationSlug = pathname => {
    let slug = pathname.substring(1);
    const parsedSlug = slug.substring(0, slug.indexOf("/"));
        
     return parsedSlug.length === 0
    ? slug
    : parsedSlug;
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
    achievements: isLoggedIn ? 2 : 1,
    injuries: isLoggedIn ? 3 : 2,
    news: isLoggedIn ? 4 : 3,
    users: isLoggedIn ? 5 : 4,
    blog: isLoggedIn ? 6 : 4
};
