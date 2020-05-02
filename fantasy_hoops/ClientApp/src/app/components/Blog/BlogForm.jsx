import React, {useState} from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import {
    Formik, Form, Field
} from 'formik';
import {parse} from '../../utils/auth';
import {blogValidation} from '../../utils/validation';
import 'markdown-toolbar';
import {useSnackbar} from "notistack";
import '@github/markdown-toolbar-element'
import markdown from 'markdown';
import IconButton from "@material-ui/core/IconButton";
import FormatBoldIcon from '@material-ui/icons/FormatBold';
import FormatItalicIcon from '@material-ui/icons/FormatItalic';
import {Checkbox, FormControlLabel, TextField} from "@material-ui/core";
import CodeIcon from '@material-ui/icons/Code';
import LinkIcon from '@material-ui/icons/Link';
import ImageIcon from '@material-ui/icons/Image';
import FormatListBulletedIcon from '@material-ui/icons/FormatListBulleted';
import FormatListNumberedIcon from '@material-ui/icons/FormatListNumbered';

import $ from 'jquery';
import './BlogForm.css';

const author = parse();

function BlogForm(props) {
    const {enqueueSnackbar} = useSnackbar();
    const [previewEnabled, setPreviewEnabled] = useState(false);

    function togglePreview() {
        setPreviewEnabled(!previewEnabled);
    }

    return (
        <>
            <p>
                <button className="btn btn-primary" type="button" data-toggle="collapse"
                        data-target="#blogForm" aria-expanded="false" aria-controls="blogForm">
						<span>
							<i className="fas fa-pencil-alt"/>
                            {' New Post'}
						</span>
                </button>
            </p>
            <div className="collapse" id="blogForm">
                <div className="form-group">
                    <label htmlFor="post-author">Author</label>
                    <input className="form-control" id="post-author" disabled value={author.username}/>
                </div>
                <Formik
                    initialValues={{}}
                    validationSchema={blogValidation}
                    onSubmit={(values, actions) => {
                        actions.setSubmitting(true);
                        const {savePost} = props;
                        savePost({title: values.postTitle, body: values.postBody, authorID: author.id})
                            .then(response => {
                                if (response.isSuccess) {
                                    enqueueSnackbar(response.data.data, {variant: 'success'});
                                } else {
                                    enqueueSnackbar(response.data.data, {variant: 'error'});
                                }

                            });
                        actions.resetForm({});
                        document.getElementById('post-body').value = null;
                        $('#blogForm').toggle()
                        actions.setSubmitting(false);
                    }}
                    render={(formProps) => {
                        const {
                            errors,
                            values,
                            setFieldValue,
                            handleChange,
                            setFieldTouched
                        } = formProps;

                        const change = (name, e) => {
                            handleChange(e);
                            setFieldTouched(name, true, false);
                            setFieldValue(name, e.target.value);
                        };

                        return (
                            <Form>
                                <div className="form-group">
                                    <label htmlFor="post-title">Title</label>
                                    <Field
                                        className={`form-control ${errors.postTitle && 'is-invalid'}`}
                                        id="post-title"
                                        name="postTitle"
                                        component="input"
                                        type="text"
                                        error={errors.postTitle}
                                        placeholder="Post title"
                                        value={values.postTitle ? values.postTitle : ''}
                                    />
                                    <small id="title-error" className="form-text text-danger">
                                        {errors.postTitle}
                                    </small>
                                </div>
                                <div className="form-group">
                                    <label htmlFor="post-body">Body</label>
                                    <br/>
                                    <markdown-toolbar for="post-body">
                                        <IconButton size="small">
                                            <md-ref><strong>H1</strong></md-ref>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-header><strong>H2</strong></md-header>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-bold><FormatBoldIcon/></md-bold>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-italic><FormatItalicIcon/></md-italic>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-code><CodeIcon/></md-code>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-link><LinkIcon/></md-link>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-image><ImageIcon/></md-image>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-unordered-list><FormatListBulletedIcon/></md-unordered-list>
                                        </IconButton>
                                        <IconButton size="small">
                                            <md-ordered-list><FormatListNumberedIcon/></md-ordered-list>
                                        </IconButton>
                                        <FormControlLabel
                                            style={{float: 'right', margin: '0'}}
                                            control={<Checkbox size="small" checked={previewEnabled}
                                                               onChange={togglePreview}/>}
                                            label="Preview"
                                        />
                                    </markdown-toolbar>
                                    <TextField
                                        id="post-body"
                                        name="postBody"
                                        variant="outlined"
                                        onChange={change.bind(null, 'postBody')}
                                        error={!_.isEmpty(errors.postBody)}
                                        multiline
                                        fullWidth
                                    />
                                    {previewEnabled && values.postBody && <div className="BlogForm__preview my-4"
                                                                               dangerouslySetInnerHTML={{__html: markdown.parse(values.postBody)}}/>}
                                    <small id="body-error" className="form-text text-danger">
                                        {errors.postBody}
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
                    }}/>
            </div>
        </>
    );
}

BlogForm.propTypes = {
    savePost: PropTypes.func.isRequired
};

export default BlogForm;
