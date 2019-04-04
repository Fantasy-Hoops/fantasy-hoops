import React from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import {
	Formik, Form, Field
} from 'formik';
import $ from 'jquery';
import { parse } from '../../utils/auth';
import { blogValidation } from '../../utils/validation';
import 'markdown-toolbar';

const author = parse();
$(document).ready(() => {
	$('.toolbar').markdownToolbar();
});

const BlogForm = props => (
	<>
		<p>
			<button className="btn btn-primary" type="button" data-toggle="collapse" data-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample">
				<span>
					<i className="fas fa-pencil-alt" />
					{' New Post'}
				</span>
			</button>
		</p>
		<div className="collapse" id="collapseExample">
			<div className="form-group">
				<label htmlFor="post-author">Author</label>
				<input className="form-control" id="post-author" disabled value={author.username} />
			</div>
			<Formik
				validationSchema={blogValidation}
				onSubmit={(values, actions) => {
					actions.setSubmitting(true);
					const { savePost } = props;
					savePost({ title: values.title, body: values.body, authorID: author.id });
					actions.resetForm({});
					actions.setSubmitting(false);
				}}
				render={(formProps) => {
					const {
						errors,
						values
					} = formProps;
					return (
						<Form>
							<div className="form-group">
								<label htmlFor="post-title">Title</label>
								<Field
									className={`form-control ${errors.title && 'is-invalid'}`}
									id="post-title"
									component="input"
									type="text"
									error={errors.title}
									name="title"
									placeholder="Post title"
									value={values.title ? values.title : ''}
								/>
								<small id="title-error" className="form-text text-danger">
									{errors.title}
								</small>
							</div>
							<div className="form-group">
								<label htmlFor="post-body">Body</label>
								<Field
									className={`toolbar form-control ${errors.body && 'is-invalid'}`}
									id="post-body"
									component="textarea"
									rows="10"
									type="text"
									name="body"
									placeholder="Post body"
									value={values.body ? values.body : ''}
								/>
								<small id="body-error" className="form-text text-danger">
									{errors.body}
								</small>
							</div>
							<button
								type="submit"
								className="btn btn-outline-info btn-block"
								disabled={!_.isEmpty(errors) || _.isEmpty(values)}
							>
								{'Submit'}
							</button>
						</Form>
					);
				}}
			/>
		</div>
	</>
);

BlogForm.propTypes = {
	savePost: PropTypes.func.isRequired
};

export default BlogForm;
