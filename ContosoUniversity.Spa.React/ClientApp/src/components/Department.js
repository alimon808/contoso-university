import React, {Component} from 'react';

export class Department extends Component {
    constructor(props){
        super(props);
        this.state = { departments: [], loading: true};

        fetch('api/department')
            .then(response => response.json())
            .then(data => {
                this.setState({departments: data, loading: false});
            });
    }

    departmentTable(departments){
        return(
            <table className='table'>
                <thead>
                    <tr>
                        <th>Name</th>
                    </tr>
                </thead>
                <tbody>
                    {departments.map((elem, index) => 
                        <tr key={index}>
                            <td>{elem}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading 
            ? <p><em>Loading...</em></p> 
            : this.departmentTable(this.state.departments)
        return (
            <div>
                <h1>Departments</h1>
                {contents}
            </div>
        );
    }
}