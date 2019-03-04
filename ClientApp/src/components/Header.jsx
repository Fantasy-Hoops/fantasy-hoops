import React, { Component } from 'react';
import { isAuth, parse, logout } from '../utils/auth';
import defaultPhoto from '../content/images/default.png';
import { Notifications } from './Notifications/Notifications';
import Img from 'react-image';
import { loadImage } from '../utils/loadImage';

export class Header extends Component {
  constructor(props) {
    super(props);

    this.state = {
      navHeight: '2.5rem',
      userNotifications: '',
      unreadCount: 0
    };
  }

  async componentWillMount() {
    if (isAuth())
      this.setState({
        avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${parse().id}.png`, defaultPhoto)
      });
  }

  render() {
    // Login button in case user is not signed in
    const login = (
      <ul className="nav navbar-nav ml-auto Header__Login">
        <li className="nav-item">
          <a className="nav-link btn-no-outline" href="/login">Login</a>
        </li>
      </ul>
    );

    // Showing friend requests and profile when player has signed in
    let profile = '';
    if (isAuth()) {
      const user = parse();
      profile = (
        <ul className="nav navbar-nav ml-auto Header__LoggedIn">
          <Notifications />
          <li className="dropdown">
            <a
              className="text-light ml-2 mr-2 nav-link dropdown-toggle no-arrow btn-no-outline"
              id="navbarDropdownMenuLink"
              data-toggle="dropdown"
              aria-haspopup="true"
              aria-expanded="false"
            >
              <Img
                width="36rem"
                alt={user.username}
                src={this.state.avatar}
              />
            </a>
            <ul className="dropdown-menu dropdown-menu-right">
              <h6 className="dropdown-header">Account</h6>
              <li>
                <div className="navbar-login" style={{ width: '20rem' }}>
                  <div className="row">
                    <div className="col-lg-4">
                      <p className="text-center">
                        <a className="btn-no-outline" href='/profile'>
                          <Img
                            width="100rem"
                            height="100rem"
                            alt={user.username}
                            src={this.state.avatar}
                          />
                        </a>
                      </p>
                    </div>
                    <div className="col-lg-8">
                      <a className="btn-no-outline text-dark" href='/profile'><h4 className="text-left"><strong>{user.username}</strong></h4></a>
                      <p className="text-left small">{user.email}</p>
                      <p className="text-left">
                        <a role="button" href={`/profile/${user.username}/edit`} className="btn btn-outline-primary btn-block btn-sm">Edit profile</a>
                      </p>
                    </div>
                  </div>
                </div>
              </li>
              <li className="divider"></li>
              <li>
                <div className="navbar-login navbar-login-session w-100">
                  <div className="row">
                    <div className="col-lg-12">
                      <p>
                        <button type="button" onClick={logout} className="btn btn-outline-danger btn-block">Logout</button>
                      </p>
                    </div>
                  </div>
                </div>
              </li>
            </ul>
          </li>
        </ul>
      );
    }

    return (
      <div style={{ paddingTop: this.state.navHeight }}>
        <nav className="navbar fixed-top navbar-expand-lg navbar-dark bg-primary">
          <a className="navbar-brand btn-no-outline Header__Logo" href="/">
            <img src={require('../../src/content/favicon.ico')} width="40" height="40" alt="Fantasy Hoops" />
            <span className="Header__Title">Fantasy Hoops</span>
          </a>
          <button className="navbar-toggler Header__Burger"
            type="button" data-toggle="collapse"
            data-target="#navbarNavDropdown"
            aria-controls="navbarNavDropdown"
            aria-expanded="false"
            aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNavDropdown">
            <ul className="navbar-nav">
              {isAuth() &&
                <li className="nav-item">
                  <a className="nav-link btn-no-outline" href="/lineup">Lineup</a>
                </li>
              }
              {isAuth() &&
                <li className="nav-item dropdown">
                  <a
                    className="nav-link dropdown-toggle btn-no-outline"
                    id="navbarDropdownMenuLink"
                    data-toggle="dropdown"
                    aria-haspopup="true"
                    aria-expanded="false">
                    Leaderboards</a>
                  <div className="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
                    <a className="dropdown-item" href="/leaderboard/users">Top Users</a>
                    <a className="dropdown-item" href="/leaderboard/players">Top NBA Players</a>
                    <a className="dropdown-item" href="/leaderboard/season">Top Season Performers</a>
                  </div>
                </li>
              }
              <li className="nav-item">
                <a className="nav-link btn-no-outline" href="/injuries">Injuries</a>
              </li>
              <li className="nav-item">
                <a className="nav-link btn-no-outline" href="/news">News</a>
              </li>
              {isAuth() &&
                <li className="nav-item">
                  <a className="nav-link btn-no-outline" href="/users">Users</a>
                </li>
              }
            </ul>
          </div>
          {isAuth() ? profile : login}
        </nav>
      </div>
    );
  }
}