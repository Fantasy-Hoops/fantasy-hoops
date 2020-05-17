import React, {Component, useEffect, useState} from 'react';
import PropTypes from 'prop-types';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {parse, isAuth, isCreator} from '../utils/auth';
import * as actionCreators from '../actions/blog';
import BlogForm from '../components/Blog/BlogForm';
import BlogPosts from '../components/Blog/BlogPosts';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../utils/helpers";
import {ConfirmDialog} from "../components/Inputs/ConfirmDialog";
import EditPostDialog from "../components/Blog/EditPostDialog";
import {useSnackbar} from "notistack";
import $ from "jquery";

const mapStateToProps = state => ({
    posts: state.blogContainerReducer.posts,
    blogLoader: state.blogContainerReducer.loader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

const user = parse();

export function BlogContainer(props) {
    const {enqueueSnackbar} = useSnackbar();
    const [confirmOpen, setConfirmOpen] = React.useState(false);
    const [confirmDialogPost, setConfirmDialogPost] = React.useState({});
    const [editPostOpen, setEditPostOpen] = useState(false);
    const [editablePost, setEditablePost] = useState({});

    useEffect(() => {
        const {loadPosts} = props;
        loadPosts();
    }, []);

    const handleConfirmOpen = post => {
        setConfirmOpen(true);
        setConfirmDialogPost(post);
    };

    const handleConfirmClose = () => {
        setConfirmOpen(false);
        setConfirmDialogPost({});
    };

    function handleRemove(post) {
        const {removePost} = props;
        return removePost(post.id);
    }

    const handleEditPostOpen = post => {
        setEditablePost(post);
        setEditPostOpen(true);
    }

    const handleEditPostClose = () => {
        setEditablePost({});
        setEditPostOpen(false);
    }

    function handleCreatePost(values) {
        const {savePost} = props;
        $('#blog-form-button').click();
        savePost({title: values.postTitle, body: values.postBody, authorID: parse().id})
            .then(response => {
                if (response.isSuccess) {
                    enqueueSnackbar(response.data.data, {variant: 'success'});
                } else {
                    enqueueSnackbar(response.data.data, {variant: 'error'});
                }
            });
    }

    function handleEditPost(values) {
        const {updatePost} = props;
        handleEditPostClose();
        updatePost({id: values.id, title: values.postTitle, body: values.postBody})
            .then(response => {
                if (response.isSuccess) {
                    enqueueSnackbar(response.data.data, {variant: 'success'});
                } else {
                    enqueueSnackbar(response.data.data, {variant: 'error'});
                }
            })
    }

    let blogForm = null;
    if (isAuth()) {
        blogForm = isCreator() && (
            <>
                <p>
                    <button id="blog-form-button" className="btn btn-primary" type="button" data-toggle="collapse"
                            data-target="#blogForm" aria-expanded="false" aria-controls="blogForm">
						<span>
							<i className="fas fa-pencil-alt"/>
                            {' New Post'}
						</span>
                    </button>
                </p>
                <div className="collapse" id="blogForm">
                    <BlogForm {...props} handleSubmit={handleCreatePost}/>
                </div>
            </>
        );
    }
    return (
        <>
            <Helmet>
                <title>Blog | Fantasy Hoops</title>
                <meta property="title" content="Blog | Fantasy Hoops"/>
                <meta property="og:title" content="Blog | Fantasy Hoops"/>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <meta property="og:description" content={Meta.DESCRIPTION}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.BLOG}/>
            </Helmet>
            <div className="col-12 col-lg-9 mx-auto">
                {blogForm}
                <BlogPosts user={user} {...props} handleRemove={handleConfirmOpen}
                           handleEdit={handleEditPostOpen}/>
            </div>
            <ConfirmDialog
                open={confirmOpen}
                handleClose={handleConfirmClose}
                title="Are you sure want to delete selected blog post?"
                description="The blog post will be deleted permanently"
                callbackFunction={() => handleRemove(confirmDialogPost)}
            />
            <EditPostDialog
                post={editablePost}
                open={editPostOpen}
                handleClose={handleEditPostClose}
                title="Are you sure want to delete selected blog post?"
                description="The blog post will be deleted permanently"
                handleSubmit={handleEditPost}
            />
        </>
    );
}

BlogContainer.propTypes = {
    loadPosts: PropTypes.func.isRequired,
    removePost: PropTypes.func.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(BlogContainer);
