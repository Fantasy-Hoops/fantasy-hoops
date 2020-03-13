import {Alert} from "@material-ui/lab";
import React from "react";
import AlertTitle from "@material-ui/lab/AlertTitle";

export default function CoronaAlert() {
    return <Alert className="CoronaAlert" severity="error">
        <AlertTitle>The COVID-19 Suspension</AlertTitle>
        Due to Coronavirus the NBA season is suspended as well as Fantasy Hoops.
        <br />
        We will be back as soon as the NBA season continues.
    </Alert>;
}