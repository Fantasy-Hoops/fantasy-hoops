import React from "react";
import Dialog from "@material-ui/core/Dialog";
import {Button, DialogActions, DialogTitle} from "@material-ui/core";
import DialogContent from "@material-ui/core/DialogContent";
import BlogForm from "./BlogForm";
import {bindActionCreators} from "redux";
import * as actionCreators from "../../actions/blog";
import {connect} from "react-redux";

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

function EditPostDialog(props) {
    const {open, handleSubmit, handleClose, post, savePost} = props;

    if (!post) {
        return null;
    }

    return (
        <Dialog maxWidth="sm" open={open} onClose={handleClose}>
            <DialogTitle>
                Edit post
            </DialogTitle>
            <DialogContent>
                <BlogForm formId="edit-post" initialValues={{id: post.id, postTitle: post.title, postBody: post.body, author: post.author}}
                          savePost={savePost} handleSubmit={handleSubmit}/>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default connect(mapDispatchToProps)(EditPostDialog);