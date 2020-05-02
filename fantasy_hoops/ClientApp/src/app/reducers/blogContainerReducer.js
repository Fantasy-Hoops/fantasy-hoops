import Blog from '../constants/blog';

const initialState = {
  posts: [],
  pendingPosts: [],
  loader: true
};

export default (state = initialState, action = {}) => {
  switch (action.type) {
    case Blog.LOAD_POSTS:
      return {
        ...state,
        posts: action.posts,
        loader: false
      };
    case Blog.LOAD_PENDING_POSTS:
      return {
        ...state,
        pendingPosts: action.pendingPosts,
        loader: false
      };
    default:
      return state;
  }
};
