import React from 'react';
import PropTypes from 'prop-types';
import CssBaseline from '@material-ui/core/CssBaseline';
import Drawer from '@material-ui/core/Drawer';
import Hidden from '@material-ui/core/Hidden';
import {useTheme} from '@material-ui/core/styles';
import Routes from "../../routes/routes";
import {Link} from "react-router-dom";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import {getLocationEnumValue} from "../../utils/locationSlug";
import {useStyles} from "./HeaderStyle";
import MobileDrawer from "./MobileDrawer";
import DesktopAppBar from "./DesktopAppBar";
import {logout} from "../../utils/networkFunctions";
import {isAuth} from "../../utils/auth";

function Header(props) {
    const {container} = props;
    const classes = useStyles();
    const theme = useTheme();
    const [mobileOpen, setMobileOpen] = React.useState(false);
    const [anchorEl, setAnchorEl] = React.useState(null);
    const isMenuOpen = Boolean(anchorEl);
    const [selectedTab, setSelectedTab] = React.useState(0);

    React.useEffect(() => {
        const {pathname} = location;
        setSelectedTab(getLocationEnumValue(pathname));
    });

    const handleDrawerToggle = () => {
        setMobileOpen(!mobileOpen);
    };

    const handleDrawerClose = () => {
        setMobileOpen(false);
    };

    const handleProfileMenuOpen = event => {
        setAnchorEl(event.currentTarget);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
    };

    function handleLogout() {
        handleMenuClose();
        logout();
    }

    const menuId = 'primary-search-account-menu';
    const renderMenu = (
        <Menu
            anchorEl={anchorEl}
            anchorOrigin={{vertical: 'top', horizontal: 'right'}}
            id={menuId}
            keepMounted
            transformOrigin={{vertical: 'top', horizontal: 'right'}}
            open={isMenuOpen}
            onClose={handleMenuClose}
        >
            {isAuth()
                ? [
                    <MenuItem key="profile" onClick={handleMenuClose} component={Link} to={Routes.PROFILE}>Profile</MenuItem>,
                    <MenuItem key="logout" onClick={handleLogout}>Logout</MenuItem>
                ]
                : [
                    <MenuItem key="login" onClick={handleMenuClose} component={Link} to={Routes.LOGIN}>Login</MenuItem>,
                    <MenuItem key="register" onClick={handleMenuClose} component={Link} to={Routes.REGISTER}>Register</MenuItem>
                ]
            }

        </Menu>
    );

    return (
        <div className={classes.root}>
            <CssBaseline/>
            <DesktopAppBar
                selectedTab={selectedTab}
                handleProfileMenuOpen={handleProfileMenuOpen}
                handleDrawerToggle={handleDrawerToggle}
                menuId={menuId}
            />
            {renderMenu}
            <nav className={classes.drawer} aria-label="mailbox folders">
                <Hidden smUp implementation="css">
                    <Drawer
                        container={container}
                        variant="temporary"
                        anchor={theme.direction === 'rtl' ? 'right' : 'left'}
                        open={mobileOpen}
                        onClose={handleDrawerToggle}
                        classes={{
                            paper: classes.drawerPaper,
                        }}
                        ModalProps={{
                            keepMounted: true, // Better open performance on mobile.
                        }}
                    >
                        <MobileDrawer selectedTab={selectedTab} handleDrawerClose={handleDrawerClose}/>
                    </Drawer>
                </Hidden>
            </nav>
        </div>
    );
}

Header.propTypes = {
    container: PropTypes.any,
};

export default Header;
