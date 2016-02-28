using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.Deploy.Core
{
    [Export(typeof(IUserSettings))]
    public class VsUserSettings : IUserSettings
    {
        private const string SettingsPath = nameof(KSP4VS);

        private WritableSettingsStore settingsStore;

        [ImportingConstructor]
        public VsUserSettings([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            settingsStore = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public string GetString(string settingName)
        {
            return settingsStore.GetString(SettingsPath, settingName);
        }

        public bool PropertyExists(string settingName)
        {
            return settingsStore.PropertyExists(SettingsPath, settingName);
        }

        public void SetString(string settingName, string value)
        {
            settingsStore.SetString(SettingsPath, settingName, value);
        }
    }
}
