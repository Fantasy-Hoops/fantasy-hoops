import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import { parse } from '../../utils/auth';
import { handleErrors } from '../../utils/errors';
import { NotificationCard } from './NotificationCard';
import defaultPhoto from '../../../content/images/default.png';
import gameLogo from '../../../content/favicon.ico';

const user = parse();

export class Notifications extends Component {
  constructor(props) {
    super(props);
    this.readAll = this.readAll.bind(this);

    this.state = {
      userNotifications: '',
      unreadCount: 0
    };
  }

  async componentDidMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/${user.id}`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          userNotifications: res,
          unreadCount: res.filter(n => n.readStatus === false).length
        });
      });
  }

  getNotifications() {
    const { userNotifications } = this.state;
    if (userNotifications.length < 1) { return <a className="dropdown-item cursor-default text-center">No notifications</a>; }
    return _.slice(userNotifications, 0, 4)
      .map((notification) => {
        if (notification.score) {
          const text = (
            <span>
              Your lineup scored
              {' '}
              <span className="text-dark font-weight-bold">
                {notification.score.toFixed(1)}
                {' '}
                FP
              </span>
            </span>
          );

          return (
            <NotificationCard
              key={shortid()}
              notification={notification}
              imageSrc={[gameLogo]}
              title="The game has finished!"
              text={text}
              link="/leaderboard/users"
            />
          );
        } if (notification.friend) {
          const text = (
            <span>
              {notification.requestMessage}
            </span>
          );

          return (
            <NotificationCard
              key={shortid()}
              notification={notification}
              title={notification.friend.userName}
              imageSrc={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${notification.friendID}.png`, defaultPhoto]}
              text={text}
              link={`/profile/${notification.friend.userName}`}
            />
          );
        } if (notification.player) {
          const title = `${notification.player.abbrName} is ${notification.injuryStatus.toLowerCase()}`;

          return (
            <NotificationCard
              key={shortid()}
              notification={notification}
              title={title}
              circleImage
              imageSrc={[
                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${notification.player.nbaID}.png`,
                require(`../../../content/images/positions/${notification.player.position.toLowerCase()}.png`)
              ]}
              imageClass="InjuryCard__Image"
              text={notification.injuryDescription}
              link="/lineup"
            />
          );
        }
        return <div />;
      });
  }

  async readAll(e) {
    e.preventDefault();
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/readall/${user.id}`, {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      }
    })
      .then(res => handleErrors(res))
      .then(res => res.text());

    fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/${user.id}`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          userNotifications: res,
          unreadCount: res.filter(n => n.readStatus === false).length
        });
      });
  }


  render() {
    const { unreadCount } = this.state;

    const badge = unreadCount > 0
      ? (
        <span
          className="badge badge-danger"
          style={{ fontSize: '0.8rem', position: 'absolute', marginLeft: '-0.6rem' }}
        >
          {unreadCount}
        </span>
      )
      : '';
    const notifications = this.getNotifications();
    return (
      <li className="dropdown">
        <a
          className="fa fa-bell text-light mt-1 mr-1 ml-3 nav-link dropdown-toggle no-arrow btn-no-outline"
          id="navbarDropdownMenuLink"
          data-toggle="dropdown"
          aria-haspopup="true"
          aria-expanded="false"
          style={{ fontSize: '2rem' }}
        >
          {badge}
        </a>
        <div className="dropdown-menu dropdown-menu-right" aria-labelledby="navbarDropdownMenuLink" style={{ width: '18rem' }}>
          <h6 className="dropdown-header">
            Notifications
            <a
              role="link"
              tabIndex="-1"
              onClick={this.readAll}
              onKeyDown={this.readAll}
              className="position-absolute btn-no-outline"
              style={{ right: '1rem' }}
            >
              Mark All as Read
            </a>
          </h6>
          <div style={{ marginBottom: '-0.5rem' }}>
            {notifications}
          </div>
          <h6 className="dropdown-header text-center mt-2" style={{ height: '1.5rem' }}>
            <Link
              className="btn-no-outline"
              to="/notifications"
            >
              See all
            </Link>
          </h6>
        </div>
      </li>
    );
  }
}

export default Notifications;
