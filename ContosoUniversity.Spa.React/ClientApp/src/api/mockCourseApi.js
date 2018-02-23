import delay from './delay';

// original source: https://raw.githubusercontent.com/coryhouse/pluralsight-redux-starter/master/src/api/mockCourseApi.js
// This file mocks a web API by working with the hard-coded data below.
// It uses setTimeout to simulate the delay of an AJAX call.
// All calls return promises.
const courses = [
  {
    "courseNumber": 1050,
    "title": "Chemistry",
    "credits": 3,
    "departmentID": 3,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 1,
    "addedDate": "2017-11-09T19:53:51.559982",
    "modifiedDate": "2017-11-09T19:53:51.5599823",
    "rowVersion": "AAAAAAAAB+I="
  },
  {
    "courseNumber": 4022,
    "title": "Microeconomics",
    "credits": 3,
    "departmentID": 4,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 2,
    "addedDate": "2017-11-09T19:53:51.5775125",
    "modifiedDate": "2017-11-09T19:53:51.5775129",
    "rowVersion": "AAAAAAAAB+M="
  },
  {
    "courseNumber": 4041,
    "title": "Macroeconomics",
    "credits": 3,
    "departmentID": 4,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 3,
    "addedDate": "2017-11-09T19:53:51.5776416",
    "modifiedDate": "2017-11-09T19:53:51.5776416",
    "rowVersion": "AAAAAAAAB+Q="
  },
  {
    "courseNumber": 1045,
    "title": "Calculus",
    "credits": 4,
    "departmentID": 2,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 4,
    "addedDate": "2017-11-09T19:53:51.5777097",
    "modifiedDate": "2017-11-09T19:53:51.5777097",
    "rowVersion": "AAAAAAAAB+U="
  },
  {
    "courseNumber": 3141,
    "title": "Trigonometry",
    "credits": 4,
    "departmentID": 2,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 5,
    "addedDate": "2017-11-09T19:53:51.5777787",
    "modifiedDate": "2017-11-09T19:53:51.5777787",
    "rowVersion": "AAAAAAAAB+Y="
  },
  {
    "courseNumber": 2021,
    "title": "Composition",
    "credits": 3,
    "departmentID": 1,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 6,
    "addedDate": "2017-11-09T19:53:51.5778388",
    "modifiedDate": "2017-11-09T19:53:51.5778392",
    "rowVersion": "AAAAAAAAB+c="
  },
  {
    "courseNumber": 2042,
    "title": "Literature",
    "credits": 4,
    "departmentID": 1,
    "department": null,
    "enrollments": null,
    "courseAssignments": null,
    "id": 7,
    "addedDate": "2017-11-09T19:53:51.5779147",
    "modifiedDate": "2017-11-09T19:53:51.5779147",
    "rowVersion": "AAAAAAAAB+g="
  }
]

// function replaceAll(str, find, replace) {
//   return str.replace(new RegExp(find, 'g'), replace);
// }

// This would be performed on the server in a real app. Just stubbing in.
const generateId = (course) => {
  // return replaceAll(course.title, ' ', '-');
  return courses.length + 1;
};

const generateCourseNumber = (course) => {
  let departmentCourseNumbers = courses.filter(c => Number(c.departmentID) === Number(course.departmentID))
                                        .map(c => c.courseNumber);
  if(departmentCourseNumbers.length)  {
    return Math.max(...departmentCourseNumbers) + 1;
  } else {
    return 1;
  }    
}

class CourseApi {
  static getAllCourses() {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(Object.assign([], courses));
      }, delay);
    });
  }

  static saveCourse(course) {
    course = Object.assign({}, course); // to avoid manipulating object passed in.
    // course.departmentID = Number(course.departmentID);
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        // Simulate server-side validation
        const minCourseTitleLength = 1;
        if (course.title.length < minCourseTitleLength) {
          reject(`Title must be at least ${minCourseTitleLength} characters.`);
        }

        if (course.id) {
          const existingCourseIndex = courses.findIndex(a => a.id === course.id);
          courses.splice(existingCourseIndex, 1, course);
        } else {
          //Just simulating creation here.
          //The server would generate ids and watchHref's for new courses in a real app.
          //Cloning so copy returned is passed by value rather than by reference.
          course.id = generateId(course);
          course.courseNumber = generateCourseNumber(course);
          courses.push(course);
        }

        resolve(course);
      }, delay);
    });
  }

  // static deleteCourse(courseId) {
  //   return new Promise((resolve, reject) => {
  //     setTimeout(() => {
  //       const indexOfCourseToDelete = courses.findIndex(course => {
  //         course.id === courseId;
  //       });
  //       courses.splice(indexOfCourseToDelete, 1);
  //       resolve();
  //     }, delay);
  //   });
  // }
}

export default CourseApi;