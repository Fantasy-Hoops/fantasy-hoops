import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import Img from 'react-image';
import markdown from 'markdown';
import defaultPhoto from '../../../content/images/default.png';
import PostCardMenu from './PostCardMenu';

const canEdit = user => user && user.isAdmin;

class PostCard extends PureComponent {
  constructor(props) {
    super(props);
    this.handleRemove = this.handleRemove.bind(this);
  }

  handleRemove(post) {
    const { handleRemove } = this.props;
    handleRemove(post);
  }

  render() {
    const { post, user } = this.props;
    return (
      <div className="PostCard card mt-5 mb-5">
        <div className="card-header bg-primary text-white dropleft no-arrow">
          <h3 className="PostCard__Title card-title">
            {post.title}
          </h3>
          {canEdit(user) ? <PostCardMenu handleRemove={this.handleRemove} {...this.props} /> : null}
        </div>
        <div className="card-header text-muted">
          <Img
            className="PostCard__AuthorImage"
            alt=""
            src={[
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${post.author.avatarURL}.png`,
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
  }
}

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
