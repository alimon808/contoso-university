import * as types from './actionTypes';
import departmentApi from '../api/mockDepartmentApi';

export function loadDepartmentsSuccess(departments) {
    return {type: types.LOAD_DEPARTMENTS_SUCCESS, departments};
}

export function loadDepartments(){
    return function(dispatch){
        return departmentApi.getAllDepartments().then(departments => {
            dispatch(loadDepartmentsSuccess(departments));
        }).catch(error => { 
            throw(error);
        });
    }
}