import React, {Component} from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import {parse} from '../../utils/auth';
import {NotificationCard} from './NotificationCard';
import EmptyJordan from '../EmptyJordan';
import defaultPhoto from '../../../content/images/default.png';
import gameLogo from '../../../content/images/logo.png';
import {getUserNotifications} from '../../utils/networkFunctions';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../utils/helpers";
import Routes from "../../routes/routes";

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
        await getUserNotifications(user.id, {count: 10})
            .then((res) => {
                this.setState({
                    userNotifications: res.data,
                    loader: false
                });
            });
    }

    getNotifications() {
        const {loader, userNotifications} = this.state;
        if (userNotifications.length < 1 && !loader) {
            return (
                <div className="p-5">
                    <EmptyJordan message="Such empty..."/>
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
                            imageClass="NotificationCard__Image"
                            link="/profile"
                        />
                    );
                }
                if (notification.senderID) {
                    const text = <span>{notification.requestMessage}</span>;
                    const link = notification.tournamentId
                        ? `${Routes.TOURNAMENT_INVITATION}/${notification.tournamentId}`
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
                                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${notification.nbaId}.png`,
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

    async loadMore() {
        const {loadCounter, userNotifications} = this.state;
        this.setState({
            loader: true,
            loadCounter: loadCounter + 1
        });
        await getUserNotifications(user.id, {start: userNotifications.length, count: LOAD_COUNT})
            .then((res) => {
                this.setState({
                    userNotifications: userNotifications.concat(res.data),
                    loader: false
                });
            });
    }

    render() {
        const {loader, loadCounter, userNotifications} = this.state;
        const notifications = this.getNotifications();
        const btn = loadCounter * LOAD_COUNT + 10 > userNotifications.length
            ? ''
            : <button type="button" className="btn btn-primary mt-2" onClick={this.loadMore}>See more</button>;
        return (
            <>
                <Helmet>
                    <title>Notifications | Fantasy Hoops</title>
                    <meta property="title" content="Notifications | Fantasy Hoops"/>
                    <meta property="og:title" content="Notifications | Fantasy Hoops"/>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <meta property="og:description" content={Meta.DESCRIPTION}/>
                    <meta name="robots" content="noindex,nofollow"/>
                    <link rel="canonical" href={Canonicals.NOTIFICATIONS}/>
                </Helmet>
                <h1 className="text-center">
                    <i className="fa fa-bell"/>
                    {' '}
                    User notifications
                </h1>
                <div className="AllNotifications mt-3 mb-3 mx-auto">
                    {notifications}
                </div>
                <div className="text-center m-3">
                    {!loader ? btn : ''}
                </div>
                {loader ? <div className="Loader"/> : null}
            </>
        );
    }
}

export default AllNotificationsPage;
