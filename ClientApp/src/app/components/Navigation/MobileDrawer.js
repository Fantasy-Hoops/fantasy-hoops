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

const useStyles = makeStyles(theme => ({
    listItemText: {
        fontWeight: 600
    }
}));

function MobileDrawer(props) {
    const {selectedTab, handleDrawerClose} = props;
    const classes = useStyles();
    return (
        <div>
            <Link id="NavbarLogo" className={`navbar-brand btn-no-outline`} to={Routes.MAIN}  onClick={() => handleDrawerClose()}>
                <img className="Navbar__Icon" src={require('../../../content/images/logo.png')} width="35" height="35"
                     alt="FH"/>
                <img className="Navbar__Title ml-2 mt-2" src={require('../../../content/images/title.png')} height="30"
                     alt="Fantasy Hoops"/>
            </Link>
            <Divider/>
            <List>
                {isAuth()
                    && <ListItem button component={Link} to={Routes.LINEUP} onClick={() => handleDrawerClose()}
                                selected={selectedTab === 0}>
                        <ListItemIcon>
                            <img
                                width="40" height="40"
                                src={require('../../../content/icons/2330831-basketball/svg/008-tactic.svg')}
                                alt=""
                            />
                        </ListItemIcon>
                        <ListItemText classes={{primary: classes.listItemText}} className={classes.listItemText}
                                      primary="Lineup"/>
                    </ListItem>}
                <ListItem button component={Link} to={Routes.LEADERBOARDS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === 1}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/2330831-basketball/svg/025-podium.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Leaderboards"/>
                </ListItem>
                <ListItem button component={Link} to={Routes.INJURIES} onClick={() => handleDrawerClose()}
                          selected={selectedTab === 2}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/2330831-basketball/svg/013-first aid kit.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Injuries"/>
                </ListItem>
                <ListItem button component={Link} to={Routes.NEWS} onClick={() => handleDrawerClose()}
                          selected={selectedTab === 3}>
                    <ListItemIcon>
                        <img
                            width="40" height="40"
                            src={require('../../../content/icons/2330831-basketball/svg/019-live streaming.svg')}
                            alt=""
                        />
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="News"/>
                </ListItem>
            </List>
            <Divider/>
            <List>
                {isAuth()
                    && <ListItem button component={Link} to={Routes.USER_POOL} onClick={() => handleDrawerClose()}
                                selected={selectedTab === 4}>
                        <ListItemIcon>
                            <img width="40" height="40"
                                 src={require('../../../content/icons/2330831-basketball/svg/024-player.svg')}
                                 alt=""/>
                        </ListItemIcon>
                        <ListItemText classes={{primary: classes.listItemText}} primary="Users"/>
                    </ListItem>}
                <ListItem button component={Link} to={Routes.BLOG} onClick={() => handleDrawerClose()}
                          selected={selectedTab === 5}>
                    <ListItemIcon>
                        <img width="35" height="35" src={require('../../../content/icons/pencil.svg')} alt=""/>
                    </ListItemIcon>
                    <ListItemText classes={{primary: classes.listItemText}} primary="Blog"/>
                </ListItem>
            </List>
        </div>
    );
}

export default MobileDrawer;