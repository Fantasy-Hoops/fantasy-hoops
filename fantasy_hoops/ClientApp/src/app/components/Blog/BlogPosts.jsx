import React, { PureComponent } from 'react';
import shortid from 'shortid';
import PropTypes from 'prop-types';
import _ from 'lodash';
import PostCard from './PostCard';
import EmptyJordan from '../EmptyJordan';

class BlogPosts extends PureComponent {
  constructor(props) {
    super(props);
    this.handleRemove = this.handleRemove.bind(this);
  }

  handleRemove(post) {
    const { handleRemove } = this.props;
    handleRemove(post);
  }

  render() {
    const { posts, blogLoader, user } = this.props;
    const postCards = _.map(posts,
      post => (
        <PostCard
          key={shortid()}
          post={post}
          user={user}
          handleRemove={this.handleRemove}
        />
      ));
    if (blogLoader) {
      return <div className="Loader" />;
    }
    return (
      <div>
        {postCards.length > 0 ? postCards : <EmptyJordan className="mt-5" message="No blog posts yet..." />}
      </div>
    );
  }
}

BlogPosts.propTypes = {
  handleRemove: PropTypes.func.isRequired,
  blogLoader: PropTypes.bool.isRequired,
  user: PropTypes.shape({
    userName: PropTypes.string
  }).isRequired,
  posts: PropTypes.arrayOf(PropTypes.shape({
    post: PropTypes.shape({
      title: PropTypes.string.isRequired,
      body: PropTypes.string.isRequired,
      author: PropTypes.shape({
        userName: PropTypes.string.isRequired,
        id: PropTypes.string.isRequired
      }).isRequired
    })
  })).isRequired
};

export default BlogPosts;
