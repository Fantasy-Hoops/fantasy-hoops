import React from 'react';
import { Provider } from 'react-redux';
import configureStore from './store';
import configureRoutes from './routes';

const store = configureStore();

export default () => (
  <Provider store={store}>
    {configureRoutes(store)}
  </Provider>
);
