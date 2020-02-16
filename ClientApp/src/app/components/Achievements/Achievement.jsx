import React, { useState, useEffect } from 'react';

function Achievement({achievement}) {
    
    return (
        <div className="">
            <h3 className="">
                {achievement.title}
            </h3>
            <p>
                {achievement.description}
            </p>
            <img
                alt={achievement.title}
                src={require(`../../../content/icons${achievement.icon}`)}
                width="30"
            />
        </div>
    );
}

export default Achievement;