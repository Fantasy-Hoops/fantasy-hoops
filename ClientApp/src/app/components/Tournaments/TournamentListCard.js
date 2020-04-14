import React from 'react';
import Button from '@material-ui/core/Button';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import {Avatar} from "@material-ui/core";
import moment from "moment";
import {TOURNAMENT_DATE_FORMAT} from "../../utils/helpers";
import clsx from "clsx";
import {Link} from "react-router-dom";
import Routes from "../../routes/routes";
import {useStyles} from "./TournamentListCardStyle";
import Badge from "@material-ui/core/Badge";

export default function TournamentListCard(props) {
    const classes = useStyles();
    const {tournament, isInvitation, clickable, handleAcceptInvitation, handleDeclineInvitation} = props;
    const clickableProps = clickable
        ? {
            component: Link,
            to: `${Routes.TOURNAMENT_INVITATIONS}/${tournament.id}`
        }
        : {};

    function getButtons() {
        if (!isInvitation && !clickable) {
            return (
                <Link to={`${Routes.TOURNAMENTS_SUMMARY}/${tournament.id}`}>
                    <Button className={classes.button}>View Summary</Button>
                </Link>
            );
        }

        if (isInvitation && !clickable) {
            return (
                <>
                    <Button className={classes.button} onClick={handleAcceptInvitation}>Accept</Button>
                    <Button className={classes.button} onClick={handleDeclineInvitation}>Decline</Button>
                </>
            );
        }

        return null;
    }

    return (
        <Card className={classes.card} {...clickableProps}>
            <CardContent className={classes.content}>
                <Typography className={classes.overline} variant={'overline'}>
                    {tournament.description}
                </Typography>
                <Typography className={classes.heading} variant={'h5'} gutterBottom>
                    {tournament.title}
                </Typography>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {tournament.typeName}
                </Typography>
                {getButtons()}
            </CardContent>
            {!clickable && <CardContent className={clsx(classes.content, classes.tournamentDetails)}>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {
                        tournament.isActive
                        && <><Badge classes={{badge: classes.badge}} badgeContent={""}/> Active</>
                    }
                </Typography>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {`${moment(tournament.startDate).isBefore() ? 'Started' : 'Starts'}
                    ${moment(tournament.startDate).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
                <Typography className={classes.heading} variant="subtitle2" gutterBottom>
                    {`${moment(tournament.endDate).isBefore() ? 'Ended' : 'Ends'}
                    ${moment(tournament.endDate).format(TOURNAMENT_DATE_FORMAT)}`}
                </Typography>
            </CardContent>}
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