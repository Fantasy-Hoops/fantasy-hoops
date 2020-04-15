import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
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
    }
}));