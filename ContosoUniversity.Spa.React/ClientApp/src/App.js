import React, { Component } from 'react';
import { Route } from 'react-router';
import PropTypes from 'prop-types';
import { Layout } from './components/common/Layout';
import DepartmentsPage from './components/department/DepartmentsPage';
import CoursesPage from './components/course/CoursesPage';
import ManageCoursePage from './components/course/ManageCoursePage';
import AboutPage from './components/about/AboutPage';
import ContactPage from './components/contact/ContactPage';
import RegisterPage from './components/register/RegisterPage';
import LoginPage from './components/login/LoginPage';
import HomePage from './components/home/HomePage.js';

class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={HomePage} />
        <Route path='/about' component={AboutPage} />
        <Route path='/contact' component={ContactPage} />
        <Route path='/departments' component={DepartmentsPage} />
        <Route path='/courses' component={CoursesPage} />
        <Route path='/course/:id' component={ManageCoursePage} />
        <Route exact path='/course' component={ManageCoursePage} />
        <Route path='/register' component={RegisterPage} />
        <Route path='/login' component={LoginPage} />
      </Layout>
    );
  }
}

App.propTypes = {
  router: PropTypes.object
}

export default App;
