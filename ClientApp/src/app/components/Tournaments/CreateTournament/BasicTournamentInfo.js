import React from "react";
import _ from 'lodash';
import Avatar from "@material-ui/core/Avatar";

import './BasicTournamentInfo.css';
import TournamentIcon from "./TournamentIcon";
import {TextField} from "@material-ui/core";

function importAll(r) {
    return r.keys().map(r);
}
const iconFiles = importAll(require.context('../../../../content/icons/tournaments', false, /\.(png|jpe?g|svg)$/));

export default function BasicTournamentInfo(props) {
    const {formProps} = props;
    const { values, errors, touched, handleChange, setFieldTouched, setFieldValue } = formProps;
    
    function handleSetSelectedIcon(selectedIcon, selectedIndex) {
        // icons.forEach((icon, index) => {
        //     if (index === selectedIndex) {
        //         icon.props.customProps.selected = true;
        //     } else {
        //         icon.props.customProps.selected = false;
        //     }
        // });
        setFieldValue('tournamentIcon', selectedIcon, true);
    }
    
    const change = (name, e) => {
        e.persist();
        handleChange(e);
        setFieldTouched(name, true, false);
    };

    const icons = iconFiles.map((iconPath, index) => (
        <TournamentIcon iconPath={iconPath} key={index} uniqueKey={index} handleSetSelectedIcon={handleSetSelectedIcon} customProps={{selected: false}}/>
    ));
    
    return (
        <>
            <p>Select the avatar for your tournament, create a title and description.</p>
            <Avatar
                src={values.tournamentIcon}
                style={{
                    margin: "3rem",
                    width: "15rem",
                    height: "15rem",
                }}
            />
            <div className="BasicTournamentInfo__IconsWrapper">
                <div className="BasicTournamentInfo__Icons__TopFade" />
                <div className="BasicTournamentInfo__Icons">
                    {icons}
                </div>
                <div className="BasicTournamentInfo__Icons__BottomFade" />
            </div>
            <TextField
                margin="normal"
                fullWidth
                required
                id="tournamentTitle"
                label="Title"
                name="tournamentTitle"
                value={values.tournamentTitle}
                onChange={change.bind(null, "tournamentTitle")}
                error={touched.tournamentTitle && !_.isEmpty(errors.tournamentTitle)}
                helperText={touched.tournamentTitle ? errors.tournamentTitle : ''}
            />
            <br />
            <TextField
                margin="normal"
                fullWidth
                multiline
                required
                id="tournamentDescription"
                label="Description"
                name="tournamentDescription"
                value={values.tournamentDescription}
                onChange={change.bind(null, "tournamentDescription")}
                error={touched.tournamentDescription && !_.isEmpty(errors.tournamentDescription)}
                helperText={touched.tournamentDescription ? errors.tournamentDescription : ''}
            />
        </>
    );
}