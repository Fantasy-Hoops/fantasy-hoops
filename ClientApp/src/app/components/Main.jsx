import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { isAuth, parse } from '../utils/auth';

export default class Main extends Component {
  constructor(props) {
    super(props);
    this.state = {
      team: ''
    };
  }

  async componentDidMount() {
    if (isAuth()) {
      const user = parse();
      await fetch(
        `${process.env.REACT_APP_SERVER_NAME}/api/user/team/${user.id}`
      )
        .then(res => res.json())
        .then((res) => {
          const { team } = res[0];
          this.setState({
            team
          });
        });
    } else {
      this.setState({
        team: { abbreviation: 'nba' }
      });
    }
  }

  componentDidUpdate() {
    let deferredPrompt;
    if (document.querySelector('.A2HS-Button')) {
      window.addEventListener('beforeinstallprompt', (e) => {
        const addBtn = document.querySelector('.A2HS-Button');
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

  render() {
    const { team } = this.state;
    if (team === '') {
      return <div />;
    }
    const image = team
      ? require(`../../content/images/backgrounds/${team.abbreviation.toLowerCase()}.png`)
      : require('../../content/images/backgrounds/nba.png');
    return (
      <div>
        <button type="button" className="btn btn-success A2HS-Button">
          Add to home screen
        </button>
        <h1 id="main-text" className="text-center title">
          Fantasy Hoops
        </h1>
        <div className="text-center">
          <Link
            to="/lineup"
            className="btn btn-primary mt-4"
            role="button"
          >
            Play Now!
          </Link>
        </div>
        <div
          className="background-image"
          style={{
            backgroundImage: `url(${image})`,
            backgroundPosition: 'top'
          }}
        />
      </div>
    );
  }
}
