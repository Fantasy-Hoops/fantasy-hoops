/* eslint-disable react/jsx-filename-extension */
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import browserHistory from './app/utils/history';
import configureStore from './app/store';
import configureRoutes from './app/routes';
import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';

const rootElement = document.getElementById('root');

ReactDOM.render(
  <Provider store={configureStore()}>
    <ConnectedRouter history={browserHistory}>
      {configureRoutes()}
    </ConnectedRouter>
  </Provider>,
  rootElement
);
