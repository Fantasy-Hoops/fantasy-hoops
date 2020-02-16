import React, { PureComponent } from 'react';

class PostCardMenu extends PureComponent {
	constructor(props) {
		super(props);
		this.handleRemove = this.handleRemove.bind(this);
	}

	handleRemove() {
		const { post, handleRemove } = this.props;
		handleRemove(post);
	}

	render() {
		return (
			<>
				<span className="PostCard__Dots" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i className="fas fa-ellipsis-v" /></span>
				<div className="dropdown-menu">
					<button className="PostCard__MenuItem dropdown-item">
						<i className="fas fa-edit" />
						{' Edit'}
					</button>
					<button role="button" className="PostCard__MenuItem dropdown-item" onClick={this.handleRemove} onKeyDown={this.handleRemove}>
						<i className="fas fa-trash-alt text-danger" />
						{' Remove'}
					</button>
				</div>
			</>
		);
	}
}

export default PostCardMenu;
