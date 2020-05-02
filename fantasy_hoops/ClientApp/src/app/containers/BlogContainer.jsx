import React, {Component, useEffect} from 'react';
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

const mapStateToProps = state => ({
    posts: state.blogContainerReducer.posts,
    blogLoader: state.blogContainerReducer.loader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

const Intro = {
    TITLE: "",
    SUBTITLE: ""
};

const user = parse();

export function BlogContainer(props) {
    const [confirmOpen, setConfirmOpen] = React.useState(false);
    const [confirmDialogPost, setConfirmDialogPost] = React.useState({});

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
    };

    function handleRemove(post) {
        const {removePost} = props;
        return removePost(post.id);
    }

    let blogForm = null;
    if (isAuth()) {
        blogForm = isCreator() && <BlogForm {...props} />;
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
            <div className="container">
                <div className="row pt-5">
                    <div className="col-12 col-lg-9 mx-auto">
                        {blogForm}
                        <BlogPosts user={user} {...props} handleRemove={handleConfirmOpen}/>
                    </div>
                </div>
            </div>
            <ConfirmDialog
                open={confirmOpen}
                handleClose={handleConfirmClose}
                title="Are you sure want to delete selected blog post?"
                description="The blog post will be deleted permanently"
                callbackFunction={() => handleRemove(confirmDialogPost)}
            />
        </>
    );
}

BlogContainer.propTypes = {
    loadPosts: PropTypes.func.isRequired,
    removePost: PropTypes.func.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(BlogContainer);
