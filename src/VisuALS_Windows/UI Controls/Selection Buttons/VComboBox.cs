using System.Collections.Generic;
using System.Linq;
using System.Windows;
namespace VisuALS_WPF_App
{
    class VComboBox : VButton
    {
        public string SelectedItemName
        {
            get { return (string)base.GetValue(SelectedItemNameProperty); }
            set
            {
                base.SetValue(SelectedItemNameProperty, value);
                Content = Prefix + value;
                RoutedEventArgs args = new RoutedEventArgs(VComboBox.ItemSelectedEvent);
                RaiseEvent(args);
            }
        }

        public static readonly DependencyProperty SelectedItemNameProperty = DependencyProperty.Register(
            "SelectedItemName", typeof(string), typeof(VComboBox), new PropertyMetadata(default(string), SelectedItemNameChanged));

        public object SelectedItem
        {
            get { return (object)base.GetValue(SelectedItemProperty); }
            set { base.SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(object), typeof(VComboBox), new PropertyMetadata(default(object), SelectedItemChanged));

        public int Rows
        {
            get { return (int)base.GetValue(RowsProperty); }
            set { base.SetValue(RowsProperty, value); }
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            "Rows", typeof(int), typeof(VComboBox), new PropertyMetadata(default(int)));

        public int Cols
        {
            get { return (int)base.GetValue(ColsProperty); }
            set { base.SetValue(ColsProperty, value); }
        }

        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            "Cols", typeof(int), typeof(VComboBox), new PropertyMetadata(default(int)));

        public string Prompt
        {
            get { return (string)base.GetValue(PromptProperty); }
            set
            {
                base.SetValue(PromptProperty, value);
            }
        }

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(VComboBox), new PropertyMetadata(default(string)));

        public string Prefix
        {
            get { return (string)base.GetValue(PrefixProperty); }
            set { base.SetValue(PrefixProperty, value); }
        }

        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
            "Prefix", typeof(string), typeof(VComboBox), new PropertyMetadata(default(string)));

        public static readonly RoutedEvent ItemSelectedEvent = EventManager.RegisterRoutedEvent(
            "ItemSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VComboBox));

        public event RoutedEventHandler ItemSelected
        {
            add { AddHandler(ItemSelectedEvent, value); }
            remove { RemoveHandler(ItemSelectedEvent, value); }
        }

        private Dictionary<string, object> items = new Dictionary<string, object>();
        private bool syncedFlag = false;

        public VComboBox() : base()
        {
            Rows = 3;
            Cols = 3;
            Click += Button_Click;
            Prefix = "";
            Loaded += Button_Loaded;
        }

        public VComboBox(int rows, int cols) : base()
        {
            Rows = rows;
            Cols = cols;
            Click += Button_Click;
            Prefix = "";
            Loaded += Button_Loaded;
        }

        private static void SelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VComboBox cb = o as VComboBox;
            if (!cb.syncedFlag)
            {
                cb.syncedFlag = true;
                try
                {
                    cb.SelectedItemName = cb.items.Where(x => x.Value.Equals(cb.SelectedItem)).ToList().First().Key;
                }
                catch
                {
                    cb.SelectedItemName = cb.SelectedItem.ToString();
                }
            }
            else
            {
                cb.syncedFlag = false;
            }

        }

        private static void SelectedItemNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VComboBox cb = o as VComboBox;
            if (!cb.syncedFlag)
            {
                cb.syncedFlag = true;
                if (cb.items.ContainsKey(cb.SelectedItemName))
                {
                    cb.SelectedItem = cb.items[cb.SelectedItemName];
                }
                else
                {
                    cb.SelectedItem = null;
                }
            }
            else
            {
                cb.syncedFlag = false;
            }

        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null && SelectedItemName == null)
            {
                if (items.Count > 0)
                {
                    SelectedItemName = items.Keys.ToList()[0];
                    SelectedItem = items.Values.ToList()[0];
                }
                else
                {
                    SelectedItemName = "";
                }
            }
            else
            {
                SelectedItem = SelectedItem;
                SelectedItemName = SelectedItemName;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowList(Prompt, items, Rows, Cols);
            if (r.ResponseObject != null)
            {
                SelectedItemName = r.ResponseString;
                SelectedItem = r.ResponseObject;
                RoutedEventArgs args = new RoutedEventArgs(VComboBox.ItemSelectedEvent);
                RaiseEvent(args);
            }
        }

        public void AddItem(string label)
        {
            items[label] = label;
        }

        public void AddItem(string label, object item)
        {
            items[label] = item;
        }

        public void SetItems(Dictionary<string, object> itemdict)
        {
            items.Clear();
            foreach (var item in itemdict)
            {
                items[item.Key] = item.Value;
            }
        }

        public void SetItems(Dictionary<string, string> itemdict)
        {
            items.Clear();
            foreach (var item in itemdict)
            {
                items[item.Key] = item.Value;
            }
        }

        public void SetItems(IEnumerable<string> labels)
        {
            items.Clear();
            foreach (string lbl in labels)
            {
                items[lbl] = lbl;
            }
        }

        public void SetItems(IEnumerable<object> itemlist)
        {
            items.Clear();
            foreach (object obj in itemlist)
            {
                items[obj.ToString()] = obj;
            }
        }
    }
}
