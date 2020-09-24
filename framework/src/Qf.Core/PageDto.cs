using System.Collections.Generic;

namespace Qf.Core
{
    public class PageDto<T>
    {
        public int Records { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public IEnumerable<T> Rows { get; set; }
    }
}
