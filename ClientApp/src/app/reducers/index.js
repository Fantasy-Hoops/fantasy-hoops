import { combineReducers } from 'redux';
import { connectRouter } from 'connected-react-router';
import simpleReducer from './simpleReducer';

import history from '../utils/history';

export default combineReducers({
  router: connectRouter(history),
  simpleReducer
});
