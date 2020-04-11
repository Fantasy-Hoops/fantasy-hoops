import React, {useEffect, useState} from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import List from '@material-ui/core/List';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import Checkbox from '@material-ui/core/Checkbox';
import Button from '@material-ui/core/Button';
import Divider from '@material-ui/core/Divider';
import Avatar from "@material-ui/core/Avatar";
import Alert from "@material-ui/lab/Alert";

const ERROR_MESSAGE = friendsCount => `You must choose up to ${friendsCount} friends!`;

const useStyles = makeStyles(theme => ({
    root: {
        margin: 'auto',
    },
    cardHeader: {
        padding: theme.spacing(1, 2),
    },
    list: {
        width: 200,
        height: 230,
        backgroundColor: theme.palette.background.paper,
        overflow: 'auto',
    },
    button: {
        margin: theme.spacing(0.5, 0),
    },
    avatar: {
        width: theme.spacing(3),
        height: theme.spacing(3),
        marginRight: '1rem',
    }
}));

function not(a, b) {
    return a.filter(user => b.map(usr => usr.id).indexOf(user.id) === -1);
}

function intersection(a, b) {
    return a.filter(user => b.map(usr => usr.id).indexOf(user.id) !== -1);
}

function union(a, b) {
    return [...a, ...not(b, a)];
}

export default function InviteFriends(props) {
    const {userFriends, formProps} = props;
    const {values} = formProps;
    const classes = useStyles();
    const [checked, setChecked] = React.useState([]);
    const [left, setLeft] = React.useState(not(userFriends, values.userFriends));
    const [right, setRight] = React.useState(values.userFriends);
    const [error, setError] = useState(null);
    
    const maxFriendsToInvite = values.entrants - 1;
    
    const leftChecked = intersection(checked, left);
    const rightChecked = intersection(checked, right);

    const handleToggle = user => () => {
        const currentIndex = checked.map(usr => usr.id).indexOf(user.id);
        const newChecked = [...checked];

        if (currentIndex === -1) {
            newChecked.push(user);
        } else {
            newChecked.splice(currentIndex, 1);
        }

        setChecked(newChecked);
    };

    const numberOfChecked = items => intersection(checked, items).length;

    const handleToggleAll = items => () => {
        if (numberOfChecked(items) === items.length) {
            setChecked(not(checked, items));
        } else {
            setChecked(union(checked, items));
        }
    };

    const handleCheckedRight = () => {
        const {values, errors, setFieldValue, setFieldError, validateField} = formProps;
        const rightValues = right.concat(leftChecked);
        const rightValuesTrimmed = right.concat(leftChecked).slice(0, maxFriendsToInvite);

        setError(null);
        if (right.length >= maxFriendsToInvite) {
            setError(<Alert severity="error">{ERROR_MESSAGE(maxFriendsToInvite)}</Alert>);
            return;
        }
        
        if (rightValues.length !== rightValuesTrimmed.length) {
            setError(<Alert severity="error">{ERROR_MESSAGE(maxFriendsToInvite)}</Alert>);
        }
        
        const notAddedUsers = not(rightValues, rightValuesTrimmed);
        
        setRight(rightValuesTrimmed);
        setLeft(not(left, rightValuesTrimmed));
        setChecked(notAddedUsers);
        setFieldValue('userFriends', rightValuesTrimmed);
    };

    const handleCheckedLeft = () => {
        const {setFieldValue} = formProps;

        setError(null);
        const rightValues = not(right, rightChecked);
        setLeft(left.concat(rightChecked));
        setRight(rightValues);
        setChecked([]);
        setFieldValue('userFriends', rightValues);
    };

    const customList = (title, items) => (
        <Card>
            <CardHeader
                className={classes.cardHeader}
                avatar={
                    <Checkbox
                        onClick={handleToggleAll(items)}
                        checked={numberOfChecked(items) === items.length && items.length !== 0}
                        indeterminate={numberOfChecked(items) !== items.length && numberOfChecked(items) !== 0}
                        disabled={items.length === 0}
                        inputProps={{ 'aria-label': 'all items selected' }}
                    />
                }
                title={title}
                subheader={`${numberOfChecked(items)}/${items.length} selected`}
            />
            <Divider />
            <List className={classes.list} dense component="div" role="list">
                {items.map(user => {
                    const labelId = `transfer-list-all-item-${user.username}-label`;
                    return (
                        <ListItem key={user.id} role="listitem" button onClick={handleToggle(user)}>
                            <ListItemIcon>
                                <Checkbox
                                    checked={checked.map(usr => usr.id).indexOf(user.id) !== -1}
                                    tabIndex={-1}
                                    disableRipple
                                    inputProps={{ 'aria-labelledby': labelId }}
                                />
                            </ListItemIcon>
                            <Avatar className={classes.avatar} alt={user.userName} src={user.imageSrc}>
                            </Avatar>
                            <ListItemText id={labelId} primary={user.userName} />
                        </ListItem>
                    );
                })}
                <ListItem />
            </List>
        </Card>
    );
    
    return (
        <>
            <p>
                {'Choose your friends from the list below to send invitations to their emails or finish the creation ' +
                'of the tournament to get a generated invitation link'}
            </p>
            {error}
            <Grid container spacing={2} justify="center" alignItems="center" className={classes.root}>
                <Grid item>{customList('Choices', left)}</Grid>
                <Grid item>
                    <Grid container direction="column" alignItems="center">
                        <Button
                            variant="outlined"
                            size="small"
                            className={classes.button}
                            onClick={handleCheckedRight}
                            disabled={leftChecked.length === 0}
                            aria-label="move selected right"
                        >
                            &gt;
                        </Button>
                        <Button
                            variant="outlined"
                            size="small"
                            className={classes.button}
                            onClick={handleCheckedLeft}
                            disabled={rightChecked.length === 0}
                            aria-label="move selected left"
                        >
                            &lt;
                        </Button>
                    </Grid>
                </Grid>
                <Grid item>{customList('Chosen', right)}</Grid>
            </Grid>
        </>
    );
}
