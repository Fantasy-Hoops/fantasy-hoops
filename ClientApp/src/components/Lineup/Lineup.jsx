import React, { Component } from "react";
import { PlayerPool } from "./PlayerPool";
import { PlayerCard } from "./PlayerCard";
import { ProgressBar } from "./ProgressBar";
import { parse } from "../../utils/auth";
import { handleErrors } from "../../utils/errors";
import { Alert } from "../Alert";
import { PlayerModal } from "../PlayerModal/PlayerModal";
import Countdown from "react-countdown-now";
import { InfoModal } from "./InfoModal";
import { Loader } from "../Loader";
import { EmptyJordan } from "../EmptyJordan";
const $ = window.$;
const budget = 300; // thousands

export class Lineup extends Component {
  constructor() {
    super();

    this.selectPlayer = this.selectPlayer.bind(this);
    this.filter = this.filter.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.showModal = this.showModal.bind(this);

    this.state = {
      position: "",
      pg: <PlayerCard filter={this.filter} status={0} position="PG" />,
      sg: <PlayerCard filter={this.filter} status={0} position="SG" />,
      sf: <PlayerCard filter={this.filter} status={0} position="SF" />,
      pf: <PlayerCard filter={this.filter} status={0} position="PF" />,
      c: <PlayerCard filter={this.filter} status={0} position="C" />,
      loadedPlayers: false,
      showAlert: false,
      alertType: "",
      alertText: "",
      nextGame: "",
      playerLoader: false,
      submit: true,
      isGame: true,
      modalLoader: true,
      poolLoader: true,
      renderChild: true
    };
  }

  setModal() {
    $("#playerModal").on("hidden.bs.modal", () => {
      this.setState({
        modalLoader: true,
        renderChild: false
      });
    });
  }

