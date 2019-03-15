/* eslint-disable jsx-a11y/anchor-is-valid */
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import Img from 'react-image';
import $ from 'jquery';
import { isAuth, parse, logout } from '../utils/auth';
import defaultPhoto from '../../content/images/default.png';
import { Notifications } from './Notifications/Notifications';
import { loadImage } from '../utils/loadImage';
import { registerPush } from '../utils/push';

export class Navbar extends Component {
  constructor(props) {
    super(props);

    this.state = {
      navHeight: '4.2rem'
    };
  }

  async componentDidMount() {
    $('.navbar-nav>li>div').on('click', () => {
      $('.navbar-collapse').removeClass('show');
    });
    if (isAuth()) {
      this.setState({
        avatar: await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${parse().id}.png`, defaultPhoto)
      });
    }
  }

  render() {
    const { avatar, navHeight } = this.state;
    // Login button in case user is not signed in
    const login = (
      <ul className="nav navbar-nav ml-auto Header__Login">
        <li className="nav-item">
          <Link className="nav-link btn-no-outline" to="/login">Login</Link>
        </li>
      </ul>
    );

    // Showing friend requests and profile when player has signed in
    let profile = '';
    if (isAuth()) {
      const user = parse();
      if (Notification.permission !== 'denied') { registerPush(); }
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
                src={avatar}
              />
            </a>
            <ul className="dropdown-menu dropdown-menu-right">
              <h6 className="dropdown-header">Account</h6>
              <li>
                <div className="navbar-login" style={{ width: '20rem' }}>
                  <div className="row">
                    <div className="col-lg-4">
                      <p className="text-center">
                        <Link className="btn-no-outline" to="/profile">
                          <Img
                            width="100rem"
                            height="100rem"
                            alt={user.username}
                            src={avatar}
                          />
                        </Link>
                      </p>
                    </div>
                    <div className="col-lg-8">
                      <a className="btn-no-outline text-dark" href="/profile"><h4 className="text-left"><strong>{user.username}</strong></h4></a>
                      <p className="text-left small">{user.email}</p>
                      <p className="text-left">
                        <a role="button" href={`/profile/${user.username}/edit`} className="btn btn-outline-primary btn-block btn-sm">Edit profile</a>
                      </p>
                    </div>
                  </div>
                </div>
              </li>
              <li className="divider" />
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
      <div style={{ paddingTop: navHeight }}>
        <nav className="navbar fixed-top navbar-expand-lg navbar-dark bg-primary">
          <Link className="navbar-brand btn-no-outline Header__Logo" to="/">
            <img src={require('../../content/favicon.ico')} width="40" height="40" alt="Fantasy Hoops" />
            <span className="Header__Title">Fantasy Hoops</span>
          </Link>
          <button
            className="navbar-toggler Header__Burger"
            type="button"
            data-toggle="collapse"
            data-target="#navbarNavDropdown"
            aria-controls="navbarNavDropdown"
            aria-expanded="false"
            aria-label="Toggle navigation"
            tabIndex="-1"
          >
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse" id="navbarNavDropdown">
            <ul className="navbar-nav">
              {isAuth()
                && (
                  <li className="nav-item">
                    <div><Link className="nav-link btn-no-outline" to="/lineup">Lineup</Link></div>
                  </li>
                )
              }
              {isAuth()
                && (
                  <li className="nav-item dropdown">
                    <a
                      className="nav-link dropdown-toggle btn-no-outline"
                      id="navbarDropdownMenuLink"
                      data-toggle="dropdown"
                      aria-haspopup="true"
                      aria-expanded="false"
                    >
                      Leaderboards
                    </a>
                    <div className="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
                      <div><Link className="dropdown-item" to="/leaderboard/users">Top Users</Link></div>
                      <div><Link className="dropdown-item" to="/leaderboard/players">Top NBA Players</Link></div>
                      <div><Link className="dropdown-item" to="/leaderboard/season">Top Season Performers</Link></div>
                    </div>
                  </li>
                )
              }
              <li className="nav-item">
                <div><Link className="nav-link btn-no-outline" to="/injuries">Injuries</Link></div>
              </li>
              <li className="nav-item">
                <div><Link className="nav-link btn-no-outline" to="/news">News</Link></div>
              </li>
              {isAuth()
                && (
                  <li className="nav-item">
                    <div><Link className="nav-link btn-no-outline" to="/users">Users</Link></div>
                  </li>
                )
              }
            </ul>
          </div>
          {isAuth() ? profile : login}
        </nav>
      </div>
    );
  }
}

export default connect()(Navbar);
