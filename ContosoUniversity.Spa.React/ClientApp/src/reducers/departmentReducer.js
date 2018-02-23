import * as types from '../actions/actionTypes';

export default function departmentReducer(state = [], action){
    switch(action.type) {
        case types.LOAD_DEPARTMENTS_SUCCESS:
            return action.departments;
        default:
            return state;
    }
}