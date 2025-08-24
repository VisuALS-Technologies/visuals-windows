using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VList.xaml
    /// </summary>
    public partial class VList : VSelectionControl
    {
        public int Rows
        {
            get { return (int)base.GetValue(RowsProperty); }
            set
            {
                SetRows(value);
                base.SetValue(RowsProperty, value);
            }
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            "Rows", typeof(int), typeof(VList), new PropertyMetadata(default(int)));

        public int Cols
        {
            get { return (int)base.GetValue(ColsProperty); }
            set
            {
                SetCols(value);
                base.SetValue(ColsProperty, value);
            }
        }

        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            "Cols", typeof(int), typeof(VList), new PropertyMetadata(default(int)));

        public string Prompt
        {
            get { return (string)base.GetValue(PromptProperty); }
            set { base.SetValue(PromptProperty, value); }
        }

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(VList), new PropertyMetadata(default(string)));

        public bool ShowSelection
        {
            get { return (bool)base.GetValue(ShowSelectionProperty); }
            set { base.SetValue(ShowSelectionProperty, value); }
        }

        public static readonly DependencyProperty ShowSelectionProperty = DependencyProperty.Register(
            "ShowSelection", typeof(bool), typeof(VList), new PropertyMetadata(false));

        public bool ShowPrompt
        {
            get { return (bool)base.GetValue(ShowPromptProperty); }
            set { base.SetValue(ShowPromptProperty, value); }
        }

        public static readonly DependencyProperty ShowPromptProperty = DependencyProperty.Register(
            "ShowPrompt", typeof(bool), typeof(VList), new PropertyMetadata(true, ShowPromptChanged));

        private Dictionary<string, object> items = new Dictionary<string, object>();

        private Dictionary<object, VButton> buttons = new Dictionary<object, VButton>();

        public List<object> Items { get { return items.Values.ToList(); } }

        public List<string> ItemStrings { get { return items.Keys.ToList(); } }

        public delegate string StringLabelExtractor<T>(T obj);

        public delegate UIElement LabelExtractor<T>(T obj);

        public int page
        {
            get;
            set;
        }

        public VList()
        {
            InitializeComponent();
            Rows = 3;
            Cols = 3;
            SetRows(Rows);
            SetCols(Cols);
        }

        public VList(int rows, int cols)
        {
            InitializeComponent();
            Rows = rows;
            Cols = cols;
            SetRows(Rows);
            SetCols(Cols);
        }

        public void AddItem(string label)
        {
            items[label] = label;
            UpdateItemList();
        }

        public void AddItem(string label, object item)
        {
            items[label] = item;
            UpdateItemList();
        }

        public void SetItems(IEnumerable<string> labels)
        {
            items.Clear();
            foreach (string lbl in labels)
            {
                items[lbl] = lbl;
            }
            UpdateItemList();
        }

        public void SetItems(IEnumerable<object> itemlist)
        {
            items.Clear();
            foreach (object obj in itemlist)
            {
                items[obj.ToString()] = obj;
            }
            UpdateItemList();
        }

        public void SetItems<T>(IEnumerable<T> itemlist, StringLabelExtractor<T> labelExtractor)
        {
            items.Clear();
            foreach (T obj in itemlist)
            {
                items[labelExtractor(obj)] = obj;
            }
            UpdateItemList();
        }

        public void SetItems(Dictionary<string, object> itemdict)
        {
            items.Clear();
            foreach (var item in itemdict)
            {
                items[item.Key] = item.Value;
            }
            UpdateItemList();
        }

        public void SelectItem(object item)
        {
            if (Items.Contains(item))
            {
                if (SelectedItem != null && buttons.ContainsKey(SelectedItem) && ShowSelection)
                    buttons[SelectedItem].Role = ButtonRole.Item;
                SelectedItem = item;
                SelectedItemString = items.FirstOrDefault(x => x.Value == item).Key;
                if (ShowSelection)
                    buttons[SelectedItem].Role = ButtonRole.Selected;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(item), "The given item is not in the list.");
            }
        }

        public void UpdateItemList()
        {
            ListGrid.Children.Clear();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    try
                    {
                        int index = i * Cols + j + Rows * Cols * page;
                        VButton b = new VButton();
                        string item_string = items.Keys.ToList()[index];
                        object item_obj = items[item_string];
                        b.Role = ButtonRole.Item;
                        b.Content = item_string;
                        b.Click += Item_Clicked;
                        ListGrid.Children.Add(b);
                        Grid.SetRow(b, i);
                        Grid.SetColumn(b, j);
                        buttons[item_obj] = b;
                        if (ShowSelection && item_obj == SelectedItem)
                        {
                            b.Role = ButtonRole.Selected;
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {

                    }
                }
            }
            if (items.Count() <= Rows * Cols)
            {
                NavCol.Width = new GridLength(0);
            }
            else
            {
                NavCol.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private void Item_Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && buttons.ContainsKey(SelectedItem) && ShowSelection)
                buttons[SelectedItem].Role = ButtonRole.Item;
            SelectedItemString = (string)((VButton)sender).Content;
            SelectedItem = items[SelectedItemString];
            if (ShowSelection)
                buttons[SelectedItem].Role = ButtonRole.Selected;
            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
            RaiseEvent(args);
        }

        public void ClearSelection()
        {
            if (SelectedItem != null)
            {
                buttons[SelectedItem].Role = ButtonRole.Item;
            }
            SelectedItem = null;
            SelectedItemString = null;
            UpdateItemList();
        }

        private void SetRows(int rows)
        {
            ListGrid.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++)
            {
                ListGrid.RowDefinitions.Add(new RowDefinition());
            }
            UpdateItemList();
        }

        private void SetCols(int cols)
        {
            ListGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++)
            {
                ListGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            UpdateItemList();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (items.Count > Rows * Cols * (page + 1))
            {
                page++;
            }
            UpdateItemList();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page--;
            }
            UpdateItemList();
        }

        private static void ShowPromptChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VList list = o as VList;
            if ((bool)e.NewValue)
            {
                list.PromptRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                list.PromptRow.Height = new GridLength(0);
            }
        }
    }
}
