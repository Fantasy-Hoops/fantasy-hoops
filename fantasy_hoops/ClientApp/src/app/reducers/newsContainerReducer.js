import News from '../constants/news';

const initialState = {
  news: [],
  previews: [],
  recaps: [],
  hasMoreNews: true,
  newsLoader: true,
  hasMorePreviews: true,
  previewsLoader: true,
  hasMoreRecaps: true,
  recapsLoader: true
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
        hasMoreNews: action.hasMoreNews
      };
    case News.LOAD_PREVIEWS:
      return {
        ...state,
        previews: action.previews,
        previewsLoader: false
      };
    case News.LOAD_MORE_PREVIEWS:
      return {
        ...state,
        previews: state.previews.concat(action.previews),
        hasMorePreviews: action.hasMorePreviews
      };
    case News.LOAD_RECAPS:
      return {
        ...state,
        recaps: action.recaps,
        recapsLoader: false
      };
    case News.LOAD_MORE_RECAPS:
      return {
        ...state,
        recaps: state.recaps.concat(action.recaps),
        hasMoreRecaps: action.hasMoreRecaps
      };
    default:
      return state;
  }
};
