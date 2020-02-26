import React from "react";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Slide from "@material-ui/core/Slide";

import './InjuriesInfoDialog.css';

function InjuriesInfoDialog(props) {
    const {handleClose, open} = props;

    return (
        <Dialog
            onClose={handleClose}
            aria-labelledby="injuries-info-dialog"
            open={open}
        >
            <DialogTitle id="injuries-info-dialog" onClose={handleClose}>
                INFO
            </DialogTitle>
            <DialogContent dividers>
                <article>
                    <span className="InjuryStatus__Badge injury-out">
                        <span className="InjuryStats__Badge--text">Out</span>
                    </span>
                    or
                    <span className="InjuryStatus__Badge injury-out">
                        <span className="InjuryStats__Badge--text">Injured</span>
                    </span>
                    <p>Player is definitely not playing (0%)</p>
                    <span className="InjuryStatus__Badge injury-questionable">
                        <span className="InjuryStats__Badge--text">Doubtful</span>
                    </span>
                    <p>Player almost certainly isn't playing (10%)</p>
                    <span className="InjuryStatus__Badge injury-questionable">
                        <span className="InjuryStats__Badge--text">Questionable</span>
                    </span>
                    <p>Player probably won't play, but don't be surprised if he does (30%)</p>
                    <span className="InjuryStatus__Badge injury-questionable">
                        <span className="InjuryStats__Badge--text">Probable</span>
                    </span>
                    <p>Player is expected to play, but there is a chance he sits (80%)</p>
                    <span className="InjuryStatus__Badge injury-active">
                        <span className="InjuryStats__Badge--text">Active</span>
                    </span>
                    <p>Player is definitely playing (100%)</p>
                </article>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default InjuriesInfoDialog;