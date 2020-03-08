import React from 'react';
import {Route} from 'react-router-dom';
import Routes from "./routes";

export default (
    <Route>
        <Route path={Routes.LINEUP}/>
        <Route path={Routes.INJURIES}/>
        <Route path={Routes.NEWS}/>
        <Route path={Routes.LEADERBOARD_USERS}/>
        <Route path={Routes.LEADERBOARD_PLAYERS}/>
        <Route path={Routes.LEADERBOARD_SELECTED}/>
        <Route path={Routes.LEADERBOARD_SEASON}/>
        <Route path={Routes.USER_POOL}/>
        <Route path={Routes.BLOG}/>
        <Route path={Routes.LEADERBOARDS}/>
    </Route>
);