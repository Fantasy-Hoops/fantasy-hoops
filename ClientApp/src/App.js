import {ThemeProvider} from "@material-ui/styles";
import React, {useEffect} from "react";
import {theme} from "./Theme";
import Layout from "./Layout";
import Routes from "./app/routes/Routes.jsx";

import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';
import ScrollToTop from "./ScrollToTop";
import Snackbar from "./Snackbar";

export default () => (
    <ThemeProvider theme={theme}>
        <Snackbar>
            <ScrollToTop>
                <Layout>
                    <Routes/>
                </Layout>
            </ScrollToTop>
        </Snackbar>
    </ThemeProvider>
);