import React, { Component } from 'react';
import shortid from 'shortid';
import moment from 'moment';
import { Loader } from '../Loader';

const LOAD_COUNT = 10;

export default class Gamelog extends Component {
  constructor(props) {
    super(props);
    this.compare = this.compare.bind(this);
    this.loadMore = this.loadMore.bind(this);

    this.state = {
      games: this.props.stats.games,
      nbaID: this.props.stats.nbaID,
      loadCounter: 0,
      loader: false
    };
  }

  getRows(btn) {
    if (this.props.loader) { return ''; }

    const rows = this.state.games.sort(this.compare).map((s) => {
      const abbreviation = s.opponent ? s.opponent.abbreviation : '';
      let score = '';
      if (!s.score) { return <div />; }
      const scoreTokens = s.score.split(';');
      const str = scoreTokens[0].split('-');
      if (parseInt(str[0], 10) > parseInt(str[1], 10)) { score = <span className="text-success">W</span>; } else score = <span className="text-danger">L</span>;
      return (
        <tr key={shortid()}>
          <th style={{ width: '80px' }}>
            {moment(s.date).format('MMM. DD')}
            <br />
            <span style={{ fontWeight: 900 }}>{scoreTokens[1] || 'vs'}</span>
            {' '}
            {abbreviation || '?'}
          </th>
          <td style={{ width: '3.5rem' }}>{s.min}</td>
          <td style={{ width: '40px' }}>{s.pts}</td>
          <td style={{ width: '40px' }}>{s.treb}</td>
          <td style={{ width: '40px' }}>{s.ast}</td>
          <td style={{ width: '40px' }}>{s.stl}</td>
          <td style={{ width: '40px' }}>{s.blk}</td>
          <td style={{ width: '40px' }}>{s.fls}</td>
          <td style={{ width: '40px' }}>{s.tov}</td>
          <td style={{ width: '40px' }}>{s.oreb}</td>
          <td style={{ width: '40px' }}>{s.dreb}</td>
          <td style={{ width: '50px' }}>
            {s.fgm}
            /
            {s.fga}
          </td>
          <td style={{ width: '50px' }}>{s.fgp}</td>
          <td style={{ width: '50px' }}>
            {s.ftm}
            /
            {s.fta}
          </td>
          <td style={{ width: '50px' }}>{s.ftp}</td>
          <td style={{ width: '50px' }}>
            {s.tpm}
            /
            {s.tpa}
          </td>
          <td style={{ width: '50px' }}>{s.tpp}</td>
          <td style={{ width: '50px' }}>{s.gs}</td>
          <td style={{ width: '50px' }}>{s.fp}</td>
          <td style={{ width: '7rem' }}>
            {score}
            {' '}
            {scoreTokens[0]}
          </td>
        </tr>
      );
    });
    if (!(this.state.loadCounter * LOAD_COUNT + 10 > this.state.games.length) || this.state.loader) {
      rows.push(
        <tr className="no-hover" key={shortid()} style={{ height: '7rem' }}>
          <td className="align-middle">{btn}</td>
        </tr>
      );
    }
    return rows;
  }

  compare(a, b) {
    if (a.date < b.date) { return 1; }
    if (a.date > b.date) { return -1; }
    return 0;
  }

  async loadMore() {
    this.setState({
      loader: true,
      loadCounter: this.state.loadCounter + 1
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/stats/${this.state.nbaID}?start=${this.state.games.length}&count=${LOAD_COUNT}`)
      .then(res => res.json())
      .then((res) => {
        this.setState({
          games: this.state.games.concat(res.games),
          loader: false
        });
      });
  }

  render() {
    const btn = this.state.loadCounter * LOAD_COUNT + 10 > this.state.games.length
      ? (
        <div className="p-1 float-left">
          <Loader show={this.state.loader} />
        </div>
      )
      : <button type="button" className="btn btn-primary float-left m-2" onClick={this.loadMore}>See more</button>;
    return (
      <div id="table-scroll" className="table-responsive table-scroll">
        <table id="main-table" className="table table-sm table-hover table-bordered text-justify main-table">
          <thead>
            <tr className="bg-primary text-light">
              <th scope="col" style={{ fontWeight: 700, width: '80px' }}>DATE</th>
              <th scope="col" style={{ width: '3.5rem' }}>MIN</th>
              <th scope="col" style={{ width: '40px' }}>PTS</th>
              <th scope="col" style={{ width: '40px' }}>REB</th>
              <th scope="col" style={{ width: '40px' }}>AST</th>
              <th scope="col" style={{ width: '40px' }}>STL</th>
              <th scope="col" style={{ width: '40px' }}>BLK</th>
              <th scope="col" style={{ width: '40px' }}>PF</th>
              <th scope="col" style={{ width: '40px' }}>TO</th>
              <th scope="col" style={{ width: '40px' }}>OREB</th>
              <th scope="col" style={{ width: '40px' }}>DREB</th>
              <th scope="col" style={{ width: '50px' }}>FG</th>
              <th scope="col" style={{ width: '50px' }}>FG%</th>
              <th scope="col" style={{ width: '50px' }}>FT</th>
              <th scope="col" style={{ width: '50px' }}>FT%</th>
              <th scope="col" style={{ width: '50px' }}>3P</th>
              <th scope="col" style={{ width: '50px' }}>3P%</th>
              <th scope="col" style={{ width: '50px' }}>GS</th>
              <th scope="col" style={{ width: '50px' }}>FP</th>
              <th scope="col" style={{ width: '7rem' }}>SCORE</th>
            </tr>
          </thead>
          <tbody>
            {this.getRows(btn)}
          </tbody>
        </table>
      </div>
    );
  }
}
