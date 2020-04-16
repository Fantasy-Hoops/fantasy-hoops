import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
    standingsHeading: {
        padding: 5,
        height: '7.8rem',
        position: 'sticky',
        top: 0,
        zIndex: 3,
        textAlign: 'center'
    },
    standingsLineHeight: {
        lineHeight: '6.8rem'
    }
}));