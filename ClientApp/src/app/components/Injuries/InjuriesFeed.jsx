import React, { Component } from 'react';
import _ from 'lodash';
import shortid from 'shortid';
import $ from 'jquery';
import { InjuryCard } from './InjuryCard';
import { Loader } from '../Loader';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import { EmptyJordan } from '../EmptyJordan';

export class InjuriesFeed extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);

    this.state = {
      injuries: [],
      injuryLoader: true,
      modalLoader: true,
      renderChild: true
    };
  }

  async componentDidMount() {
    this._isMounted = true;

    $('#playerModal').on('hidden.bs.modal', () => {
      if (this._isMounted) {
        this.setState({
          modalLoader: true,
          renderChild: false
        });
      }
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/injuries`)
      .then(res => res.json())
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            injuries: res,
            injuryLoader: false
          });
        }
      });
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  async showModal(player) {
    if (this._isMounted) {
      this.setState({ modalLoader: true });
    }
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/stats/${player.nbaID}`)
      .then(res => res.json())
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            stats: res,
            modalLoader: false,
            renderChild: true
          });
        }
      });
  }

  render() {
    const {
      injuries, injuryLoader, renderChild, modalLoader, stats
    } = this.state;
    if (injuryLoader) {
      return (
        <div className="m-5">
          <Loader show={injuryLoader} />
        </div>
      );
    }

    if (injuries.length === 0) {
      return (
        <div className="p-5">
          <EmptyJordan message="No injuries report today..." />
        </div>
      );
    }

    const injuryCards = _.map(injuries, injury => (
      <InjuryCard
        key={shortid()}
        injury={injury}
        showModal={this.showModal}
      />
    ));
    return (
      <div className="container bg-light">
        <div className="row">{injuryCards}</div>
        <PlayerModal
          renderChild={renderChild}
          loader={modalLoader}
          stats={stats}
        />
      </div>
    );
  }
}

export default InjuriesFeed;
