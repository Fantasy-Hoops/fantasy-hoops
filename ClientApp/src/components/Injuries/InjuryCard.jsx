import React, { Component } from 'react';
import moment from 'moment';
import Img from 'react-image';
import { UTCNow } from "../../utils/UTCNow";

export class InjuryCard extends Component {
  constructor() {
    super();
    this.showModal = this.showModal.bind(this);
  }

  showModal() {
    this.props.showModal(this.props.injury.player);
  }

  render() {
    let status = "";
    if (this.props.injury.status.toLowerCase().includes("active"))
      status = "injury-active";
    else if (
      this.props.injury.status.toLowerCase().includes("out") ||
      this.props.injury.status.toLowerCase().includes("injured")
    )
      status = "injury-out";
    else status = "injury-questionable";
    const link =
      this.props.injury.link !== "" ? (
        <span style={{ float: "left" }} className="comments">
          <i className="fa fa-comments" />
          <a target="_blank" href={this.props.injury.link}>
            {" "}
            Read more{" "}
          </a>
        </span>
      ) : (
        ""
      );
    const injuryDateUTC = new Date(this.props.injury.date).getTime();
    return (
      <div className="mx-auto" style={{ transform: "scale(0.9, 0.9)" }}>
        <div className="column">
          <div className="post-module">
            <div className="thumbnail">
              <div className="date">
                <div className="day badge badge-dark">
                  {this.props.injury.player.position}
                </div>
              </div>
              <Img
                style={{ backgroundColor: this.props.injury.player.team.color }}
                alt={`${this.props.injury.player.firstName} ${
                  this.props.injury.player.lastName
                }`}
                src={[
                  `http://fantasyhoops.org/content/images/players/${
                    this.props.injury.player.nbaID
                  }.png`,
                  require(`../../content/images/positions/${this.props.injury.player.position.toLowerCase()}.png`)
                ]}
                loader={
                  <img
                    height="179.55px"
                    src={require(`../../content/images/imageLoader2.gif`)}
                    alt="Loader"
                  />
                }
                decode={false}
              />
            </div>
            <div className="post-content">
              <div className={"category " + status}>
                {this.props.injury.status}
              </div>
              <h1 className="title">
                {this.props.injury.player.firstName[0]}.{" "}
                {this.props.injury.player.lastName}
              </h1>
              <h2 className="sub_title line-clamp">
                {this.props.injury.title}
              </h2>
              <p className="description">{this.props.injury.description}</p>
              <div className="post-meta">
                <div
                  data-toggle="modal"
                  data-target="#playerModal"
                  onClick={this.showModal}
                  style={{ overflow: "hidden", cursor: "pointer" }}
                >
                  <i className="fa fa-chart-bar" />
                  {" Stats"}
                </div>
                {link}
                <span style={{ float: "right" }} className="timestamp">
                  <i className="fa fa-clock-o" />{" "}
                  {moment(injuryDateUTC).from(UTCNow().Time)}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
