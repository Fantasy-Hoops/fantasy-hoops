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
          <h1 className="overflow-hidden">{stats.fullName}</h1>
          <h5>{stats.position} | {stats.team.city + " " + stats.team.name}</h5>
          <h5>#{stats.number}</h5>
        </div>
        <div className="table-responsive">
          <table className="table text-right" style={{ maxWidth: '60%' }}>
            <thead>
              <tr>
                <th scope="col" style={{ position: 'relative', backgroundColor: 'white', width: '5rem' }}><h6>FPPG</h6><h3>{stats.fppg.toFixed(1)}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>PTS</h6><h3>{stats.pts}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>REB</h6><h3>{stats.reb}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>AST</h6><h3>{stats.ast}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>STL</h6><h3>{stats.stl}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>BLK</h6><h3>{stats.blk}</h3></th>
                <th scope="col" style={{ width: '5rem' }}><h6>TOV</h6><h3>{stats.tov}</h3></th>
              </tr>
            </thead>
          </table>
        </div>
      </div >
    );
  }
}
