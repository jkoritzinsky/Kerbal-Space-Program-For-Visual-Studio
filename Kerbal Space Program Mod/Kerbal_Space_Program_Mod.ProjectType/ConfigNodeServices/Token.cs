using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNodeServices
{
    public class Token
    {
        public int Location { get; set; }
        public int Length { get; set; }
        public enum Type
        {
            Name,
            Value,
            Comment,
            Other
        }

        public Type TokenType { get; set; }
    }
}
