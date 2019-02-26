import React, { Component } from 'react';
import { UTCNow } from "../../utils/UTCNow";
import { handleErrors } from '../../utils/errors';
import { loadImage } from '../../utils/loadImage';
import Img from 'react-image';
import moment from 'moment';

export class NotificationCard extends Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    async componentWillMount() {
        let images = this.props.imageSrc;
        if (!Array.isArray(images)) {
            images = [images];
        }

        if (images) {
            this.setState({
                isRead: this.props.notification.readStatus,
                image: await loadImage(...images)
            });
            return;
        }

        this.setState({
            isRead: this.props.notification.readStatus
        });
    }

    render() {
        const notificationDateUTC = new Date(
            this.props.notification.dateCreated
        ).getTime();

        let bgStyle;
        if (this.props.circleImage) {
            bgStyle = {
                backgroundColor: this.props.notification.player.team.color
            };
        }

        return (
            <div onClick={this.readNotification.bind(this)} className={"card cursor-pointer link mx-auto NotificationCard" + (!this.state.isRead ? "--unread" : "")}>
                <div className="card-body NotificationCard__Body">
                    <div className={"NotificationCard__Image ml-1 mr-1" + (this.props.circleImage ? " notification-circle" : "")} style={bgStyle}>
                        <Img
                            className={this.props.imageClass}
                            width={this.props.circleImage ? "50px" : "40px"}
                            alt={this.props.imageAlt}
                            src={this.state.image}
                            loader={
                                <img
                                    width="40px"
                                    src={require(`../../content/images/imageLoader2.gif`)}
                                    alt="Loader"
                                />
                            }
                            decode={false}
                        />
                    </div>
                    <div className="ml-2">
                        <h5 className="card-title NotificationCard__Title">
                            {this.props.title}
                        </h5>
                        <p className="card-text NotificationCard__Text">
                            {this.props.text}
                        </p>
                        <p className="NotificationCard__Date">
                            {moment(notificationDateUTC).from(UTCNow().Time)}
                        </p>
                    </div>
                </div>
            </div>
        );
    }

    async readNotification() {
        window.location.href = this.props.link;

        if (this.props.notification.readStatus)
            return;

        this.setState({
            isRead: true
        })
        await fetch('${process.env.REACT_APP_SERVER_NAME}/api/notification/read', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(this.props.notification)
        });
    }
}