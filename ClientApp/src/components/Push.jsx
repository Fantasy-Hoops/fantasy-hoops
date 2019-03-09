import React, { Component } from 'react';
import { parse } from '../utils/auth';

export class Push extends Component {
  constructor(props) {
    super(props);
    this.state = {
    }

    this.handleClick = this.handleClick.bind(this);
  }

  handleClick(e) {
    e.preventDefault();
    this.sendMessage('Testing',
      'Dolor ipsum proident sint laborum dolor et sint id do aliquip officia est commodo. ');
  }

  sendMessage(title, message) {
    const notification = {
      title: title,
      body: message
    };

    let apiUrl = `./api/push/send/${parse().id}`;

    return fetch(apiUrl, {
      method: 'post',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(notification)
    });
  }

  render() {
    return (
      <div className="p-5">
        <button type="button" className="btn btn-success" onClick={this.handleClick}>Initiate Push</button>
      </div>
    );
  }
}
