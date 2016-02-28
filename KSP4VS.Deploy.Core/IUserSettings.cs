using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy.Core
{
    public interface IUserSettings
    {
        string GetString(string settingName);
        void SetString(string settingName, string value);
        bool PropertyExists(string settingName);
    }
}
