import { combineReducers } from 'redux';
import { connectRouter } from 'connected-react-router';
import history from '../utils/history';
import injuriesContainerReducer from './injuries';
import newsContainerReducer from './news';

export default combineReducers({
  router: connectRouter(history),
  injuriesContainerReducer,
  newsContainerReducer
});
