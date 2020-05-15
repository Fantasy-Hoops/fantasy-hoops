import React, {useState} from 'react';
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
import SaveIcon from '@material-ui/icons/Save';

import './TournamentSettings.css';
import {ConfirmDialog} from "../../Inputs/ConfirmDialog";
import {deleteTournament, simulateTournament, startTournament, updateTournament} from "../../../utils/networkFunctions";
import FormLabel from "@material-ui/core/FormLabel";
import Divider from "@material-ui/core/Divider";
import CopyToClipboard from "../../Inputs/CopyToClipboard";
import Routes from "../../../routes/routes";
import {TournamentStatus} from "../../../utils/helpers";
import {Formik} from "formik";
import _ from "lodash";
import {TextField} from "@material-ui/core";
import FullscreenLoader from "../../FullscreenLoader";
import {useSnackbar} from "notistack";
import {isAdmin} from "../../../utils/auth";
import {updateTournamentValidation} from "../../../utils/validation";

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
    const [loader, setLoader] = useState(false);
    const {enqueueSnackbar} = useSnackbar();

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

    const handleStartTournament = () => {
        handleSettingsClose();
        setLoader(true);
        startTournament(tournamentId)
            .then(response => {
                enqueueSnackbar(response.data, {variant: 'success'});
                window.location.reload();
                setLoader(false);
            })
            .catch(error => {
                enqueueSnackbar(error.message, {variant: 'error'});
                setLoader(false);
            });
    }

    const handleSimulateTournament = () => {
        handleSettingsClose();
        setLoader(true);
        simulateTournament(tournamentId)
            .then(response => {
                enqueueSnackbar(response.data, {variant: 'success'});
                window.location.reload();
                setLoader(false);
            })
            .catch(error => {
                enqueueSnackbar(error.message, {variant: 'error'});
                setLoader(false);
            });
    }

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
                    <FormLabel component="legend">Change tournament info</FormLabel>
                    <Formik
                        validationSchema={updateTournamentValidation}
                        initialValues={{
                            tournamentTitle: tournament.title,
                            tournamentDescription: tournament.description
                        }}
                        onSubmit={(values, actions) => {
                            setLoader(true);
                            handleSettingsClose();
                            updateTournament({
                                tournamentTitle: values.tournamentTitle,
                                tournamentDescription: values.tournamentDescription
                            }, tournamentId)
                                .then(response => {
                                    enqueueSnackbar(response.data, {variant: 'success'});
                                    window.location.reload();
                                    setLoader(false);
                                })
                                .catch(error => {
                                    enqueueSnackbar(error.message, {variant: 'error'});
                                    setLoader(false);
                                })
                        }}
                        render={(formProps) => {
                            const {values, errors, handleSubmit, handleChange, setFieldTouched, touched} = formProps;
                            const change = (name, e) => {
                                e.persist();
                                handleChange(e);
                                setFieldTouched(name, true, false);
                            };
                            return (
                                <>
                                    <TextField
                                        margin="normal"
                                        fullWidth
                                        required
                                        id="tournamentTitle"
                                        label="Title"
                                        name="tournamentTitle"
                                        value={values.tournamentTitle}
                                        onChange={change.bind(null, "tournamentTitle")}
                                        error={touched.tournamentTitle && !_.isEmpty(errors.tournamentTitle)}
                                        helperText={touched.tournamentTitle ? errors.tournamentTitle : ''}
                                    />
                                    <br/>
                                    <TextField
                                        margin="normal"
                                        fullWidth
                                        multiline
                                        required
                                        id="tournamentDescription"
                                        label="Description"
                                        name="tournamentDescription"
                                        value={values.tournamentDescription}
                                        onChange={change.bind(null, "tournamentDescription")}
                                        error={touched.tournamentDescription && !_.isEmpty(errors.tournamentDescription)}
                                        helperText={touched.tournamentDescription ? errors.tournamentDescription : ''}
                                    />
                                    <Button
                                        onClick={handleSubmit}
                                        variant="text"
                                        startIcon={<SaveIcon/>}
                                    >
                                        Update
                                    </Button>
                                </>
                            );
                        }}
                    />
                    <Divider className="TournamentSettings__Divider"/>
                    <FormLabel component="legend">Delete tournament</FormLabel>
                    <Button
                        onClick={handleConfirmOpen}
                        variant="text"
                        classes={{label: "TournamentSettings_DeleteButton"}}
                        startIcon={<DeleteForeverIcon/>}
                    >
                        Delete
                    </Button>
                    {isAdmin() && tournament.status === TournamentStatus.CREATED && (
                        <>
                            <Divider className="TournamentSettings__Divider"/>
                            <FormLabel component="legend">Start Tournament</FormLabel>
                            <Button
                                onClick={handleStartTournament}
                                variant="text"
                            >
                                Start
                            </Button>
                        </>
                    )}
                    {isAdmin() && tournament.status === TournamentStatus.ACTIVE && (
                        <>
                            <Divider className="TournamentSettings__Divider"/>
                            <FormLabel component="legend">Simulate Tournament</FormLabel>
                            <Button
                                onClick={handleSimulateTournament}
                                variant="text"
                            >
                                Simulate
                            </Button>
                        </>
                    )}

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
            {loader && <FullscreenLoader/>}
        </>
    );
}
