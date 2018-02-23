import React from 'react';
import {Redirect} from 'react-router';
import PropTypes from 'prop-types';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import CourseForm from './CourseForm';
import * as courseActions from '../../actions/courseActions';
import { departmentsFormattedForDropdown } from '../selectors/selectors';
import toastr from 'toastr';

class ManageCoursePage extends React.Component {
    constructor(props, context) {
        super(props, context);
        this.state = {
            course: Object.assign({}, this.props.course),
            errors: {},
            saving: false,
            toCourses: false
        };

        this.updateCourseState = this.updateCourseState.bind(this);
        this.saveCourse = this.saveCourse.bind(this);
    }

    componentWillReceiveProps(nextProps) {
        if(this.props.course.id !== nextProps.course.id) {
            // necessary to populate from when existing course is loaded directly
            this.setState({course: Object.assign({}, nextProps.course) });
        }
    }

    updateCourseState(event){
        const field = event.target.name;
        let course = this.state.course;
        course[field] = event.target.value;
        return this.setState({course: course});
    }

    saveCourse(event) {
        event.preventDefault();

        this.setState( {saving: true});
        this.props.actions.saveCourse(this.state.course)
            .then(() => this.redirect())
            .catch(error => {
                toastr.error(error);
                this.setState( {saving: false});
            });
    }

    redirect() {
        this.setState({saving: false, toCourses: true});
        toastr.success('Course saved');
    }

    render () {
        if(this.state.toCourses === true) {
            return <Redirect to='/courses' />
        }
        return (
            <CourseForm 
                allDepartments = {this.props.departments}
                onChange={this.updateCourseState}
                onSave={this.saveCourse}
                course={this.state.course}
                errros={this.state.errors}
                saving={this.state.saving}
            />
        );
    };
}

ManageCoursePage.propTypes = {
    course: PropTypes.object.isRequired,
    departments: PropTypes.array.isRequired,
    actions: PropTypes.object.isRequired
};

ManageCoursePage.contextTypes = {
    router: PropTypes.object
};

function getCourseById(courses, id){
    const course = courses.filter(course => Number(course.id) === Number(id));
    if(course.length) return course[0];
    return null;
}

function mapStateToProps(state, ownProps) {
    const courseId = ownProps.match.params.id;
    let course = {id:'', title:'', departmentID:'',credits:''};
    if(courseId && state.courses.length > 0){
        course = getCourseById(state.courses, courseId);
    }
    
    return {
        course: course,
        departments: departmentsFormattedForDropdown(state.departments)
    }
}

function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(courseActions, dispatch)
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(ManageCoursePage);