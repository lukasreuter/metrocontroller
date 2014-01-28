﻿using System;

namespace MetroController.XInputWrapper {

    /// <summary>This structure holds the current and previous controller state</summary>
    public class XboxControllerStateChangedEventArgs : EventArgs {

        /// <summary>Represents the current input state of the controller</summary>
        public XInputState CurrentInputState { get; set; }

        /// <summary>Represents the previous input state of the controller</summary>
        public XInputState PreviousInputState { get; set; }
    }
}
