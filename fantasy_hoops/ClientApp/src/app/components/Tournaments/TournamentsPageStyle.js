import makeStyles from "@material-ui/core/styles/makeStyles";

export const useStyles = makeStyles((theme) => ({
    fab: {
        marginBottom: theme.spacing(2),
        [theme.breakpoints.up('sm')]: {
            marginBottom: 0,
        },
    },
    extendedIcon: {
        marginRight: theme.spacing(1),
    },
}));