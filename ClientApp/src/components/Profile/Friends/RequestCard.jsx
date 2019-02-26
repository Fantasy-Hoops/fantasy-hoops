import React, { Component } from 'react';
import Img from 'react-image';
import defaultPhoto from '../../../content/images/default.png';

export class RequestCard extends Component {

  render() {
    const img = new Image();
    let avatar;
    if (this.props.id) {
      img.src = `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${this.props.id}.png`;
      avatar = img.height !== 0 ? img.src : defaultPhoto;
    }
    return (
      <div className="card bg-white rounded mt-1 mx-auto" style={{ width: '100%', height: '4.5rem' }}>
        <div className="card-body">
          <a href={`/profile/${this.props.userName}`}>
            <div className="d-inline-block position-absolute ml-3" style={{ top: '0.2rem' }}>
              <Img
                className="user-card-player"
                alt={this.props.userName}
                src={avatar}
                decode={false}
              />
            </div>
            <div className="d-inline-block">
              <p className="align-middle player-name" style={{ paddingLeft: '5rem', paddingTop: '0.3rem' }}>{this.props.userName}</p>
            </div>
          </a>
          <div className="d-inline-block float-right">
            {this.props.type === 'pending' ?
              <div className="row">
                <div className="col">
                  <button type="button" onClick={e => this.props.decline(this.props.id)} className="btn btn-outline-danger mx-auto">Decline Request</button>
                </div>
                <div className="col">
                  <button type="button" onClick={e => this.props.accept(this.props.id)} className="btn btn-success mx-auto">Accept Request</button>
                </div>
              </div>
              :
              <div className="row">
                <div className="col">
                  <button type="button" onClick={e => this.props.cancel(this.props.id)} className="btn btn-outline-danger mx-auto">Cancel Request</button>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    );
  }
}