using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {

    /// <summary>Provides access to the XInput methods</summary>
    public class XboxController {

        private static bool     keepRunning;
        private static int      waitTime;
        private static bool     isRunning;
        private static object   SyncLock;
        private static Thread   pollingThread;
        private static XboxController[] Controllers;

        private int             _playerIndex;
        private bool            _stopMotorTimerActive;
        private DateTime        _stopMotorTime;

        private XInputState gamepadStatePrev    = new XInputState();
        private XInputState gamepadStateCurrent = new XInputState();

        /// <summary>Contains the method that is called when a controller has changed its state</summary>
        public event EventHandler<XboxControllerStateChangedEventArgs> StateChanged = null;

        /// <summary>The frequency (updates per second) with which updates for the controllers are fetched</summary>
        public static int UpdateFrequency {
            get { return updateFrequency; }
            set {
                updateFrequency = value;
                waitTime = 1000 / Math.Max(1, Math.Min(1000, updateFrequency));
            }
        }
        private static int      updateFrequency;

        /// <summary>Indicates if the controller is powered on and connected</summary>
        public bool IsConnected { get; internal set; }

        /// <summary>General Information about the controller battery (type and charge level)</summary>
        public XInputBatteryInformation BatteryInformationGamepad { get; internal set; }

        /// <summary>General information about the controller-connected Headset battery (type and charge level)</summary>
        public XInputBatteryInformation BatteryInformationHeadset { get; internal set; }

        /// <summary>Maximum number of controllers that are supported</summary>
        public const int MAX_CONTROLLER_COUNT   = 4;
        /// <summary>First index of the Controllers[] array</summary>
        public const int FIRST_CONTROLLER_INDEX = 0;
        /// <summary>Last index of the Controller[] array</summary>
        public const int LAST_CONTROLLER_INDEX  = MAX_CONTROLLER_COUNT - 1;



        static XboxController()
        {
            Controllers = new XboxController[MAX_CONTROLLER_COUNT];
            SyncLock = new object();
            for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i) {
                Controllers[i] = new XboxController(i);
            }
            UpdateFrequency = 25;
        }

        private XboxController(int playerIndex)
        {
            _playerIndex = playerIndex;
            gamepadStatePrev.Copy(gamepadStateCurrent);
            // Update here to get the battery information for the controller
            UpdateBatteryState();
        }

        /// <summary>
        /// Retrieves a reference to the Controller instance with the given index from Controllers[]
        /// </summary>
        /// <param name="index">The index of the controller from 0 to 3</param>
        /// <returns>A reference to the XboxController object</returns>
        public static XboxController RetrieveController(int index)
        {
            return Controllers[index];
        }

        /// <summary>
        /// Called from UpdateState when a controller has differing packetnumbers
        /// </summary>
        protected void OnStateChanged()
        {
            if (StateChanged != null) {
                StateChanged(this, new XboxControllerStateChangedEventArgs() { CurrentInputState = gamepadStateCurrent, PreviousInputState = gamepadStatePrev });
            }
        }

        /// <summary>
        /// Retrieves the capabilities of a controller (e.g. which controller type is it, can it vibrate etc.)
        /// <seealso cref="XInputCapabilities"/>
        /// </summary>
        /// <returns>A reference to an XInputCapabilities object</returns>
        public XInputCapabilities GetCapabilities()
        {
            XInputCapabilities capabilities = new XInputCapabilities();
            XInputNativeMethods.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, ref capabilities);
            return capabilities;
        }

        #region Digital Button States
        /// <summary>
        /// Checks if Up is pressed
        /// </summary>
        public bool IsDPadUpPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_UP); }
        }

        /// <summary>
        /// Checks if Down is pressed
        /// </summary>
        public bool IsDPadDownPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN); }
        }

        /// <summary>
        /// Checks if Left is pressed
        /// </summary>
        public bool IsDPadLeftPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT); }
        }

        /// <summary>
        /// Checks if right is pressed
        /// </summary>
        public bool IsDPadRightPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT); }
        }

        /// <summary>
        /// Checks if A is pressed
        /// </summary>
        public bool IsAPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_A); }
        }

        /// <summary>
        /// Checks if B is pressed
        /// </summary>
        public bool IsBPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_B); }
        }

        /// <summary>
        /// Checks if X is pressed
        /// </summary>
        public bool IsXPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_X); }
        }

        /// <summary>
        /// Checks if Y is pressed
        /// </summary>
        public bool IsYPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_Y); }
        }

        /// <summary>
        /// Checks if Back is pressed
        /// </summary>
        public bool IsBackPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_BACK); }
        }

        /// <summary>
        /// Checks if Start is pressed
        /// </summary>
        public bool IsStartPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_START); }
        }

        /// <summary>
        /// Checks if Guide is pressed
        /// </summary>
        public bool IsGuidePressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_GUIDE); }
        }

        /// <summary>
        /// Checks if Left shoulder button is pressed
        /// </summary>
        public bool IsLeftShoulderPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER); }
        }

        /// <summary>
        /// Checks if Right shoulder button is pressed
        /// </summary>
        public bool IsRightShoulderPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER); }
        }

        /// <summary>
        /// Checks if Left Stick is pressed
        /// </summary>
        public bool IsLeftStickPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB); }
        }

        /// <summary>
        /// Checks if Right Stick is pressed
        /// </summary>
        public bool IsRightStickPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB); }
        }
        #endregion

        #region Analogue Input States
        /// <summary>
        /// Returns the state of the left trigger as integer
        /// Minimum: 0  Maximum: 255
        /// </summary>
        public int LeftTrigger {
            get { return (int) gamepadStateCurrent.Gamepad.bLeftTrigger; }
        }

        /// <summary>
        /// Returns the state of the right trigger as integer
        /// Minimum: 0  Maximum: 255
        /// </summary>
        public int RightTrigger {
            get { return (int) gamepadStateCurrent.Gamepad.bRightTrigger; }
        }

        /// <summary>
        /// Returns the current Coordinates of the left thumb stick as a Point.
        /// Minimum: -32768  Maximum: 32767
        /// </summary>
        public Point LeftThumbStick {
            get {
                return new Point() {
                    X = gamepadStateCurrent.Gamepad.sThumbLX,
                    Y = gamepadStateCurrent.Gamepad.sThumbLY
                };
            }
        }

        /// <summary>
        /// Returns the current Coordinates of the right thumb stick as a Point
        /// Minimum: -32768  Maximum: 32767
        /// </summary>
        public Point RightThumbStick {
            get {
                return new Point() {
                    X = gamepadStateCurrent.Gamepad.sThumbRX,
                    Y = gamepadStateCurrent.Gamepad.sThumbRY
                };
            }
        }
        #endregion

        #region Polling
        /// <summary>
        /// Starts polling the controller state every <see cref="waitTime"/>
        /// </summary>
        public static void StartPolling()
        {
            if (!isRunning) {
                lock (SyncLock) {
                    if (!isRunning) {
                        pollingThread = new Thread(PollerLoop);
                        pollingThread.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Stops the continous polling and updating of the controller states
        /// </summary>
        public static void StopPolling()
        {
            if (isRunning) keepRunning = false;
        }

        private static void PollerLoop()
        {
            lock (SyncLock) {
                if (isRunning) return;
                isRunning = true;
            }
            keepRunning = true;
            while (keepRunning) {
                for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; i++) {
                    Controllers[i].UpdateState();
                }
                Thread.Sleep(waitTime);
            }
            lock (SyncLock) {
                isRunning = false;
            }
        }

        /// <summary>
        /// Gets the current input state of the controller instance through <see cref="XInput.XInputGetStateEx"/> and
        /// updates the state depending on differing packetnumbers
        /// </summary>
        /// <returns>A reference of the current <see cref="XboxController"/> </returns>
        public XboxController UpdateState()
        {
            int result = XInputNativeMethods.XInputGetStateEx(_playerIndex, ref gamepadStateCurrent);
            IsConnected = (result == 0);

            //TODO: maybe only check for battery updates every x cycles to save some performance
            if ((byte) this.BatteryInformationGamepad.BatteryType > (byte) BatteryTypes.BATTERY_TYPE_WIRED) {
                UpdateBatteryState();
            }

            if (gamepadStateCurrent.PacketNumber != gamepadStatePrev.PacketNumber) {
                OnStateChanged();
            }
            gamepadStatePrev.Copy(gamepadStateCurrent);

            if (_stopMotorTimerActive && (DateTime.Now >= _stopMotorTime)) {
                XInputVibration stopStrength = new XInputVibration() { LeftMotorSpeed = 0, RightMotorSpeed = 0 };
                XInputNativeMethods.XInputSetState(_playerIndex, ref stopStrength);
            }

            //we return a reference of this, so we can chain another method call
            return this;
        }

        /// <summary>
        /// Updates the state of the controller and headset battery
        /// </summary>
        public void UpdateBatteryState()
        {
            var headset = new XInputBatteryInformation();
            var gamepad = new XInputBatteryInformation();

            XInputNativeMethods.XInputGetBatteryInformation(_playerIndex, (byte) BatteryDeviceTypes.BATTERY_DEVTYPE_GAMEPAD, ref gamepad);
            XInputNativeMethods.XInputGetBatteryInformation(_playerIndex, (byte) BatteryDeviceTypes.BATTERY_DEVTYPE_HEADSET, ref headset);

            BatteryInformationHeadset = headset;
            BatteryInformationGamepad = gamepad;
        }
        #endregion

        #region Motor Functions
        /// <summary>
        /// Initiates a controller vibration with the shortest amount of time possible.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="leftMotor">Strength of the left (low frequency) motor</param>
        /// <param name="rightMotor">Strength of the right (high frequency) motor</param>
        public void Vibrate(double leftMotor, double rightMotor)
        {
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        /// <summary>
        /// Initiates a controller vibration for the given timespan.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="leftMotor">Strength of the left (low frequency) motor</param>
        /// <param name="rightMotor">Strength of the right (high frequency) motor</param>
        /// <param name="length">a <see cref="TimeSpan"/></param>
        public void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            XInputVibration vibration = new XInputVibration() { LeftMotorSpeed = (ushort) (65535d * leftMotor), RightMotorSpeed = (ushort) (65535d * rightMotor) };
            Vibrate(vibration, length);
        }

        /// <summary>
        /// Initiates a controller vibration for the given timespan with the given strength.
        /// Needs enabled polling to work correctly
        /// </summary>
        /// <param name="strength">A struct of <see cref="XInputVibration"/></param>
        /// <param name="length">A Reference to an <see cref="TimeSpan"/> object</param>
        public void Vibrate(XInputVibration strength, TimeSpan length)
        {
            _stopMotorTimerActive = false;
            XInputNativeMethods.XInputSetState(_playerIndex, ref strength);
            if (length != TimeSpan.MinValue) {
                _stopMotorTime = DateTime.Now.Add(length);
                _stopMotorTimerActive = true;
            }
        }
        #endregion

        /// <summary>
        /// Returns a string with the controller number and the current input state
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return ("Controller: " + _playerIndex.ToString() + " " + gamepadStateCurrent.ToString());
        }

    }
}
