using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public class VKeyboardTab : UserControl
    {
        public virtual UIElement FocusElement
        {
            set
            {
                InputSimulator.SetFocus(this, value);
                focusElement = value;
            }
            get
            {
                return focusElement;
            }
        }
        protected UIElement focusElement;
        public virtual VKeyboard ParentKeyboard { get; set; }
        public string name;
        public virtual void Refocus()
        {
            InputSimulator.SetFocus(this, focusElement);
        }
        public virtual void Update() { }
    }
}
