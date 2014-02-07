using MetroController.WindowsInput;
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

namespace MetroController {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged, IDisposable {
        //Fields---------------------------------------------------------------

        /// <summary>
        /// Contains the EventHandler that is called when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents the current reference to XboxController that we take inputs from
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public XboxController SelectedController { get; private set; }

        /// <summary>
        /// Represents the current index of the currently polled controller
        /// </summary>
        private int ActiveController
        {
            get { return _activeController; }
            set { _activeController = Math.Min(4, Math.Max(0, value)); }
        }

        private int _activeController;

        // ReSharper disable once UnusedMember.Local
        private bool IdlePolling
        {
            get { return _idlePolling; }
            set
            {
                _idlePolling = value;
                //TODO: Call the desired method here
            }
        }

        private bool _idlePolling;

        private const string APP_ID = "org.XInput.MetroController";

        #region Virtual Key Codes Constants

        private const WindowsInput.Native.VirtualKeyCode RWIN = WindowsInput.Native.VirtualKeyCode.RWIN;
        private const WindowsInput.Native.VirtualKeyCode LWIN = WindowsInput.Native.VirtualKeyCode.LWIN;
        private const WindowsInput.Native.VirtualKeyCode RETURN = WindowsInput.Native.VirtualKeyCode.RETURN;
        private const WindowsInput.Native.VirtualKeyCode LEFT = WindowsInput.Native.VirtualKeyCode.LEFT;
        private const WindowsInput.Native.VirtualKeyCode RIGHT = WindowsInput.Native.VirtualKeyCode.RIGHT;
        private const WindowsInput.Native.VirtualKeyCode DOWN = WindowsInput.Native.VirtualKeyCode.DOWN;
        private const WindowsInput.Native.VirtualKeyCode UP = WindowsInput.Native.VirtualKeyCode.UP;
        private const WindowsInput.Native.VirtualKeyCode TAB = WindowsInput.Native.VirtualKeyCode.TAB;
        private const WindowsInput.Native.VirtualKeyCode SHIFT = WindowsInput.Native.VirtualKeyCode.SHIFT;

        #endregion Virtual Key Codes Constants

        private bool _isHidden;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private bool ControllersConnected;

        private System.Windows.Forms.NotifyIcon Ni;

        private readonly InputSimulator _sim = InputSimulator.Instance;

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
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            _isHidden = false; //if minimize on start is not set to true


            //Initialize the System Tray Notification icon
            Ni = new System.Windows.Forms.NotifyIcon();
            Ni.Icon = new System.Drawing.Icon(GetResource("MetroController.ico"));
            Ni.DoubleClick += (sender, args) => Maximize();
            Ni.ContextMenu = new System.Windows.Forms.ContextMenu();
            Ni.ContextMenu.Popup += (sender, args) => {
                XboxController.StopPolling();
                Application.Current.Shutdown();
            };
            Ni.Visible = false;
            Tools.Dbg("Ni setup complete");
#if DEBUG
            Minimize();
#endif
            DispatchNotification("Hello", "World");

            //Initialize Shortcut so we can send Notifications

            ControllersConnected = SetFirstConnectedController();
            if (!ControllersConnected) {
                Tools.Dbg("No connected controllers found, idleling and waiting for user to connect one");
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
            if (!_isHidden) goto Finish;

            //special controller shortcut Guide+LS+RS
            if (SelectedController.IsGuidePressed && SelectedController.IsLeftShoulderPressed && SelectedController.IsRightShoulderPressed) {
            }

            //call metro themed startmenu
            if (SelectedController.IsGuidePressed && _sim.InputDeviceState.IsKeyUp(LWIN)) {
                _sim.Keyboard.KeyDown(LWIN);
            } else if (!SelectedController.IsGuidePressed && _sim.InputDeviceState.IsKeyDown(LWIN)) {
                //Play sound here
                _sim.Keyboard.KeyUp(LWIN);
                goto Finish;
            }

            //IntPtr bla = FindWindow("ImmersiveLauncher", null);

            if (true) { //check here if in metro ui
                DDown(); DUp(); DLeft(); DRight();

                AButton(); BButton(); YButton(); XButton();
            }

            //Main window is hidden so we dont have to waste resources to update the layout
            if (_isHidden) goto Exit;

        Finish:
            OnPropertyChanged("SelectedController");
        Exit:
            ;
        }

