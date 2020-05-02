import React, {useEffect} from "react";
import {isAdmin} from "../../utils/auth";
import {Error} from "../Error";
import Tab from "@material-ui/core/Tab";
import Tabs from "@material-ui/core/Tabs";
import Box from "@material-ui/core/Box";
import AppBar from "@material-ui/core/AppBar";
import useTheme from "@material-ui/core/styles/useTheme";
import makeStyles from "@material-ui/core/styles/makeStyles";
import {connect} from "react-redux";
import {bindActionCreators} from "redux";
import * as blogActionCreators from "../../actions/blog";
import * as userActionCreators from "../../actions/userPool";
import {UsersPanel} from "./UsersPanel";
import {BlogPostsPanel} from "./BlogPostsPanel";
import {JobsPanel} from "./JobsPanel";

const mapStateToProps = state => ({
    allUsers: state.userPoolContainerReducer.allUsers,
    pendingPosts: state.blogContainerReducer.pendingPosts
});

const mapDispatchToProps = dispatch => {
    return {
        ...bindActionCreators(blogActionCreators, dispatch),
        ...bindActionCreators(userActionCreators, dispatch)
    }
};

function TabPanel(props) {
    const {children, value, index, ...other} = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`full-width-tabpanel-${index}`}
            aria-labelledby={`full-width-tab-${index}`}
            {...other}
        >
            {value === index && (
                <Box>
                    {children}
                </Box>
            )}
        </div>
    );
}

function a11yProps(index) {
    return {
        id: `tab-${index}`,
        'aria-controls': `tabpanel-${index}`,
    };
}

const useStyles = makeStyles((theme) => ({
    root: {
        width: '100%',
        maxWidth: '50rem',
        margin: '0 auto'
    },
    tabs: {
        margin: '0 auto'
    }
}));

export function AdminPanel(props) {
    const classes = useStyles();
    const theme = useTheme();
    const [value, setValue] = React.useState(0);

    useEffect(() => {
        props.loadUserPool();
        props.loadPendingPosts();
    }, []);

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    const handleChangeIndex = (index) => {
        setValue(index);
    };

    if (!isAdmin()) {
        return <Error status={403} message={"Access forbidden"}/>
    }

    return (
        <div className={classes.root}>
            <h1 className="PageTitle">
                Admin panel
            </h1>
            <AppBar position="static" color="default">
                <Tabs className={classes.tabs}
                    value={value}
                    onChange={handleChange}
                    indicatorColor="primary"
                    textColor="primary"
                    variant="scrollable"
                    scrollButtons="auto"
                >
                    <Tab label="Roles" {...a11yProps(0)} />
                    <Tab label="Jobs" {...a11yProps(1)} />
                    <Tab label="Blog posts" {...a11yProps(2)} />
                </Tabs>
            </AppBar>
            <TabPanel value={value} index={0} dir={theme.direction}>
                <UsersPanel users={props.allUsers}/>
            </TabPanel>
            <TabPanel value={value} index={1} dir={theme.direction}>
                <JobsPanel/>
            </TabPanel>
            <TabPanel value={value} index={2} dir={theme.direction}>
                <BlogPostsPanel pendingPosts={props.pendingPosts} approvePost={props.approveBlogPost}
                                removePost={props.removePost}/>
            </TabPanel>
        </div>
    );
}

export default connect(mapStateToProps, mapDispatchToProps)(AdminPanel);