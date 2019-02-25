import React, { Component } from 'react';
import { isAuth, parse } from '../utils/auth';

export class Main extends Component {
  constructor(props) {
    super(props);
    this.state = {
      team: ''
    }
  }

  async componentWillMount() {
    if (isAuth()) {
      const user = parse();
      await fetch(`http://fantasyhoops.org/api/user/team/${user.id}`)
        .then(res => res.json())
        .then(res => {
          const team = res[0].team;
          this.setState({
            team: team,
          });
        });
    } else {
      this.setState({
        team: { abbreviation: 'nba' }
      })
    }
  }

  render() {
    if (this.state.team === '') {
      return <div></div>;
    }
    const image = this.state.team
      ? require(`../content/images/backgrounds/${this.state.team.abbreviation.toLowerCase()}.png`)
      : require(`../content/images/backgrounds/nba.png`);
    return (
      <div>
        <h1 id="main-text" className="text-center title" style={{ marginTop: '15%' }}>
          Fantasy Hoops
          </h1>
        <div className="text-center">
          <a href="/lineup" className="btn btn-primary mt-4" role="button" style={{ width: '20%', fontSize: '80%' }}>
            Play Now!
            </a>
        </div>
        <div className="background-image" style={{ backgroundImage: `url(${image})`, backgroundPosition: 'top' }}></div>
      </div>
    );
  }
}