import React, {useState} from "react";
import {Table, TableBody, TableContainer, TableHead, TableRow} from "@material-ui/core";
import TableCell from "@material-ui/core/TableCell";
import moment from "moment";
import IconButton from "@material-ui/core/IconButton";
import CheckIcon from '@material-ui/icons/Check';
import DeleteForeverIcon from '@material-ui/icons/DeleteForever';
import ImportContactsIcon from '@material-ui/icons/ImportContacts';
import {BlogPostPreviewDialog} from "./BlogPostPreviewDialog";
import {useSnackbar} from "notistack";
import FullscreenLoader from "../FullscreenLoader";
import _ from 'lodash';
import EmptyJordan from "../EmptyJordan";
import {ConfirmDialog} from "../Inputs/ConfirmDialog";

const tableHead = (
    <TableHead style={{background: 'black'}}>
        <TableRow>
            <TableCell style={{color: 'white'}}>
                Author
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Created at
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Preview
            </TableCell>
            <TableCell style={{color: 'white'}}>
                Actions
            </TableCell>
        </TableRow>
    </TableHead>
);

export function BlogPostsPanel(props) {
    const {pendingPosts} = props;
    const {enqueueSnackbar} = useSnackbar();
    const [previewPost, setPreviewPost] = useState(null);
    const [previewDialogOpen, setPreviewDialogOpen] = useState(false);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [confirmDialogPost, setConfirmDialogPost] = React.useState({});
    const [loader, setLoader] = useState(false);

    const handlePreviewDialogOpen = (post) => {
        setPreviewDialogOpen(true);
        setPreviewPost(post);
    };

    const handlePreviewDialogClose = () => {
        setPreviewDialogOpen(false);
        setPreviewPost(null)
    };

    const handleApprovePost = (postId) => {
        setLoader(true);
        const {approvePost} = props;
        const response = approvePost(postId);
        if (response.status === 200) {
            enqueueSnackbar(response.data, {variant: 'success'});
        } else {
            enqueueSnackbar(response.data, {variant: 'error'});
        }
        setLoader(false);
    };

    const handleConfirmOpen = post => {
        setConfirmOpen(true);
        setConfirmDialogPost(post);
    };

    const handleConfirmClose = () => {
        setConfirmOpen(false);
    };

    function handleRemove(post) {
        const {removePost} = props;
        return removePost(post.id);
    }

    const pendingPostsList = !_.isEmpty(pendingPosts)
        ?
        pendingPosts.map((post, index) => (
            <TableRow key={index}>
                <TableCell>
                    {post.author.username}
                </TableCell>
                <TableCell>
                    {moment(post.createdAt).calendar()}
                </TableCell>
                <TableCell>
                    <IconButton size="small" onClick={() => handlePreviewDialogOpen(post)}>
                        <ImportContactsIcon/>
                    </IconButton>
                </TableCell>
                <TableCell>
                    <IconButton size="small" onClick={() => handleApprovePost(post.id)}>
                        <CheckIcon style={{color: 'green'}}/>
                    </IconButton>
                    <IconButton size="small" onClick={() => handleConfirmOpen(post)}>
                        <DeleteForeverIcon color="error"/>
                    </IconButton>
                </TableCell>
            </TableRow>
        ))
        : <TableRow>
            <TableCell colSpan={4}>
                <EmptyJordan message={"No pending posts to approve"}/>
            </TableCell>
        </TableRow>;

    return (
        <>
            <TableContainer>
                <Table>
                    {tableHead}
                    <TableBody>
                        {pendingPostsList}
                    </TableBody>
                </Table>
            </TableContainer>
            <BlogPostPreviewDialog post={previewPost} open={previewDialogOpen} handleClose={handlePreviewDialogClose}/>
            <ConfirmDialog
                open={confirmOpen}
                handleClose={handleConfirmClose}
                title="Are you sure want to delete selected blog post?"
                description="The blog post will be deleted permanently"
                callbackFunction={() => handleRemove(confirmDialogPost)}
            />
            {loader && <FullscreenLoader/>}
        </>
    );
}