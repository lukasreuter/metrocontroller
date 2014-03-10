using MetroController.WindowsInput;
using MetroController.WindowsInput.Native;
using MetroController.XInputWrapper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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
        // ReSharper disable once MemberCanBePrivate.Global
        public int ActiveController
        {
            get { return _activeController; }
            private set { _activeController = Math.Min(4, Math.Max(0, value)); }
        }

        private int _activeController;

        // ReSharper disable once UnusedMember.Local
        private bool IdlePolling
        {
            get { return _idlePolling; }
            set
            {
                _idlePolling = value;
                // 25 is the default value for UpdateFrequency
                XboxController.UpdateFrequency = value ? 1 : 25;
                if (value && !_isHidden) Minimize();
            }
        }

        private bool _idlePolling;

        private bool _isHidden;

        private const string APP_ID = "org.XInput.MetroController";

        #region Virtual Key Codes Constants

        private const VirtualKeyCode RWIN = VirtualKeyCode.RWIN;
        private const VirtualKeyCode LWIN = VirtualKeyCode.LWIN;
        private const VirtualKeyCode RETURN = VirtualKeyCode.RETURN;
        private const VirtualKeyCode LEFT = VirtualKeyCode.LEFT;
        private const VirtualKeyCode RIGHT = VirtualKeyCode.RIGHT;
        private const VirtualKeyCode DOWN = VirtualKeyCode.DOWN;
        private const VirtualKeyCode UP = VirtualKeyCode.UP;
        private const VirtualKeyCode TAB = VirtualKeyCode.TAB;
        private const VirtualKeyCode SHIFT = VirtualKeyCode.SHIFT;
        private const VirtualKeyCode MENU = VirtualKeyCode.APPS;
        private const VirtualKeyCode C_KEY = VirtualKeyCode.VK_C;

        #endregion Virtual Key Codes Constants

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private bool _controllersConnected;

        private readonly NotifyIcon _ni;

        //Methods--------------------------------------------------------------

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //TitleBar.MouseLeftButtonDown += (o, e) => DragMove();
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));

            //Initialize the System Tray Notification icon
            // ReSharper disable once UseObjectOrCollectionInitializer
            _ni = new NotifyIcon();
            _ni.Text = Properties.Resources.MainWindow_Tooltip;
            _ni.Icon = new Icon(Tools.GetResource("TrayIcon.ico"));
            _ni.Click += (sender, args) => Maximize();
            _ni.ContextMenu = new ContextMenu();
            _ni.ContextMenu.Popup += (sender, args) => {
                XboxController.StopPolling();
                System.Windows.Application.Current.Shutdown();
            };
            _ni.Visible = false;
            Tools.Dbg("Ni setup complete");
#if DEBUG
            Minimize();
