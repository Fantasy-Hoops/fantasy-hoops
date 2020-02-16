import React, {useState, useEffect} from 'react';
import {getExistingAchievements} from "../../utils/networkFunctions";
import _ from 'lodash';
import Achievement from "./Achievement";

function Achievements() {
    const [achievements, setAchievements] = useState([]);

    useEffect(() => {
        async function handleGetExistingAchievements() {
            const achievements = await getExistingAchievements()
                .then(response => response.data)
                .catch(err => console.error(err.message));
            setAchievements(achievements);
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

    return (
        <div className="tab-pane" id="achievements">
            {parseExistingAchievements()}
        </div>
    );

    function parseExistingAchievements() {
        return _.map(achievements, (achievement, key) => (
            <Achievement key={key} achievement={achievement}/>
        ))
    }
}

export default Achievements;