using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// The Dev Tools applet is meant to be used by the developers to provide some
    /// handy features for debugging or development testing.
    /// It may be added as an optional applet in the future.
    /// </summary>
    class DevManager: Applet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DevManager() : base("ap_dev_manager", "ap_desc_dev_manager")
        {
            IconSource = "Resources/Images/cog.png";
            Role = ButtonRole.Settings;
        }

        /// <summary>
        /// Returns the main page.
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new DevManagerPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return null;
        }

        /// <summary>
        /// Initialize config and settings.
        /// </summary>
        public override void InitializeSettingsValues()
        {
            
        }
    }
}
