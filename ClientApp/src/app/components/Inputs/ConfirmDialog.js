import React, {useState} from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import FullscreenLoader from "../FullscreenLoader";
import {useSnackbar} from "notistack";

export function ConfirmDialog(props) {
    const {open, handleClose, title, description, callbackFunction, locationChange} = props;
    const [loader, setLoader] = useState(false);
    const {enqueueSnackbar} = useSnackbar();
    
    function handleCallbackFunction() {
        setLoader(true);
        handleClose();
        callbackFunction().then(response => {
            enqueueSnackbar(response.data, {variant: 'success'});
            locationChange && window.location.replace(locationChange);
            !locationChange && setLoader(false);
        }).catch(error => {
            enqueueSnackbar(error.message, {variant: 'error'});
            setLoader(false);
        });
    }
    
    return (
        <>
            <Dialog
                maxWidth="xs"
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{title}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        {description}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={handleCallbackFunction} color="primary" autoFocus>
                        Confirm
                    </Button>
                </DialogActions>
            </Dialog>
            {loader && <FullscreenLoader/>}
        </>
    );
}
