import React from 'react';
import PropTypes from 'prop-types';
import TextInput from '../common/TextInput';
import SelectInput from '../common/SelectInput';

const CourseForm = ({course, allDepartments, onChange, onSave, saving, errors}) => {
    return (
        <form>
            <h1>Manage Course</h1>
            <TextInput 
                name="title"
                label="Title"
                value={course.title}
                onChange={onChange}
            />
            <SelectInput 
                name="departmentID"
                label="Department"
                value={course.departmentID}
                defaultOption="Select Department"
                options={allDepartments}
                onChange={onChange}
            />
            <TextInput 
                name="credits"
                label="Credits"
                value={course.credits}
                onChange={onChange}
            />
            <input 
                type="submit"
                value={saving ? 'Saving...' : 'Save'}
                className="btn btn-primary"
                onClick={onSave}
            />
        </form>
    );
};

CourseForm.propTypes = {
    course: PropTypes.object.isRequired,
    allDepartments: PropTypes.array,
    onChange: PropTypes.func.isRequired,
    onSave: PropTypes.func.isRequired,
    saving: PropTypes.bool,
    errors: PropTypes.object
};

export default CourseForm;