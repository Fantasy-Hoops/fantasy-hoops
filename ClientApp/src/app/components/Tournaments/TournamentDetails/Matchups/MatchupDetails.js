import React from 'react';
import Typography from "@material-ui/core/Typography";
import {Avatar} from "@material-ui/core";
import {makeStyles} from "@material-ui/core/styles";

import defaultPhoto from "../../../../../content/images/default.png";
import './MatchupDetails.css';

const useStyles = makeStyles(theme => ({
    avatar: {
        width: theme.spacing(6),
        height: theme.spacing(6)
    }
}));

export function MatchupDetails(props) {
    const classes = useStyles();
    const {matchup, heading, future} = props;
    
    if (!matchup) {
        return  null;
    }

    return (
        <>
            {heading && <Typography variant="h5" className="MatchupDetails__Heading">
                {heading}
            </Typography>}
            <div className="MatchupDetails">
                <Typography className="MatchupDetails__Item"
                            variant="subtitle2">
                    {matchup.firstUser.username}
                </Typography>
                <Avatar
                    className={classes.avatar}
                    src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${matchup.firstUser.avatarUrl}.png`}
                >
                    <img className={classes.avatar} alt="" src={defaultPhoto}/>
                </Avatar>
                <Typography className="MatchupDetails__Item" variant="h5">
                    {!future
                        ? `${matchup.firstUserScore} - ${matchup.secondUserScore}`
                        : 'vs.'
                    }
                </Typography>
                <Avatar
                    className={classes.avatar}
                    src={`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${matchup.secondUser.avatarUrl}.png`}
                >
                    <img className={classes.avatar} alt="" src={defaultPhoto}/>
                </Avatar>
                <Typography className="MatchupDetails__Item"
                            variant="subtitle2">
                    {matchup.secondUser.username}
                </Typography>
            </div>
        </>
    );
}