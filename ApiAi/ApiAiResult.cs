using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAi
{
    public class ApiAiResult
    {
        public string Platform { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }

        public ICollection<string> Errors { get; set; }

        public ApiAiResult()
        {
            Errors = new List<string>();
        }

    }
}
