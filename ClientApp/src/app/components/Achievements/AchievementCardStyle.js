import { makeStyles } from '@material-ui/core/styles';

export const useStyles = makeStyles({
    root: {
        maxWidth: '10rem',
        maxHeight: '12rem',
        margin: '.5rem'
    },
    content: {
        padding: '1rem !important',
    },
    title: {
        textAlign: 'center',
        fontSize: 16,
    }
});