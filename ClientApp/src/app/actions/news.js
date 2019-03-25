import News from '../constants/news';
import { getNews } from '../utils/networkFunctions';

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
      hasMore: res.data.length === 6
    });
  });
};
