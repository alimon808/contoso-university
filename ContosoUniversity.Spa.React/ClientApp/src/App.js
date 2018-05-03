import React, { Component } from 'react';
import { Route } from 'react-router';
import PropTypes from 'prop-types';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import DepartmentsPage from './components/department/DepartmentsPage';
import CoursesPage from './components/course/CoursesPage';
import ManageCoursePage from './components/course/ManageCoursePage';
import AboutPage from './components/about/AboutPage';
import ContactPage from './components/contact/ContactPage';

class App extends Component {
  displayName = App.name
  
  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/about' component={AboutPage} />
        <Route path='/contact' component={ContactPage} />
        <Route path='/departments' component={DepartmentsPage} />
        <Route path='/courses' component={CoursesPage} />
        <Route path='/course/:id' component={ManageCoursePage} />        
        <Route exact path='/course' component={ManageCoursePage} />
      </Layout>
    );
  }
}

App.propTypes = {
  router: PropTypes.object
}

export default App;
