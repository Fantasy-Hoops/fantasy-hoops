import React, {useEffect} from 'react';
import {Route, Switch} from 'react-router';
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
import Routes from './routes';
import {Error} from '../components/Error';
import Header from "../components/Navigation/Header";
import Leaderboards from "../components/Leaderboard/Leaderboards";

function ScrollToTop(props) {
    useEffect(() => {
        if ('scrollRestoration' in history) {
            history.scrollRestoration = 'manual';
        }
        window.scrollTo(0, 0);
    })

    return props.children;
}

export default function configureRoutes() {
    return (
        <ScrollToTop>
            {/*<Route path="/" component={Navbar} />*/}
            <Route path="/" component={Header}/>
            <main>
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
                    <Route render={() => <Error status={404} message="Page not found"/>}/>
                </Switch>
            </main>
        </ScrollToTop>
    );
}
