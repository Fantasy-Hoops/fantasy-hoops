import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
    root: {
        backgroundColor: 'white'
    },
    appBar: {
        backgroundColor: 'white',
        height: '7rem'
    },
    tabPanel: {
        maxHeight: '100%',
        overflowY: 'auto'
    },
    standingsHeading: {
        padding: 5,
        height: '7rem',
        position: 'sticky',
        top: 0,
        zIndex: 3,
        lineHeight: '8rem',
        textAlign: 'center'
    },
    standingsLineHeight: {
        lineHeight: '6rem'
    },
    scheduleButton: {
        height: '100%'
    },
    avatar: {
        width: theme.spacing(6),
        height: theme.spacing(6)
    }
}));