using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public enum GridRole
    {
        NotSpecified,
        MainGrid,
        SubGrid,
        DialogBox,
        ControlGrid,
        Invisible
    }

    public class VGrid : Grid
    {
        public GridRole Role
        {
            get { return (GridRole)base.GetValue(RoleProperty); }
            set { base.SetValue(RoleProperty, value); }
        }

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(GridRole), typeof(VGrid), new PropertyMetadata(default(GridRole)));

        public VGrid() : base() { }
    }
}
