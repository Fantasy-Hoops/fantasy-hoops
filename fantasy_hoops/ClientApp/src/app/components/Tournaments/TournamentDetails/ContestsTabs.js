import React from 'react';
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import moment from "moment";
import AppBar from "@material-ui/core/AppBar";
import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
    appBar: {
        backgroundColor: 'white',
        height: '100%'
    },
    tabs: {
        height: '100%'
    },
    flexContainer: {
        height: '100%'
    }
}));

function a11yProps(index) {
    return {
        id: `scrollable-force-tab-${index}`,
        'aria-controls': `scrollable-force-tabpanel-${index}`,
    };
}

export function ContestsTabs(props) {
    const classes = useStyles();
    const {tournament, handleChange, value} = props;
    
    return (
        <AppBar className={classes.appBar} position="static" color="default">
            <Tabs
                className={classes.tabs}
                classes={{
                    flexContainer: classes.flexContainer
                }}
                value={value}
                onChange={handleChange}
                variant="scrollable"
                scrollButtons="on"
                indicatorColor="primary"
                textColor="primary"
                aria-label="scrollable force tabs example"
            >
                {tournament.contests.map((contest, index) => (
                    <Tab key={index} label={moment(contest.contestStart).format('MM/DD/YYYY')}
                         icon={<div>{`Contest #${index + 1}`}</div>} {...a11yProps(index)} />
                ))}
            </Tabs>
        </AppBar>  
    );
}