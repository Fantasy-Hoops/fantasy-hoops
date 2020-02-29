import React from 'react';
import {Route, Switch} from 'react-router';
import Routes from "./routes";

export default (
    <Route>
        <Route path={Routes.LOGIN}/>
        <Route path={Routes.REGISTER}/>
        <Route path={`${Routes.PROFILE}/:name?`}/>
        <Route path={Routes.LINEUP}/>
        <Route path={Routes.INJURIES}/>
        <Route path={Routes.NEWS}/>
        <Route path={Routes.LEADERBOARD_USERS}/>
        <Route path={Routes.LEADERBOARD_PLAYERS}/>
        <Route path={Routes.LEADERBOARD_SELECTED}/>
        <Route path={Routes.LEADERBOARD_SEASON}/>
        <Route path={Routes.USER_POOL}/>
        <Route path={Routes.ALL_NOTIFICATIONS}/>
        <Route path={Routes.LINEUP_HISTORY}/>
        <Route path={Routes.BLOG}/>
        <Route path={Routes.LEADERBOARDS}/>
    </Route>
);