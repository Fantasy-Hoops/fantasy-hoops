import { combineReducers } from 'redux';
import { connectRouter } from 'connected-react-router';
import history from '../utils/history';
import injuriesContainer from './injuries';

export default combineReducers({
  router: connectRouter(history),
  injuriesContainer
});
