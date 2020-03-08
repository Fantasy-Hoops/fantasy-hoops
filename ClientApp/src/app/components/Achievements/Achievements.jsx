import React, {useState, useEffect} from 'react';
import {getExistingAchievements, getUserAchievements} from "../../utils/networkFunctions";
import _ from 'lodash';
import AchievementCard from "./AchievementCard";
import AchievementDialog from "./AchievementDialog";

import './Achievements.css';
import {isAuth} from "../../utils/auth";

function Achievements(props) {
    const isLoggedIn = isAuth();
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

        async function handleGetExistingAchievements() {
            await getExistingAchievements()
                .then(response => setAchievements(response.data))
                .catch(err => console.error(err.message));
        }

        if (isLoggedIn) {
            handleGetUserAchievements();
        } else {
            handleGetExistingAchievements();
        }
    }, []);

    const handleDialogOpen = achievement => {
        setDialogAchievement(achievement);
        setDialogOpen(true);
    };

    const handleDialogClose = () => {
        setDialogOpen(false);
    };

    function parseExistingAchievements() {
        return _.map(achievements, (ach, key) => {
                const achievement = {
                    id: ach.achievement ? ach.achievement.id : ach.id,
                    type: ach.achievement ? ach.achievement.type : ach.type,
                    title: ach.achievement ? ach.achievement.title : ach.title,
                    description: ach.achievement ? ach.achievement.description : ach.description,
                    completedMessage: ach.achievement ? ach.achievement.completedMessage : ach.achievement,
                    icon: ach.achievement ? ach.achievement.icon : ach.icon,
                    goalBase: ach.achievement ? ach.achievement.goalBase : ach.goalBase,
                    progress: ach.progress,
                    level: ach.level,
                    levelUpGoal: ach.levelUpGoal,
                    isAchieved: ach.isAchieved
                };
                return <AchievementCard isLoggedIn={isLoggedIn} readOnly={readOnly} key={key}
                                        achievement={achievement}
                                        onDialogOpen={handleDialogOpen}/>
            }
        )
    }

    return (
        <div className="tab-pane" id="achievements">
            <div className="Achievements">
                {parseExistingAchievements()}
                {dialogOpen && dialogAchievement ?
                    <AchievementDialog open={dialogOpen} readOnly={readOnly} handleClose={handleDialogClose}
                                       achievement={dialogAchievement}/> : null}
            </div>
        </div>
    );
}

export default Achievements;