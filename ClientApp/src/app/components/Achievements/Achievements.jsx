import React, {useState, useEffect} from 'react';
import {getExistingAchievements, getUserAchievements} from "../../utils/networkFunctions";
import _ from 'lodash';
import AchievementCard from "./AchievementCard";
import AchievementDialog from "./AchievementDialog";

import './Achievements.css';

function Achievements(props) {
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogAchievement, setDialogAchievement] = useState(null);
    const [achievements, setAchievements] = useState([]);
    const {user, readOnly} = props;
    
    useEffect(() => {
        async function handleGetUserAchievements() {
            await getUserAchievements(user.id)
                .then(response => setAchievements(response.data))
                .catch(err => console.error(err.message));
        }

        handleGetUserAchievements();
    }, []);

    const handleDialogOpen = achievement => {
        setDialogAchievement(achievement);
        setDialogOpen(true);
    };
    
    const handleDialogClose = () => {
        setDialogOpen(false);
    };

    function parseExistingAchievements() {
        return _.map(achievements, (achievement, key) => (
            <AchievementCard className={readOnly && "no-pointer-events"} key={key} achievement={achievement} onDialogOpen={handleDialogOpen}/>
        ))
    }
    
    return (
        <div className="tab-pane" id="achievements">
            <div className="Achievements">
                {parseExistingAchievements()}
                {dialogAchievement ? <AchievementDialog open={dialogOpen} handleClose={handleDialogClose}
                                                        achievement={dialogAchievement}/> : null}
            </div>
        </div>
    );
}

export default Achievements;