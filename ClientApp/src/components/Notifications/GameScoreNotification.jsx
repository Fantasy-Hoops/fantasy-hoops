import React, { Component } from 'react';
import moment from 'moment';
import gameLogo from '../../../src/content/favicon.ico';
import { UTCNow } from "../../utils/UTCNow";

export class GameScoreNotification extends Component {
  constructor(props) {
    super(props);
    this.select = this.select.bind(this);
  }

  async select() {
    await this.props.toggleNotification(this.props.notification);
    window.location.href = "/profile";
  }

  render() {
    const notificationDateUTC = new Date(
      this.props.notification.dateCreated
    ).getTime();
    let read = "card-body text-primary";
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
              <img
                className="pt-1"
                src={gameLogo}
                width="40rem"
                alt="Fantasy Hoops"
              />
            </div>
            <div className="col ml-1"> 
              <h5 className="card-title">The game has finished!</h5>
              <p
                className="card-text"
                style={{ marginTop: "-0.7rem", fontWeight: "400" }}
              >
                Your lineup scored{" "}
                <span className="text-dark font-weight-bold">
                  {this.props.notification.score} FP
                </span>
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
