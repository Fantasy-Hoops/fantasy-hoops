import React from "react";
import _ from 'lodash';
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import DialogContent from "@material-ui/core/DialogContent";
import Dialog from "@material-ui/core/Dialog";
import Slide from "@material-ui/core/Slide";
import makeStyles from "@material-ui/core/styles/makeStyles";
import {Card as UserScoreCard} from "../../../Leaderboard/Users/Card";
import EmptyJordan from "../../../EmptyJordan";
import leaderboardLogo from "../../../../../content/icons/1021175-winning/svg/006-winner-5.svg";

const useStyles = makeStyles(theme => ({
    table: {
        maxWidth: '100%',
    },
    appBar: {
        position: 'relative',
    },
    title: {
        marginLeft: theme.spacing(2),
        flex: 1,
    },
    stepper: {
        maxWidth: '70rem',
        margin: '2rem auto',
        padding: 0
    },
    tableContainer: {
        padding: 0
    },
    dialogContent: {
        display: 'flex',
        flexDirection: 'column'
    }
}));

const Transition = React.forwardRef(function Transition(props, ref) {
    return <Slide direction="up" ref={ref} {...props} />;
});

export function ContestLeaderboard(props) {
    const classes = useStyles();
    const {handleLeaderboardClose, open, matchups, index} = props;

    const contestLeaderboard = matchups.map((matchup, index) => {
        const user = {
            username: matchup.firstUser.username,
            avatarUrl: matchup.firstUser.avatarUrl,
            fp: matchup.firstUserScore
        };
        return <UserScoreCard key={index} index={index} user={user}/>;
    });

    return (
        <Dialog
            open={open}
            onClose={handleLeaderboardClose}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
            fullScreen
            TransitionComponent={Transition}
        >
            <AppBar className={classes.appBar}>
                <Toolbar>
                    <IconButton edge="start" color="inherit" onClick={handleLeaderboardClose} aria-label="close">
                        <CloseIcon/>
                    </IconButton>
                    <Typography variant="h6" className={classes.title}>
                        Contest #{index} Leaderboard
                    </Typography>
                    <Button autoFocus color="inherit" onClick={handleLeaderboardClose}>
                        Close
                    </Button>
                </Toolbar>
            </AppBar>
            <DialogContent className={classes.dialogContent}>
                {!_.isEmpty(matchups)
                    ? (
                        <>
                            <div className="text-center">
                                <img
                                    src={leaderboardLogo}
                                    alt="Leaderboard Logo"
                                    width="60rem"
                                />
                                <h1>Top Users</h1>
                            </div>
                            {contestLeaderboard}
                        </>
                    )
                    : <div className="m-auto"><EmptyJordan message="Contest leaderboard not available..."/></div>}
            </DialogContent>
        </Dialog>
    );
}