import {makeStyles} from "@material-ui/styles";

export const useStyles = makeStyles(({breakpoints, spacing}) => ({
    card: {
        width: '100%',
        margin: '1rem auto',
        borderRadius: spacing(2), // 16px
        transition: '0.3s',
        boxShadow: '0px 14px 80px rgba(34, 35, 58, 0.2)',
        position: 'relative',
        overflow: 'initial',
        display: 'flex',
        flexDirection: 'column-reverse',
        alignItems: 'center',
        textAlign: 'center',
        paddingLeft: 8,
        paddingRight: 8,
        background:
            'linear-gradient(34deg, rgba(55,16,83,1) 0%, rgba(162,73,190,1) 29%, rgba(33,16,83,1) 92%)',
        [breakpoints.up('sm')]: {
            textAlign: 'left',
            flexDirection: 'row',
        },
    },
    content: {
        width: '100%',
        [breakpoints.up('sm')]: {
            width: '50%'
        },
    },
    avatar: {
        flexShrink: 0,
        margin: '2rem auto 0',
        width: '7rem',
        height: '7rem',
        [breakpoints.up('sm')]: {
            margin: 'auto 2rem auto auto',
        }
    },
    overline: {
        lineHeight: 2,
        color: '#ffffff',
        fontWeight: 'bold',
        fontSize: '0.9rem',
        opacity: 0.7,
    },
    heading: {
        fontWeight: '900',
        color: '#ffffff',
        letterSpacing: 0.5,
    },
    button: {
        margin: '.5rem 0',
        backgroundColor: 'rgba(255, 255, 255, 0.15)',
        borderRadius: 100,
        paddingLeft: 32,
        paddingRight: 32,
        color: '#ffffff',
        textTransform: 'none',
        width: '100%',
        '&:hover': {
            backgroundColor: 'rgba(255, 255, 255, 0.32)',
        },
        [breakpoints.up('sm')]: {
            width: 'auto',
            marginRight: 10,
        },
    },
    tournamentDetails: {
        display: 'none',
        alignSelf: 'flex-end',
        [breakpoints.up('sm')]: {
            display: 'flex',
            flexDirection: 'column'
        },
    },
    badge : {
        height: '1.5rem',
        minHeight: '1.5rem',
        width: '1.5rem',
        minWidth: '1.5rem',
        backgroundColor: 'green',
        position: 'relative',
        transform: 'translate(0%)'
    }
}));