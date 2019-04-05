import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { parse, isAuth } from '../utils/auth';
import * as actionCreators from '../actions/blog';
import BlogForm from '../components/Blog/BlogForm';
import BlogPosts from '../components/Blog/BlogPosts';

const mapStateToProps = state => ({
  posts: state.blogContainerReducer.posts,
  blogLoader: state.blogContainerReducer.loader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

export class BlogContainer extends Component {
  constructor(props) {
    super(props);
    this.user = parse();
  }

  async componentDidMount() {
    const { loadPosts } = this.props;
    await loadPosts();
  }

  render() {
    let blogForm = null;
    if (isAuth()) {
      blogForm = (this.user.username === 'Naidze' || this.user.username === 'bennek') && <BlogForm {...this.props} />;
    }
    return (
      <div className="container">
        <div className="row pt-5">
          <div className="col-12 col-lg-9 mx-auto">
            {blogForm}
            <BlogPosts user={this.user} {...this.props} />
          </div>
        </div>
      </div>
    );
  }
}

BlogContainer.propTypes = {
  loadPosts: PropTypes.func.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(BlogContainer);
