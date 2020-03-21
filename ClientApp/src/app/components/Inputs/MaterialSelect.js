import React from 'react';
import {makeStyles} from '@material-ui/core/styles';
import InputLabel from '@material-ui/core/InputLabel';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import _ from "lodash";
import MenuItem from "@material-ui/core/MenuItem";
import FormHelperText from "@material-ui/core/FormHelperText";

const useStyles = makeStyles(theme => ({
    formControl: {
        margin: theme.spacing(1),
        width: 175
    },
    selectEmpty: {
        marginTop: theme.spacing(2),
    },
}));

export default function MaterialSelect(props) {
    const {id, label, values, value, onChange, emptyOption, disabled, required, error, helperText} = props;
    const classes = useStyles();

    const handleChange = event => {
        onChange(event);
    };

    const menuItems = _.map(values, (item, index) => (
        <MenuItem key={index} value={item.value}>{item.label}</MenuItem>
    ));

    return (
        <FormControl required={required} error={!disabled && error} className={classes.formControl}>
            <InputLabel htmlFor="age-native-simple">{label}</InputLabel>
            <Select
                id={id}
                name={id}
                value={value}
                onChange={handleChange}
                disabled={disabled}
            >
                {emptyOption && <MenuItem value="">
                    <em>None</em>
                </MenuItem>}
                {menuItems}
            </Select>
            {!disabled && <FormHelperText>{helperText}</FormHelperText>}
        </FormControl>
    );
}