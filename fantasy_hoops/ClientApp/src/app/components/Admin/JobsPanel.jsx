import React from "react";
import {TableBody, TableContainer, TableHead} from "@material-ui/core";
import TableRow from "@material-ui/core/TableRow";
import TableCell from "@material-ui/core/TableCell";
import Table from "@material-ui/core/Table";
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import IconButton from "@material-ui/core/IconButton";
import {triggerJob} from "../../utils/networkFunctions";
import {useSnackbar} from "notistack";

const JOBS = [
    {
        title: "NewsJob",
        endpoint: "news"
    },
    {
        title: "PlayersJob",
        endpoint: "players"
    },
    {
        title: "AchievementsJob",
        endpoint: "achievements"
    },
    {
        title: "BestLineupJob",
        endpoint: "best-lineup"
    },
    {
        title: "CalculateTournamentsJob",
        endpoint: "calculate-tournaments"
    },
    {
        title: "LeagueScheduleJob",
        endpoint: "league-schedule"
    },
];

const tableHead = (
    <TableHead style={{background: 'black'}}>
        <TableRow>
            <TableCell style={{color: 'white'}}>
                Job title
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Actions
            </TableCell>
        </TableRow>
    </TableHead>
);

export function JobsPanel(props) {
    const {enqueueSnackbar} = useSnackbar();

    function handleTriggerJob(endpoint) {
        triggerJob(endpoint)
            .then(response => enqueueSnackbar(response.data, {variant: 'success'}))
            .catch(error => enqueueSnackbar(error.message, {variant: 'error'}));
    };

    const createRow = (job, index) => (
        <TableRow key={index}>
            <TableCell>
                {job.title}
            </TableCell>
            <TableCell>
                <IconButton size="small" style={{color: 'green'}} onClick={() => handleTriggerJob(job.endpoint)}>
                    <PlayArrowIcon/>
                </IconButton>
            </TableCell>
        </TableRow>
    );

    function createRows() {
        return JOBS.map((job, index) => createRow(job, index));
    }

    return (
        <TableContainer>
            <Table>
                {tableHead}
                <TableBody>
                    {createRows()}
                </TableBody>
            </Table>
        </TableContainer>
    )
}