#endif
            DispatchNotification("Hello", "World");

            //Initialize Shortcut so we can send Notifications

            _controllersConnected = SetFirstConnectedController();
            if (!_controllersConnected) {
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

            TogglePolling();

            if (IdlePolling) goto Exit;

            //call metro themed startmenu
            if (SelectedController.IsGuidePressed && InputDeviceStateAdaptor.IsKeyUp(LWIN)) {
                KeyboardSimulator.Instance.KeyDown(LWIN);
            } else if (!SelectedController.IsGuidePressed && InputDeviceStateAdaptor.IsKeyDown(LWIN)) {
                //Play sound here
                KeyboardSimulator.Instance.KeyUp(LWIN);
                goto Finish;
            }

            //IntPtr bla = FindWindow("ImmersiveLauncher", null);

            if (true) { //check here if in metro ui
                DDown(); DUp(); DLeft(); DRight();

                AButton(); BButton(); YButton(); XButton();

                LeftShoulderButton(); RightShoulderButton();

                RightStick();

                StartButton();
            }

            //Main window is hidden so we dont have to waste resources to update the layout
            if (_isHidden) goto Exit;

        Finish:
            OnPropertyChanged("SelectedController");
        Exit:
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        #region Button checks

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TogglePolling()
        {
            //special controller shortcut Guide+LS+RS: deactivates polling
            if (!SelectedController.IsGuidePressed || !SelectedController.IsLeftShoulderPressed || !SelectedController.IsRightShoulderPressed) return;

            if (IdlePolling) {
                Tools.Dbg("Reactivating polling, input for reactivation is polled 25 times per 1000 ms");
                IdlePolling = false;
            } else if (!IdlePolling) {
                Tools.Dbg("Deactivating polling, input for reactivation is polled only once per 1000 ms");
                IdlePolling = true;
            }
            Thread.Sleep(100);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DDown()
        {
            // D-Pad Down
            if (SelectedController.IsDPadDownPressed) {
                if (InputDeviceStateAdaptor.IsKeyDown(RWIN)) {
                    if (InputDeviceStateAdaptor.IsKeyUp(TAB)) {
                        KeyboardSimulator.Instance.KeyDown(TAB);
                    }
                } else {
                    if (InputDeviceStateAdaptor.IsKeyUp(DOWN)) {
                        KeyboardSimulator.Instance.KeyDown(DOWN);
                    }
                }
            } else if (InputDeviceStateAdaptor.IsKeyDown(RWIN) && InputDeviceStateAdaptor.IsKeyDown(TAB)) {
                KeyboardSimulator.Instance.KeyUp(TAB);
            } else if (InputDeviceStateAdaptor.IsKeyDown(DOWN)) {
                KeyboardSimulator.Instance.KeyUp(DOWN);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DUp()
        {
            // D-Pad Up
            if (SelectedController.IsDPadUpPressed) {
                if (InputDeviceStateAdaptor.IsKeyDown(RWIN)) {
                    if (InputDeviceStateAdaptor.IsKeyUp(TAB)) {
                        KeyboardSimulator.Instance.KeyDown(SHIFT).KeyDown(TAB).KeyUp(SHIFT);
                    }
                } else {
                    if (InputDeviceStateAdaptor.IsKeyUp(UP)) {
                        KeyboardSimulator.Instance.KeyDown(UP);
                    }
                }
            } else if (InputDeviceStateAdaptor.IsKeyDown(RWIN) && InputDeviceStateAdaptor.IsKeyDown(TAB)) {
                KeyboardSimulator.Instance.KeyUp(TAB).KeyUp(SHIFT).Sleep(100);
            } else if (InputDeviceStateAdaptor.IsKeyDown(UP)) {
                KeyboardSimulator.Instance.KeyUp(UP).KeyUp(SHIFT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DLeft()
        {
            // D-Pad Left
            if (SelectedController.IsDPadLeftPressed && InputDeviceStateAdaptor.IsKeyUp(LEFT)) {
                KeyboardSimulator.Instance.KeyDown(LEFT);
            } else if (!SelectedController.IsDPadLeftPressed && InputDeviceStateAdaptor.IsKeyDown(LEFT)) {
                KeyboardSimulator.Instance.KeyUp(LEFT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DRight()
        {
            // D-Pad Right
            if (SelectedController.IsDPadRightPressed && InputDeviceStateAdaptor.IsKeyUp(RIGHT)) {
                KeyboardSimulator.Instance.KeyDown(RIGHT);
            } else if (!SelectedController.IsDPadRightPressed && InputDeviceStateAdaptor.IsKeyDown(RIGHT)) {
                KeyboardSimulator.Instance.KeyUp(RIGHT);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AButton()
        {
            // A Button: ENTER
            if (SelectedController.IsAPressed && InputDeviceStateAdaptor.IsKeyUp(RETURN)) {
                KeyboardSimulator.Instance.KeyDown(RETURN);
            } else if (!SelectedController.IsAPressed && InputDeviceStateAdaptor.IsKeyDown(RETURN)) {
                KeyboardSimulator.Instance.KeyUp(RETURN);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BButton()
        {
            // B Button: WIN+TAB (Switch between apps)
            if (SelectedController.IsBPressed && InputDeviceStateAdaptor.IsKeyUp(RWIN)) {
                KeyboardSimulator.Instance.KeyDown(RWIN).KeyPress(TAB).Sleep(100);
            } else if (!SelectedController.IsBPressed && InputDeviceStateAdaptor.IsKeyDown(RWIN)) {
                KeyboardSimulator.Instance.KeyUp(RWIN);
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
        private void XButton()
        {
            // X Button: Context menu button
            if (SelectedController.IsXPressed && InputDeviceStateAdaptor.IsKeyUp(MENU)) {
                KeyboardSimulator.Instance.KeyDown(MENU);
            } else if (!SelectedController.IsXPressed && InputDeviceStateAdaptor.IsKeyDown(MENU)) {
                KeyboardSimulator.Instance.KeyUp(MENU);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LeftShoulderButton()
        {
            if (SelectedController.IsLeftShoulderPressed && InputDeviceStateAdaptor.IsKeyUp(VirtualKeyCode.LBUTTON)) {
                MouseSimulator.LeftButtonDown();
            } else if (!SelectedController.IsLeftShoulderPressed && InputDeviceStateAdaptor.IsKeyDown(VirtualKeyCode.LBUTTON)) {
                MouseSimulator.LeftButtonUp();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RightShoulderButton()
        {
            if (SelectedController.IsRightShoulderPressed && InputDeviceStateAdaptor.IsKeyUp(VirtualKeyCode.RBUTTON)) {
                MouseSimulator.RightButtonDown();
            } else if (!SelectedController.IsRightShoulderPressed && InputDeviceStateAdaptor.IsKeyDown(VirtualKeyCode.RBUTTON)) {
                MouseSimulator.RightButtonUp();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RightStick()
        {
            var x = 0;
            var y = 0;
            if (SelectedController.RightThumbStick.X > XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE) {
                x = 10;
            } else if (SelectedController.RightThumbStick.X < (XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE * -1)) {
                x = -10;
            }
            if (SelectedController.RightThumbStick.Y > XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE) {
                y = -10;
            } else if (SelectedController.RightThumbStick.Y < (XInputConstants.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE * -1)) {
                y = 10;
            }
            if (x != 0 || y != 0) {
                MouseSimulator.MoveMouseBy(x, y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartButton()
        {
            // Start Button: WIN+C (Charmbar)
            if (SelectedController.IsStartPressed && InputDeviceStateAdaptor.IsKeyUp(C_KEY) && InputDeviceStateAdaptor.IsKeyUp(LWIN)) {
                KeyboardSimulator.Instance.KeyDown(LWIN).KeyPress(C_KEY).KeyUp(LWIN).Sleep(100);
            }
        }

        #endregion Button checks

        #region Helper Methods

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
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            // Fill in the text elements
            var stringElements = toastXml.GetElementsByTagName("text");
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
            SendKeys.SendWait(key);
            Thread.Sleep(time);
        }

        private void Minimize()
        {
            if (_isHidden) return;
            Hide();
            ShowInTaskbar = false;
            _ni.Visible = true;
            _isHidden = true;
            MinimizeFootprint();
        }

        private void Maximize()
        {
            if (!_isHidden) return;
            Show();
            ShowInTaskbar = true;
            _ni.Visible = false;
            _isHidden = false;
        }

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("psapi.dll")]
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
        }

        #endregion Form callbacks

        #region INotifyInterface callbacks

        /// <summary>
        /// Invokes PropertyChanged through Dispatcher.BeginInvoke
        /// </summary>
        /// <param name="name">The name of the property that has changed</param>
        private void OnPropertyChanged(string name)
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
                if (_ni != null) {
                    _ni.Dispose();
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
