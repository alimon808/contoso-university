import delay from './delay';

// original source: https://raw.githubusercontent.com/coryhouse/pluralsight-redux-starter/master/src/api/mockCourseApi.js
// This file mocks a web API by working with the hard-coded data below.
// It uses setTimeout to simulate the delay of an AJAX call.
// All calls return promises.
const departments = [
  {
    "id": 1,
    "instructorID": 1,
    "name": "English",
    "budget": 350000.0000,
    "startDate": "2007-09-01T00:00:00"
  },
  {
    "id": 2,
    "instructorID": 2,
    "name": "Mathematics",
    "budget": 100000.0000,
    "startDate": "2007-09-01T00:00:00"
  },
  {
    "id": 3,
    "instructorID": 3,
    "name": "Engineering",
    "budget": 350000.0000,
    "startDate": "2007-09-01T00:00:00"
  },
  {
    "id": 4,
    "instructorID": 4,
    "name": "Economics",
    "budget": 100000.0000,
    "startDate": "2007-09-01T00:00:00"
  }
];

class departmentApi {
  static getAllDepartments() {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(Object.assign([], departments));
      }, delay);
    });
  }

}

export default departmentApi;