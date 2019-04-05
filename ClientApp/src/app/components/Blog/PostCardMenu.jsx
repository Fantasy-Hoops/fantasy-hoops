import React from 'react';

const PostCardMenu = () => (
		<>
			<span className="PostCard__Dots" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i className="fas fa-ellipsis-v " /></span>
			<div className="dropdown-menu">
				<a className="PostCard__MenuItem dropdown-item">
					<i className="fas fa-edit" />
					{' Edit'}
				</a>
				<a className="PostCard__MenuItem dropdown-item">
					<i className="fas fa-trash-alt text-danger" />
					{' Remove'}
				</a>
			</div>
  </>
	);

export default PostCardMenu;
