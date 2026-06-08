using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;

namespace Courses.Core.Specifications.EnrollmentSpecifications
{
    public class EnrollmentWithStudentSpec: BaseSpecifications<Enrollment>
    {
        /// <summary>
        /// Used for searching/filtering students by instructor.
        /// applyPagination: false is used in the two-phase query approach where
        /// pagination is applied in-memory at the student level after grouping.
        /// includeCourse: false skips loading Course navigation in Phase 1 (search phase).
        /// </summary>
        public EnrollmentWithStudentSpec(int? instructorId, StudentParams @params, bool forCount = false, bool applyPagination = true, bool includeCourse = true)
            :base(x =>
                (string.IsNullOrEmpty(@params.Search) ||
                 x.Course.Name.ToLower().Contains(@params.Search.Trim().ToLower()) ||
                 x.Student.Name.ToLower().Contains(@params.Search.Trim().ToLower())) &&
                (x.Course.InstructorId == instructorId)&&
                (x.Status == EnrollStatus.Active)
            )
        {
            IsTracking = false;

            if(!forCount)
            {
                Includes.Add(x => x.Student);
                if(includeCourse)
                    Includes.Add(x => x.Course);

                if(applyPagination)
                    AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);
                AddSorting(@params);
            }
        }

        /// <summary>
        /// Used for getting all enrollments for a specific student with an instructor.
        /// Used by GetStudentInstructorAsync (single student detail).
        /// </summary>
        public EnrollmentWithStudentSpec(int? instructorId, int studentId)
            :base(x =>
                (x.Course.InstructorId == instructorId)&&
                (x.Student.Id == studentId)&&
                (x.Status == EnrollStatus.Active)
            )
        {
            Includes.Add(x => x.Student);
            Includes.Add(x => x.Course);
            IsTracking = false;
        }

        /// <summary>
        /// Used for getting all enrollments for specific students with an instructor.
        /// Phase 2 of two-phase query: retrieves complete data for matching students
        /// without search filter, ensuring correct CourseCount and full Courses list.
        /// </summary>
        public EnrollmentWithStudentSpec(int? instructorId, List<int> studentIds)
            :base(x =>
                (x.Course.InstructorId == instructorId) &&
                (studentIds.Contains(x.StudentId)) &&
                (x.Status == EnrollStatus.Active)
            )
        {
            Includes.Add(x => x.Student);
            Includes.Add(x => x.Course);
            IsTracking = false;
        }

        #region Helper method
        private void AddSorting(StudentParams @params)
        {
            // Note: These DB-level sorts are applied to enrollment rows.
            // For the two-phase student query approach, final sorting is applied
            // in-memory on student-level fields after grouping (in the service).
            // These DB-level sorts help order enrollment rows in Phase 1 for
            // consistent grouping results.
            switch (@params.Sort)
            {
                case "nameDesc":
                    AddOrderByDesc(x => x.Student.Name);
                    break;

                case "firstEnrollment":
                    AddOrderBy(x => x.CreatedAt);
                    break;

                case "firstEnrollmentDesc":
                    AddOrderByDesc(x => x.CreatedAt);
                    break;

                case "courseName":
                    AddOrderBy(x => x.Course.Name);
                    break;

                case "courseNameDesc":
                    AddOrderByDesc(x => x.Course.Name);
                    break;

                default:
                    AddOrderBy(x => x.Student.Name);
                    break;
            }
        }
        #endregion
    }
}
