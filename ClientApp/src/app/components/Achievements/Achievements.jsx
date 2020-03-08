import React, {useState, useEffect} from 'react';
import {getExistingAchievements, getUserAchievements} from "../../utils/networkFunctions";
import _ from 'lodash';
import AchievementCard from "./AchievementCard";
import AchievementDialog from "./AchievementDialog";

import './Achievements.css';
import {isAuth, parse} from "../../utils/auth";
import {Helmet} from "react-helmet";
import {Intro} from "./utils";
import {Canonicals} from "../../utils/helpers";

function Achievements(props) {
    const user = isAuth();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [dialogAchievement, setDialogAchievement] = useState(null);
    const [achievements, setAchievements] = useState([]);
    const {readOnly} = props;


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

        if (user) {
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
                return <AchievementCard className={readOnly && 'no-pointer-events'} readOnly={!user} key={key}
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
                    <AchievementDialog open={dialogOpen} readOnly={!user} handleClose={handleDialogClose}
                                       achievement={dialogAchievement}/> : null}
            </div>
        </div>
    );
}

export default Achievements;