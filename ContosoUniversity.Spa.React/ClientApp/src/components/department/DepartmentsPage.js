import React, {Component} from 'react';
import DepartmentList from './DepartmentList';

class DepartmentsPage extends Component {
    constructor(props){
        super(props);
        this.state = {departments: [], loading: true};
        fetch('api/department/details')
            .then(response => response.json())
            .then(data => {
                this.setState({departments: data, loading: false});
            });
    }

    render() {
        let contents = this.state.loading 
            ? <p><em>Loading...</em></p> 
            : <DepartmentList departments={this.state.departments} />;
        return (
            <div>
                <h1>Departments</h1>
                {contents}
            </div>
        );
    }
}

export default DepartmentsPage;