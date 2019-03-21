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
    document.querySelector('body').classList.add('Main__Background');
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

  render() {
    const { team } = this.state;
    if (team === '') {
      return <div />;
    }
    return (
      <div className="Main__Background">
        <button type="button" className="btn btn-danger A2HS-Button">
          Add to home screen
        </button>
        <div className="Main__LogoContainer">
          <img
            className="Main__Logo"
            alt="Fantasy Hoops"
            src={`${require('../../content/images/FH_Logo.png')}`}
          />
          <Link
            to="/lineup"
            className="Main__PlayNowButton text-center btn btn-danger"
            role="button"
          >
            Play Now!
          </Link>
        </div>
      </div>
    );
  }
}
