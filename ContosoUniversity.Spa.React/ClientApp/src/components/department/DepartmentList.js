import React from 'react';
import PropTypes from 'prop-types';
import DepartmentListRow from './DepartmentListRow';

const DepartmentList =({departments}) => {
    return(
        <table className="table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Budget</th>
                    <th>Start Date</th>
                </tr>
            </thead>
            <tbody>
                {departments.map((department) => <DepartmentListRow key={department.id} department={department} />)}
            </tbody>
        </table>
    );
}

DepartmentList.propTypes = {
    departments: PropTypes.array.isRequired
}

export default DepartmentList;