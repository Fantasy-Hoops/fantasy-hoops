import Blog from '../constants/blog';
import {getPosts, submitPost, deletePost, getPendingPosts, approvePost} from '../utils/networkFunctions';

export const loadPosts = () => async (dispatch) => {
    await getPosts().then((res) => {
        dispatch({
            type: Blog.LOAD_POSTS,
            posts: res.data
        });
    });
};

export const savePost = post => async (dispatch) => {
    const response = await submitPost(post);
    dispatch({type: Blog.SUBMIT_POST});
    if (response.status === 200) {
        setTimeout(() => getPosts().then((res) => {
            dispatch({
                type: Blog.LOAD_POSTS,
                posts: res.data
            });
        }), 1000);
        return {isSuccess: true, data: response};
    } else {
        return {isSuccess: false, data: response};
    }
};

export const removePost = id => async (dispatch) => {
    const response = await deletePost(id);
    dispatch({type: Blog.DELETE_POST});
    if (response.status === 200) {
        setTimeout(() => getPosts().then((res) => {
            dispatch({
                type: Blog.LOAD_POSTS,
                posts: res.data
            });
        }), 1000);
        setTimeout(() => getPendingPosts().then((res) => {
            dispatch({
                type: Blog.LOAD_PENDING_POSTS,
                pendingPosts: res.data
            });
        }), 1000);
    }
    return response;
};

export const loadPendingPosts = () => async (dispatch) => {
    await getPendingPosts().then((res) => {
        dispatch({
            type: Blog.LOAD_PENDING_POSTS,
            pendingPosts: res.data
        });
    });
};

export const approveBlogPost = id => async (dispatch) => {
    const response = await approvePost(id);
    dispatch({type: Blog.DELETE_POST});
    if (response.status === 200) {
        setTimeout(() => getPosts().then((res) => {
            dispatch({
                type: Blog.LOAD_POSTS,
                posts: res.data
            });
        }), 1000);
        setTimeout(() => getPendingPosts().then((res) => {
            dispatch({
                type: Blog.LOAD_PENDING_POSTS,
                pendingPosts: res.data
            });
        }), 1000);
    }
    return response;
};
