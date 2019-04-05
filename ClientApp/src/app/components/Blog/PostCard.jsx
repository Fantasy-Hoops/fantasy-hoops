import React from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import Img from 'react-image';
import markdown from 'markdown';
import defaultPhoto from '../../../content/images/default.png';

const PostCard = (props) => {
  const { post } = props;
  return (
    <div className="PostCard card mt-5 mb-5">
      <div className="card-header bg-primary text-white">
        <h3 className="card-title">
          {post.title}
        </h3>
      </div>
      <div className="card-header text-muted">
        <Img
          className="PostCard__AuthorImage"
          alt=""
          src={[
            `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${post.author.id}.png`,
            defaultPhoto
          ]}
          loader={<img height="50px" src={require('../../../content/images/imageLoader.gif')} alt="Loader" />}
        />
        {post.author.userName}
        <span style={{ float: 'right' }}>
          {moment(post.createdAt).format('MM/DD/YYYY')}
        </span>
      </div>
      <div
        className="PostCard__Body card-body"
        dangerouslySetInnerHTML={{ __html: markdown.parse(post.body) }}
      />
    </div>
  );
};

PostCard.propTypes = {
  post: PropTypes.shape({
    title: PropTypes.string.isRequired,
    body: PropTypes.string.isRequired,
    author: PropTypes.shape({
      userName: PropTypes.string.isRequired,
      id: PropTypes.string.isRequired
    }).isRequired
  }).isRequired
};

export default PostCard;