        #region Button checks

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DDown()
        {
            // D-Pad Down
            if (SelectedController.IsDPadDownPressed) {
                if (_sim.InputDeviceState.IsKeyDown(RWIN)) {
                    if (_sim.InputDeviceState.IsKeyUp(TAB)) {
                        _sim.Keyboard.KeyDown(TAB);
                    }
                    //EmitKeyPress("{TAB}");
                } else {
                    if (_sim.InputDeviceState.IsKeyUp(DOWN)) {
                        _sim.Keyboard.KeyDown(DOWN);
                    }
                    //EmitKeyPress("{DOWN}");
                }
            } else if (_sim.InputDeviceState.IsKeyDown(RWIN) && _sim.InputDeviceState.IsKeyDown(TAB)) {
                _sim.Keyboard.KeyUp(TAB);
            } else if (_sim.InputDeviceState.IsKeyDown(DOWN)) {
                _sim.Keyboard.KeyUp(DOWN);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DUp()
        {
            // D-Pad Up
            if (SelectedController.IsDPadUpPressed) {
                if (_sim.InputDeviceState.IsKeyDown(RWIN)) {
                    if (_sim.InputDeviceState.IsKeyUp(TAB)) {
                        _sim.Keyboard.KeyDown(SHIFT).KeyDown(TAB).KeyUp(SHIFT);
                    }
                    //EmitKeyPress("+({TAB})");
                } else {
                    if (_sim.InputDeviceState.IsKeyUp(UP)) {
                        _sim.Keyboard.KeyDown(UP);
                    }
                    //EmitKeyPress("{UP}");
                }
            } else if (_sim.InputDeviceState.IsKeyDown(RWIN) && _sim.InputDeviceState.IsKeyDown(TAB)) {
                _sim.Keyboard.KeyUp(TAB).KeyUp(SHIFT).Sleep(100);
            } else if (_sim.InputDeviceState.IsKeyDown(UP)) {
                _sim.Keyboard.KeyUp(UP).KeyUp(SHIFT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DLeft()
        {
            // D-Pad Left
            if (SelectedController.IsDPadLeftPressed && _sim.InputDeviceState.IsKeyUp(LEFT)) {
                _sim.Keyboard.KeyDown(LEFT);
            } else if (!SelectedController.IsDPadLeftPressed && _sim.InputDeviceState.IsKeyDown(LEFT)) {
                _sim.Keyboard.KeyUp(LEFT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DRight()
        {
            // D-Pad Right
            if (SelectedController.IsDPadRightPressed && _sim.InputDeviceState.IsKeyUp(RIGHT)) {
                _sim.Keyboard.KeyDown(RIGHT);
            } else if (!SelectedController.IsDPadRightPressed && _sim.InputDeviceState.IsKeyDown(RIGHT)) {
                _sim.Keyboard.KeyUp(RIGHT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AButton()
        {
            // A Button: ENTER
            if (SelectedController.IsAPressed && _sim.InputDeviceState.IsKeyUp(RETURN)) {
                _sim.Keyboard.KeyDown(RETURN);
            } else if (!SelectedController.IsAPressed && _sim.InputDeviceState.IsKeyDown(RETURN)) {
                _sim.Keyboard.KeyUp(RETURN);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BButton()
        {
            // B Button: WIN+TAB (Switch between apps)
            if (SelectedController.IsBPressed && _sim.InputDeviceState.IsKeyUp(RWIN)) {
                _sim.Keyboard.KeyDown(RWIN).KeyPress(TAB).Sleep(100);
            } else if (!SelectedController.IsBPressed && _sim.InputDeviceState.IsKeyDown(RWIN)) {
                _sim.Keyboard.KeyUp(RWIN);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void YButton()
        {
            // Y Button: CTRL+TAB (Switch categories in ImmersiveLauncher)
            if (SelectedController.IsYPressed) {
                EmitKeyPress("^({TAB})");
                //INVESTIGATE: For some weird/unknown reason these two lines do not work in the ImmersiveLauncher, they do though work in every thing else
                //sim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL).KeyPress(WindowsInput.Native.VirtualKeyCode.TAB).KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL).Sleep(100);
                //sim.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.TAB);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XButton() { }

        #endregion Button checks

        #region Helper Methods

        /// <summary>
        /// Gets the matching Resource from MetroController/Resources/.
        /// </summary>
        /// <param name="name">The name of the file in the project</param>
        /// <returns>A generic System.IO.Stream</returns>
        private static Stream GetResource(string name)
        {
            try {
                return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MetroController.Resources." + name);
            } catch {
                Tools.Dbg("An error occurred while loading the Resource: " + name);
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
                if (!XboxController.RetrieveController(i).UpdateState().IsConnected) continue;
                Tools.Dbg("Found connected controller: " + (i + 1));
                ActiveController = i;
                return true;
            }
            Tools.Dbg("No connected controller found!");
            return false;
        }

        private static void DispatchNotification(string title, string cont)
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
            var toast = new ToastNotification(toastXml);
            //toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        private static void WaitForNewControllers()
        {
        }

        /// <summary>
        /// Emits the given key through SendKeys and then sleeps for some milliseconds
        /// </summary>
        /// <param name="key">The key that should be pressed in an String representation</param>
        /// <param name="time">The time that the Thread should sleep in (ushort) milliseconds</param>
        private static void EmitKeyPress(string key, ushort time = 100)
        {
            System.Windows.Forms.SendKeys.SendWait(key);
            Thread.Sleep(time);
        }

        private void Minimize()
        {
            if (_isHidden) return;
            Hide();
            ShowInTaskbar = false;
            Ni.Visible = true;
            _isHidden = true;
            MinimizeFootprint();
        }

        private void Maximize()
        {
            if (!_isHidden) return;
            Show();
            ShowInTaskbar = true;
            Ni.Visible = false;
            _isHidden = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        [Conditional("RELEASE")]
        private static void MinimizeFootprint()
        {
            var err = EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            Tools.TestReturnValue(err);
            if (err != 0) {
                Tools.Dbg("Emptying the working set was not successful!");
            }
        }

        #endregion Helper Methods

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
            var leftMotorSpeed = LeftMotorSpeed.Value;
            var rightMotorSpeed = RightMotorSpeed.Value;
            SelectedController.Vibrate(leftMotorSpeed, rightMotorSpeed, TimeSpan.FromSeconds(2));
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
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
            if (PropertyChanged == null) return;
            Action a = () => PropertyChanged(this, new PropertyChangedEventArgs(name));
            Dispatcher.BeginInvoke(a, null);
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
        private void Dispose(bool native)
        {
            if (native) {
                // Release managed resources
                if (Ni != null) {
                    Ni.Dispose();
                }
            }
            // Release umanaged resources
        }

        #endregion IDisposable callbacks

        /// <summary>Destructor of MainWindow</summary>
        ~MainWindow()
        {
            Tools.Dbg("Finalizer called");
            Dispose(false);
        }
    }
}
