using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public PlayDates PlayDatesSingleton { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            PlayDatesSingleton = PlayDates.GetInstance;
        }

        private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(CalendarView_CalendarViewDayItemChanging);
            }
            else if (args.Phase == 1)
            {
                if (args.Item.Date < DateTimeOffset.Now)
                {
                    args.Item.IsBlackout = true;
                }
                args.RegisterUpdateCallback(CalendarView_CalendarViewDayItemChanging);
            }
            else if (args.Phase == 2)
            {
                if (args.Item.Date.Date >= DateTimeOffset.Now)
                {
                    var currentPlaydates = PlayDatesSingleton.PlaydatesList.FindAll(x => x.Date.Date >= DateTime.Now.Date);
                    List<Color> densityColors = new List<Color>();
                    foreach (var play in currentPlaydates)
                    {
                        if (args.Item.Date.Date == play.Date)
                        {
                            densityColors.Add(Colors.Green);
                        }
                    }
                    args.Item.SetDensityColors(densityColors);
                }
            }
        }

        private async void seeEventButton_Click(object sender, RoutedEventArgs e)
        {
            var i = CalendarViewet.SelectedDates.FirstOrDefault();
            try
            {
                var j = PlayDatesSingleton.PlaydatesList.FindAll(x => x.Date.Date == i.Date);
                if (j.Any())
                {
                    Frame.Navigate(typeof(PlayViewer), j[0]);
                }
                else
                {
                    await new MessageDialog("Noob, der er ingen legedage her").ShowAsync();
                }
            }
            catch (Exception)
            {
                await new MessageDialog("Noob, der er ingen legedage her").ShowAsync();
            }
        }
    }
}
