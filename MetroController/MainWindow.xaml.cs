using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

using WindowsInput;
using MetroController.XInputWrapper;

namespace MetroController {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable {

        //Fields---------------------------------------------------------------
        private const string APP_ID = "org.XInput.MetroController";

        private static bool IsHidden;

        private System.Windows.Forms.NotifyIcon Ni;

        public event PropertyChangedEventHandler PropertyChanged;

        public
        XboxController SelectedController {
            get { return _selectedController; }
            private set { _selectedController = value; }
        }
        private XboxController _selectedController;

        public
        int ActiveController {
            get { return _ActiveController; }
            private set { _ActiveController = value; }
        }
        private int _ActiveController;



        //Methods--------------------------------------------------------------

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public
        MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //TitleBar.MouseLeftButtonDown += (o, e) => DragMove();
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            IsHidden = false; //if minimize on start is not set to true

            //Initialize the System Tray Notification icon
            Ni = new System.Windows.Forms.NotifyIcon();
            Ni.Icon = new System.Drawing.Icon(GetResource("MetroController.ico"));
            Ni.ContextMenu = new System.Windows.Forms.ContextMenu();
            Ni.ContextMenu.Popup += delegate(object sender, EventArgs args) {
                                        XboxController.StopPolling();
                                        Application.Current.Shutdown();
                                    };
            Ni.DoubleClick += delegate(object sender, EventArgs args) { Maximize(); };
            Ni.Visible = false;
            dbg("Ni setup complete");

            //Initialize Shortcut so we can send Notifications

            //Prepare Notification toast
            //DispatchNotification("")
            //TODO: change layout
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            //XmlNodeList stringElements;
            //XmlNodeList imageElements = toasXml.GetElementsByTagName("image");
            //imageElements[0].Attributes.GetNamedItem("src").NodeValue =



            if (GetFirstConnectedController() == -1) {
                dbg("No connected controllers found, idleling and waiting for user to connect one");
                //dispatch notification here, then jump to loop where we check cont. for new controllers
                DispatchNotification("No Controllers detected", "Please connect a wired or wireless Xbox360 Controller.");
                WaitForNewControllers();
            }



            _selectedController = XboxController.RetrieveController(ActiveController);
            _selectedController.StateChanged += _selectedController_StateChanged;
            XboxController.StartPolling();
        }

        /// <summary>
        /// Is called when an controller has updated its state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _selectedController_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            //Main Window is being displayed so dont simulate keypresses
            if (!IsHidden) { goto Finish; }


            //TODO: add the keysend events here
            var controller = XboxController.RetrieveController(ActiveController);
            var sim = new InputSimulator();

            if (controller.IsGuidePressed// &&
                /*sim.In*/) {
                //Play sounds here
                EmitKeyPress("^({ESC})");
                //EmitKeyPress("#({TAB})");
                goto Finish;
            }

            if (true) { //check here if in metro ui


                if (controller.IsDPadDownPressed) {
                    if (sim.InputDeviceState.IsKeyDown(WindowsInput.Native.VirtualKeyCode.LWIN)) {
                        EmitKeyPress("{TAB}");
                    } else {
                        EmitKeyPress("{DOWN}");
	                }
                } else if (controller.IsDPadUpPressed) {
                    if (sim.InputDeviceState.IsKeyDown(WindowsInput.Native.VirtualKeyCode.LWIN)) {
                        EmitKeyPress("+({TAB})");
                    } else {
                        EmitKeyPress("{UP}");
	                }
                } else if (controller.IsDPadLeftPressed) {
                    EmitKeyPress("{LEFT}");
                } else if (controller.IsDPadRightPressed) {
                    EmitKeyPress("{RIGHT}");
                }



                if (controller.IsAPressed) {
                    EmitKeyPress("{ENTER}");
                }

                if (controller.IsBPressed) {
                //    EmitKeyPress("^({ESC})({TAB})");
                    sim.Keyboard.Sleep(100).KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN).KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);

