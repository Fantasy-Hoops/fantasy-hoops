import {ThemeProvider} from "@material-ui/styles";
import React, {useEffect} from "react";
import {theme} from "./Theme";
import Layout from "./Layout";
import Routes from "./app/routes/Routes.jsx";

import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';

function ScrollToTop(props) {
    useEffect(() => {
        if ('scrollRestoration' in history) {
            history.scrollRestoration = 'manual';
        }
        window.scrollTo(0, 0);
    });
    return props.children;
}

export default () => (
    <ThemeProvider theme={theme}>
        <ScrollToTop>
            <Layout>
                <Routes/>
            </Layout>
        </ScrollToTop>
    </ThemeProvider>
);