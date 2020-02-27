import React, {useState, useEffect} from 'react';
import {useStyles} from "./AchievementCardStyle";
import CardContent from "@material-ui/core/CardContent";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import LinearProgress from "@material-ui/core/LinearProgress";

import './AchievementCard.css';

/**
 * @return {null}
 */
function AchievementCard(props) {
    const classes = useStyles();
    const {achievement, onDialogOpen, className} = props;
    
    const handleDialogOpen = achievement => {
        onDialogOpen(achievement);
    };
    
    if (!achievement) {
        return null;
    }
    
    const progressBarValue = achievement.progress / achievement.levelUpGoal * 100;
    return (
        <Card className={`${classes.root} ${className}`} onClick={() => handleDialogOpen(achievement)}>
            <CardContent className={classes.content}>
                <Typography className={classes.title}>
                    {achievement.achievement.title}
                </Typography>
                <div className="AchievementCard__Icon">
                    <img
                        className="AchievementCard__Icon__Image"
                        alt={achievement.achievement.title}
                        src={require(`../../../content/icons${achievement.achievement.icon}`)}
                        width="30"
                    />
                </div>
                <Typography className="AchievementCard__Level" variant="body2" component="p">
                    LEVEL {achievement.level}
                </Typography>
                <LinearProgress variant="determinate" value={progressBarValue} />
            </CardContent>
        </Card>
    );
}

export default AchievementCard;