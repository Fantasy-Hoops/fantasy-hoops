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
          <th>
            {moment(s.date).format('MMM. DD')}
            <br />
            <span style={{ fontWeight: 900 }}>{scoreTokens[1] || 'vs'}</span>
            {' '}
            {abbreviation || '?'}
          </th>
          <td>{s.min}</td>
          <td>{s.pts}</td>
          <td>{s.treb}</td>
          <td>{s.ast}</td>
          <td>{s.stl}</td>
          <td>{s.blk}</td>
          <td>{s.fls}</td>
          <td>{s.tov}</td>
          <td>{s.oreb}</td>
          <td>{s.dreb}</td>
          <td>
            {s.fgm}
            /
            {s.fga}
          </td>
          <td>{s.fga !== 0 ? s.fgp : '-'}</td>
          <td>
            {s.ftm}
            /
            {s.fta}
          </td>
          <td>{s.fta !== 0 ? s.ftp : '-'}</td>
          <td>
            {s.tpm}
            /
            {s.tpa}
          </td>
          <td>{s.tpa !== 0 ? s.tpp : '-'}</td>
          <td>{s.gs.toFixed(1)}</td>
          <td>{s.fp.toFixed(1)}</td>
          <td>
            {score}
            {' '}
            {scoreTokens[0]}
          </td>
        </tr>
      );
    });
    if (!(this.state.loadCounter * LOAD_COUNT + 10 > this.state.games.length) || this.state.loader) {
      rows.push(
        <tr className="no-hover" key={shortid()}>
          <td colSpan="20" className="align-middle">{btn}</td>
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
        <table id="main-table" className="table table-sm table-hover text-right main-table">
          <thead>
            <tr className="bg-primary text-light">
              <th style={{ fontWeight: 700 }}>DATE</th>
              <th>MIN</th>
              <th>PTS</th>
              <th>REB</th>
              <th>AST</th>
              <th>STL</th>
              <th>BLK</th>
              <th>PF</th>
              <th>TO</th>
              <th>OREB</th>
              <th>DREB</th>
              <th>FG</th>
              <th>FG%</th>
              <th>FT</th>
              <th>FT%</th>
              <th>3P</th>
              <th>3P%</th>
              <th>GS</th>
              <th>FP</th>
              <th>SCORE</th>
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
