import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { DepartmentsPage } from './components/department/DepartmentsPage';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/departments' component={DepartmentsPage} />
      </Layout>
    );
  }
}
