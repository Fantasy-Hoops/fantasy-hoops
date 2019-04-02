/* eslint-disable jsx-a11y/anchor-is-valid */
import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import Img from 'react-image';
import $ from 'jquery';
import { isAuth, parse } from '../utils/auth';
import defaultPhoto from '../../content/images/default.png';
import { Notifications } from './Notifications/Notifications';
import { loadImage } from '../utils/loadImage';
import { registerPush } from '../utils/push';
import { logout } from '../utils/networkFunctions';
import Routes from '../routes/routes';

export default class Navbar extends Component {
  constructor(props) {
    super(props);

    this.state = {
    };
  }

  async componentDidMount() {
    $('#NavbarLogo').on('click', () => {
      $('.navbar-collapse').removeClass('show');
    });
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
    const { avatar } = this.state;
    // Login button in case user is not signed in
    const login = (
      <ul className="nav navbar-nav ml-auto Header__Login">
        <li className="nav-item">
          <Link className="nav-link btn-no-outline" to={Routes.LOGIN}>Login</Link>
        </li>
      </ul>
    );

    // Showing friend requests and profile when player has signed in
    let profile = '';
    if (isAuth()) {
      const user = parse();
      if (typeof Notification !== 'undefined' && Notification.permission !== 'denied') { registerPush(); }
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
                <div className="navbar-login">
                  <div className="row">
                    <div className="col-lg-4">
                      <p className="text-center">
                        <Link className="btn-no-outline" to={Routes.PROFILE}>
                          <Img
                            width="90rem"
                            height="90rem"
                            alt={user.username}
                            src={avatar}
                          />
                        </Link>
                      </p>
                    </div>
                    <div className="col-lg-8">
                      <Link className="Navbar__user-dropdown--username btn-no-outline text-dark" to="/profile">
                        <p className="text-left">{user.username}</p>
                      </Link>
                      <p className="text-left small">{user.email}</p>
                      <p className="text-left">
                        <Link to={`/profile/${user.username}/edit`} className="btn btn-outline-primary btn-block">Edit profile</Link>
                      </p>
                    </div>
                  </div>
                </div>
              </li>
              <li className="divider" />
              <li>
                <div className="navbar-login navbar-login-session">
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
      <nav className="navbar fixed-top navbar-expand-lg navbar-dark bg-primary">
        {window.location.pathname !== '/'
          ? (
            <Link id="NavbarLogo" className="navbar-brand btn-no-outline Navbar__Logo" to={Routes.MAIN}>
              <img className="Navbar__Icon" src={require('../../content/images/logo.png')} width="35" height="35" alt="FH" />
              <img className="Navbar__Title ml-2 mt-2" src={require('../../content/images/title.png')} height="30" alt="Fantasy Hoops" />
            </Link>
          )
          : null}
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
                  <div><Link className="nav-link btn-no-outline" to={Routes.LINEUP}>Lineup</Link></div>
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
                    {'Leaderboards'}
                  </a>
                  <div className="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
                    <div><Link className="dropdown-item" to={Routes.LEADERBOARD_USERS}>Top Users</Link></div>
                    <div><Link className="dropdown-item" to={Routes.LEADERBOARD_PLAYERS}>Top NBA Players</Link></div>
                    <div><Link className="dropdown-item" to={Routes.LEADERBOARD_SEASON}>Top Season Performers</Link></div>
                    <div><Link className="dropdown-item" to={Routes.LEADERBOARD_SELECTED}>Most Selected  NBA Players</Link></div>
                  </div>
                </li>
              )
            }
            <li className="nav-item">
              <div><Link className="nav-link btn-no-outline" to={Routes.INJURIES}>Injuries</Link></div>
            </li>
            <li className="nav-item">
              <div><Link className="nav-link btn-no-outline" to={Routes.NEWS}>News</Link></div>
            </li>
            {!isAuth()
              && (
                <li className="nav-item">
                  <div><Link className="nav-link btn-no-outline" to={Routes.LEADERBOARD_PLAYERS}>Top NBA Players</Link></div>
                </li>
              )
            }
            {isAuth()
              && (
                <li className="nav-item">
                  <div><Link className="nav-link btn-no-outline" to={Routes.USER_POOL}>Users</Link></div>
                </li>
              )
            }
          </ul>
        </div>
        {isAuth() ? profile : login}
      </nav>
    );
  }
}
