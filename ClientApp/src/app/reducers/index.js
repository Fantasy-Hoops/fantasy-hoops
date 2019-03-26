import { combineReducers } from 'redux';
import { connectRouter } from 'connected-react-router';
import history from '../utils/history';
import injuriesContainerReducer from './injuriesContainerReducer';
import newsContainerReducer from './newsContainerReducer';
import userPoolContainerReducer from './userPoolContainerReducer';

export default combineReducers({
  router: connectRouter(history),
  injuriesContainerReducer,
  newsContainerReducer,
  userPoolContainerReducer
});
