using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Core.ModelsDTO.RequestDTO.Enrollments
{
    public class EnrollmentsParams
    {
        public string? Search { get; set; }
        public string? Sorting { get; set; }
        public int MaxPageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;

        private int pageSize = 5;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? value : MaxPageSize; }
        }

    }
}
