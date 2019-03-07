import React, { Component } from 'react';
import { Route } from 'react-router';
import { PrivateRoute } from './components/Authentication/PrivateRoute';
import { Lineup } from './components/Lineup/Lineup';
import { Header } from './components/Header';
import { InjuriesFeed } from './components/Injuries/InjuriesFeed';
import { Registration } from './components/Authentication/Registration';
import { UserProfile } from './components/Profile/UserProfile';
import { Login } from './components/Authentication/Login'
import { NewsFeed } from './components/News/NewsFeed';
import { Leaderboard as UserLeaderboard } from './components/Leaderboard/Users/Leaderboard';
import { Leaderboard as PlayerLeaderboard } from './components/Leaderboard/Players/Leaderboard';
import { Leaderboard as SeasonLeaderboard } from './components/Leaderboard/Season/Leaderboard';
import { UserPool } from './components/UserPool';
import { Main } from './components/Main';
import { AllNotificationsPage } from './components/Notifications/AllNotificationsPage';
import { LineupHistory } from './components/Profile/LineupHistoryPage';
import { Push } from './components/Push';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <div>
        <Route path='/' component={Header} />
        <Route exact path='/' component={Main} />
        <Route exact path='/login' component={Login} />
        <Route exact path='/register' component={Registration} />
        <PrivateRoute path='/profile/:name?/:edit?' component={UserProfile} />
        <PrivateRoute path='/lineup' component={Lineup} />
        <Route path='/injuries' component={InjuriesFeed} />
        <Route path='/news' component={NewsFeed} />
        <PrivateRoute exact path='/leaderboard/users' component={UserLeaderboard} />
        <PrivateRoute exact path='/leaderboard/players' component={PlayerLeaderboard} />
        <PrivateRoute exact path='/leaderboard/season' component={SeasonLeaderboard} />
        <PrivateRoute path='/users' component={UserPool} />
        <PrivateRoute path='/notifications' component={AllNotificationsPage} />
        <PrivateRoute path='/history' component={LineupHistory} />
        <PrivateRoute path='/push' component={Push} />
      </div>
    );
  }
}
