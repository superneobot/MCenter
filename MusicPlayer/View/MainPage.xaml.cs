using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BlurryControls.Controls;
using MusicPlayer.ViewModel;

namespace MusicPlayer.View
{
    public partial class MainPage : BlurryWindow
    {
        private readonly DispatcherTimer _timer;
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            
            _timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += Timer_Tick; ;
            _timer.Start();
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            MainViewModel vm = (MainViewModel)DataContext;
            if (vm != null)
                vm.Value = vm.MPlayer.getPosition();
        }
        
        private void seeker_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _timer.Stop();
        }

        private void seeker_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Slider slider = sender as Slider;
            MainViewModel vm = (MainViewModel)DataContext;
            vm.MPlayer.setPosition((long)slider.Value);
            _timer.Start();
        }
        
        private void seeker_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainViewModel vm = (MainViewModel)DataContext;
            Slider slider = sender as Slider;
            var Faktor = (vm.MPlayer.getTotalTime() / slider.ActualWidth) / 360;
            var time_pos = Faktor * e.GetPosition(Seeker).X;
            var time_tip = TimeSpan.FromMilliseconds(time_pos).ToString();
            var index = time_tip.IndexOf('.');
            if (index >= 0)
            {
                time_tip = time_tip.Substring(0, index);
            }
            vm.TimeTip = time_tip;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TopPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private DependencyObject findParentTreeItem(DependencyObject CurrentControl, Type ParentType)
        {
            bool notfound = true;
            while (notfound)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(CurrentControl);
                string ParentTypeName = ParentType.Name;
                //Compare current type name with what we want
                if (parent == null)
                {
                    System.Diagnostics.Debugger.Break();
                    notfound = false;
                    continue;
                }
                if (parent.GetType().Name == ParentTypeName)
                {
                    return parent;
                }
                //we haven't found it so walk up the tree.
                CurrentControl = parent;
            }
            return null;
        }
        
        private void MakeItemSelected(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            DependencyObject tvi = findParentTreeItem(button, typeof(ListViewItem));
            if (tvi != null)
            {
                ListViewItem lbi = tvi as ListViewItem;
                lbi.IsSelected = true;
            }
        }
    }
}