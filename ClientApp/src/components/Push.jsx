import React, { Component } from 'react';
import { subscribePush, saveSubscription, deleteSubscription } from '../utils/push';
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
    this.registerPush()
      .then(() => {
        this.sendMessage('Fantasy Hoops',
          'Blagadariu nx 200FP surinkai!', 5000);
      });
  }

  sendMessage(title, message, delay) {
    const notification = {
      title: title,
      body: message,
      //tag: 'demo_testmessage' //we don't want a tag, as it would cause every notification after the first one to appear in the notification drawer only
    };

    const userId = localStorage.getItem('userId');
    let apiUrl = `./api/push/send/${parse().id}`;
    if (delay) apiUrl += `?delay=${delay}`;

    return fetch(apiUrl, {
      method: 'post',
      headers: {
        'Content-type': 'application/json'
      },
      body: JSON.stringify(notification)
    });
  }

  subscribePushAndUpdateButtons(registration) {
    return subscribePush(registration).then((subscription) => {
      return subscription;
    });
  }

  unsubscribePush() {
    return this.getPushSubscription().then((subscription) => {
      return subscription.unsubscribe().then(function () {
        deleteSubscription(subscription);
      });
    });
  }

  getPushSubscription() {
    return navigator.serviceWorker.ready
      .then((registration) => {
        return registration.pushManager.getSubscription();
      });
  }

  registerPush() {
    return navigator.serviceWorker.ready
      .then((registration) => {
        return registration.pushManager.getSubscription().then((subscription) => {
          if (subscription) {
            // // renew subscription if we're within 5 days of expiration
            if (subscription.expirationTime && Date.now() > subscription.expirationTime - 432000000) {
              return this.unsubscribePush().then(() => {
                return this.subscribePushAndUpdateButtons(registration);
              });
            }

            return subscription;
          }
          return this.subscribePushAndUpdateButtons(registration);
        });
      })
      .then((subscription) => {
        return saveSubscription(subscription);
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
