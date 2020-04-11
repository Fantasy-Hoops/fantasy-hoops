import {Link} from "react-router-dom";
import Routes from "../../routes/routes";
import Divider from "@material-ui/core/Divider";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import React from "react";
import {makeStyles} from "@material-ui/core/styles";
import {isAuth} from "../../utils/auth";
import {Route} from "../../utils/locationSlug";

const useStyles = makeStyles(theme => ({
    listItemText: {
        fontWeight: 600
    }
}));

function MobileDrawer(props) {
    const isAuthenticatedUser = isAuth();
    const {selectedTab, handleDrawerClose} = props;
    const classes = useStyles();
    return (
        <div>
            <Link id="NavbarLogo" className={`navbar-brand btn-no-outline`} to={Routes.MAIN}
                  onClick={() => handleDrawerClose()}>
                <img className="Navbar__Icon" src={require('../../../content/images/logo.png')} width="35" height="35"
                     alt="FH"/>
                <img className="Navbar__Title ml-2 mt-2" src={require('../../../content/images/title.png')} height="30"
                     alt="Fantasy Hoops"/>
            </Link>
            <Divider/>
            <List>
                {isAuthenticatedUser
                && <ListItem button component={Link} to={Routes.LINEUP} onClick={() => handleDrawerClose()}
                             selected={selectedTab === Route.lineup}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/lineup.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} className={classes.listItemText}
                                  primary="Lineup"/>
                </ListItem>}
                <ListItem button component={Link} to={Routes.LEADERBOARDS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.leaderboard}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/leaderboards.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Leaderboards"/>
                </ListItem>
                {isAuthenticatedUser
                &&
                <ListItem button component={Link} to={Routes.TOURNAMENTS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.tournaments}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/tournaments.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Tournaments"/>
                </ListItem>}
                <ListItem button component={Link} to={Routes.ACHIEVEMENTS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.achievements}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/achievements.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Achievements"/>
                </ListItem>
                <ListItem button component={Link} to={Routes.INJURIES} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.injuries}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/injuries.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Injuries"/>
                </ListItem>
                <ListItem button component={Link} to={Routes.NEWS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.news}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/navigation/news.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="News"/>
                </ListItem>
            </List>
            <Divider/>
            <List>
                {isAuthenticatedUser
                && <ListItem button component={Link} to={Routes.USER_POOL} onClick={() => handleDrawerClose()}
                             selected={selectedTab === Route.users}>
                    <ListItemIcon>
                        <img width="40" height="40"
                             src={require('../../../content/icons/navigation/users.svg')}
                             alt=""/>
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Users"/>
                </ListItem>}
                <ListItem button component={Link} to={Routes.BLOG} onClick={() => handleDrawerClose()}
                          selected={selectedTab === Route.blog}>
                    <ListItemIcon>
                        <img width="35" height="35" src={require('../../../content/icons/navigation/blog.svg')} alt=""/>
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Blog"/>
                </ListItem>
            </List>
        </div>
    );
}

export default MobileDrawer;