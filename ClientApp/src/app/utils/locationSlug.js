import Routes from "../routes/routes";

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
    leaderboards: 1,
    leaderboard: 1,
    injuries: 2,
    news: 3,
    users: 4,
    blog: 5
};
