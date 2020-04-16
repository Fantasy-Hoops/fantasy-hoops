import React from "react";
import Typography from "@material-ui/core/Typography";
import moment from "moment";
import {TOURNAMENT_DATE_FORMAT} from "../../../utils/helpers";

export function TournamentNotStarted(props) {
    const {contest} = props;
    
    return (
        <>
            <Typography variant="h4" className="MatchupDetails__Heading">
                Tournament hasn't started yet
            </Typography>
            <Typography variant="subtitle1" className="MatchupDetails__Heading">
                Contest starts {moment(contest.contestStart).format(TOURNAMENT_DATE_FORMAT)}
            </Typography>
        </>
    )
}