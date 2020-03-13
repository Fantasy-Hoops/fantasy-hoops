import * as React from 'react';
import Header from "./app/components/Navigation/Header";
import Container from "@material-ui/core/Container";
import CoronaAlert from "./app/components/CoronaAlert";


export default (props) => (
    <>
        <Header />
        <CoronaAlert/>
        <Container maxWidth="md">
            {props.children}
        </Container>
    </>
);
