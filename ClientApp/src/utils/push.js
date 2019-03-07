import { parse } from './auth';

export const urlB64ToUint8Array = (base64String) => {
  const padding = '='.repeat((4 - base64String.length % 4) % 4);
  const base64 = (base64String + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);

  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }

  return outputArray;
}

export const subscribePush = (registration) => {
  return getPublicKey().then(function (key) {
    return registration.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: key
    });
  });
}

export const getPublicKey = () => {
  return fetch('./api/push/vapidpublickey')
    .then(function (response) {
      return response.json();
    })
    .then(function (data) {
      return urlB64ToUint8Array(data);
    });
}

export const saveSubscription = (subscription) => {
  return fetch(`./api/push/subscribe/${parse().id}`, {
    method: 'post',
    headers: {
      'Content-type': 'application/json'
    },
    body: JSON.stringify({
      subscription: subscription
    })
  })
    .then(response => response.json())
    .then(response => {
      localStorage.setItem('userId', response.userId);
    });
}

export const deleteSubscription = (subscription) => {
  return fetch('./api/push/unsubscribe', {
    method: 'post',
    headers: {
      'Content-type': 'application/json'
    },
    body: JSON.stringify({
      subscription: subscription
    })
  });
}
