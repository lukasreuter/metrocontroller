using MetroController.XInputWrapper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using WindowsInput;

namespace MetroController {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable {
        //Fields---------------------------------------------------------------

        /// <summary>
        /// Contains the EventHandler that is called when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents the current reference to XboxController that we take inputs from
        /// </summary>
        public XboxController SelectedController { get; private set; }

        /// <summary>
        /// Represents the current index of the currently polled controller
        /// </summary>
        public int ActiveController
        {
            get { return _ActiveController; }
            private set { _ActiveController = Math.Min(4, Math.Max(0, value)); }
        }

        private int _ActiveController;

        private bool IdlePolling
        {
            get { return IdlePolling; }
            set
            {
                IdlePolling = value;
                //TODO: Call the desired method here
            }
        }

        private const string APP_ID = "org.XInput.MetroController";

        #region Virtual Key Codes Constants

        private const WindowsInput.Native.VirtualKeyCode RWIN = WindowsInput.Native.VirtualKeyCode.RWIN;
        private const WindowsInput.Native.VirtualKeyCode LWIN = WindowsInput.Native.VirtualKeyCode.LWIN;
        private const WindowsInput.Native.VirtualKeyCode CONTROL = WindowsInput.Native.VirtualKeyCode.CONTROL;
        private const WindowsInput.Native.VirtualKeyCode RETURN = WindowsInput.Native.VirtualKeyCode.RETURN;
        private const WindowsInput.Native.VirtualKeyCode LEFT = WindowsInput.Native.VirtualKeyCode.LEFT;
        private const WindowsInput.Native.VirtualKeyCode RIGHT = WindowsInput.Native.VirtualKeyCode.RIGHT;
        private const WindowsInput.Native.VirtualKeyCode DOWN = WindowsInput.Native.VirtualKeyCode.DOWN;
        private const WindowsInput.Native.VirtualKeyCode UP = WindowsInput.Native.VirtualKeyCode.UP;
        private const WindowsInput.Native.VirtualKeyCode TAB = WindowsInput.Native.VirtualKeyCode.TAB;
        private const WindowsInput.Native.VirtualKeyCode SHIFT = WindowsInput.Native.VirtualKeyCode.SHIFT;

        #endregion Virtual Key Codes Constants

        private bool IsHidden;

        private bool ControllersConnected;

        private System.Windows.Forms.NotifyIcon Ni;

        private WindowsInput.InputSimulator Simulator;

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

            //Initialize the global InputSimulator for this program
            Simulator = new InputSimulator();

            //Initialize the System Tray Notification icon
            Ni = new System.Windows.Forms.NotifyIcon();
            Ni.Icon = new System.Drawing.Icon(GetResource("MetroController.ico"));
            Ni.DoubleClick += delegate(object sender, EventArgs args) { Maximize(); };
            Ni.ContextMenu = new System.Windows.Forms.ContextMenu();
            Ni.ContextMenu.Popup += delegate(object sender, EventArgs args)
            {
                XboxController.StopPolling();
                Application.Current.Shutdown();
            };
            Ni.Visible = false;
            dbg("Ni setup complete");
#if DEBUG
            Minimize();
#endif
            DispatchNotification("Hello", "World");

            //Initialize Shortcut so we can send Notifications

            ControllersConnected = SetFirstConnectedController();
            if (!ControllersConnected) {
                dbg("No connected controllers found, idleling and waiting for user to connect one");
                //dispatch notification here, then jump to loop where we check cont. for new controllers
                DispatchNotification("No Controllers detected", "Please connect a wired or wireless Xbox360 Controller.");
                WaitForNewControllers();
            }

            SelectedController = XboxController.RetrieveController(ActiveController);
            SelectedController.StateChanged += _selectedController_StateChanged;
            XboxController.StartPolling();
        }

        /// <summary>
        /// Is called when an controller has updated its state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _selectedController_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            //Main Window is being displayed so dont simulate keypresses
            if (!IsHidden) { goto Finish; }

            //special controller shortcut Guide+LS+RS
            if (SelectedController.IsGuidePressed && SelectedController.IsLeftShoulderPressed && SelectedController.IsRightShoulderPressed) {
            }

            //call metro themed startmenu
            if (SelectedController.IsGuidePressed && Simulator.InputDeviceState.IsKeyUp(LWIN)) {
                Simulator.Keyboard.KeyDown(LWIN);
            } else if (!SelectedController.IsGuidePressed && Simulator.InputDeviceState.IsKeyDown(LWIN)) {
                //Play sound here
                Simulator.Keyboard.KeyUp(LWIN);
                goto Finish;
            }

            //IntPtr bla = FindWindow("ImmersiveLauncher", null);

