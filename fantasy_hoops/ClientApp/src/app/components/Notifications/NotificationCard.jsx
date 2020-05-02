import React, {Component} from 'react';
import PropTypes from 'prop-types';
import Img from 'react-image';
import moment from 'moment';
import {UTCNow} from '../../utils/date';
import {loadImage} from '../../utils/loadImage';
import {readNotification} from '../../utils/networkFunctions';

export class NotificationCard extends Component {
    _isMounted = false;

    constructor(props) {
        super(props);
        this.state = {};
        this.readNotification = this.readNotification.bind(this);
    }

    async componentDidMount() {
        this._isMounted = true;

        const {imageSrc, notification} = this.props;

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

    async readNotification() {
        const {notification} = this.props;

        if (notification.readStatus) {
            return;
        }

        if (this._isMounted) {
            this.setState({
                isRead: true
            });
        }
        await readNotification(notification);
    }

    render() {
        const {
            imageClass, imageAlt, circleImage, notification, title, text, link
        } = this.props;
        const {image} = this.state;

        const notificationDateUTC = new Date(
            notification.dateCreated
        ).getTime();

        let bgStyle;
        if (circleImage) {
            bgStyle = {
                backgroundColor: notification.teamColor
            };
        }

        const {isRead} = this.state;
        return (
            <a href={link} role="button" tabIndex={-1} onClick={this.readNotification} onKeyDown={this.readNotification}
               className={`card cursor-pointer mx-auto NotificationCard${!isRead ? '--unread' : ''}`}>
                <div className="card-body NotificationCard__Body">
                    <div className={`ml-1 mr-1${circleImage ? ' notification-circle' : ''}`} style={bgStyle}>
                        <Img
                            className={imageClass}
                            alt={imageAlt}
                            src={image}
                            loader={(
                                <img
                                    width="4rem"
                                    src={require('../../../content/images/imageLoader2.gif')}
                                    alt="Loader"
                                />
                            )}
                            decode={false}
                        />
                    </div>
                    <div className="ml-2">
                        <div className="card-title NotificationCard__Title">
                            {title}
                        </div>
                        <div className="card-text NotificationCard__Text">
                            {text}
                        </div>
                        <div className="NotificationCard__Date">
                            {moment(notificationDateUTC).from(UTCNow().Time)}
                        </div>
                    </div>
                </div>
            </a>
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
