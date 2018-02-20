import React from 'react';
import PropTypes from 'prop-types';
import {Link} from 'react-router-dom';

const CourseListRow = ({course}) => {
    return (
        <tr>
            <td><Link to={'/course/' + course.id}>{course.courseNumber}</Link></td>
            <td>{course.title}</td>
            <td>{course.credits}</td>
        </tr>
    );
};

CourseListRow.propTypes = {
    course: PropTypes.object.isRequired
};

export default CourseListRow;