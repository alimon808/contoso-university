import {combineReducers} from 'redux';
import courses from './courseReducer';
import departments from './departmentReducer';

const rootReducer = combineReducers({
    courses,
    departments
});

export default rootReducer;