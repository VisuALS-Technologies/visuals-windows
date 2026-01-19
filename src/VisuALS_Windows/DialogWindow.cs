using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public class DialogResponse
    {
        public string ResponseString;
        public object ResponseObject;

        public DialogResponse(string repStr, object repObj)
        {
            ResponseString = repStr;
            ResponseObject = repObj;
        }
    }

    static class DialogWindow
    {
        public static Grid DialogOverlay = new Grid();
        private static VWindow DialogVWin = new VWindow();
        private static VBanner DialogVBan = new VBanner();
        private static TaskCompletionSource<DialogResponse> itemSelected;
        private static VSelectionControl currentSelectionControl;
        private static RoutedEventHandler lastBannerClickHandler;

        public static event EventHandler ShowDialogEvent;
        public static event EventHandler CloseDialogEvent;

        static DialogWindow()
        {
            DialogOverlay.Children.Add(DialogVWin);
            DialogOverlay.Children.Add(DialogVBan);
            DialogVWin.CloseClick += CloseClickHandler;
            DialogVWin.Visibility = Visibility.Hidden;
            DialogVBan.Hide();
        }

        public static bool DisableClose
        {
            get
            {
                return DialogVWin.DisableClose;
            }
            set
            {
                DialogVWin.DisableClose = value;
            }
        }

        private static void CloseClickHandler(object sender, RoutedEventArgs e)
        {
            Close();
            if (itemSelected != null)
            {
                itemSelected.TrySetResult(new DialogResponse("", null));
            }

            if (currentSelectionControl != null)
            {
                currentSelectionControl.ItemSelectionChanged -= ItemSelectionHandler;
                currentSelectionControl = null;
            }
        }

        private static void DismissBannerHandler(object sender, RoutedEventArgs e)
        {
            DialogVBan.Click -= lastBannerClickHandler;
            DialogVBan.BannerButton.Content = null;
            DialogVBan.Text = null;
        }

        private static void ItemSelectionHandler(object sender, RoutedEventArgs e)
        {
            if (currentSelectionControl != null)
            {
                if (itemSelected != null)
                {
                    itemSelected.TrySetResult(new DialogResponse(currentSelectionControl.SelectedItemString, currentSelectionControl.SelectedItem));
                }
                currentSelectionControl.ItemSelectionChanged -= ItemSelectionHandler;
                currentSelectionControl = null;
            }
            Close();
        }

        public static void Close()
        {
            DialogVWin.Visibility = Visibility.Hidden;
            DialogVWin.Navigate(null);
            CloseDialogEvent(DialogVWin, EventArgs.Empty);
        }

        public static async Task<DialogResponse> Show(VSelectionControl selectionControl, bool fullscreen = false)
        {
            itemSelected = new TaskCompletionSource<DialogResponse>();
            selectionControl.ItemSelectionChanged += ItemSelectionHandler;
            currentSelectionControl = selectionControl;
            ShowDialogEvent(DialogVWin, EventArgs.Empty);
            DialogVWin.Visibility = Visibility.Visible;
            DialogVWin.Navigate(selectionControl);
            DialogVWin.FullScreen(fullscreen);
            return await itemSelected.Task;
        }

        public static void Show(UIElement element, bool fullscreen = false)
        {
            ShowDialogEvent(DialogVWin, EventArgs.Empty);
            DialogVWin.Visibility = Visibility.Visible;
            DialogVWin.Navigate(element);
            DialogVWin.FullScreen(fullscreen);
        }

        public static void VerbalConf(string message)
        {
            if (message != null && message != "" && App.globalConfig.Get<bool>("verbal_confirmations"))
            {
                SpeechUtils.Speak(message);
            }
        }

        public static async Task<DialogResponse> ShowMessage(string message)
        {
            VerbalConf(message);
            return await Show(new VMessageBox(message));
        }

        public static async Task<DialogResponse> ShowMessage(string message, params string[] options)
        {
            VerbalConf(message);
            return await Show(new VMessageBox(message, options));
        }

        public static async Task<DialogResponse> ShowMessage(string message, IEnumerable<string> options)
        {
            VerbalConf(message);
            return await Show(new VMessageBox(message, options));
        }

        public static void ShowPaginatedText(ICollection<string> messages)
        {
            Show(new VPaginatedText(messages, Close));
        }

        public static void ShowPaginatedText(string header, ICollection<string> messages)
        {
            VPaginatedText t = new VPaginatedText(messages, Close);
            t.Title = header;
            Show(t);
        }

        public static async Task<DialogResponse> ShowList(string prompt, IEnumerable<string> items, int rows = 3, int cols = 3)
        {
            VerbalConf(prompt);
            VList list = new VList(rows, cols);
            list.SetItems(items);
            list.Prompt = prompt;
            return await Show(list);
        }

        public static async Task<DialogResponse> ShowList(string prompt, IEnumerable<object> items, int rows = 3, int cols = 3)
        {
            VerbalConf(prompt);
            VList list = new VList(rows, cols);
            list.SetItems(items);
            list.Prompt = prompt;
            return await Show(list);
        }

        public static async Task<DialogResponse> ShowList(string prompt, Dictionary<string, object> items, int rows = 3, int cols = 3)
        {
            VerbalConf(prompt);
            VList list = new VList(rows, cols);
            list.SetItems(items);
            list.Prompt = prompt;
            return await Show(list);
        }

        public static async Task<DialogResponse> ShowKeyboardInput(string textHint = "")
        {
            VTextInputDialog kybdin = new VTextInputDialog();
            kybdin.Prompt = textHint;
            return await Show(kybdin, true);
        }

        public static void ShowBanner(string text, string buttonText = null, RoutedEventHandler onclick = null, int timeout = 5)
        {
            DialogVBan.Text = text;
            DialogVBan.BannerButton.Content = buttonText;
            if (lastBannerClickHandler != null)
            {
                DialogVBan.Click -= lastBannerClickHandler;
            }
            if (onclick != null)
            {
                DialogVBan.Click += onclick;
                lastBannerClickHandler = onclick;
            }
            DialogVBan.ShowBannerButton = buttonText != null && onclick != null;
            DialogVBan.Show();
            if (timeout > -1)
            {
                Task.Delay(timeout * 1000).ContinueWith((x) => Application.Current.Dispatcher.Invoke(DismissBanner));
            }
        }

        public static void DismissBanner()
        {
            if (lastBannerClickHandler != null)
            {
                DialogVBan.Click -= lastBannerClickHandler;
            }
            DialogVBan.BannerButton.Content = null;
            DialogVBan.Text = null;
            DialogVBan.Hide();
        }
    }
}
