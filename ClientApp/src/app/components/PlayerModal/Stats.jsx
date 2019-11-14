import React, { Component } from 'react';
import Img from 'react-image';
import defaultLogo from '../../../content/images/defaultLogo.png';

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
        <div className="PlayerModal__image--background"></div>
        <div className='position-absolute'>
          <Img
            className="img-modal pt-4 mb-2"
            alt={stats.team.abbreviation}
            src={[
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${stats.team.abbreviation}.svg`,
              defaultLogo
            ]}
            loader={<img height='150px' src={require(`../../../content/images/imageLoader2.gif`)} alt="Loader" />}
            decode={false}
          />
        </div>
        <div className="position-absolute">
          <Img
            className="ml-3 img-modal mb-2"
            style={{ zIndex: '1', paddingTop: '1.2rem' }}
            alt={stats.fullName}
            src={[
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${stats.nbaID}.png`,
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/positions/${stats.position.toLowerCase()}.png`
            ]}
            loader={<img src={require(`../../../content/images/imageLoader2.gif`)} alt="Loader" />}
          />
        </div>
        <div className="col">
          <h1 className="PlayerModal__PlayerName overflow-hidden">{stats.fullName}</h1>
          <h4>{stats.position} | {stats.team.city + " " + stats.team.name}</h4>
          <h4>#{stats.number}</h4>
        </div>
        <div className="table-responsive">
          <table className="table text-right" style={{ maxWidth: '60%' }}>
            <thead>
              <tr>
                <th scope="col" style={{ position: 'relative', backgroundColor: 'white', width: '5rem' }}><h4>FPPG</h4><h1>{stats.fppg.toFixed(1)}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>PTS</h4><h1>{stats.pts}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>REB</h4><h1>{stats.reb}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>AST</h4><h1>{stats.ast}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>STL</h4><h1>{stats.stl}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>BLK</h4><h1>{stats.blk}</h1></th>
                <th scope="col" style={{ width: '5rem' }}><h4>TOV</h4><h1>{stats.tov}</h1></th>
              </tr>
            </thead>
          </table>
        </div>
      </div >
    );
  }
}
