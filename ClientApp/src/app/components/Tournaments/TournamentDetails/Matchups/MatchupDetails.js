import React from 'react';
import Typography from "@material-ui/core/Typography";
import {Avatar} from "@material-ui/core";
import {makeStyles} from "@material-ui/core/styles";

import defaultPhoto from "../../../../../content/images/default.png";
import './MatchupDetails.css';
import Divider from "@material-ui/core/Divider";

const useStyles = makeStyles(theme => ({
    avatar: {
        width: theme.spacing(5),
        height: theme.spacing(5)
    },
    flex: {
        display: 'flex'
    },
    itemText: {
        lineHeight: '3.5rem'
    },
    userStatus: {
        display: 'flex',
        height: '100%',
        alignItems: 'center'
    }
}));

export function MatchupDetails(props) {
    const classes = useStyles();
    const {matchup, heading, future, divider} = props;

    if (!matchup) {
        return null;
    }

    return (
        <>
            {heading && <Typography variant="h5" className="MatchupDetails__Heading">
                {heading}
            </Typography>}
            <div className="MatchupDetails__Layout">
                <div className="MatchupDetails__Item MatchupDetails__FirstUser">
                    <Avatar
                        className={classes.avatar}
                        src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${matchup.firstUser.avatarUrl}.png`}
                    >
                        <img className={classes.avatar} alt="" src={defaultPhoto}/>
                    </Avatar>
                    <Typography className={classes.itemText} variant="subtitle2">
                        {matchup.firstUser.username}
                    </Typography>
                </div>
                <div className={`MatchupDetails__Item MatchupDetails__Status ${!future && 'MatchupDetails__DiagonalSeparator'}`}>
                    <Typography className={classes.userStatus} variant="h5">
                        {!future
                            ? <>
                                <span className="MatchupDetails__FirstUserScore">{matchup.firstUserScore}</span>
                                <span className="MatchupDetails__UsersSeparator">-</span>
                                <span className="MatchupDetails__SecondUserScore">{matchup.secondUserScore}</span>
                            </>
                            : 'vs.'
                        }
                    </Typography>
                </div>
                <div className="MatchupDetails__Item MatchupDetails__SecondUser">
                    <Avatar
                        className={classes.avatar}
                        src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${matchup.secondUser.avatarUrl}.png`}
                    >
                        <img className={classes.avatar} alt="" src={defaultPhoto}/>
                    </Avatar>
                    <Typography className={classes.itemText} variant="subtitle2">
                        {matchup.secondUser.username}
                    </Typography>
                </div>
            </div>
            {divider && <Divider/>}
        </>
    );
}