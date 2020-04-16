import {createMuiTheme} from "@material-ui/core/styles";

export const theme = createMuiTheme({
    body: {
        fontFamily: [
            "Lato",
            'sans-serif'
        ],
        fontWeight: 400,
        fontSize: '1.6rem',
        lineHeight: '1.7',
        margin: 0, // Remove the margin in all browsers.
        backgroundColor: 'red',
        '@media print': {
            // Save printer ink.
            backgroundColor: 'red',
        },
    },
    typography: {
        // Tell Material-UI what's the font-size on the html element is.
        htmlFontSize: 10,
    },
    palette: {
        primary: {
            main: '#2c3e50'
        },
        secondary: {
            main: '#F1592A'
        },
        tertiary: {
            main: '#ecf0f1'
        }
    },
    status: {
        danger: {
            main: '#F1592A'
        },
    },
});