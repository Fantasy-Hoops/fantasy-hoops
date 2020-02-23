import React, {Component} from 'react';
import PropTypes from 'prop-types';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import {parse, isAuth} from '../utils/auth';
import * as actionCreators from '../actions/blog';
import BlogForm from '../components/Blog/BlogForm';
import BlogPosts from '../components/Blog/BlogPosts';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../utils/helpers";

const mapStateToProps = state => ({
    posts: state.blogContainerReducer.posts,
    blogLoader: state.blogContainerReducer.loader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

const Intro = {
    TITLE: "",
    SUBTITLE: ""
}

export class BlogContainer extends Component {
    constructor(props) {
        super(props);
        this.user = parse();
        this.handleRemove = this.handleRemove.bind(this);
    }

    async componentDidMount() {
        const {loadPosts} = this.props;
        await loadPosts();
    }

    handleRemove(post) {
        const response = window.confirm(`Are you sure want to delete '${post.title}' post?`);
        if (response === true) {
            const {removePost} = this.props;
            removePost(post.id);
        }
    }

    render() {
        let blogForm = null;
        if (isAuth()) {
            blogForm = (this.user.username === 'Naidze' || this.user.username === 'bennek') &&
                <BlogForm {...this.props} />;
        }
        return (
            <>
                <Helmet>
                    <title>Blog | Fantasy Hoops</title>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <link rel="canonical" href={Canonicals.BLOG}/>
                </Helmet>
                <div className="container">
                    <div className="row pt-5">
                        <div className="col-12 col-lg-9 mx-auto">
                            {blogForm}
                            <BlogPosts user={this.user} {...this.props} handleRemove={this.handleRemove}/>
                        </div>
                    </div>
                </div>
            </>
        );
    }
}

BlogContainer.propTypes = {
    loadPosts: PropTypes.func.isRequired,
    removePost: PropTypes.func.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(BlogContainer);
