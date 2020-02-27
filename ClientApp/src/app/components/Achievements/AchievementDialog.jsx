import React from 'react';
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import Typography from "@material-ui/core/Typography";
import Dialog from "@material-ui/core/Dialog";
import LinearProgress from "@material-ui/core/LinearProgress";
import HighlightOffIcon from '@material-ui/icons/HighlightOff';

import './AchievementDialog.css';

function AchievementDialog(props) {
    const {achievement, handleClose, open} = props;
    const progressBarValue = achievement.progress / achievement.levelUpGoal * 100;
    return (
        <Dialog maxWidth="xs" classes={{paper: 'AchievementDialog__Paper'}} onClose={handleClose} aria-labelledby="achievement-dialog" open={open}>
            <HighlightOffIcon className="AchievementDialog__CloseIcon" onClick={handleClose} />
            <DialogContent>
                <div className="AchievementCard__Icon">
                    <img
                        className="AchievementCard__Icon__Image"
                        alt={achievement.achievement.title}
                        src={require(`../../../content/icons${achievement.achievement.icon}`)}
                        width="30"
                    />
                </div>
                <p className="AchievementDialog__Title">{achievement.achievement.title}</p>
                <Typography gutterBottom>
                    {achievement.achievement.description}
                </Typography>
                <LinearProgress classes={{root: 'AchievementDialog__ProgressBar'}} variant="determinate" value={progressBarValue}/>
                <p className="AchievementDialog__ProgressNumbers">{`${achievement.progress}/${achievement.levelUpGoal}`}</p>
            </DialogContent>
        </Dialog>
    );
}

export default AchievementDialog;