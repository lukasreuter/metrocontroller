using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MetroController.XInputWrapper;

namespace MetroController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _selectedController = XboxController.RetrieveController(0);
            _selectedController.StateChanged += _selectedController_StateChanged;
            XboxController.StartPolling();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            XboxController.StopPolling();
            base.OnClosing(e);
        }

        void _selectedController_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            OnPropertyChanged("SelectedController");
            
        }

        
        public XboxController SelectedController
        {

            get { return _selectedController; }
        }


        volatile bool _keepRunning;
        XboxController _selectedController;


        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                Action a = ()=>{PropertyChanged(this, new PropertyChangedEventArgs(name));};
                Dispatcher.BeginInvoke(a, null);
                
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void CheckboxBButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void SelectedControllerChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedController = XboxController.RetrieveController(((ComboBox)sender).SelectedIndex);
            OnPropertyChanged("SelectedController");
        }



        private void SendVibration_Click(object sender, RoutedEventArgs e)
        {
            double leftMotorSpeed = LeftMotorSpeed.Value;
            double rightMotorSpeed = RightMotorSpeed.Value;
            _selectedController.Vibrate(leftMotorSpeed, rightMotorSpeed, TimeSpan.FromSeconds(2));
        }
    }
}
