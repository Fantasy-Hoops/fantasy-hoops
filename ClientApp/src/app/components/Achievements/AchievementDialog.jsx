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
    return (
        <Dialog maxWidth="xs" classes={{paper: 'AchievementDialog__Paper'}} onClose={handleClose} aria-labelledby="customized-dialog-title" open={open}>
            <HighlightOffIcon className="AchievementDialog__CloseIcon" onClick={handleClose} />
            <DialogContent>
                <div className="AchievementCard__Icon">
                    <img
                        className="AchievementCard__Icon__Image"
                        alt={achievement.title}
                        src={require(`../../../content/icons${achievement.icon}`)}
                        width="30"
                    />
                </div>
                <p className="AchievementDialog__Title">{achievement.title}</p>
                <Typography gutterBottom>
                    {achievement.description}
                </Typography>
                <LinearProgress classes={{root: 'AchievementDialog__ProgressBar'}} variant="determinate" value={70}/>
                <p className="AchievementDialog__ProgressNumbers">4/7</p>
            </DialogContent>
        </Dialog>
    );
}

export default AchievementDialog;