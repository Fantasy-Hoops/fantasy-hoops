import React from 'react';
import shortid from 'shortid';
import PropTypes from 'prop-types';
import _ from 'lodash';
import PostCard from './PostCard';
import EmptyJordan from '../EmptyJordan';

const BlogPosts = (props) => {
  const { posts, blogLoader, user } = props;
  const postCards = _.map(posts, post => (<PostCard key={shortid()} post={post} user={user} />));
  if (blogLoader) {
    return <div className="Loader" />;
  }
  return (
    <div>
      {postCards.length > 0 ? postCards : <EmptyJordan className="mt-5" message="No blog posts yet..." />}
    </div>
  );
};

export default BlogPosts;
