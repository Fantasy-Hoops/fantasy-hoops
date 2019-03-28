import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import shortid from 'shortid';
import _ from 'lodash';
import $ from 'jquery';
import Routes from '../routes/routes';
import { Card } from './Leaderboard/Players/Card';
import { getPlayersLeaderboard } from '../utils/networkFunctions';

export default class Main extends Component {
  constructor(props) {
    super(props);
    this.state = {
      daily: []
    };
  }

  async componentDidMount() {
    $('#PlayNowBtn').on('click', () => {
      $('.navbar-collapse').removeClass('show');
    });
    document.querySelector('body').classList.add('Main__Background');
    await getPlayersLeaderboard({ type: 'daily', limit: 3 })
      .then((res) => {
        this.setState({
          daily: res.data
        });
      });
    await this.setState({
      PG: require('../../content/images/positions/pg.png'),
      SG: require('../../content/images/positions/sg.png'),
      SF: require('../../content/images/positions/sf.png'),
      PF: require('../../content/images/positions/pf.png'),
      C: require('../../content/images/positions/c.png')
    });
  }

  componentDidUpdate() {
    let deferredPrompt;
    if (document.querySelector('.A2HS-Button')) {
      window.addEventListener('beforeinstallprompt', (e) => {
        const addBtn = document.querySelector('.A2HS-Button');
        if (!addBtn) { return; }
        addBtn.style.display = 'none';
        // Prevent Chrome 67 and earlier from automatically showing the prompt
        e.preventDefault();
        // Stash the event so it can be triggered later.
        deferredPrompt = e;
        // Update UI to notify the user they can add to home screen
        addBtn.style.display = 'block';

        addBtn.addEventListener('click', () => {
          // hide our user interface that shows our A2HS button
          addBtn.style.display = 'none';
          // Show the prompt
          deferredPrompt.prompt();
          // Wait for the user to respond to the prompt
          deferredPrompt.userChoice.then((choiceResult) => {
            if (choiceResult.outcome === 'accepted') {
              console.log('User accepted the A2HS prompt');
            } else {
              console.log('User dismissed the A2HS prompt');
            }
            deferredPrompt = null;
          });
        });
      });
    }
  }

  componentWillUnmount() {
    document.querySelector('body').classList.remove('Main__Background');
  }

  createPlayers(players) {
    return _.map(
      players,
      (player, index) => (
        <Card
          className="Main__TopPlayersCard"
          index={index}
          key={shortid()}
          player={player}
          showModal={this.showModal}
          image={this.state[player.position]}
        />
      )
    );
  }

  render() {
    const { daily } = this.state;
    const topPlayers = daily && daily.length > 0
      ? (
        <div className="Main__TopPlayers">
          <h2 className="Main__TopPlayersHeading">Top NBA Players Today</h2>
          {this.createPlayers(daily)}
        </div>
      )
      : null;
    return (
      <div className="Main__Background">
        <button type="button" className="btn btn-outline-success A2HS-Button">
          <i className="far fa-bookmark" />
          {' Save'}
        </button>
        {topPlayers}
        <div className="Main__LogoContainer">
          <img
            className="Main__Logo"
            alt="Fantasy Hoops"
            src={`${require('../../content/images/FH_Logo.png')}`}
          />
          <Link
            id="PlayNowBtn"
            to={Routes.LINEUP}
            className="Main__PlayNowButton text-center btn btn-outline-success"
            role="button"
          >
            {'Play Now!'}
          </Link>
        </div>
      </div>
    );
  }
}
