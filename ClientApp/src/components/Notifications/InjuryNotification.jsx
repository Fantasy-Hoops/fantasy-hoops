import React, { Component } from 'react';
import moment from 'moment';
import Img from 'react-image';
import { UTCNow } from "../../utils/UTCNow";

export class InjuryNotification extends Component {
  constructor(props) {
    super(props);
    this.select = this.select.bind(this);
  }

  async select() {
    await this.props.toggleNotification(this.props.notification);
    window.location.href = "/lineup";
  }

  render() {
    const notificationDateUTC = new Date(
      this.props.notification.dateCreated
    ).getTime();
    let read = "card-body rounded text-primary";
    if (this.props.notification.readStatus) read = "card-body text-muted";
    return (
      <a
        onClick={this.select}
        className="card cursor-pointer link mx-auto"
        style={{ maxWidth: `${this.props.width}` }}
      >
        <div className={read} style={{ margin: "-0.6rem" }}>
          <div className="row">
            <div className="col-1 mr-3">
              <div
                className="notification-circle position-absolute"
                style={{
                  backgroundColor: this.props.notification.player.team.color
                }}
              >
                <Img
                  className="notification-card-player"
                  alt={`${this.props.notification.player.firstName} ${
                    this.props.notification.player.lastName
                  }`}
                  src={[
                    `http://fantasyhoops.org/content/images/players/${
                      this.props.notification.player.nbaID
                    }.png`,
                    require(`../../content/images/positions/${this.props.notification.player.position.toLowerCase()}.png`)
                  ]}
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
            </div>
            <div className="col ml-1 mr-4" style={{ overflow: "hidden" }}>
              <h5 className="card-title text-nowrap">
                {this.props.notification.player.firstName[0]}.
                {" " + this.props.notification.player.lastName} is{" "}
                {this.props.notification.injuryStatus.toLowerCase()}
              </h5>
              <p
                className="card-text"
                style={{ marginTop: "-0.7rem", fontWeight: "400" }}
              >
                {this.props.notification.injuryDescription}
              </p>
              <p style={{ margin: "-1rem 0 0 0" }}>
                {moment(notificationDateUTC).from(UTCNow().Time)}
              </p>
            </div>
          </div>
        </div>
      </a>
    );
  }
}
