import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import moment from 'moment';
import Img from 'react-image';
import { UTCNow } from "../../utils/UTCNow";
import defaultLogo from '../../content/images/defaultLogo.png';

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
      this.props.injury.link
        ? (
          <span style={{ float: "left" }} className="comments">
            <Link target="_blank" to={this.props.injury.link}>
              <i className="fa fa-external-link-alt" />
              {" "}Link
          </Link>
          </span>
        ) : (
          ""
        );
    const injuryDateUTC = new Date(this.props.injury.date).getTime();

    const teamLogo = <Img
      className="injury-card__team-logo--behind"
      alt={this.props.injury.player.team.abbreviation}
      src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${this.props.injury.player.team.abbreviation}.svg`,
        defaultLogo]}
      decode={false}
    />
    return (
      <div className="mx-auto" style={{ transform: "scale(0.9, 0.9)" }}>
        <div className="column">
          <div className="post-module">
            <div className="thumbnail" style={{ backgroundColor: this.props.injury.player.team.color, position: 'relative' }}>
              <div className="date">
                <div className="injury-card__position--badge badge">
                  {this.props.injury.player.position}
                </div>
              </div>
              <Img
                className="injury-card__player-img"
                alt={this.props.injury.player.fullName}
                src={[
                  `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${
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
              {teamLogo}
            </div>
            <div className="post-content" style={{ zIndex: 3 }}>
              <div className={"category " + status}>
                {this.props.injury.status}
              </div>
              <h1 className="title">
                {this.props.injury.player.abbrName}
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
