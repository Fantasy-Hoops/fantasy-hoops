import { parse } from './auth';

export const urlB64ToUint8Array = (base64String) => {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding)
    .replace(/-/g, '+')
    .replace(/_/g, '/');
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);

  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }

  return outputArray;
};

export const getPublicKey = () => fetch('./api/push/vapidpublickey')
  .then(response => response.json())
  .then(data => urlB64ToUint8Array(data));

export const subscribePush = registration => getPublicKey()
  .then(key => registration.pushManager.subscribe({
    userVisibleOnly: true,
    applicationServerKey: key
  }));

export const saveSubscription = subscription => fetch(`./api/push/subscribe/${parse().id}`, {
  method: 'post',
  headers: {
    'Content-type': 'application/json'
  },
  body: JSON.stringify({
    subscription
  })
})
  .then(response => response.json())
  .then((response) => {
    localStorage.setItem('userId', response.userId);
  });

export const deleteSubscription = subscription => fetch('./api/push/unsubscribe', {
  method: 'post',
  headers: {
    'Content-type': 'application/json'
  },
  body: JSON.stringify({
    subscription
  })
});

export const unsubscribePush = () => this.getPushSubscription()
  .then(subscription => subscription.unsubscribe().then(() => {
    deleteSubscription(subscription);
  }));

export const getPushSubscription = () => navigator.serviceWorker.ready
  .then(registration => registration.pushManager.getSubscription());

export const registerPush = () => navigator.serviceWorker.ready
  .then(registration => {
    if (registration.pushManager) {
      registration.pushManager.getSubscription().then((subscription) => {
        if (subscription) {
          // // renew subscription if we're within 5 days of expiration
          if (subscription.expirationTime && Date.now() > subscription.expirationTime - 432000000) {
            return unsubscribePush()
              .then(() => subscribePush(registration)
                .then(sub => sub));
          }
          return subscription;
        }
        return subscribePush(registration);
      })
    }
  })
  .then(subscription => {
      if (subscription)
        saveSubscription(subscription)
    }
  );