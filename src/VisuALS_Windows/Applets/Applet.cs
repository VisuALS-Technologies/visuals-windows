using System.IO;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Applets are the main content of the VisuALS application. They are essentially small apps inside of the VisuALS app, hence "applet".
    /// These include things like the Notepad, Web Browser, and Text to Speech.
    /// This class represents an applet, and contains methods for getting the main page of the app, accessing it's settings, 
    /// and other data like the name and icon for the app.
    /// This is only the abstract base class and is meant to be subclassed for each new applet. Each sublcassed applet is only meant to
    /// be instantiated once as it is essentially meta-data for the applet and doesn't actually hold an instance of a page or anything
    /// like that. This abstract class contains a dictionary and a method for tracking applet instances.
    /// </summary>
    public abstract class Applet
    {
        /// <summary>
        /// Language token for the name of the applet.
        /// </summary>
        public string Name;

        /// Language token for the description of the applet.
        /// </summary>
        public string Description;

        /// <summary>
        /// Path to the icon for the applet.
        /// </summary>
        public string IconSource;

        /// <summary>
        /// Role for the applet button on the main screen.
        /// This may be changed to allow the applet to have a distinct color instead
        /// in the future.
        /// </summary>
        public ButtonRole Role;

        /// <summary>
        /// The path to the settings folder for this applet.
        /// This is set automatically on instantiation based on the app name.
        /// Since this should not be dynamically changed, the setter is private.
        /// </summary>
        public string SettingsPath { private set; get; }

        /// <summary>
        /// The config file for the applet.
        /// This should be used to store config data for the applet,
        /// such as the web browser style for the web browser,
        /// in other words it is for "preferences".
        /// </summary>
        public SettingsFile Config;

        /// <summary>
        /// The session data file for the applet.
        /// This should be used to store persistency data
        /// such as what the user was last typing in a notepad.
        /// </summary>
        public SettingsFile Session;

        /// <summary>
        /// Default constructor. Does NO setup whatsoever. Generally shouldn't be used.
        /// </summary>
        public Applet() { }

        /// <summary>
        /// Sets up the applet paths and such based on the apps name.
        /// This is the base constructor that most subclasses of Applet should use
        /// as it performs necessary setup.
        /// </summary>
        /// <param name="name"></param>
        public Applet(string name, string description)
        {
            Name = name;
            Description = description;
            SettingsPath = Path.Combine(AppPaths.SettingsPath, "applets\\" + Name);
            Config = new SettingsFile("applets\\" + Name + "\\config");
            Session = new SettingsFile("applets\\" + Name + "\\session");
            InitializeSettingsValues();
        }

        /// <summary>
        /// "Runs" an applet, ie. creates the main page and navigates the main window's frame to it.
        /// </summary>
        public void Run()
        {
            MainWindow.ShowPage(CreateMainPage());
        }

        /// <summary>
        /// Initializes any config or session data.
        /// This should be overwritten by the subclass
        /// to do necessary setup.
        /// </summary>
        public abstract void InitializeSettingsValues();

        /// <summary>
        /// Creates and returns the main page.
        /// This should be overwritten by the subclass to return
        /// the appropriate page.
        /// This is more flexible then simply setting a property, 
        /// as some applets like the Web Browser have more than
        /// one possible main page (in that particular case depending on a
        /// config option).
        /// </summary>
        /// <returns></returns>
        public abstract AppletPage CreateMainPage();

        /// <summary>
        /// Creates and returns the settings page for the applet.
        /// This should be overwritten by the subclass to return
        /// the appropriate page.
        /// This is more flexible then simply setting a property
        /// as it allows the application to have more than one settings page
        /// it could display based on circumstances.
        /// </summary>
        /// <returns></returns>
        public abstract AppletPage CreateSettingsPage();
    }
}
