export default function departmentReducer(state = [], action){
    switch(action.type) {
        case 'CREATE_DEPARTMENT':
            return [...state, Object.assign({}, action.department)];
        default:
            return state;
    }
}