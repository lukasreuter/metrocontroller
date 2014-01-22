using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetroController.XInputWrapper {

    /// <summary>
    /// Provides access to the XInput methods
    /// </summary>
    public class XboxController {
        
        private static bool     keepRunning;
        private static int      updateFrequency;
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

        /// <summary>
        /// The frequency with which updates for the controllers are fetched
        /// </summary>
        public static int UpdateFrequency {
            get { return updateFrequency; }
            set {
                updateFrequency = value;
                waitTime = 1000 / updateFrequency;
            }
        }

        public bool IsConnected { get; internal set; }

        /// <summary>
        /// General Information about the controller battery (type and charge level)
        /// </summary>
        public XInputBatteryInformation BatteryInformationGamepad { get; internal set; }

        /// <summary>
        /// General information about the controller-connected Headset battery (type and charge level)
        /// </summary>
        public XInputBatteryInformation BatteryInformationHeadset { get; internal set; }

        /// <summary>
        /// Maximum number of controllers that are supported
        /// </summary>
        public const int MAX_CONTROLLER_COUNT   = 4;
        /// <summary>
        /// First index of the Controllers[] array
        /// </summary>
        public const int FIRST_CONTROLLER_INDEX = 0;
        /// <summary>
        /// Last index of the Controller[] array
        /// </summary>
        public const int LAST_CONTROLLER_INDEX  = MAX_CONTROLLER_COUNT - 1;



        static XboxController()
        {
            Controllers = new XboxController[MAX_CONTROLLER_COUNT];
            SyncLock = new object();
            for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i)
            {
                Controllers[i] = new XboxController(i);
            }
            UpdateFrequency = 25;
        }

        private XboxController(int playerIndex)
        {
            _playerIndex = playerIndex;
            gamepadStatePrev.Copy(gamepadStateCurrent);
        }

        /// <summary>
        /// Contains the method that is called when a controller has changed its state
        /// </summary>
        public event EventHandler<XboxControllerStateChangedEventArgs> StateChanged = null;

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
        /// Updates the state of the controller and headset battery
        /// </summary>
        public void UpdateBatteryState()
        {
            var headset = new XInputBatteryInformation();
            var gamepad = new XInputBatteryInformation();

            XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceTypes.BATTERY_DEVTYPE_GAMEPAD, ref gamepad);
            XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceTypes.BATTERY_DEVTYPE_HEADSET, ref headset);

            BatteryInformationHeadset = headset;
            BatteryInformationGamepad = gamepad;
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
            XInput.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, ref capabilities);
            return capabilities;
        }

        #region Digital Button States
        public
        bool IsDPadUpPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_UP); }
        }

        public
        bool IsDPadDownPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN); }
        }

        public
        bool IsDPadLeftPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT); }
        }

        public
        bool IsDPadRightPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT); }
        }

        public
        bool IsAPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_A); }
        }

        public
        bool IsBPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_B); }
        }

        public
        bool IsXPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_X); }
        }

        public
        bool IsYPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_Y); }
        }

        public
        bool IsBackPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_BACK); }
        }

        public
        bool IsStartPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_START); }
        }

        public
        bool IsGuidePressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_GUIDE); }
        }

        public
        bool IsLeftShoulderPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER); }
        }

        public
        bool IsRightShoulderPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER); }
        }

        public
        bool IsLeftStickPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB); }
        }

        public
        bool IsRightStickPressed {
            get { return gamepadStateCurrent.Gamepad.IsButtonPressed((int) ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB); }
        }
        #endregion

        #region Analogue Input States
        public
        int LeftTrigger {
            get { return (int) gamepadStateCurrent.Gamepad.bLeftTrigger; }
        }

        public
        int RightTrigger {
            get { return (int) gamepadStateCurrent.Gamepad.bRightTrigger; }
        }

        public
        Point LeftThumbStick {
            get {
                Point p = new Point() {
                    X = gamepadStateCurrent.Gamepad.sThumbLX,
                    Y = gamepadStateCurrent.Gamepad.sThumbLY
                };
                return p;
            }
        }

        public
        Point RightThumbStick {
            get {
                Point p = new Point() {
                    X = gamepadStateCurrent.Gamepad.sThumbRX,
                    Y = gamepadStateCurrent.Gamepad.sThumbRY
                };
                return p;
            }
        }
        #endregion

        #region Polling
        public static
        void StartPolling()
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

        public static
        void StopPolling()
        {
            if (isRunning) keepRunning = false;
        }

        static
        void PollerLoop()
        {
            lock (SyncLock) {
                if (isRunning) return;
                isRunning = true;
            }
            keepRunning = true;
            while (keepRunning) {
                for (var i = FIRST_CONTROLLER_INDEX; i < LAST_CONTROLLER_INDEX; i++) {
                    Controllers[i].UpdateState();
                }
                Thread.Sleep(updateFrequency);
            }
            lock (SyncLock) {
                isRunning = false;
            }
        }

        public
        XboxController UpdateState()
        {
            int result = XInput.XInputGetStateEx(_playerIndex, ref gamepadStateCurrent);
            IsConnected = (result == 0);

            //TODO: maybe only check for battery updates every x cycles to save some performance
            UpdateBatteryState();
            if (gamepadStateCurrent.PacketNumber != gamepadStatePrev.PacketNumber) {
                OnStateChanged();
            }
            gamepadStatePrev.Copy(gamepadStateCurrent);

            if (_stopMotorTimerActive && (DateTime.Now >= _stopMotorTime)) {
                XInputVibration stopStrength = new XInputVibration() { LeftMotorSpeed = 0, RightMotorSpeed = 0 };
                XInput.XInputSetState(_playerIndex, ref stopStrength);
            }

            //we return a reference of this, so we can chain another method call
            return this;
        }
        #endregion

        #region Motor Functions
        public
        void Vibrate(double leftMotor, double rightMotor)
        {
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        public
        void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            XInputVibration vibration = new XInputVibration() { LeftMotorSpeed = (ushort) (65535d * leftMotor), RightMotorSpeed = (ushort) (65535d * rightMotor) };
            Vibrate(vibration, length);
        }

        public
        void Vibrate(XInputVibration strength, TimeSpan length)
        {
            _stopMotorTimerActive = false;
            XInput.XInputSetState(_playerIndex, ref strength);
            if (length != TimeSpan.MinValue) {
                _stopMotorTime = DateTime.Now.Add(length);
                _stopMotorTimerActive = true;
            }
        }
        #endregion

        public override
        string ToString()
        {
            return _playerIndex.ToString();
        }

    }
}
