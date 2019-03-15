import React, { Component } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import { parse } from '../../utils/auth';
import { NotificationCard } from './NotificationCard';
import { Loader } from '../Loader';
import { EmptyJordan } from '../EmptyJordan';
import defaultPhoto from '../../content/images/default.png';
import gameLogo from '../../content/favicon.ico';


const LOAD_COUNT = 5;
const user = parse();

export class AllNotificationsPage extends Component {
  constructor(props) {
    super(props);
    this.loadMore = this.loadMore.bind(this);

    this.state = {
      loadCounter: 0,
      userNotifications: [],
      loader: true
    };
  }

  async componentDidMount() {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/${user.id}?count=10`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          userNotifications: res,
          loader: false
        });
      });
  }

  getNotifications() {
    const { loader, userNotifications } = this.state;
    if (userNotifications.length < 1 && !loader) {
      return (
        <div className="p-5">
          <EmptyJordan message="Such empty..." />
        </div>
      );
    }
    return _.slice(userNotifications)
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
              link="/profile"
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
          const title = `${notification.player.firstName[0]}. ${notification.player.lastName} is ${notification.injuryStatus.toLowerCase()}`;

          return (
            <NotificationCard
              key={shortid()}
              notification={notification}
              title={title}
              circleImage
              imageSrc={[
                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${notification.player.nbaID}.png`,
                require(`../../content/images/positions/${notification.player.position.toLowerCase()}.png`)
              ]}
              imageClass="InjuryCard__Image"
              text={notification.injuryDescription}
              link="/lineup"
            />
          );
        } return <div />;
      });
  }

  async loadMore() {
    const { loadCounter, userNotifications } = this.state;
    this.setState({
      loader: true,
      loadCounter: loadCounter + 1
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/${user.id}?start=${userNotifications.length}&count=${LOAD_COUNT}`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          userNotifications: userNotifications.concat(res),
          loader: false
        });
      });
  }

  render() {
    const { loader, loadCounter, userNotifications } = this.state;
    const notifications = this.getNotifications();
    const btn = loadCounter * LOAD_COUNT + 10 > userNotifications.length
      ? ''
      : <button type="button" className="btn btn-primary mt-2" onClick={this.loadMore}>See more</button>;
    return (
      <div className="container bg-light pt-4">
        <h3 className="text-center">
          <i className="fa fa-bell" />
          {' '}
          User notifications
        </h3>
        <div className="mt-3 mb-3 mx-auto" style={{ width: '60%' }}>
          {notifications}
        </div>
        <div className="text-center">
          {!loader ? btn : ''}
        </div>
        <Loader show={loader} />
      </div>
    );
  }
}

export default AllNotificationsPage;
