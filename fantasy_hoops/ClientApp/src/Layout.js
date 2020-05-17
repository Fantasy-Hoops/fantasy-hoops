import * as React from 'react';
import Header from "./app/components/Navigation/Header";
import Container from "@material-ui/core/Container";
import CoronaAlert from "./app/components/CoronaAlert";
import {makeStyles} from "@material-ui/core/styles";
import {CssBaseline} from "@material-ui/core";

export const useStyles = makeStyles(theme => ({
    root: {
        position: 'relative',
        padding: '3rem 1rem',
        background: '#fafafa'
    }
}));

export default (props) => (
    <>
        <Header />
        <CssBaseline />
        <Container className={useStyles().root} maxWidth="md">
            <CoronaAlert/>
            {props.children}
        </Container>
    </>
);
