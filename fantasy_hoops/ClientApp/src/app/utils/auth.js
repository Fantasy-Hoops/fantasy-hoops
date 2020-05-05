import decode from 'jwt-decode';
import _ from 'lodash';

const ROLE_KEY = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
let AUTH_TOKEN = null;

export const parse = () => {
    if (AUTH_TOKEN) {
        return AUTH_TOKEN;
    }

    const token = localStorage.getItem('accessToken');
    try {
        const decoded = decode(token);
        if (decoded.exp > Date.now() / 1000) {
            AUTH_TOKEN = decoded;
            return decoded;
        }

        localStorage.removeItem('accessToken');
        window.location.reload();
        return null;
    } catch (err) {
        return null;
    }
};

export const isAuth = () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
        return false;
    }

    return parse();
};

export const roles = () => {
    if (!AUTH_TOKEN || !AUTH_TOKEN[ROLE_KEY]) {
        parse();
    }
    
    return Array.isArray(AUTH_TOKEN[ROLE_KEY])
        ? AUTH_TOKEN[ROLE_KEY]
        : Array.of(AUTH_TOKEN[ROLE_KEY]).filter(item => item);
}

export const isAdmin = (userRoles) => {
    if (userRoles) {
        return userRoles.map(role => role.toLowerCase()).indexOf('admin') > -1;
    }
    
    if (!isAuth() || _.isEmpty(roles())) {
        return false;
    }
    
    return roles().map(role => role.toLowerCase()).indexOf('admin') > -1;
}

export const isCreator = (userRoles) => {
    if (userRoles) {
        return userRoles.map(role => role.toLowerCase()).indexOf('creator') > -1;
    }

    if (!isAuth() || _.isEmpty(roles())) {
        return false;
    }
    
    return roles().map(role => role.toLowerCase()).indexOf('creator') > -1;
}
