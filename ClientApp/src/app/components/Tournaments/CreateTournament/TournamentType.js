import React, {useEffect, useState} from 'react';
import MaterialSelect from "../../Inputs/MaterialSelect";
import {getTournamentTypes} from "../../../utils/networkFunctions";

export default function TournamentType(props) {
    const [tournamentTypes, setTournamentTypes] = useState([]);
    
    useEffect(() => {
        async function handleGetTournamentTypes() {
            getTournamentTypes().then(response => {
                const tournamentTypes = response.data.map(tournament => ({
                    value: tournament.id,
                    label: tournament.name
                }));
                setTournamentTypes(tournamentTypes);
            }).catch(error => console.error(error.message));
        }
        
        handleGetTournamentTypes();
    }, []);
    
    const {formProps} = props;
    const {values, errors, touched, handleChange, setFieldTouched, setFieldValue} = formProps;

    const change = (name, e) => {
        e.persist();
        handleChange(e);
        setFieldTouched(name, true, false);
    };
    
    return (
        <>
            <p>Select tournament type, start date, number of contests, number of dropped contests and maximum number of entrants (if applicable).</p>
            <MaterialSelect
                id="startDate"
                label="Start Date"
                values={[]}
                value={values.startDate}
                onChange={change.bind(null, "startDate")}
            />
            <MaterialSelect
                id="tournamentType"
                label="Tournament Type"
                values={tournamentTypes}
                value={values.tournamentType}
                onChange={change.bind(null, "tournamentType")}
            />
            <br/>
            <MaterialSelect
                id="contests"
                label="Contests"
                values={[]}
                value={values.contests}
                onChange={change.bind(null, "contests")}
            />
            <MaterialSelect
                id="droppedContests"
                label="Dropped Contests"
                values={[]}
                value={values.droppedContests}
                onChange={change.bind(null, "droppedContests")}
            />
            <MaterialSelect
                id="maximumEntrants"
                label="Maximum Entrants"
                values={[]}
                value={values.maximumEntrants}
                onChange={change.bind(null, "maximumEntrants")}
            />
        </>
    );
}