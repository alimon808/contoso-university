import {combineReducers} from 'redux';
import courses from './courseReducer';
import departments from './departmentReducer';
import ajaxCallsInProgress from './ajaxStatusReducer';

const rootReducer = combineReducers({
    courses,
    departments,
    ajaxCallsInProgress
});

export default rootReducer;