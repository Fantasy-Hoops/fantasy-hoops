import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
    button: {
        marginTop: theme.spacing(1),
        marginRight: theme.spacing(1),
    },
    resetContainer: {
        padding: theme.spacing(3),
    },
    table: {
        minWidth: 650,
    },
    small: {
        width: theme.spacing(3),
        height: theme.spacing(3),
    },
    title: {
        fontSize: '3rem',
        margin: '1rem'
    }
}));