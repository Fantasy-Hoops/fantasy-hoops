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

        return (
            <div onClick={this.readNotification.bind(this)} className={"card cursor-pointer link mx-auto NotificationCard" + (!this.state.isRead ? "--unread" : "")}>
                <div className="card-body NotificationCard__Body">
                    <div className="NotificationCard__Image ml-1 mr-1">
                        <Img
                            width="40rem"
                            height="40rem"
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
        await fetch('http://fantasyhoops.org/api/notification/read', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(this.props.notification)
        });
    }
}