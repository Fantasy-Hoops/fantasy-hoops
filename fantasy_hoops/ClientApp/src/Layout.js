import * as React from 'react';
import Header from "./app/components/Navigation/Header";
import Container from "@material-ui/core/Container";
import CoronaAlert from "./app/components/CoronaAlert";


export default (props) => (
    <>
        <Header />
        <Container maxWidth="md">
            <CoronaAlert/>
            {props.children}
        </Container>
    </>
);
