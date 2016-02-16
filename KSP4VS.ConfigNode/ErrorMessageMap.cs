using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace KSP4VS.ConfigNode
{
    public static class ErrorMessageMap
    {
        public static string Map(string elementName)
        {
            if (elementName == null) throw new ArgumentException(nameof(elementName));
            return ErrorMessages.ResourceManager.GetString(elementName, ErrorMessages.Culture);
        }
    }
}
