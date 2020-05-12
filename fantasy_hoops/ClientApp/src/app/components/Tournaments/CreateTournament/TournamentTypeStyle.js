import makeStyles from "@material-ui/core/styles/makeStyles";

export const useStyles = makeStyles(theme => ({
    table: {
        minWidth: 650,
    },
    tableRow: {
        '&:nth-of-type(odd)': {
            backgroundColor: theme.palette.background.default,
        },
    },
    textField: {
        margin: theme.spacing(1),
        width: 175
    },
    scheduleEntry: {
        display: 'inline-block',
        minWidth: '20rem'
    },
    blockIcon: {
        color: 'red'
    }
}));