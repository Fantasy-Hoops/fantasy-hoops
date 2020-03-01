import React from 'react';
import ReactDOM from 'react-dom';
import {Provider} from 'react-redux';
import * as Sentry from '@sentry/browser';
import configureStore from './app/store';
import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';
import { createBrowserHistory } from 'history';
import App from "./App";
import {Router} from "react-router-dom";

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
    Sentry.init({dsn: 'https://5029893979d14e1ba1eabd3f946bb03b@sentry.io/1820868'});
}

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const history = createBrowserHistory({ basename: baseUrl });
const rootElement = document.getElementById('root');
const store = configureStore(history);

ReactDOM.render(
        <Provider store={store}>
            <Router history={history}>
                <App />
            </Router>
        </Provider>,
    rootElement
);
