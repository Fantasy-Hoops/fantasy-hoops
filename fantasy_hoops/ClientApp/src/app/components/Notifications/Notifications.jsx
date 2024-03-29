import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import {parse} from '../../utils/auth';
import {NotificationCard} from './NotificationCard';
import defaultPhoto from '../../../content/images/default.png';
import gameLogo from '../../../content/images/logo.png';
import {getUserNotifications, readAllNotifications} from '../../utils/networkFunctions';
import Routes from "../../routes/routes";

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
        await getUserNotifications(user.id)
            .then((res) => {
                this.setState({
                    userNotifications: res.data,
                    unreadCount: res.data.filter(n => n.readStatus === false).length
                });
            });
    }

    getNotifications() {
        const {userNotifications} = this.state;
        if (userNotifications.length < 1) {
            return <a className="dropdown-item cursor-default text-center">No notifications</a>;
        }
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
                            imageClass="NotificationCard__Image"
                            link="/leaderboard/users"
                        />
                    );
                }
                if (notification.senderID) {
                    const text = (
                        <span>{notification.requestMessage}</span>
                    );
                    const link = notification.tournamentId
                        ? `${Routes.TOURNAMENT_INVITATIONS}/${notification.tournamentId}`
                        : `${Routes.PROFILE}/${notification.friendUsername}`;

                    return (
                        <NotificationCard
                            key={shortid()}
                            notification={notification}
                            title={notification.friendUsername}
                            imageSrc={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${notification.friendAvatarUrl}.png`, defaultPhoto]}
                            text={text}
                            imageClass="NotificationCard__Image Avatar--round"
                            link={link}
                        />
                    );
                }
                if (notification.playerID) {
                    const title = `${notification.abbrName} is ${notification.injuryStatus.toLowerCase()}`;

                    return (
                        <NotificationCard
                            key={shortid()}
                            notification={notification}
                            title={title}
                            circleImage
                            imageSrc={[
                                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${notification.nbaID}.png`,
                                require(`../../../content/images/positions/${notification.position.toLowerCase()}.png`)
                            ]}
                            imageClass="NotificationCard__Image NotificationCard__Image--player"
                            text={notification.injuryDescription}
                            link="/lineup"
                        />
                    );
                }
                return null;
            });
    }

    async readAll(e) {
        e.preventDefault();
        await readAllNotifications(user.id);
        await getUserNotifications(user.id)
            .then((res) => {
                this.setState({
                    userNotifications: res.data,
                    unreadCount: res.data.filter(n => n.readStatus === false).length
                });
            });
    }


    render() {
        const {unreadCount} = this.state;
        const badge = unreadCount > 0
            ? (
                <span className="Notifications__UnreadCounter badge badge-danger">
          {unreadCount}
        </span>
            )
            : '';
        const notifications = this.getNotifications();
        return (
            <li className="NotificationDropdown dropdown" style={{listStyleType: 'none'}}>
                <a
                    className="fa fa-bell text-light mt-1 mr-1 ml-3 nav-link dropdown-toggle no-arrow btn-no-outline position-relative NotificationBell"
                    id="navbarDropdownMenuLink"
                    data-toggle="dropdown"
                    aria-haspopup="true"
                    aria-expanded="false"
                >
                    {badge}
                </a>
                <div className="dropdown-menu dropdown-menu-right" aria-labelledby="navbarDropdownMenuLink"
                     style={{width: '28rem'}}>
                    <p className="Notifications__dropdown dropdown-header">
                        Notifications
                        <a
                            role="link"
                            tabIndex="-1"
                            onClick={this.readAll}
                            onKeyDown={this.readAll}
                            className="position-absolute btn-no-outline"
                            style={{right: '1rem'}}
                        >
                            Mark All as Read
                        </a>
                    </p>
                    <div style={{marginBottom: '-0.5rem'}}>
                        {notifications}
                    </div>
                    <p className="dropdown-header text-center mt-2">
                        <Link
                            className="btn-no-outline"
                            to={Routes.ALL_NOTIFICATIONS}
                        >
                            See all
                        </Link>
                    </p>
                </div>
            </li>
        );
    }
}

export default Notifications;
