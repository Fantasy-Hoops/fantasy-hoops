import Blog from '../constants/blog';
import { getPosts, submitPost, deletePost } from '../utils/networkFunctions';

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
  dispatch({ type: Blog.SUBMIT_POST });
  if (response.status === 200) {
    setTimeout(() => getPosts().then((res) => {
      dispatch({
        type: Blog.LOAD_POSTS,
        posts: res.data
      });
    }), 1000);
  }
};

export const removePost = id => async (dispatch) => {
  const response = await deletePost(id);
  dispatch({ type: Blog.DELETE_POST });
  if (response.status === 200) {
    setTimeout(() => getPosts().then((res) => {
      dispatch({
        type: Blog.LOAD_POSTS,
        posts: res.data
      });
    }), 1000);
  }
};
