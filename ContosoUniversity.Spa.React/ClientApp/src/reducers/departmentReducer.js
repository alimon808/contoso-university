import * as types from '../actions/actionTypes';
import initialState from './initialState';

export default function departmentReducer(state = initialState.departments, action){
    switch(action.type) {
        case types.LOAD_DEPARTMENTS_SUCCESS:
            return action.departments;
        default:
            return state;
    }
}