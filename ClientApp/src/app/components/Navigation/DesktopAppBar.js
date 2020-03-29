import AppBar from "@material-ui/core/AppBar";
import {Link} from "react-router-dom";
import Routes from "../../routes/routes";
import IconButton from "@material-ui/core/IconButton";
import AccountCircle from "@material-ui/icons/AccountCircle";
import MenuIcon from "@material-ui/icons/Menu";
import Toolbar from "@material-ui/core/Toolbar";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import React, {useState} from "react";
import {useStyles} from "./DesktopAppBarStyle";
import Notifications from "../Notifications/Notifications";
import {loadImage} from "../../utils/loadImage";
import {isAuth, parse} from "../../utils/auth";
import defaultPhoto from "../../../content/images/default.png";
import Img from "react-image";

function DesktopAppBar(props) {
    const isAuthenticated = isAuth();
    const classes = useStyles();
    const {selectedTab, handleProfileMenuOpen, handleDrawerToggle, menuId} = props;
    const [avatar, setAvatar] = useState(null);

    React.useEffect(() => {
        async function loadAvatar() {
            const avatar = await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${parse().avatarURL}.png`, defaultPhoto);
            setAvatar(avatar);
        }

        if(isAuthenticated) {
            loadAvatar();
        }
    }, []);

    const user = parse();
    return (
        <AppBar position="fixed" className={classes.appBar}>
            <div className={classes.logo}>
                <Link id="NavbarLogo" className="navbar-brand btn-no-outline"
                      to={Routes.MAIN}>
                    <img className="Navbar__Icon" src={require('../../../content/images/logo.png')} width="25"
                         height="25" alt="FH"/>
                    <img className="Navbar__Title ml-2 mt-2" src={require('../../../content/images/title.png')}
                         height="25" alt="Fantasy Hoops"/>
                </Link>
            </div>
            <div className={classes.sectionDesktop}>
                {isAuthenticated && <Notifications/>}
                <IconButton
                    className={classes.avatar}
                    aria-label="account of current user"
                    aria-controls={menuId}
                    aria-haspopup="true"
                    onClick={handleProfileMenuOpen}
                    color="inherit"
                >
                    {avatar
                        ? <Img
                            className="Avatar--round"
                            width="25"
                            alt={user.username}
                            src={avatar}
                        />
                        : <AccountCircle/>}

                </IconButton>
            </div>
            <IconButton
                color="inherit"
                aria-label="open drawer"
                edge="start"
                onClick={handleDrawerToggle}
                className={classes.menuButton}
            >
                <MenuIcon className={classes.menuIcon}/>
            </IconButton>
            <Toolbar className={classes.navbarToolbar}>
                <Tabs
                    value={selectedTab}
                    className={classes.tabs}
                    classes={{ indicator: classes.indicator }}
                    variant="scrollable"
                    scrollButtons="auto"
                    indicatorColor="secondary"
                    textColor="secondary"
                    aria-label="tabs"
                >
                    {isAuthenticated && <Tab className={classes.tab} component={Link} to={Routes.LINEUP} label="Lineup"/>}
                    <Tab className={classes.tab} component={Link} to={Routes.LEADERBOARDS} label="Leaderboards"/>
                    {isAuthenticated && <Tab className={classes.tab} component={Link} to={Routes.TOURNAMENTS} label="Tournaments"/>}
                    <Tab className={classes.tab} component={Link} to={Routes.ACHIEVEMENTS} label="Achievements"/>
                    <Tab className={classes.tab} component={Link} to={Routes.INJURIES} label="Injuries"/>
                    <Tab className={classes.tab} component={Link} to={Routes.NEWS} label="News"/>
                    {isAuthenticated && <Tab className={classes.tab} component={Link} to={Routes.USER_POOL} label="Users"/>}
                    <Tab className={classes.tab} component={Link} to={Routes.BLOG} label="Blog"/>
                </Tabs>
            </Toolbar>
        </AppBar>
    );
};

export default DesktopAppBar;