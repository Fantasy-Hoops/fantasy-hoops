import React from "react";
import {SnackbarProvider} from "notistack";

export default function Snackbar(props) {
    return (
        <SnackbarProvider
            classes={{
                variantSuccess: 'Snackbar',
                variantError: 'Snackbar',
                variantWarning: 'Snackbar',
                variantInfo: 'Snackbar',
            }}
            maxSnack={3}
        >
            {props.children}
        </SnackbarProvider>
    )
}