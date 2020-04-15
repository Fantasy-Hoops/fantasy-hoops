import React from 'react';
import {withStyles} from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import MuiDialogTitle from '@material-ui/core/DialogTitle';
import MuiDialogContent from '@material-ui/core/DialogContent';
import MuiDialogActions from '@material-ui/core/DialogActions';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';
import Typography from '@material-ui/core/Typography';
import DeleteForeverIcon from '@material-ui/icons/DeleteForever';

import './TournamentSettings.css';
import {InputLabel} from "@material-ui/core";
import {ConfirmDialog} from "../../Inputs/ConfirmDialog";
import {deleteTournament} from "../../../utils/networkFunctions";
import FormLabel from "@material-ui/core/FormLabel";
import TextField from "@material-ui/core/TextField";
import Divider from "@material-ui/core/Divider";
import CopyToClipboard from "../../Inputs/CopyToClipboard";
import Routes from "../../../routes/routes";
import {useHistory} from "react-router";
import {TournamentStatus} from "../../../utils/helpers";

const styles = (theme) => ({
    root: {
        margin: 0,
        padding: theme.spacing(2),
    },
    closeButton: {
        position: 'absolute',
        right: theme.spacing(1),
        top: theme.spacing(1),
        color: theme.palette.grey[500],
    },
});

const DialogTitle = withStyles(styles)((props) => {
    const {children, classes, onClose, ...other} = props;
    return (
        <MuiDialogTitle disableTypography className={classes.root} {...other}>
            <Typography variant="h6">{children}</Typography>
            {onClose ? (
                <IconButton aria-label="close" className={classes.closeButton} onClick={onClose}>
                    <CloseIcon/>
                </IconButton>
            ) : null}
        </MuiDialogTitle>
    );
});

const DialogContent = withStyles((theme) => ({
    root: {
        padding: theme.spacing(2),
    },
}))(MuiDialogContent);

const DialogActions = withStyles((theme) => ({
    root: {
        margin: 0,
        padding: theme.spacing(1),
    },
}))(MuiDialogActions);

export function TournamentSettings(props) {
    const {tournamentId, tournament, isSettingsOpen, handleSettingsClose} = props;
    const [confirmOpen, setConfirmOpen] = React.useState(false);

    const handleConfirmOpen = () => {
        setConfirmOpen(true);
    };

    const handleConfirmClose = () => {
        setConfirmOpen(false);
    };

    const handleDeleteTournament = () => {
        handleSettingsClose();
        return deleteTournament(tournamentId);
    };

    return (
        <>
            <Dialog onClose={handleSettingsClose} aria-labelledby="customized-dialog-title" open={isSettingsOpen}
                    fullWidth>
                <DialogTitle id="customized-dialog-title" onClose={handleSettingsClose}>
                    {`${tournament.title} Settings`}
                </DialogTitle>
                <DialogContent className="TournamentSettings__Content" dividers>
                    {tournament.status === TournamentStatus.CREATED && (
                        <>
                            <FormLabel component="legend">Invitation link</FormLabel>
                            <CopyToClipboard
                                inputText={`https://fantasyhoops.org/tournaments/invitations/${tournamentId}`}/>
                            <Divider className="TournamentSettings__Divider"/>
                        </>
                    )}
                    <FormLabel component="legend">Delete tournament</FormLabel>
                    <Button
                        onClick={handleConfirmOpen}
                        variant="text"
                        classes={{label: "TournamentSettings_DeleteButton"}}
                        startIcon={<DeleteForeverIcon/>}
                    >
                        Delete
                    </Button>
                </DialogContent>
                <DialogActions>
                    <Button autoFocus onClick={handleSettingsClose} color="primary">
                        Close
                    </Button>
                </DialogActions>
            </Dialog>
            <ConfirmDialog
                open={confirmOpen}
                handleClose={handleConfirmClose}
                title="Are you sure want to delete this tournament?"
                description="By deleting this tournament you will lose all you progress including your upcoming 
                achievements, matchups' scores, tournament standings and history"
                callbackFunction={handleDeleteTournament}
                locationChange={Routes.TOURNAMENTS}
            />
        </>
    );
}
