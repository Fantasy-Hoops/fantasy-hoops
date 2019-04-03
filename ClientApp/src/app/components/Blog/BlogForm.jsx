import React from 'react';
import PropTypes from 'prop-types';
import {
	Formik, Form, Field
} from 'formik';
import { parse } from '../../utils/auth';
import { blogValidation } from '../../utils/validation';

const author = parse();

const BlogForm = props => (
	<>
		<h1 className="text-center">New Post</h1>
		<div className="form-group">
			<input className="form-control" disabled value={author.username} />
		</div>
		<Formik
			validationSchema={blogValidation}
			onSubmit={(values, actions) => {
				actions.setSubmitting(true);
				const { savePost } = props;
				savePost({ title: values.title, body: values.body, authorID: author.id });
				actions.resetForm({ title: '', body: '' });
				actions.setSubmitting(false);
			}}
			render={(props) => {
				const {
					errors,
					values,
					isSubmitting
				} = props;
				return (
					<Form>
						<div className="form-group">
							<Field
								className="form-control"
								component="input"
								type="text"
								error={errors.title}
								name="title"
								placeholder="Post title"
								value={values.title ? values.title : ''}
							/>
						</div>
						<div className="form-group">
							<Field
								className="form-control"
								component="textarea"
								rows="10"
								type="text"
								error={errors.body}
								name="body"
								placeholder="Post body"
								value={values.body ? values.body : ''}
							/>
						</div>
						<button
							type="submit"
							className="btn btn-outline-success btn-block"
							disabled={isSubmitting}
						>
							{'Submit'}
						</button>
					</Form>
				);
			}}
		/>
	</>
);

export default BlogForm;
