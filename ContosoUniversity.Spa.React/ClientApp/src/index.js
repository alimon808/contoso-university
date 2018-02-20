import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/css/bootstrap-theme.css';
import './index.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import {Provider} from 'react-redux';
import configureStore from './store/configureStore';
import { loadCourses } from './actions/courseActions';

const store = configureStore();
store.dispatch(loadCourses());

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
  <Provider store={store}>
  <BrowserRouter basename={baseUrl}>
    <App />
  </BrowserRouter>
  </Provider>,
  rootElement);

registerServiceWorker();
