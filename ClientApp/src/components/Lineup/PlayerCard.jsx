import React, { Component } from 'react';
import Scroll from 'react-scroll';
import Img from 'react-image';
import PG from '../../content/images/positions/pg.png';
import SG from '../../content/images/positions/sg.png';
import SF from '../../content/images/positions/sf.png';
import PF from '../../content/images/positions/pf.png';
import C from '../../content/images/positions/c.png';
import defaultLogo from '../../content/images/defaultLogo.png';
const $ = window.$;

export class PlayerCard extends Component {
  constructor() {
    super();

    this.state = {
      PG: PG,
      SG: SG,
      SF: SF,
      PF: PF,
      C: C
    };

    this.filter = this.filter.bind(this);
    this.select = this.select.bind(this);
    this.showModal = this.showModal.bind(this);
    this.handleSelect = this.handleSelect.bind(this);
  }

  componentDidMount() {
    $('[data-toggle="tooltip"]').tooltip()
  }

  render() {
    if (this.props.status > 0) {
      const innerHTML = this.props.player.selected
        ? <i className="fa fa-times"></i>
        : <i className="fa fa-plus"></i>;
      const buttonState = this.props.status === 1
        ? <div className="card__button--select">
          <button className={`btn-no-outline center btn-circle btn-lg ${this.props.player.selected ? 'btn-danger' : 'btn-primary'} text-center`}
            onClick={this.select}>
            {innerHTML}
          </button>
        </div>
        : '';

      let injuryStatus = '';
      if (this.props.player.injuryStatus.toLowerCase().includes("out")
        || this.props.player.injuryStatus.toLowerCase().includes("injured"))
        injuryStatus = 'injury-out';
      else
        injuryStatus = 'injury-questionable';

      let injuryBadge = '';
      if (!this.props.player.injuryStatus.toLowerCase().includes("active"))
        injuryBadge = <div className={"card__injury-badge " + injuryStatus}>{this.props.player.injuryStatus}</div>

      const image = <Img
        onClick={this.props.status === 2 ? this.filter : undefined}
        className="card__player-img card-img-top"
        alt={this.getDisplayName(this.props.player)}
        src={[`http://fantasyhoops.org/content/images/players/${this.props.player.id}.png`,
        require(`../../content/images/positions/${this.props.player.position.toLowerCase()}.png`)]}
        loader={<img height='151px' width='206px' src={require(`../../content/images/imageLoader2.gif`)} alt="Loader" />}
        decode={false}
      />
      const teamLogo = <Img
        className="card__team-logo--behind"
        alt={this.props.player.team.abbreviation}
        src={[`http://fantasyhoops.org/content/images/logos/${this.props.player.team.abbreviation}.svg`,
          defaultLogo]}
        decode={false}
      />
      return (
        <div>
          <div className="player-card card">
            <div className="card__player-attributes">
              {this.props.status === 1 ? <div className="card__player-position">{this.props.player.position}</div> : ''}
              {this.props.status === 1 ? <div className="ppg">{this.props.player.fppg.toFixed(1)}</div> : ''}
              {this.props.status === 1 ? <div className="ppg ppg-label">FPPG</div> : ''}
              <div className="card__player-price--badge badge badge-dark">
                {this.props.player.price + 'K'}
              </div>
            </div>
            <div className="card__image-backgound" style={{ backgroundColor: `${this.props.player.team.teamColor}` }}>
              {image}
              {teamLogo}
            </div>
            {injuryBadge}
            <div className="card__title-block--bottom card-block position-relative" >
              <a
                className="card__title-tooltip"
                data-toggle="tooltip"
                data-placement="top"
                title="Click for stats"
              >
                <h2
                  data-toggle="modal"
                  data-target="#playerModal"
                  className="card__title-text--bottom card-title"
                  onClick={this.showModal}
                  style={{ cursor: 'pointer' }}
                >
                  {this.getDisplayName(this.props.player)}
                </h2>
              </a>
              {buttonState}
            </div>
          </div>
        </div>
      );
    }
    else {
      return (
        <div onClick={this.filter} className="player-card card" tabIndex="1">
          <Img
            className="player-card-img-top card-img-top"
            alt={this.props.position}
            src={this.state[this.props.position]}
            loader={<img height='140px' src={require(`../../content/images/imageLoader2.gif`)} alt="Loader" />}
            decode={false}
          />
          <div className="card-block" >
            <h2 className="player-card-title card-title">{this.props.position}</h2>
          </div>
        </div>
      );
    }
  }

  getDisplayName(player) {
    if (!player)
      return;
    if (player.firstName && player.firstName.length > 1)
      return `${player.firstName[0]}. ${player.lastName}`;
    else
      return player.lastName;
  }

  select() {
    this.props.player.selected = !this.props.player.selected;
    this.props.player.status = this.props.player.selected ? 2 : 1;
    this.props.selectPlayer(this.props.player);
    this.handleSelect();
  }

  async showModal() {
    $('[data-toggle="tooltip"]').tooltip("hide");
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
