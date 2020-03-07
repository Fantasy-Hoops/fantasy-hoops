import React, {Component} from 'react';
import shortid from 'shortid';
import moment from 'moment';
import {getPlayerStats} from '../../utils/networkFunctions';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import IconButton from "@material-ui/core/IconButton";

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

    getRows() {
        if (this.props.loader) {
            return '';
        }

        const rows = this.state.games.sort(this.compare).map((s) => {
            const abbreviation = s.opponent ? s.opponent.abbreviation : '?';
            let resultLetter = '';
            if (!s.score) {
                return <div/>;
            }
            const scoreTokens = s.score.split(';');
            const teamPoints = scoreTokens[0].split('-');
            if (scoreTokens[2] === 'LIVE') {
                resultLetter = <span className="GameLog__LiveBadge LiveBadge--pulse badge badge-danger">LIVE</span>;
            } else if (scoreTokens[1] === 'vs') {
                resultLetter = parseInt(teamPoints[1], 10) > parseInt(teamPoints[0], 10)
                    ? <span className="text-green">W</span>
                    : <span className="text-red">L</span>;
            } else {
                resultLetter = parseInt(teamPoints[0], 10) > parseInt(teamPoints[1], 10)
                    ? <span className="text-green">W</span>
                    : <span className="text-red">L</span>;
            }
            return (
                <tr key={shortid()}>
                    <th>
                        {moment(s.date).format('MMM. DD')}
                        <br/>
                        <span style={{fontWeight: 900}}>{scoreTokens[1] || 'vs'}</span>
                        {' '}
                        {abbreviation}
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
                        {`${s.fgm}/${s.fga}`}
                    </td>
                    <td>{s.fga !== 0 ? s.fgp : '-'}</td>
                    <td>
                        {`${s.ftm}/${s.fta}`}
                    </td>
                    <td>{s.fta !== 0 ? s.ftp : '-'}</td>
                    <td>
                        {`${s.tpm}/${s.tpa}`}
                    </td>
                    <td>{s.tpa !== 0 ? s.tpp : '-'}</td>
                    <td>{s.gs.toFixed(1)}</td>
                    <td>{s.fp.toFixed(1)}</td>
                    <td>
                        {resultLetter}
                        {' '}
                        {scoreTokens[0]}
                    </td>
                </tr>
            );
        });
        return rows;
    }

    compare(a, b) {
        if (a.date < b.date) {
            return 1;
        }
        if (a.date > b.date) {
            return -1;
        }
        return 0;
    }

    async loadMore() {
        this.setState({
            loader: true,
            loadCounter: this.state.loadCounter + 1
        });
        await getPlayerStats(this.state.nbaID, {start: this.state.games.length, count: LOAD_COUNT})
            .then((res) => {
                this.setState({
                    games: this.state.games.concat(res.data.games),
                    loader: false
                });
            });
    }

    render() {
        const btn = (!(this.state.loadCounter * LOAD_COUNT + 10 > this.state.games.length)
            && !this.state.loader)
            ? (
                <IconButton type="button" classes={{root: "GameLog__SeeMoreButton"}} onClick={this.loadMore}>
                    See more <ExpandMoreIcon/>
                </IconButton>
            )
            : this.state.loader ? <div className="Loader"/> : null;
        return (
            <div>
                <div id="table-scroll" className="table-responsive table-scroll">
                    <table id="main-table" className="table table-sm table-hover text-right main-table">
                        <thead>
                        <tr className="bg-primary text-light">
                            <th className="GameLog__corner-cell">DATE</th>
                            <th>MIN</th>
                            <th>PTS</th>
                            <th>REB</th>
                            <th>AST</th>
                            <th>STL</th>
                            <th>BLK</th>
                            <th>PF</th>
                            <th>TO</th>
                            <th>ORB</th>
                            <th>DRB</th>
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
                        {this.getRows()}
                        </tbody>
                    </table>
                </div>
                {btn}
            </div>
        );
    }
}
