import Routes from "../routes/routes";
import {isAuth} from "./auth";

const isLoggedIn = isAuth();
export const getLocationSlug = pathname => {
    let slug = pathname.substring(1);
    const parsedSlug = slug.substring(0, slug.indexOf("/"))
        
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

const Route = {
    lineup: 0,
    leaderboards: isLoggedIn ? 1 : 0,
    leaderboard: isLoggedIn ? 1 : 0,
    injuries: isLoggedIn ? 2 : 1,
    news: isLoggedIn ? 3 : 2,
    users: isLoggedIn ? 4 : 3,
    blog: isLoggedIn ? 5 : 3
};
