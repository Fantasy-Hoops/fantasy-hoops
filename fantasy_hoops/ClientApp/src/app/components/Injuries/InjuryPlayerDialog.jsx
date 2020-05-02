import React from 'react';
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import Img from "react-image";

import './InjuryPlayerDialog.css';
import {useStyles} from "./InjuryPlayerDialogStyles";
import {Container} from "@material-ui/core";
import defaultLogo from "../../../content/images/defaultLogo.png";

/**
 * @return {null}
 */
function InjuryPlayerDialog(props) {
    const classes = useStyles();
    const {open, handleDialogClose, injury} = props;

    if (injury == null) {
        return null;
    }

    let status;
    if (injury.status.toLowerCase().includes('active')) {
        status = 'injury-active';
    } else if (
        injury.status.toLowerCase().includes('out')
        || injury.status.toLowerCase().includes('injured')
        || injury.status.toLowerCase().includes('suspended')) {
        status = 'injury-out';
    } else status = 'injury-questionable';

    const teamLogo = (
        <Img
            width="60%"
            className="InjuryCard__TeamLogo--behind"
            alt={injury.player.team.abbreviation}
            src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${injury.player.team.abbreviation}.svg`,
                defaultLogo]}
            decode={false}
        />
    );
    return (
        <Dialog
            onClose={handleDialogClose}
            aria-labelledby="injury-player-dialog"
            open={open}
        >
            <DialogContent className={classes.dialogContent}>
                <div className="InjuryPlayerDialog__Image" style={{backgroundColor: injury.player.team.color}}>
                    <div className="InjuryCard__PlayerPosition InjuryPlayerDialog__PlayerPosition badge">
                        {injury.player.position}
                    </div>
                    {teamLogo}
                    <Img
                        className="InjuryPlayerDialog__PlayerImage"
                        width="80%"
                        alt={injury.player.fullName}
                        src={
                            [
                                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${injury.player.nbaID}.png`,
                                require(`../../../content/images/positions/${injury.player.position.toLowerCase()}.png`)
                            ]
                        }
                        loader={(
                            <img
                                className="InjuryCard__loader"
                                src={require('../../../content/images/imageLoader2.gif')}
                                alt="Loader"
                            />
                        )}
                        decode={false}
                    />
                    <div className={`InjuryCard__StatusBadge ${status}`}>
                        <span style={{opacity: 0.9}}>{injury.status}</span>
                    </div>
                </div>
                <Container>
                    <article>
                        <h2 className="InjuryPlayerDialog__Title">{injury.title.slice(0, -1)}</h2>
                        <p className="InjuryPlayerDialog__Description">{injury.description}</p>
                        {injury.link
                            ? <a href={injury.link} target="_blank">
                                <i className="fas fa-external-link-alt"/>{' Source'}
                            </a>
                            : null}
                    </article>
                </Container>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleDialogClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default InjuryPlayerDialog;