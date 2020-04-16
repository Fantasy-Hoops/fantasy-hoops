import {makeStyles} from "@material-ui/core/styles";

export const useStyles = makeStyles(theme => ({
    button: {
        borderRadius: 3,
        border: 0,
        color: 'white',
        height: 28,
        padding: '0 30px',
        boxShadow: '0 3px 5px 2px rgba(255, 105, 135, .3)',
    },
    buttonLeft: {
        background: 'linear-gradient(45deg, #F1592A 30%, #FF8E53 90%)',
    },
    buttonRight: {
        background: 'linear-gradient(45deg, #FF8E53 30%, #F1592A 90%)',
    }
}));