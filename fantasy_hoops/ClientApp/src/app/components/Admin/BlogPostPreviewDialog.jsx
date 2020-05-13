import React from "react";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import markdown from 'markdown';

import './BlogPostPreviewDialog.css';
import PostCard from "../Blog/PostCard";

export function BlogPostPreviewDialog(props) {
    const {post, open, handleClose} = props;

    if (!post) {
        return null;
    }

    return (
        <Dialog
            maxWidth="lg"
            className="BlogPostPreviewDialog"
            open={open}
            onClose={handleClose}
            aria-labelledby="blog-post-preview-dialog-title"
            aria-describedby="blog-post-preview-dialog-description"
        >
            <DialogTitle id="blog-post-preview-dialog-title">Preview Blog Post</DialogTitle>
            <DialogContent>
                <PostCard post={post} noEdit/>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}