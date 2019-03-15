import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Img from 'react-image';
import moment from 'moment';
import { UTCNow } from '../../utils/UTCNow';
import { loadImage } from '../../utils/loadImage';

export class NotificationCard extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {};
    this.readNotification = this.readNotification.bind(this);
  }

  async componentDidMount() {
    this._isMounted = true;

    const { imageSrc, notification } = this.props;

    if (imageSrc) {
      if (this._isMounted) {
        this.setState({
          isRead: notification.readStatus,
          image: await loadImage(...imageSrc)
        });
      }
      return;
    }

    if (this._isMounted) {
      this.setState({
        isRead: notification.readStatus
      });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  async readNotification() {
    const { notification } = this.props;

    if (notification.readStatus) { return; }

    if (this._isMounted) {
      this.setState({
        isRead: true
      });
    }
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/notification/read`, {
      method: 'POST',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(notification)
    });
  }

  render() {
    const {
      imageClass, imageAlt, circleImage, notification, title, text, link
    } = this.props;
    const { image } = this.state;

    const notificationDateUTC = new Date(
      notification.dateCreated
    ).getTime();

    let bgStyle;
    if (circleImage) {
      bgStyle = {
        backgroundColor: notification.player.team.color
      };
    }

    const { isRead } = this.state;
    return (
      <Link to={link} role="button" tabIndex={-1} onClick={this.readNotification} onKeyDown={this.readNotification} className={`card cursor-pointer link mx-auto NotificationCard${!isRead ? '--unread' : ''}`}>
        <div className="card-body NotificationCard__Body">
          <div className={`NotificationCard__Image ml-1 mr-1${circleImage ? ' notification-circle' : ''}`} style={bgStyle}>
            <Img
              className={imageClass}
              width={circleImage ? '50px' : '40px'}
              alt={imageAlt}
              src={image}
              loader={(
                <img
                  width="40px"
                  src={require('../../content/images/imageLoader2.gif')}
                  alt="Loader"
                />
              )}
              decode={false}
            />
          </div>
          <div className="ml-2">
            <h5 className="card-title NotificationCard__Title">
              {title}
            </h5>
            <p className="card-text NotificationCard__Text">
              {text}
            </p>
            <p className="NotificationCard__Date">
              {moment(notificationDateUTC).from(UTCNow().Time)}
            </p>
          </div>
        </div>
      </Link>
    );
  }
}

NotificationCard.propTypes = {
  imageSrc: PropTypes.arrayOf(PropTypes.string).isRequired,
  notification: PropTypes.shape().isRequired,
  link: PropTypes.string.isRequired,
  imageClass: PropTypes.string,
  imageAlt: PropTypes.string,
  circleImage: PropTypes.bool,
  title: PropTypes.string.isRequired,
  text: PropTypes.node.isRequired
};

NotificationCard.defaultProps = {
  imageClass: '',
  imageAlt: '',
  circleImage: false
};

export default NotificationCard;