  async componentWillMount() {
    this.setState({
      playerLoader: true
    });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/lineup/nextGame`)
      .then(res => {
        return res.json();
      })
      .then(res => {
        if (new Date(res.nextGame).getFullYear() !== 1) {
          this.setState({
            nextGame: res.nextGame,
            playerPoolDate: res.playerPoolDate,
            poolLoader: false
          });
        } else {
          this.setState({
            isGame: false,
            poolLoader: false
          });
        }
        this.setModal();
      });

    if (!this.state.isGame) return;

    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/player`)
      .then(res => {
        return res.json();
      })
      .then(res => {
        this.setState({
          players: res,
          playerLoader: false
        });
      });
    this.filter("PG");
  }

  async componentDidUpdate() {
    if (!this.state.isGame) return;

    if (!this.state.loadedPlayers && this.state.players) {
      const user = parse();
      await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/lineup/${user.id}`)
        .then(res => {
          return res.json();
        })
        .then(res => {
          res.forEach(selectedPlayer => {
            this.state.players.forEach(player => {
              if (player.id == selectedPlayer.id) {
                player.selected = true;
                player.status = 2;
                this.selectPlayer(player);
              }
            });
          });
        });
      this.setState({
        loadedPlayers: true
      });
    }

    if (
      this.state.pg.props.player &&
      this.state.sg.props.player &&
      this.state.sf.props.player &&
      this.state.pf.props.player &&
      this.state.c.props.player &&
      this.calculateRemaining() >= 0 &&
      this.state.playerPoolDate === this.state.nextGame &&
      this.state.submit
    ) {
      const btn = document.getElementById("submit");
      btn.disabled = false;
      btn.className = "Lineup__submit-button btn btn-primary btn-block";
    } else {
      const btn = document.getElementById("submit");
      btn.disabled = true;
      btn.className = "Lineup__submit-button btn btn-outline-primary btn-block";
    }
  }

  getDate() {
    var dt = new Date();
    var toDate = new Date(this.state.nextGame);
    var tz = dt.getTimezoneOffset();
    return toDate.setMinutes(toDate.getMinutes() - tz);
  }

  render() {
    if (this.state.poolLoader)
      return (
        <div className="p-5">
          <Loader show={this.state.poolLoader} />
        </div>
      );

    if (!this.state.isGame)
      return (
        <div className="p-5">
          <EmptyJordan message="The game hasn't started yet..." />
        </div>
      );

    const remaining = this.calculateRemaining();
    const Completionist = () => (
      <span>The game already started. Come back soon!</span>
    );
    const renderer = ({ days, hours, minutes, seconds, completed }) => {
      if (completed) {
        this.state.submit = false;
        return <Completionist />;
      } else {
        if (this.state.playerPoolDate !== this.state.nextGame)
          return <h5>Please wait a moment until player pool is updated!</h5>;
        this.state.submit = true;

        days = this.getFormattedDateString(days, "day");
        hours = this.getFormattedDateString(hours, "hour");
        minutes = this.getFormattedDateString(minutes, "minute");
        seconds = this.getFormattedDateString(seconds, "second");

        return (
          <span className="Lineup__countdown">
            Game starts in{" "}
            <strong>
              {days}
              {hours}
              {minutes}
              {seconds}
            </strong>
          </span>
        );
      }
    };
    const playerPool = () => {
      if (
        this.state.playerPoolDate !== this.state.nextGame &&
        !this.state.playerLoader
      ) {
        return (
          <div className="p-5">
            <EmptyJordan message="Player pool is empty..." />
          </div>
        );
      } else {
        return (
          <PlayerPool
            position={this.state.position}
            players={this.state.players}
            selectPlayer={this.selectPlayer}
            showModal={this.showModal}
          />
        );
      }
    };

    return (
      <div className="container bg-light" style={{ width: "100%" }}>
        <div className="text-center mx-auto" style={{ width: "50%" }}>
          <Alert
            type={this.state.alertType}
            text={this.state.alertText}
            show={this.state.showAlert}
          />
        </div>
        <div className="Lineup--sticky">
          <div className="text-center">
            <Countdown
              date={this.getDate()}
              zeroPadTime={3}
              zeroPadDays={3}
              renderer={renderer}
            />
          </div>
          <div className="Lineup__body">
            {this.state.pg}
            {this.state.sg}
            {this.state.sf}
            {this.state.pf}
            {this.state.c}
          </div>
          <p
            className="text-center m-2"
            style={{ color: remaining < 0 ? "red" : "black" }}
          >
            Remaining {remaining}K
              </p>
          <ProgressBar players={this.state} />
          <div
            className="text-center mt-3 pb-3 mx-auto position-relative"
            style={{ width: "50%" }}
          >
            <form onSubmit={this.handleSubmit}>
              <button
                id="submit"
                disabled
                className="Lineup__submit-button btn btn-outline-primary"
              >
                Submit
                </button>
            </form>
            <button
              type="button"
              className="btn btn-primary absolute Lineup__info-button"
              data-toggle="modal"
              data-target="#infoModal"
            >
              <i className="fa fa-info mx-auto" aria-hidden="true" />
            </button>
          </div>
        </div>
        <Loader show={this.state.playerLoader} />
        {playerPool()}
        <PlayerModal
          renderChild={this.state.renderChild}
          loader={this.state.modalLoader}
          stats={this.state.stats}
        />
        <InfoModal />
      </div>
    );
  }

  filter(pos) {
    this.setState({
      position: pos
    });
  }

  selectPlayer(player) {
    const pos = player.position.toLowerCase();
    const playerCard = player.selected ? (
      <PlayerCard
        status={2}
        filter={this.filter}
        player={player}
        selectPlayer={this.selectPlayer}
        position={player.position}
        showModal={this.showModal}
      />
    ) : (
        <PlayerCard status={0} filter={this.filter} position={player.position} />
      );
    this.setState({
      [pos]: playerCard
    });
  }

  async showModal(player) {
    this.setState({ modalLoader: true });
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/stats/${player.id}`)
      .then(res => res.json())
      .then(res => {
        this.setState({
          stats: res,
          modalLoader: false,
          renderChild: true
        });
      });
  }

  calculateRemaining() {
    const remaining =
      budget -
      this.price(this.state.pg) -
      this.price(this.state.sg) -
      this.price(this.state.sf) -
      this.price(this.state.pf) -
      this.price(this.state.c);
    return remaining;
  }

  price(player) {
    const playerPrice =
      player.props.status === 2 ? parseInt(player.props.player.price, 10) : 0;
    return playerPrice;
  }

  handleSubmit(e) {
    e.preventDefault();
    const user = parse();
    const data = {
      UserID: user.id,
      PgID: this.state.pg.props.player.playerId,
      SgID: this.state.sg.props.player.playerId,
      SfID: this.state.sf.props.player.playerId,
      PfID: this.state.pf.props.player.playerId,
      CID: this.state.c.props.player.playerId,
      PgPrice: this.state.pg.props.player.price,
      SgPrice: this.state.sg.props.player.price,
      SfPrice: this.state.sf.props.player.price,
      PfPrice: this.state.pf.props.player.price,
      CPrice: this.state.c.props.player.price
    };

    fetch("/api/lineup/submit", {
      method: "POST",
      headers: {
        "Content-type": "application/json"
      },
      body: JSON.stringify(data)
    })
      .then(res => handleErrors(res))
      .then(res => res.text())
      .then(res => {
        this.setState({
          showAlert: true,
          alertType: "alert-success",
          alertText: res
        });
      })
      .catch(err => {
        this.setState({
          showAlert: true,
          alertType: "alert-danger",
          alertText: err.message
        });
      });
  }

  getFormattedDateString(value, word) {
    if (value === 1) {
      return `${value} ${word} `;
    } else if (value > 1) {
      return `${value} ${word}s `;
    }
    return "";
  }
}
