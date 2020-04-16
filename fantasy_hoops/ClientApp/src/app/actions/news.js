import News from '../constants/news';
import {getNews, getPreviews, getRecaps} from '../utils/networkFunctions';

export const loadNews = () => async (dispatch) => {
  await getNews().then((res) => {
    dispatch({
      type: News.LOAD_NEWS,
      news: res.data
    });
  });
};

export const loadMoreNews = start => async (dispatch) => {
  await getNews({ start }).then((res) => {
    dispatch({
      type: News.LOAD_MORE_NEWS,
      news: res.data,
      hasMoreNews: res.data.length === 6
    });
  });
};

export const loadPreviews = () => async (dispatch) => {
  await getPreviews().then((res) => {
    dispatch({
      type: News.LOAD_PREVIEWS,
      previews: res.data
    });
  });
};

export const loadMorePreviews = start => async (dispatch) => {
  await getPreviews({ start }).then((res) => {
    dispatch({
      type: News.LOAD_MORE_PREVIEWS,
      previews: res.data,
      hasMorePreviews: res.data.length === 6
    });
  });
};

export const loadRecaps = () => async (dispatch) => {
  await getRecaps().then((res) => {
    dispatch({
      type: News.LOAD_RECAPS,
      recaps: res.data
    });
  });
};

export const loadMoreRecaps = start => async (dispatch) => {
  await getRecaps({ start }).then((res) => {
    dispatch({
      type: News.LOAD_MORE_RECAPS,
      recaps: res.data,
      hasMoreRecaps: res.data.length === 6
    });
  });
};
