import React, {useState} from "react";
import _ from 'lodash';
import EmptyJordan from "../EmptyJordan";
import {Table, TableHead} from "@material-ui/core";
import TableContainer from "@material-ui/core/TableContainer";
import Paper from "@material-ui/core/Paper";
import TableRow from "@material-ui/core/TableRow";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import Checkbox from "@material-ui/core/Checkbox";
import {Formik} from "formik";
import {isAdmin, isCreator} from "../../utils/auth";
import SaveIcon from '@material-ui/icons/Save';
import {updateUserRoles} from "../../utils/networkFunctions";
import FullscreenLoader from "../FullscreenLoader";
import {useSnackbar} from "notistack";
import Fab from "@material-ui/core/Fab";
import makeStyles from "@material-ui/core/styles/makeStyles";

const useStyles = makeStyles((theme) => ({
    fab: {
        position: 'fixed',
        bottom: theme.spacing(2),
        right: theme.spacing(2),
    }
}));

const tableHead = (
    <TableHead style={{background: 'black'}}>
        <TableRow>
            <TableCell style={{color: 'white'}}>
                Username
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Admin
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Creator
            </TableCell>
        </TableRow>
    </TableHead>
);

const renderUserList = (users, formProps) => _.map(users, (user, id) => {
    const {values, errors, touched, handleChange, setFieldTouched, setFieldValue} = formProps;

    const isUserAdmin = isAdmin(user.roles);
    const isUserCreator = isCreator(user.roles);

    const change = (name, e) => {
        setFieldValue(`[${user.id}]`, {
            userId: user.id,
            userRoles: {
                ...values[user.id] && values[user.id].userRoles,
                [name.substring(name.indexOf('-') + 1)]: e.target.checked,
            }
        });
    };

    return (
        <TableRow key={id}>
            <TableCell>
                {user.userName}
            </TableCell>
            <TableCell>
                <Checkbox
                    id={`${id}-admin`}
                    name={`${id}-admin`}
                    defaultChecked={isUserAdmin}
                    onChange={e => change(`${id}-admin`, e)}
                    inputProps={{'aria-label': 'primary checkbox'}}
                />
            </TableCell>
            <TableCell>
                <Checkbox
                    id={`${id}-creator`}
                    name={`${id}-creator`}
                    defaultChecked={isUserCreator}
                    onChange={e => change(`${id}-creator`, e)}
                    inputProps={{'aria-label': 'primary checkbox'}}
                />
            </TableCell>
        </TableRow>
    );
});

export function UsersPanel(props) {
    const {enqueueSnackbar} = useSnackbar();
    const classes = useStyles();
    const [loader, setLoader] = useState(false);
    const {users} = props;

    if (_.isEmpty(users)) {
        return <EmptyJordan message="No users..."/>
    }

    return (
        <Formik
            initialValues={{}}
            onSubmit={(values, actions) => {
                setLoader(true);
                actions.setSubmitting(true);
                updateUserRoles(Object.values(values))
                    .then(response => {
                        enqueueSnackbar(response.data, {variant: 'success'});
                        setLoader(false);
                    })
                    .catch(error => {
                        enqueueSnackbar(error.message, {variant: 'error'});
                        setLoader(false);
                    });
                actions.setSubmitting(false);
            }}
            render={(formProps) => {
                const {values, errors, touched, handleChange, setFieldTouched, setFieldValue, handleSubmit} = formProps;
                return (
                    <>
                        <TableContainer component={Paper}>
                            <Table>
                                {tableHead}
                                <TableBody>
                                    {renderUserList(props.users, formProps)}
                                </TableBody>
                            </Table>
                        </TableContainer>
                        <Fab
                            className={classes.fab}
                            onClick={handleSubmit}
                            disabled={_.isEmpty(values)}
                        >
                            <SaveIcon/>
                        </Fab>
                        {loader && <FullscreenLoader/>}
                    </>
                )
            }}/>
    );
}