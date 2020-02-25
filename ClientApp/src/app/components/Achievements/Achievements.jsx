import React, {useState, useEffect} from 'react';
import {getExistingAchievements} from "../../utils/networkFunctions";
import _ from 'lodash';
import AchievementCard from "./AchievementCard";
import AchievementDialog from "./AchievementDialog";

function Achievements() {
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogAchievement, setDialogAchievement] = useState(null);
    const [achievements, setAchievements] = useState([]);

    useEffect(() => {
        async function handleGetExistingAchievements() {
            await getExistingAchievements()
                .then(response => setAchievements(response.data))
                .catch(err => console.error(err.message));
        }

        handleGetExistingAchievements();
    }, []);

    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return (
            <div className="tab-pane" id="achievements">
                <h2>Coming soon!</h2>
            </div>
        );
    }

    const handleDialogOpen = achievement => {
        setDialogAchievement(achievement);
        setDialogOpen(true);
    };
    
    const handleDialogClose = () => {
        setDialogOpen(false);
    };

    return (
        <div className="tab-pane" id="achievements">
            {parseExistingAchievements()}
            {dialogAchievement ? <AchievementDialog open={dialogOpen} handleClose={handleDialogClose} achievement={dialogAchievement} /> : null}
        </div>
    );

    function parseExistingAchievements() {
        return _.map(achievements, (achievement, key) => (
            <AchievementCard key={key} achievement={achievement} onDialogOpen={handleDialogOpen}/>
        ))
    }
}

export default Achievements;