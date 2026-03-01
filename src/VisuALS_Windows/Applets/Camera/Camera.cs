using System.IO;

namespace VisuALS_WPF_App
{
    public class Camera : Applet
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Camera() : base("ap_camera", "ap_desc_camera")
        {
            IconSource = "Resources/Images/camera.png";
            Role = ButtonRole.Notes;
        }

        /// <summary>
        /// Returns the main page
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new CameraPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return new NotepadSettings();
        }

        /// <summary>
        /// Initialize config and settings
        /// </summary>
        public override void InitializeSettingsValues()
        {

        }
    }
}
