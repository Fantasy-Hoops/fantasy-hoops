import React from 'react';
import {makeStyles} from '@material-ui/core/styles';
import Paper from '@material-ui/core/Paper';
import InputBase from '@material-ui/core/InputBase';
import Divider from '@material-ui/core/Divider';
import IconButton from '@material-ui/core/IconButton';
import FileCopyIcon from '@material-ui/icons/FileCopy';
import Tooltip from "@material-ui/core/Tooltip";
import {useSnackbar} from "notistack";

const useStyles = makeStyles(theme => ({
    root: {
        padding: '2px 4px',
        display: 'flex',
        alignItems: 'center',
    },
    input: {
        marginLeft: theme.spacing(1),
        flex: 1,
    },
    iconButton: {
        padding: 10,
    },
    divider: {
        height: 28,
        margin: 4,
    },
}));

export default function CopyToClipboard(props) {
    const classes = useStyles();
    const {inputText} = props;
    const {enqueueSnackbar} = useSnackbar();
    
    function copyToClipboard() {
        navigator.clipboard.writeText(inputText)
            .then(() => enqueueSnackbar("Invitation link copied to clipboard.", {variant: 'success', preventDuplicate: true}))
            .catch(() => enqueueSnackbar("Copying to clipboard is not supported.", {variant: 'error', preventDuplicate: true}));
    }

    return (
        <Paper component="form" className={classes.root}>
            <InputBase
                className={classes.input}
                value={inputText}
                inputProps={{'aria-label': 'copy to clipboard'}}
                fullWidth
                readOnly
            />
            <Divider className={classes.divider} orientation="vertical"/>
            <Tooltip title="Copy to Clipboard">
                <IconButton onClick={copyToClipboard} color="primary" className={classes.iconButton} aria-label="directions">
                    <FileCopyIcon/>
                </IconButton>
            </Tooltip>
        </Paper>
    );
}
