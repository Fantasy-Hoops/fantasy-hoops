export const registerServiceWorker = () => {
  return navigator.serviceWorker.register('../sw.js');
}