using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePageView : Page
    {
        public HomePageView()
        {
            this.InitializeComponent();

            //notamText.Width = notamTextStackPanel.ActualWidth;
            reloadButton.Click += ReloadButton_Click;
            goOnlineButton.Click += GoOnlineButton_Click;
        }

        private void GoOnlineButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            /*
             * CHECK VERSION AGAIN, IF ITS WRONG, DO NOT LET ATC TO USE WRONG PROFILE
             * POPUP ASKING FOR POSITION CALLSIGN LIKE LTFM_TWR
             * AFTER GETTING THE POSITION EDIT PRF FILE
             * OPEN EUROSCOPE WITH THE CORRECT PRF FILE
             */
            return;
        }

        private void ReloadButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //reload data, call the reload function
            return;
        }
    }
}
