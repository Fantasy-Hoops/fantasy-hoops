import React from 'react';
import {PrivateRoute} from '../components/Authentication/PrivateRoute';
import {Lineup} from '../components/Lineup/Lineup';
import InjuriesFeed from '../containers/InjuriesFeedContainer';
import {Registration} from '../components/Authentication/Registration';
import {UserProfile} from '../components/Profile/UserProfile';
import LoginPage from '../components/Authentication/LoginPage';
import NewsFeed from '../containers/NewsFeedContainer';
import UserLeaderboard from '../components/Leaderboard/Users/Leaderboard';
import PlayerLeaderboard from '../components/Leaderboard/Players/Leaderboard';
import SelectedLeaderboard from '../components/Leaderboard/Selected/Leaderboard';
import {Leaderboard as SeasonLeaderboard} from '../components/Leaderboard/Season/Leaderboard';
import UserPool from '../containers/UserPoolContainer';
import Main from '../components/Main';
import {AllNotificationsPage} from '../components/Notifications/AllNotificationsPage';
import {LineupHistory} from '../components/Profile/LineupHistoryPage';
import Blog from '../containers/BlogContainer';
import Routes from './routes.js';
import {Error} from '../components/Error';
import Leaderboards from "../components/Leaderboard/Leaderboards";
import {Switch, Route} from "react-router-dom";
import Achievements from "../components/Achievements/AchievementsPage";
import TournamentsPage from "../components/Tournaments/TournamentsPage";
import CreateTournament from "../components/Tournaments/CreateTournament/CreateTournament";

export default () => (
    <Switch>
        <Route exact path={Routes.MAIN} component={Main}/>
        <Route exact path={Routes.LOGIN} component={LoginPage}/>
        <Route exact path={Routes.REGISTER} component={Registration}/>
        <PrivateRoute path={`${Routes.PROFILE}/:name?/:edit?`} component={UserProfile}/>
        <PrivateRoute path={Routes.LINEUP} component={Lineup}/>
        <Route path={Routes.INJURIES} component={InjuriesFeed}/>
        <Route path={Routes.NEWS} component={NewsFeed}/>
        <PrivateRoute exact path={Routes.LEADERBOARD_USERS} component={UserLeaderboard}/>
        <Route exact path={Routes.LEADERBOARD_PLAYERS} component={PlayerLeaderboard}/>
        <Route exact path={Routes.LEADERBOARD_SELECTED} component={SelectedLeaderboard}/>
        <PrivateRoute exact path={Routes.LEADERBOARD_SEASON} component={SeasonLeaderboard}/>
        <PrivateRoute path={Routes.USER_POOL} component={UserPool}/>
        <PrivateRoute path={Routes.ALL_NOTIFICATIONS} component={AllNotificationsPage}/>
        <PrivateRoute path={Routes.LINEUP_HISTORY} component={LineupHistory}/>
        <Route exact path={Routes.BLOG} component={Blog}/>
        <Route exact path={Routes.LEADERBOARDS} component={Leaderboards}/>
        <Route exact path={Routes.ACHIEVEMENTS} component={Achievements}/>
        <PrivateRoute exact path={Routes.TOURNAMENTS} component={TournamentsPage}/>
        <PrivateRoute exact path={Routes.TOURNAMENTS_CREATE} component={CreateTournament}/>
        <Route render={() => <Error status={404} message="Page not found"/>}/>
    </Switch>
);
