import News from '../constants/news';

const initialState = {
  news: [],
  hasMore: true,
  newsLoader: true
};

export default (state = initialState, action = {}) => {
  switch (action.type) {
    case News.LOAD_NEWS:
      return {
        ...state,
        news: action.news,
        newsLoader: false
      };
    case News.LOAD_MORE_NEWS:
      return {
        ...state,
        news: state.news.concat(action.news),
        hasMore: action.hasMore
      };
    default:
      return state;
  }
};
