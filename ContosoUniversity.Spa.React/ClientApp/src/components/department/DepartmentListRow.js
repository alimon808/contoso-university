import React from 'react';
import PropTypes from 'prop-types';
import Moment from 'moment';
import Numeral from 'numeral';

const DepartmentListRow = ({department}) => {
    return(
        <tr>
            <td>{department.name}</td>
            <td>{Numeral(department.budget).format('$0,0.00')}</td>
            <td>{Moment(department.startDate).format('DD/MM/YYYY')}</td>
        </tr>
    );
};

DepartmentListRow.propTypes = {
    department: PropTypes.object.isRequired
}

export default DepartmentListRow;