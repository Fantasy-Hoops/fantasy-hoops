import React from 'react';
import {makeStyles} from '@material-ui/core/styles';
import InputLabel from '@material-ui/core/InputLabel';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import _ from "lodash";
import MenuItem from "@material-ui/core/MenuItem";

const useStyles = makeStyles(theme => ({
    formControl: {
        margin: theme.spacing(1),
        width: 200
    },
    selectEmpty: {
        marginTop: theme.spacing(2),
    },
}));

export default function MaterialSelect(props) {
    const {id, label, values, value, onChange} = props;
    const classes = useStyles();

    const handleChange = event => {
        onChange(event);
    };

    const menuItems = _.map(values, (item, index) => (
        <MenuItem key={index} value={item.value}>{item.label}</MenuItem>
    ));

    return (
        <FormControl className={classes.formControl}>
            <InputLabel htmlFor="age-native-simple">{label}</InputLabel>
            <Select
                id={id}
                name={id}
                value={value}
                onChange={handleChange}
            >
                <MenuItem value="">
                    <em>None</em>
                </MenuItem>
                {menuItems}
            </Select>
        </FormControl>
    );
}