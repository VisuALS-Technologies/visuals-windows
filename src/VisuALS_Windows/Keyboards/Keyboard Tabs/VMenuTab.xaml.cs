using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VMenuTab.xaml
    /// </summary>
    public partial class VMenuTab : VKeyboardTab
    {
        public VMenuTab()
        {
            InitializeComponent();
            name = "Menu";
            tabList.ItemSelectionChanged += Tab_Clicked;
        }

        public override void Update()
        {
            foreach (string tab in ParentKeyboard.Tabs)
            {
                tabList.AddItem(tab);
            }
        }

        private void Tab_Clicked(object sender, RoutedEventArgs e)
        {
            //VList l = sender as VList;
            //ParentKeyboard.SwitchTab(l.SelectedItemName);
        }
    }
}
