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
    const {achievement, onDialogOpen, className, isLoggedIn, readOnly} = props;

    const handleDialogOpen = achievement => {
        onDialogOpen(achievement);
    };

    if (!achievement) {
        return null;
    }

    const isSingleLevel = achievement.type === 0;

    const checkColor = () => {
        if (!isLoggedIn) {
            return false;
        }

        return achievement.progress === 0 || (isSingleLevel && !achievement.isAchieved);
    };

    const progressBarValue = achievement.progress / achievement.levelUpGoal * 100;
    const color = checkColor() ? 'no' : '';
    return (
        <Card className={`${classes.root} ${!isLoggedIn || readOnly && 'no-pointer-events'}`} onClick={() => handleDialogOpen(achievement)}>
            <CardContent className={classes.content}>
                <Typography className={classes.title}>
                    {achievement.title}
                </Typography>
                <div className="AchievementCard__IconWrapper">
                    <div className="AchievementCard__Icon">
                        <img
                            className="AchievementCard__Icon__Image"
                            alt={achievement.title}
                            src={require(`../../../content/icons/achievements/${color}color${achievement.icon}`)}
                            width="30"
                        />
                    </div>
                </div>
                {
                    isLoggedIn &&
                    <>
                        <Typography className="AchievementCard__Level" variant="body2" component="p">
                            LEVEL {achievement.level}
                        </Typography>
                        <LinearProgress className="AchievementCard__ProgressBar" variant="determinate"
                                        value={progressBarValue}/>
                    </>
                }
            </CardContent>
        </Card>
    );
}

export default AchievementCard;