import React from 'react';
import PrivateRoute, {ClassBasedPrivateRoute} from '../components/Authentication/PrivateRoute';
import Routes from './routes.js';
import {Error} from '../components/Error';
import {Switch, Route} from "react-router-dom";

const Main = React.lazy(() => import('../components/Main'));
const Login = React.lazy(() => import('../components/Authentication/LoginPage'));
const Register = React.lazy(() => import('../components/Authentication/Registration'));
const UserProfile = React.lazy(() => import('../components/Profile/UserProfile'));
const Lineup = React.lazy(() => import('../components/Lineup/Lineup'));
const InjuriesFeed = React.lazy(() => import('../containers/InjuriesFeedContainer'));
const NewsFeed = React.lazy(() => import('../containers/NewsFeedContainer'));
const UserLeaderboard = React.lazy(() => import('../components/Leaderboard/Users/Leaderboard'));
const PlayerLeaderboard = React.lazy(() => import('../components/Leaderboard/Players/Leaderboard'));
const SelectedLeaderboard = React.lazy(() => import('../components/Leaderboard/Selected/Leaderboard'));
const SeasonLeaderboard = React.lazy(() => import('../components/Leaderboard/Season/Leaderboard'));
const BestLineupsLeaderboard = React.lazy(() => import('../components/Leaderboard/BestLineups/BestLineupsLeaderboard'));
const UserPool = React.lazy(() => import('../containers/UserPoolContainer'));
const AllNotificationsPage = React.lazy(() => import('../components/Notifications/AllNotificationsPage'));
const LineupHistory = React.lazy(() => import('../components/Profile/LineupHistoryPage'));
const Blog = React.lazy(() => import('../containers/BlogContainer'));
const Leaderboards = React.lazy(() => import('../components/Leaderboard/Leaderboards'));
const Achievements = React.lazy(() => import('../components/Achievements/AchievementsPage'));
const TournamentsPage = React.lazy(() => import('../components/Tournaments/TournamentsPage'));
const CreateTournament = React.lazy(() => import('../components/Tournaments/CreateTournament/CreateTournament'));
const TournamentDetails = React.lazy(() => import('../components/Tournaments/TournamentDetails/TournamentDetails'));
const TournamentInvitations = React.lazy(() => import('../components/Tournaments/TournamentInvitations'));
const TournamentInvitation = React.lazy(() => import('../components/Tournaments/TournamentInvitation'));
const AdminPanel = React.lazy(() => import('../components/Admin/AdminPanel'));

export default function AppRoutes() {
    return (
        <React.Suspense fallback={<div>loading...</div>}>
            <Switch>
                <Route exact path={Routes.MAIN}>
                    <Main/>
                </Route>
                <Route exact path={Routes.LOGIN}>
                    <Login/>
                </Route>
                <Route exact path={Routes.REGISTER}>
                    <Register/>
                </Route>
                <ClassBasedPrivateRoute path={`${Routes.PROFILE}/:name?/:edit?`} component={UserProfile}/>
                <PrivateRoute path={Routes.LINEUP}>
                    <Lineup/>
                </PrivateRoute>
                <Route path={Routes.INJURIES}>
                    <InjuriesFeed/>
                </Route>
                <Route path={Routes.NEWS}>
                    <NewsFeed/>
                </Route>
                <PrivateRoute exact path={Routes.LEADERBOARD_USERS}>
                    <UserLeaderboard/>
                </PrivateRoute>
                <Route exact path={Routes.LEADERBOARD_PLAYERS}>
                    <PlayerLeaderboard/>
                </Route>
                <Route exact path={Routes.LEADERBOARD_SELECTED}>
                    <SelectedLeaderboard/>
                </Route>
                <PrivateRoute exact path={Routes.LEADERBOARD_SEASON}>
                    <SeasonLeaderboard/>
                </PrivateRoute>
                <Route exact path={Routes.LEADERBOARD_BEST_LINEUPS}>
                    <BestLineupsLeaderboard/>
                </Route>
                <PrivateRoute path={Routes.USER_POOL}>
                    <UserPool/>
                </PrivateRoute>
                <PrivateRoute path={Routes.ALL_NOTIFICATIONS}>
                    <AllNotificationsPage/>
                </PrivateRoute>
                <PrivateRoute path={Routes.LINEUP_HISTORY}>
                    <LineupHistory/>
                </PrivateRoute>
                <Route exact path={Routes.BLOG}>
                    <Blog/>
                </Route>
                <Route exact path={Routes.LEADERBOARDS}>
                    <Leaderboards/>
                </Route>
                <Route exact path={Routes.ACHIEVEMENTS}>
                    <Achievements/>
                </Route>
                <PrivateRoute exact path={Routes.TOURNAMENTS}>
                    <TournamentsPage/>
                </PrivateRoute>
                <PrivateRoute exact path={Routes.TOURNAMENTS_CREATE}>
                    <CreateTournament />
                </PrivateRoute>
                <PrivateRoute exact path={`${Routes.TOURNAMENTS_SUMMARY}/:id`}>
                    <TournamentDetails />
                </PrivateRoute>
                <PrivateRoute exact path={Routes.TOURNAMENT_INVITATIONS}>
                    <TournamentInvitations />
                </PrivateRoute>
                <PrivateRoute exact path={`${Routes.TOURNAMENT_INVITATIONS}/:id`}>
                    <TournamentInvitation />
                </PrivateRoute>
                <PrivateRoute exact path={Routes.ADMIN}>
                    <AdminPanel />
                </PrivateRoute>
                <Route render={() => <Error status={404} message="Page not found"/>}/>
            </Switch>
        </React.Suspense>
    );
}
