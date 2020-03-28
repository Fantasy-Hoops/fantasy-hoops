import React from 'react';
import {makeStyles} from '@material-ui/styles';
import Button from '@material-ui/core/Button';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import {Avatar} from "@material-ui/core";
import moment from "moment";
import {TOURNAMENT_DATE_FORMAT} from "../../utils/helpers";

const useStyles = makeStyles(({breakpoints, spacing}) => ({
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
    avatar: {
        flexShrink: 0,
        margin: '1rem auto 0',
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
        },
    },
    tournamentDetails: {
        display: 'none',
        alignSelf: 'flex-end',
        [breakpoints.up('sm')]: {
            display: 'flex',
            flexDirection: 'column'
        },
    }
}));

export default function TournamentListCard(props) {
    const classes = useStyles();
    const {tournament} = props;
    console.log(tournament);
    return (
        <Card className={classes.card}>
            <CardContent className={classes.content}>
                <Typography className={classes.overline} variant={'overline'}>
                    {tournament.description}
                </Typography>
                <Typography className={classes.heading} variant={'h5'} gutterBottom>
                    {tournament.name}
                </Typography>
                <Button className={classes.button}>View Summary</Button>
            </CardContent>
            <CardContent className={classes.tournamentDetails}>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {`${moment(tournament.startDate).isBefore()? 'Started' : 'Starts'}
                    ${moment(tournament.startDate).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {`${moment(tournament.endDate).isBefore()? 'Ended' : 'Ends'}
                    ${moment(tournament.endDate).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
            </CardContent>
            <Avatar
                className={classes.avatar}
                src={require(`../../../content/icons/tournaments/${tournament.imageURL}`)}
                imgProps={{
                    width: '50',
                    height: '50'
                }}
            />
        </Card>
    );
};