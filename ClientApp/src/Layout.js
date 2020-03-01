import * as React from 'react';
import Header from "./app/components/Navigation/Header";
import Container from "@material-ui/core/Container";


export default (props) => (
    <>
        <Header />
        <Container maxWidth="md">
            {props.children}
        </Container>
    </>
);
