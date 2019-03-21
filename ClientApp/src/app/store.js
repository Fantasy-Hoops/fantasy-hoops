import { compose, createStore, applyMiddleware } from 'redux';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import thunk from 'redux-thunk';
import combinedReducers from './reducers';
import history from './utils/history';

const composeEnhancer = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

export default function configureStore() {
  return createStore(
    connectRouter(history)(combinedReducers),
    composeEnhancer(applyMiddleware(routerMiddleware(history), thunk))
  );
}
