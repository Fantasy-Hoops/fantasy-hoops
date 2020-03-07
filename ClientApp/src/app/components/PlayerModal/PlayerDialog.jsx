import React, {useEffect, useState} from 'react';
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import {Stats} from "./Stats";
import Gamelog from "./Gamelog";
import {Charts} from "./Charts";

import './PlayerDialog.css';
import {useStyles} from "./PlayerDialogStyles";

/**
 * @return {null}
 */
function PlayerDialog(props) {
    const {onDialogOpen, onDialogClose, open, stats, image, renderChild} = props;
    const [playerStats, setPlayerStats] = useState(null);
    const classes = useStyles();
    
    useEffect(() => {
        if (stats) {
            setPlayerStats(stats);
        }
    }, [stats]);

    function handleDialogClose() {
        onDialogClose();
        setPlayerStats(null);
    }

    return (
        <Dialog
            id="PlayerDialog"
            maxWidth="xl"
            onClose={handleDialogClose}
            aria-labelledby="player-dialog"
            open={open || false}
        >
            <DialogContent className={classes.content} dividers>
                {
                    open && playerStats
                        ? (
                            <>
                                <Stats stats={playerStats} image={image}/>
                                <nav>
                                    <div className="nav nav-pills mb-3" id="nav-tab" role="tablist">
                                        <a className="nav-item nav-link active tab-no-outline" id="nav-gamelog-tab"
                                           data-toggle="tab" href="#nav-gamelog" role="tab" aria-controls="nav-gamelog"
                                           aria-selected="false">Gamelog</a>
                                        <a className="nav-item nav-link tab-no-outline" id="nav-charts-tab"
                                           data-toggle="tab"
                                           href="#nav-charts" role="tab" aria-controls="nav-charts"
                                           aria-selected="false">Charts</a>
                                    </div>
                                </nav>
                                <div className="tab-content" id="nav-tabContent">
                                    <div className="tab-pane fade show active" id="nav-gamelog" role="tabpanel"
                                         aria-labelledby="nav-gamelog-tab">
                                        {renderChild ? <Gamelog stats={playerStats}/> : null}
                                    </div>
                                    <div className="tab-pane fade" id="nav-charts" role="tabpanel"
                                         aria-labelledby="nav-charts-tab">
                                        <Charts stats={playerStats}/>
                                    </div>
                                </div>
                            </>
                        )
                        : (
                            <div className="p-5">
                                <div className="Loader"/>
                            </div>
                        )
                }
            </DialogContent>
            <DialogActions>
                <Button onClick={handleDialogClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default PlayerDialog;