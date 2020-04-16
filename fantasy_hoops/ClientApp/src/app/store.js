import {compose, createStore, applyMiddleware, combineReducers} from 'redux';
import { connectRouter, routerMiddleware } from 'connected-react-router';
import thunk from 'redux-thunk';
import {reducers} from './reducers';

export default function configureStore(history, initialState) {
  const middleware = [
    thunk,
    routerMiddleware(history)
  ];

  const rootReducer = combineReducers({
    ...reducers,
    router: connectRouter(history)
  });

  const enhancers = [];
  const windowIfDefined = typeof window === 'undefined' ? null : window;
  if (windowIfDefined && windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__) {
    enhancers.push(windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__());
  }

  return createStore(
      rootReducer,
      initialState,
      compose(applyMiddleware(...middleware), ...enhancers)
  );
}
