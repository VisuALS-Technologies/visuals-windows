using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

// Authored by Jonathan Ballard
namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VNumPad.xaml
    /// VNumPad allows the user to provide specific numerical input by typing the number they want to enter.
    /// Interface styles: Confirmation is applied by default
    ///     Confirmation: Input provided by the user is shown in a textbox at the top. The output of the selection control
    ///     is only updated when the user presses the submit button to confirm their input.
    ///     Direct: The confirmation bar at the top is not drawn, so each key press the user makes causes the selection control to
    ///     update its output. (useful for applications (such as a calculator) where displaying the input twice is unnecessarily redundant)
    /// Key Layouts (Applied only to buttons 1-9): Phone is applied by default
    ///     Phone: Key layout found on most phones. (Rows from top to bottom: [123/456/789])
    ///     Computer: Key layout used by most computers and calculators. (Rows from top to bottom: [789/456/123])
    /// </summary>
    public partial class VNumPad : VSelectionControl
    {
        // ***** Property / Internal Variable Management *****

        // *** Properties / Internal Variables***

        private static readonly Regex badInputRegex = new Regex("[^0-9.]+"); //Regex specifying what input isn't allowed (anything that isn't 0-9 or . by default)


        // ** User Interface Properties **

        // bool integersOnly = false; //[IDEA FOR FUTURE FEATURE] If true the user can only input integers (the decimal button is disabled), if false user can input decimals as well.
        // int decimalPointLimit = 1; //[IDEA FOR FUTURE FEATURE] The maximum number of decimal points the user can input. Default is one, so all input provided will be a real number

        /// <summary>
        /// Indicates if the direct interface style is active. If true it is active and all input is automatically updated. If false the confirmation
        /// interface is applied, so the user has the ability to review and confirm their input before an update is sent.
        /// </summary>
        public bool UseDirectInterface
        {
            get { return (bool)base.GetValue(UseDirectInterfaceProperty); }
            set
            {
                base.SetValue(UseDirectInterfaceProperty, value);  //Updating the internal property with the value provided
                UpdateUI(); //Update the user interface
            }
        }

        public static readonly DependencyProperty UseDirectInterfaceProperty = DependencyProperty.Register(
            "UseDirectInterface", typeof(bool), typeof(VNumPad), new PropertyMetadata(default(bool))); //Registering the UseDirectInterface property in XAML

        /// <summary>
        /// Indicatates if the computer key layout is applied. If true it is applied (thus 123 is at the bottom of the key pad),
        /// otherwise the phone layout is applied (123 at the top).
        /// </summary>
        public bool UseComputerKeyLayout
        {
            get { return (bool)base.GetValue(UseComputerKeyLayoutProperty); }
            set
            {
                base.SetValue(UseComputerKeyLayoutProperty, value); //Updating the internal property with the value provided
                UpdateUI(); //Update VNumPads user interface
            }
        }

        public static readonly DependencyProperty UseComputerKeyLayoutProperty = DependencyProperty.Register(
            "UseComputerKeyLayout", typeof(bool), typeof(VNumPad), new PropertyMetadata(default(bool))); //Registering the UseComputerKeyLayout property in XAML

        // ** Other Properties / Variables **
        /// <summary>
        /// The number the user has selected, represtented as a StringBuilder object to make modifications easy
        /// </summary>
        StringBuilder userInput = new StringBuilder(); //Initialized to an empty StringBuilder

        // ***** Constructors *****

        /// <summary>
        /// Default Constructor for VNumPad. The confirmation interface style and phone key layout are applied by default.
        /// </summary>
        public VNumPad()
        {
            InitializeComponent(); // Not sure what this does, but the code behind it is automatically generated, so I'm not going to mess with it.
            SelectedItemString = ""; //Initializing the SelectedItemString to a blank string (no input has been provided)

            // Setting the default layout style (redundantly updating just in case)
            UseDirectInterface = false; // By default all input needs to be confirmed, display confirmation bar
            UseComputerKeyLayout = false; // By default the phone layout is applied. (Studies have found it to be the most intuitive to use)

            UpdateUI(); //Update VNumPad to have the specified user interface
        }

        /// <summary>
        /// Constructor for VNumPad. Interface style must be specified. (Phone layout is applied by default)
        /// </summary>
        /// <param name="useDirectInterface">If true the direct interface style will be used (the input confirmation bar will not draw)</param>
        public VNumPad(bool useDirectInterface) : this() //From what I understand this calls the default constructor first
        {
            //The only setting that should be changed from what the default constructor set it to is whether or not input confirmation will be bypassed
            UseDirectInterface = useDirectInterface; //Updating the interface style to direct i.e. noting that input confirmation will be bypassed

            UpdateUI(); //Update VNumPad to have the specified user interface
        }

        /// <summary>
        /// Constructor for VNumPad. Interface style and key layout type must be specified.
        /// </summary>
        /// <param name="useDirectInterface">If true the direct interface style will be used (the input confirmation bar will not draw)</param>
        /// <param name="useComputerKeyLayout">If true the computer key layout will be applied (123 on bottom row)</param>
        public VNumPad(bool useDirectInterface, bool useComputerKeyLayout) : this(useDirectInterface)
        {
            //Because I am chaining constructors only one setting needs to be changed, the key layout type
            UseComputerKeyLayout = useComputerKeyLayout;

            UpdateUI(); //Update VNumPads user interface
        }

        // ***** Functions *****

        //*** Internal Functions ***

        /// <summary>
        /// Updates the VNumPad's user interface.
        /// </summary>
        private void UpdateUI()
        {

            // Updating Interface Style
            if (UseDirectInterface)
            {
                if (!ConfirmationBarRow.Height.Equals(0)) //If the confirmation bar height (in star units by default) isn't already set to 0
                    ConfirmationBarRow.Height = new GridLength(0, GridUnitType.Star); // Set the height to 0
            }
            else
            {
                if (!ConfirmationBarRow.Height.Equals(1)) //If the confirmation bar height isn't already 1 (in star units by default)
                    ConfirmationBarRow.Height = new GridLength(1, GridUnitType.Star); // Set the height to 1
            }

            //Updating Key Layout Type
            if (UseComputerKeyLayout)
            {
                if (!ComputerKeyGrid.Visibility.Equals(Visibility.Visible)) //If the computer key grid isn't already visible
                {
                    PhoneKeyGrid.Visibility = Visibility.Hidden;
                    ComputerKeyGrid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (!PhoneKeyGrid.Visibility.Equals(Visibility.Visible)) //If the phone key grid isn't already visible
                {
                    ComputerKeyGrid.Visibility = Visibility.Hidden;
                    PhoneKeyGrid.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Updates the output of VNumPad.
        /// Sets / updates the contents of SelectedItemString and SelectedItem from the contents of InputBox
        /// and raises the ItemSelectionChangedEvent
        /// </summary>
        private void UpdateOutput()
        {
            SelectedItemString = InputBox.Text.ToString(); // Updating SelectedItemString with the string of input that the user has provided (Pulling from the text box so users can also type in values)
            SelectedItem = Convert.ToDouble(SelectedItemString); // Updating SelectedItem (a double for VNumPad) with a double generated from SelectedItem

            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent); //Sending the ItemSelectionChangedEvent to indicate that the selection has changed
            RaiseEvent(args);
        }

        /// <summary>
        /// Verfies that the string provided satisfies the limitations specified in the Regex (Regular Expression)
        /// This is based upon the post: https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        /// </summary>
        /// <param name="input">String to verify</param>
        /// <param name="badInputRegex">Regular Expression specifying what kind of input is not allowed</param>
        /// <returns>True if the input satisfies the limitations specified by badInputRegex</returns>
        private static bool IsTextInputAllowed(String input, Regex badInputRegex)
        {
            return !badInputRegex.IsMatch(input); //Only returning true if the input doesn't fufill the Regex specifying bad input
        }

        // *** Event Handlers ***

        // ** VNumPad Base **

        /// <summary>
        /// Event handler for when VNumPad is initialized.
        /// VNumPad has various user interface configurations and the correct one needs to be applied upon initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VNumPad_Initialized(object sender, EventArgs e)
        {
            UpdateUI(); //Update / Initialize the UI
        }

        /// <summary>
        /// Event handler for when VNumPad is loaded.
        /// VNumPad has various user interface configurations and the correct one needs to be applied if any are updated.
        /// This event is always triggered after initialization happens (and can work in cases where initilization won't, such when there are properties that change the layout).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VNumPad_Loaded(object sender, RoutedEventArgs e)
        {

            UpdateUI(); // Update / Initialize the UI
        }



        // ** Confirmation Bar **

        /// <summary>
        /// Event handler for when the text input is previewed.
        /// Translation: Text has been inputed, make sure that it only contains the allowed characters (This is based on this post: https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextInputAllowed(e.Text, badInputRegex); //Not sure what this does, but if I were to guess it prevents the input from being submitted if it's bad. (The event gets set to handeled if the input is bad)
        }

        /// <summary>
        /// Event handler for when the user attempts to paste input.
        /// Responsible for verifying that the input provided only contains the characters which are allowed
        /// (This is based on this post: https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                //The thing to be pasted is a string, but does it contain any "illegal" characters
                String inputToPaste = (String)e.DataObject.GetData(typeof(String)); // Save the thing that the user is trying to input into a string

                if (!IsTextInputAllowed(inputToPaste, badInputRegex))
                {
                    e.CancelCommand(); //The thing to input was bad, cancel the attempt
                }
            }
            else
            {
                //The thing to be pasted isn't a string, cancel the attempt
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Event handler for when the submit key has been pressed, indicating the user has finished providing input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            UpdateOutput();
        }

        // ** Key Layout **

        /// <summary>
        /// The event handler for when the 1 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void One_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("1");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }
        /// <summary>
        /// The event handler for when the 2 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Two_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("2");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 3 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Three_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("3");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 4 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Four_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("4");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 5 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Five_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("5");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 6 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Six_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("6");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 7 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("7");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 8 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("8");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the 9 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("9");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        // ** Bottom Row **

        /// <summary>
        /// The event handler for when the 0 key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append("0");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the decimal point key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecimalPoint_Click(object sender, RoutedEventArgs e)
        {
            userInput.Append(".");
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }

        /// <summary>
        /// The event handler for when the backspace key has been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            userInput.Remove(userInput.Length - 1, 1); //Remove the text beginning at the last index in the string, and remove a string of 1 characters
            InputBox.Text = userInput.ToString(); //Updating the text displayed in the text box

            if (UseDirectInterface) //If using the direct interface send the update automatically as the submit bar is hidden
                UpdateOutput();
        }
    }
}
