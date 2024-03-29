﻿import React, { Component } from 'react';
import Scroll from 'react-scroll';
import Img from 'react-image';
import PG from '../../../content/images/positions/pg.png';
import SG from '../../../content/images/positions/sg.png';
import SF from '../../../content/images/positions/sf.png';
import PF from '../../../content/images/positions/pf.png';
import C from '../../../content/images/positions/c.png';
import defaultLogo from '../../../content/images/defaultLogo.png';

export class PlayerCard extends Component {
  constructor() {
    super();

    this.state = {
      PG,
      SG,
      SF,
      PF,
      C
    };

    this.filter = this.filter.bind(this);
    this.select = this.select.bind(this);
    this.showModal = this.showModal.bind(this);
    this.handleSelect = this.handleSelect.bind(this);
  }

  render() {
    if (this.props.status > 0) {
      const buttonState = this.props.status === 1
        ? (
          <div className="PlayerCard__button">
            <button className={`btn-no-outline center PlayerCard__button--circle ${this.props.player.selected ? 'btn-danger PlayerCard__Button--selected' : 'btn-primary PlayerCard__Button--default'} text-center`}
              onClick={this.select}>
              <i className={"fa fa-plus PlayerCard__Button__Icon" + (this.props.player.selected ? "--selected" : "")}></i>
            </button>
          </div>
        )
        : '';

      let injuryStatus = '';
      if (this.props.player.injuryStatus.toLowerCase().includes('out')
        || this.props.player.injuryStatus.toLowerCase().includes('injured')
        || this.props.player.injuryStatus.toLowerCase().includes('suspended')) { injuryStatus = 'injury-out'; }
      else { injuryStatus = 'injury-questionable'; }

      let injuryBadge = '';
      if (!this.props.player.injuryStatus.toLowerCase().includes('active')) { injuryBadge = <div className={`PlayerCard__player-injury-badge ${this.props.status === 2 ? 'PlayerCard__player-injury-badge--lineup' : ''} ${injuryStatus}`}>{this.props.player.injuryStatus}</div> };

      const image = (
        <Img
          className={`PlayerCard__player-img ${this.props.status === 2 ? 'PlayerCard__player-img--lineup' : ''}`}
          alt={this.props.player.abbrName}
          src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${this.props.player.id}.png`,
          require(`../../../content/images/positions/${this.props.player.position.toLowerCase()}.png`)]}
          loader={<img className="PlayerCard__loader" src={require(`../../../content/images/imageLoader2.gif`)} alt="Loader" />}
          decode={false}
        />
      );
      const teamLogo = (
        <Img
          className="PlayerCard__player-logo--behind"
          alt={this.props.player.team.abbreviation}
          src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${this.props.player.team.abbreviation}.svg`,
            defaultLogo]}
          decode={false}
        />
      );
      let color = '';
      if (this.props.selectedPlayer && this.props.selectedPlayer.props.player) {
        color = (this.props.selectedPlayer.props.player.price + this.props.remaining) < this.props.player.price ? 'red' : '';
      } else {
        color = this.props.remaining < this.props.player.price ? 'red' : '';
      }
      return (
        <div className={`PlayerCard card ${this.props.status === 1 ? 'PlayerCard--pool' : ''}`}>
          <div className="PlayerCard__player-attributes">
            {this.props.status === 1 ? <div>
              <p className="PlayerCard__player-attributes--FPPG">{this.props.player.fppg.toFixed(1)}</p>
              <p className="PlayerCard__player-attributes--FPPG-label">FPPG</p>
            </div> : ''}
            {this.props.status === 1 ? <div className="PlayerCard__player-attributes--opponent">
              {this.props.player.team.opp}
            </div> : ''}
            {this.props.status === 2 ? <div className="PlayerCard__player-attributes--position">{this.props.player.position}</div> : ''}
            <div
              className={`PlayerCard__player-attributes--price
              ${this.props.status === 2
                  ? 'PlayerCard__player-attributes--price-lineup'
                  : ''} badge badge-dark`}
              style={{ color }}
            >
              {`${this.props.player.price}K`}
            </div>
          </div>
          <div
            data-toggle={this.props.status === 1 ? "modal" : null}
            data-target={this.props.status === 1 ? "#playerModal" : null}
            style={{ cursor: 'pointer' }}
            className="PlayerCard__image" onClick={this.props.status === 2 ? this.filter : this.showModal}
            title={this.props.status === 1 ? "Click for stats" : null}
          >
            <canvas
              className="PlayerCard__background"
              style={{ backgroundColor: `${this.props.player.team.teamColor}` }}
              width="260"
              height="190">
            </canvas>
            {image}
            {teamLogo}
            {injuryBadge}
          </div>
          {buttonState}
          <div className="PlayerCard__player-name">
            <p
              data-toggle="modal"
              data-target="#playerModal"
              className={`PlayerCard__player-name${this.props.status === 2 || this.props.status === 0 ? '--lineup' : ''}`}
              onClick={this.showModal}
              style={{ cursor: 'pointer' }}
              title="Click for stats"
            >
              {this.props.player.abbrName}
            </p>
          </div>
        </div >
      );
    }

    return (
      <div
        onClick={this.filter}
        className="PlayerCard card position-relative"
        tabIndex="-1"
        style={{ cursor: 'pointer' }}>
        <canvas
          className="PlayerCard__background"
          width="260"
          height="190">
        </canvas>
        <Img
          className="PlayerCard__player-img"
          alt={this.props.position}
          src={this.state[this.props.position]}
          loader={<img className="PlayerCard__loader" src={require('../../../content/images/imageLoader2.gif')} alt="Loader" />}
          decode={false}
        />
        <div className="PlayerCard__position">{this.props.position}</div>
      </div>
    );
  }

  select() {
    this.props.player.selected = !this.props.player.selected;
    this.props.player.status = this.props.player.selected ? 2 : 1;
    this.props.selectPlayer(this.props.player);
    this.handleSelect();
  }

  async showModal() {
    await this.props.showModal(this.props.player);
  }

  filter() {
    Scroll.animateScroll.scrollToTop();
    this.props.filter(this.props.position);
  }

  handleSelect() {
    this.props.handleSelect(this.props.player.id, this.props.player.position);
  }
}
