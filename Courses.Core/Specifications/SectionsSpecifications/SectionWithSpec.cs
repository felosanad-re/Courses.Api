using Courses.Core.Models.Enrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Core.Specifications.SectionsSpecifications
{
    public class SectionWithSpec : BaseSpecifications<Section>
    {
        public SectionWithSpec(int courseId)
            : base(x => x.CourseId == courseId)
        {

        }
    }
}