            if (true) { //check here if in metro ui
                // D-Pad: Cardinal directions or forward/backward during WIN+TAB
                // D-Pad Down
                if (SelectedController.IsDPadDownPressed) {
                    if (Simulator.InputDeviceState.IsKeyDown(RWIN)) {
                        if (Simulator.InputDeviceState.IsKeyUp(TAB)) {
                            Simulator.Keyboard.KeyDown(TAB);
                        }
                        //EmitKeyPress("{TAB}");
                    } else {
                        if (Simulator.InputDeviceState.IsKeyUp(DOWN)) {
                            Simulator.Keyboard.KeyDown(DOWN);
                        }
                        //EmitKeyPress("{DOWN}");
                    }
                } else if (Simulator.InputDeviceState.IsKeyDown(RWIN) && Simulator.InputDeviceState.IsKeyDown(TAB)) {
                    Simulator.Keyboard.KeyUp(TAB);
                } else if (Simulator.InputDeviceState.IsKeyDown(DOWN)) {
                    Simulator.Keyboard.KeyUp(DOWN);
                }
                // D-Pad Up
                if (SelectedController.IsDPadUpPressed) {
                    if (Simulator.InputDeviceState.IsKeyDown(RWIN)) {
                        if (Simulator.InputDeviceState.IsKeyUp(TAB)) {
                            Simulator.Keyboard.KeyDown(SHIFT).KeyDown(TAB).KeyUp(SHIFT);
                        }
                        //EmitKeyPress("+({TAB})");
                    } else {
                        if (Simulator.InputDeviceState.IsKeyUp(UP)) {
                            Simulator.Keyboard.KeyDown(UP);
                        }
                        //EmitKeyPress("{UP}");
                    }
                } else if (Simulator.InputDeviceState.IsKeyDown(RWIN) && Simulator.InputDeviceState.IsKeyDown(TAB)) {
                    Simulator.Keyboard.KeyUp(TAB).KeyUp(SHIFT).Sleep(100);
                } else if (Simulator.InputDeviceState.IsKeyDown(UP)) {
                    Simulator.Keyboard.KeyUp(UP).KeyUp(SHIFT);
                }
                // D-Pad Left
                if (SelectedController.IsDPadLeftPressed && Simulator.InputDeviceState.IsKeyUp(LEFT)) {
                    Simulator.Keyboard.KeyDown(LEFT);
                } else if (!SelectedController.IsDPadLeftPressed && Simulator.InputDeviceState.IsKeyDown(LEFT)) {
                    Simulator.Keyboard.KeyUp(LEFT);
                }
                // D-Pad Right
                if (SelectedController.IsDPadRightPressed && Simulator.InputDeviceState.IsKeyUp(RIGHT)) {
                    Simulator.Keyboard.KeyDown(RIGHT);
                } else if (!SelectedController.IsDPadRightPressed && Simulator.InputDeviceState.IsKeyDown(RIGHT)) {
                    Simulator.Keyboard.KeyUp(RIGHT);
                }

                /*if (SelectedController.IsDPadUpPressed) {
                    if (Simulator.InputDeviceState.IsKeyDown(RWIN)) {
                        EmitKeyPress("+({TAB})");
                    } else {
                        EmitKeyPress("{UP}");
	                }
                } else if (SelectedController.IsDPadLeftPressed) {
                    EmitKeyPress("{LEFT}");
                } else if (SelectedController.IsDPadRightPressed) {
                    EmitKeyPress("{RIGHT}");
                }*/

                // A Button: ENTER
                if (SelectedController.IsAPressed && Simulator.InputDeviceState.IsKeyUp(RETURN)) {
                    Simulator.Keyboard.KeyDown(RETURN);
                } else if (!SelectedController.IsAPressed && Simulator.InputDeviceState.IsKeyDown(RETURN)) {
                    Simulator.Keyboard.KeyUp(RETURN);
                }

                // B Button: WIN+TAB
                if (SelectedController.IsBPressed && Simulator.InputDeviceState.IsKeyUp(RWIN)) {
                    Simulator.Keyboard.KeyDown(RWIN).KeyPress(TAB).Sleep(100);
                } else if (!SelectedController.IsBPressed && Simulator.InputDeviceState.IsKeyDown(RWIN)) {
                    Simulator.Keyboard.KeyUp(RWIN);
                }

                // Y Button: CTRL+TAB
                if (SelectedController.IsYPressed) {
                    EmitKeyPress("^({TAB})");
                    //INVESTIGATE: For some weird/unknown reason these two lines do not work in the ImmersiveLauncher, they do though work in every thing else
                    //sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL).KeyPress(WindowsInput.Native.VirtualKeyCode.TAB).KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL).Sleep(100);
                    //sim.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.TAB);
                }
            }

