import Blog from '../constants/blog';
import { getPosts, submitPost } from '../utils/networkFunctions';

export const loadPosts = () => async (dispatch) => {
  await getPosts().then((res) => {
    dispatch({
      type: Blog.LOAD_POSTS,
      posts: res.data
    });
  });
};

export const savePost = post => async (dispatch) => {
  await submitPost(post).then(() => {
    dispatch({
      type: Blog.SUBMIT_POST
    });
  });
  await getPosts().then((res) => {
    dispatch({
      type: Blog.LOAD_POSTS,
      posts: res.data
    });
  });
};
