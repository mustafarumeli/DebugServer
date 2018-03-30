using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugServer
{
    public enum Languages
    {
        csharp,
        c,
        cpp,
        python,
        java
    }
    public class Debug
    {
        public string _id { get; set; }
        public string Code { get; set; }
        public Languages Language { get; set; }
        public string SuccessResult { get; set; }
        public string ErrorResult { get; set; }
        public Debug()
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}
