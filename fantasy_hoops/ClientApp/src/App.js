import {ThemeProvider} from "@material-ui/styles";
import React, {useEffect, useState} from "react";
import {theme} from "./Theme";
import Layout from "./Layout";
import Routes from "./app/routes/Routes.jsx";

import './scss/bootswatch.scss';
import './css/custom-styles.css';
import './css/random-letters.css';
import './css/PageStyles.css';
import ScrollToTop from "./ScrollToTop";
import Snackbar from "./Snackbar";
import {getUpdatedToken} from "./app/utils/networkFunctions";

export default () => {
    const [isTokenUpdated, setIsTokenUpdated] = useState(false);
    useEffect(() => {
        if (!localStorage.getItem('accessToken')) {
            return;
        }
        getUpdatedToken()
            .then(response => {
                localStorage.setItem('accessToken', response.data);
            }).catch(console.error);
    }, []);

    return (
        <ThemeProvider theme={theme}>
            <Snackbar>
                <ScrollToTop>
                    <Layout>
                        <Routes/>
                    </Layout>
                </ScrollToTop>
            </Snackbar>
        </ThemeProvider>
    )
};