            //Main window is hidden so we dont have to waste resources to update the layout
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
        private Stream GetResource(string name)
        {
            try {
                return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetroController.Resources." + name);
            } catch {
                dbg("An error occurred while loading the Resource: " + name);
                throw;
            }
        }

        /// <summary>
        /// Gets the first controller that is connected and available (max four controller are supported)
        /// </summary>
        /// <returns>The index of the controller starting from 0</returns>
        private bool SetFirstConnectedController()
        {
            for (var i = XboxController.FIRST_CONTROLLER_INDEX; i <= XboxController.LAST_CONTROLLER_INDEX; i++) {
                if (XboxController.RetrieveController(i).UpdateState().IsConnected) {
                    dbg("Found connected controller: " + (i + 1));
                    ActiveController = i;
                    return true;
                }
            }
            dbg("No connected controller found!");
            return false;
        }

        private void DispatchNotification(string title, string cont)
        {
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            //for (var i = 0; i < stringElements.Length; i++) {
            //stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            //}
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(cont));

            // Specify the absolute path to an image
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");

            //XmlNodeList imageAttributes= toastXml.GetElementsByTagName("image");
            //((XmlElement) imageAttributes[0]).SetAttribute("src", "ms-appx:///Resources/MetroController.ico");
            //((XmlElement) imageAttributes[0]).SetAttribute("alt", "Icon Graphic");

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        private void WaitForNewControllers()
        {
        }

        /// <summary>
        /// Emits the given key through SendKeys and then sleeps for some milliseconds
        /// </summary>
        /// <param name="key">The key that should be pressed in an String representation</param>
        /// <param name="time">The time that the Thread should sleep in (ushort) milliseconds</param>
        private void EmitKeyPress(string key, ushort time = 100)
        {
            System.Windows.Forms.SendKeys.SendWait(key);
            Thread.Sleep(time);
        }

        private void Minimize()
        {
            if (!IsHidden) {
                this.Hide();
                this.ShowInTaskbar = false;
                Ni.Visible = true;
                IsHidden = true;
                MinimizeFootprint();
            }
        }

        private void Maximize()
        {
            if (IsHidden) {
                this.Show();
                this.ShowInTaskbar = true;
                Ni.Visible = false;
                IsHidden = false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        [Conditional("RELEASE")]
        private static void MinimizeFootprint()
        {
            int err = EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            if (err != 0) {
                dbg("Emptying the working set was not successful!");
            }
        }

        #endregion Helper Methods

        #region Debugging

        /// <summary>
        /// A simple method to ouput Debugging string to stdout
        /// </summary>
        /// <param name="str">The string that should be printed</param>
        /// <param name="name">The name of the calling method</param>
        /// <param name="line">The line number of the calling code</param>
        [Conditional("DEBUG")]
        private static void dbg(string str, [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Console.WriteLine(str + "; in member: " + name + " #" + line);
        }

        #endregion Debugging

        #region Form callbacks

        /// <summary>
        /// Is overridden to stop polling the controller states
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            XboxController.StopPolling();
            base.OnClosing(e);
        }

        private void SendVibration_Click(object sender, RoutedEventArgs e)
        {
            double leftMotorSpeed = LeftMotorSpeed.Value;
            double rightMotorSpeed = RightMotorSpeed.Value;
            SelectedController.Vibrate(leftMotorSpeed, rightMotorSpeed, TimeSpan.FromSeconds(2));
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

        private void SelectedControllerChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedController = XboxController.RetrieveController(((ComboBox) sender).SelectedIndex);
            OnPropertyChanged("SelectedController");
        }

        #endregion Form callbacks

        #region INotifyInterface callbacks

        /// <summary>
        /// Invokes PropertyChanged through Dispatcher.BeginInvoke
        /// </summary>
        /// <param name="name">The name of the property that has changed</param>
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) {
                Action a = () => { PropertyChanged(this, new PropertyChangedEventArgs(name)); };
                Dispatcher.BeginInvoke(a, null);
            }
        }

        #endregion INotifyInterface callbacks

        #region IDisposable callbacks

        /// <summary>Standard IDisposable implementation</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Used to release the system tray notification icon
        /// </summary>
        /// <param name="native">True if native (managed) resources should be releases</param>
        protected virtual void Dispose(bool native)
        {
            if (native) {
                // Release managed resources
                if (Ni != null) {
                    Ni.Dispose();
                }
            } else {
                // Release umanaged resources
            }
            // Release umanaged resources
        }

        #endregion IDisposable callbacks

        /// <summary>Destructor of MainWindow</summary>
        ~MainWindow()
        {
            dbg("Finalizer called");
            Dispose(false);
        }
    }
}
