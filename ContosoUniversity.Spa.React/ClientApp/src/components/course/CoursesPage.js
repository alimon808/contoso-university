import React from 'react';
import { Link } from 'react-router-dom';
import {connect} from 'react-redux';
import PropTypes from 'prop-types';
import {bindActionCreators} from 'redux';
import * as courseActions from '../../actions/courseActions';
import CourseList from './CourseList';
import LoadingDots from '../common/LoadingDots';  

class CoursesPage extends React.Component {
    render() {
        const isLoading = this.props.loading;
        return (
            <div>
                <h1>Courses {isLoading && <LoadingDots interval={100} dots={20} />}</h1>
                <Link to="/course" className="btn btn-primary">Add Course</Link>
                <CourseList courses={this.props.courses} />
            </div>
        );
    }
}

CoursesPage.propTypes = {
    actions: PropTypes.object.isRequired,
    courses: PropTypes.array.isRequired,
    loading: PropTypes.bool.isRequired
};

function mapStateToProps(state, ownProps){
    return {
        courses: state.courses,
        loading: state.ajaxCallsInProgress > 0
    };
}

function mapDispatchToProps(dispatch){
    return {
        actions: bindActionCreators(courseActions,dispatch)
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(CoursesPage);