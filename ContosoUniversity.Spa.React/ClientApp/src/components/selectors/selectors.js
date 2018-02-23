export function departmentsFormattedForDropdown(departments){
    return departments.map(department => {
        return {
            value: department.id,
            text: department.name
        };
    });
}