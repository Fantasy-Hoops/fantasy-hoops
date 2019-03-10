import React, { Component } from "react";

export class Offline extends Component {
  render() {
    return (
      <div className="text-center p-5">
        <img
          className="text-center"
          src={require("../content/images/jordan-crying.png")}
          style={{ height: "12.5rem" }}
          alt="Sad Jordan"
        />
        <h5>
          We're currently undergoing scheduled maintenance.
          <br />
          We will come back very shortly.
          <br />
          Thank you for your patience.
        </h5>
      </div>
    );
  }
}
