import React, { Component } from 'react';
import Img from 'react-image';
import defaultLogo from '../../../content/images/defaultLogo.png';

import './Stats.css';

export class Stats extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }
  }

  render() {
    const stats = this.props.stats;
    return (
      <div className="row">
          <div className="PlayerModal__image--background">
              <Img
                  className="PlayerModal__TeamLogoImage"
                  alt={stats.team.abbreviation}
                  src={[
                      `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${stats.team.abbreviation}.svg`,
                      defaultLogo
                  ]}
                  loader={<img height='10rem' src={require(`../../../content/images/imageLoader2.gif`)} alt="Loader" />}
                  decode={false}
              />
              <Img
                  className="PlayerModal__PlayerImage"
                  alt={stats.fullName}
                  src={[
                      `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${stats.nbaID}.png`,
                      require(`../../../content/images/positions/${stats.position.toLowerCase()}.png`)
                  ]}
                  loader={<img src={require(`../../../content/images/imageLoader2.gif`)} alt="Loader" />}
              />
              <h4 className="PlayerModal__PlayerNumber">#{stats.number}</h4>
          </div>
        <div className="PlayerModal__PlayerName col">
          <div>{stats.firstName}</div>
          <div>{stats.lastName}</div>
          <div className="PlayerModal__PositionTeam"><strong>{stats.position}</strong> | {stats.team.city + " " + stats.team.name}</div>
        </div>
        <div className="table-responsive">
          <table className="table text-right" style={{ maxWidth: '60%' }}>
            <thead>
              <tr>
                <th scope="col" className="Stats__TableCell Stats__TableCell--first">
                    <div className="Stats__TableCell--label">FPPG</div>
                    <div className="Stats__TableCell--value">{stats.fppg.toFixed(1)}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">PTS</div>
                    <div className="Stats__TableCell--value">{stats.pts}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">REB</div>
                    <div className="Stats__TableCell--value">{stats.reb}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">AST</div>
                    <div className="Stats__TableCell--value">{stats.ast}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">STL</div>
                    <div className="Stats__TableCell--value">{stats.stl}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">BLK</div>
                    <div className="Stats__TableCell--value">{stats.blk}</div>
                </th>
                <th scope="col" className="Stats__TableCell">
                    <div className="Stats__TableCell--label">TOV</div>
                    <div className="Stats__TableCell--value">{stats.tov}</div>
                </th>
              </tr>
            </thead>
          </table>
        </div>
      </div >
    );
  }
}
