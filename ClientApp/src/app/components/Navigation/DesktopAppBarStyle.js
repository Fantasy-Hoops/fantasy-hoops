import {makeStyles} from "@material-ui/core/styles";

const drawerWidth = 240;
export const useStyles = makeStyles(theme => ({
    navbarToolbar: {
        height: 'fit-content',
        minHeight: '4.8rem',
        display: 'none',
        overflowX: 'auto',
        [theme.breakpoints.up('sm')]: {
            display: 'block'
        }
    },
    tabs: {
        margin: '0 auto',
    },
    tab: {
        minWidth: 150,
        width: 150,
        color: 'white'
    },
    appBar: {
        height: '5.5rem',
        zIndex: 1050,
        display: 'flex',
        flexWrap: 'wrap',
        flexDirection: 'row',
        justifyContent: 'flex-start',
        [theme.breakpoints.up('sm')]: {
            width: '100%',
            marginLeft: drawerWidth,
            height: '9.6rem',
            justifyContent: 'center',
        },
    },
    menuButton: {
        order: 0,
        height: 'fit-content',
        width: 'fit-content',
        margin: 0,
        [theme.breakpoints.up('sm')]: {
            display: 'none'
        },
    },
    menuIcon: {
        width: '2.5rem',
        height: '2.5rem'
    },
    toolbar: theme.mixins.toolbar,
    sectionDesktop: {
        order: 3,
        width: 'fit-content',
        display: 'flex',
        margin: '0 .5rem 0 auto',
        height: '100%',
        [theme.breakpoints.up('sm')]: {
            width: 'auto',
            order: 0,
            height: '3rem',
            margin: '1rem 1rem .6rem auto'
        },
    },
    logo: {
        order: 2,
        height: 'fit-content',
        width: 'fit-content',
        margin: '.6rem 0',
        [theme.breakpoints.up('sm')]: {
            order: 0,
            width: 'auto',
            margin: '.6rem 50% .6rem 1rem'
        },
    },
    avatar: {
        padding: 0,
        margin: 0
    }
}));