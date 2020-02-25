import React, {useState, useEffect} from 'react';
import {useStyles} from "./AchievementCardStyle";
import CardContent from "@material-ui/core/CardContent";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import LinearProgress from "@material-ui/core/LinearProgress";

import './AchievementCard.css';

function AchievementCard(props) {
    const classes = useStyles();
    const {achievement, onDialogOpen} = props;
    
    const handleDialogOpen = achievement => {
        onDialogOpen(achievement);
    };
    
    return (
        <Card className={classes.root} onClick={() => handleDialogOpen(achievement)}>
            <CardContent className={classes.content}>
                <Typography className={classes.title}>
                    {achievement.title}
                </Typography>
                <div className="AchievementCard__Icon">
                    <img
                        className="AchievementCard__Icon__Image"
                        alt={achievement.title}
                        src={require(`../../../content/icons${achievement.icon}`)}
                        width="30"
                    />
                </div>
                <Typography className="AchievementCard__Level" variant="body2" component="p">
                    LEVEL 1
                </Typography>
                <LinearProgress variant="determinate" value={70} />
            </CardContent>
        </Card>
    );
}

export default AchievementCard;