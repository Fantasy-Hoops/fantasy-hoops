import React, { Component } from 'react';
import { InjuryCard } from './InjuryCard';
import _ from 'lodash';
import shortid from 'shortid';
import { Loader } from '../Loader';
import { PlayerModal } from '../PlayerModal/PlayerModal';
import { EmptyJordan } from '../EmptyJordan';
import $ from "jquery";

export class InjuriesFeed extends Component {
  constructor(props) {
    super(props);
    this.showModal = this.showModal.bind(this);

    this.state = {
      noInjuries: false,
      injuries: [],
      injuryLoader: true,
      modalLoader: true,
      renderChild: true
    }
  }

  componentDidMount() {
    $("#playerModal").on("hidden.bs.modal", () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async componentWillMount() {
    await fetch(`http://fantasyhoops.org:5001/api/lineup/nextGame`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          serverTime: res.serverTime
        });
      })
    await fetch(`http://fantasyhoops.org:5001/api/injuries`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          injuries: res,
          noInjuries: false,
          injuryLoader: false
        });
      });
  }

  componentDidUpdate(prevProps, prevState) {
    if (prevState.injuries !== this.state.injuries
      && this.state.injuries.length === 0)
      this.setState({
        noInjuries: true
      });
  }

  async showModal(player) {
    this.setState({ modalLoader: true })
    await fetch(`http://fantasyhoops.org:5001/api/stats/${player.nbaID}`)
      .then(res => res.json())
      .then(res => {
        this.setState({
          stats: res,
          modalLoader: false,
          renderChild: true
        });
      });
  }

  render() {
    if (this.state.injuryLoader)
      return <div className="m-5"><Loader show={this.state.injuryLoader} /></div>;

    if (this.state.injuries.length === 0)
      return (
        <div className="p-5">
          <EmptyJordan message="No injuries report today..." />
        </div>
      );

    const injuries = _.map(this.state.injuries,
      (injury) => {
        return <InjuryCard
          serverTime={this.state.serverTime}
          key={shortid()}
          injury={injury}
          showModal={this.showModal}
        />
      }
    );
    return (
      <div className="container bg-light">
        <div className="row">
          {injuries}
        </div>
        <PlayerModal
          renderChild={this.state.renderChild}
          loader={this.state.modalLoader}
          stats={this.state.stats}
        />
      </div>
    );
  }
}
