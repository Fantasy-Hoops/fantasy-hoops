import React from 'react';
import Avatar from "@material-ui/core/Avatar";
import IconButton from "@material-ui/core/IconButton";

import './TournamentIcon.css';

export default function TournamentIcon(props) {
    const {handleSetSelectedIcon, iconPath, uniqueKey, customProps} = props;
    return (
        <IconButton
            className={`TournamentIcon${customProps.selected ? '--Selected' : ''}`}
            onClick={() => handleSetSelectedIcon(iconPath, uniqueKey)}
        >
            <Avatar
                src={iconPath}
                style={{
                    margin: "10px",
                    width: "60px",
                    height: "60px",
                }}
            />
        </IconButton>
    );
}