import Blog from '../constants/blog';

const initialState = {
  posts: [],
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
    default:
      return state;
  }
};
