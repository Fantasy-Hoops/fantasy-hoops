/* eslint-disable react/jsx-filename-extension */
import React from 'react';
import ReactDOM from 'react-dom';
import {Provider} from 'react-redux';
import {BrowserRouter} from 'react-router-dom';
import * as Sentry from '@sentry/browser';
import browserHistory from './app/utils/history';
import configureStore from './app/store';
import configureRoutes from './app/routes';
import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';

// :root {
//     --primary-color: #2c3e50;
//     --secondary-color: #F1592A;
//     --tertiary-color: #ecf0f1;
//     --grey-text: #939598;
//     --grey-background: #f5f5f5;
// }

import {createMuiTheme} from '@material-ui/core/styles';
import {ThemeProvider} from "@material-ui/styles";

const theme = createMuiTheme({
    body: {
        fontFamily: [
            "Lato",
            'sans-serif'
        ],
        fontWeight: 400,
        fontSize: '1.6rem',
        lineHeight: '1.7',
        margin: 0, // Remove the margin in all browsers.
        backgroundColor: 'red',
        '@media print': {
            // Save printer ink.
            backgroundColor: 'red',
        },
    },
    typography: {
        // Tell Material-UI what's the font-size on the html element is.
        htmlFontSize: 10,
    },
    palette: {
        primary: {
            main: '#2c3e50'
        },
        secondary: {
            main: '#F1592A'
        },
        tertiary: {
            main: '#ecf0f1'
        }
    },
    status: {
        danger: {
            main: '#F1592A'
        },
    },
});

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
    Sentry.init({dsn: 'https://5029893979d14e1ba1eabd3f946bb03b@sentry.io/1820868'});
}

const rootElement = document.getElementById('root');

ReactDOM.render(
    <ThemeProvider theme={theme}>
        <Provider store={configureStore()}>
            <BrowserRouter history={browserHistory}>
                {configureRoutes()}
            </BrowserRouter>
        </Provider>
    </ThemeProvider>,
    rootElement
);