                    //dbg(Key.LWin);
                }

                if (controller.IsYPressed) {
                    EmitKeyPress("^{TAB}");
                }

                //dbg(Key.Escape.ToString());

            }

            if (IsHidden) {
                goto Exit;
            }

        Finish:
            OnPropertyChanged("SelectedController");
        Exit:
            return;
        }


        #region Helper Methods
        /// <summary>
        /// Gets the matching Resource from MetroController/Resources/.
        /// </summary>
        /// <param name="name">The name of the file in the project</param>
        /// <returns>A generic System.IO.Stream</returns>
        private static
        Stream GetResource(string name)
        {
            try {
                return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetroController.Resources." + name);
            } catch /*(Exception e)*/ {
                MessageBox.Show("An error occurred while loading the Resource.");
                //throw e;
                return null;
            }
        }

        /// <summary>
        /// Gets the first controller that is connected and available (max four controller are supported)
        /// </summary>
        /// <returns>The index of the controller starting from 0</returns>
        private
        int GetFirstConnectedController()
        {
            _ActiveController = -1;
            for (int i = XboxController.FIRST_CONTROLLER_INDEX; i <= XboxController.LAST_CONTROLLER_INDEX; i++) {
                if (XboxController.RetrieveController(i).UpdateState().IsConnected) {
                    _ActiveController = i;
                    dbg("Found connected controller: " + (i + 1));
                    break;
                }
            }
            return ActiveController;
        }

        private
        void DispatchNotification(string title, string cont)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            // Fill in the text elements
            Windows.Data.Xml.Dom.XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            //for (int i = 0; i < stringElements.Length; i++) {
                //stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            //}
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(cont));

            // Specify the absolute path to an image
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");

            XmlNodeList imageAttributes= toastXml.GetElementsByTagName("image");
            ((XmlElement) imageAttributes[0]).SetAttribute("src", "ms-appx:///Resources/MetroController.ico");
            ((XmlElement) imageAttributes[0]).SetAttribute("alt", "Icon Graphic");

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        private
        void WaitForNewControllers()
        {

        }

        /// <summary>
        /// Emits the given key and then sleeps for some milliseconds
        /// </summary>
        /// <param name="key">The key that should be pressed in an String representation</param>
        /// <param name="time">The time that the Thread should sleep in (ushort) milliseconds</param>
        private
        void EmitKeyPress(string key, ushort time = 100)
        {
            System.Windows.Forms.SendKeys.SendWait(key);
            Thread.Sleep(time);
        }

        private
        void Minimize()
        {
            if (!IsHidden) {
                this.Hide();
                this.ShowInTaskbar = false;
                Ni.Visible = true;
                IsHidden = true;
            }
        }

        private
        void Maximize()
        {
            if (IsHidden) {
                this.Show();
                this.ShowInTaskbar = true;
                Ni.Visible = false;
                IsHidden = false;
            }
        }
        #endregion

        #region Debugging
#if DEBUG
        /// <summary>
        /// A simple method to ouput Debugging string to stdout
        /// </summary>
        /// <param name="str">The string that should be printed</param>
        private static
        void dbg(string str)
        {
            Console.WriteLine(str);
        }
#else
        private static void dbg(string str) {}
#endif
        #endregion

        #region Event handlers
        protected override
        void OnClosing(CancelEventArgs e)
        {
            XboxController.StopPolling();
            base.OnClosing(e);
        }

        public
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) {
                Action a = () => { PropertyChanged(this, new PropertyChangedEventArgs(name)); };
                Dispatcher.BeginInvoke(a, null);
            }
        }

        private
        void SelectedControllerChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedController = XboxController.RetrieveController(((ComboBox) sender).SelectedIndex);
            OnPropertyChanged("SelectedController");
        }

        private
        void SendVibration_Click(object sender, RoutedEventArgs e)
        {
            double leftMotorSpeed = LeftMotorSpeed.Value;
            double rightMotorSpeed = RightMotorSpeed.Value;
            _selectedController.Vibrate(leftMotorSpeed, rightMotorSpeed, TimeSpan.FromSeconds(2));
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            Minimize();
            //SystemCommands.MinimizeWindow(this);
        }
        #endregion

        void System.IDisposable.Dispose()
        {
            Ni.Dispose();
        }

    }
}
