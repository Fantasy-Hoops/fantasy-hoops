import React from 'react';
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import MenuIcon from '@material-ui/icons/Menu';
import IconButton from "@material-ui/core/IconButton";

function PostCardMenu(props) {
    const [anchorEl, setAnchorEl] = React.useState(null);

    const handleClick = event => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    function handleRemove() {
        const {post, handleRemove} = props;
        handleRemove(post);
    }

    return (
        <div className="PostCard__Menu">
            <IconButton aria-controls="post-card-menu" aria-haspopup="true" onClick={handleClick}>
                <MenuIcon fontSize="inherit"/>
            </IconButton>
            <Menu
                id="simple-menu"
                anchorEl={anchorEl}
                keepMounted
                open={Boolean(anchorEl)}
                onClose={handleClose}
            >
                <MenuItem onClick={handleClose}><i className="fas fa-edit"/>Edit</MenuItem>
                <MenuItem onClick={handleRemove}><i className="fas fa-trash-alt text-danger"/>Remove</MenuItem>
            </Menu>
        </div>
    );
}

export default PostCardMenu;
