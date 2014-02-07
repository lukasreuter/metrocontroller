using MetroController.WindowsInput.Native;
using System.Threading;

namespace MetroController.WindowsInput {

    /// <summary>
    /// Implements the MouseSimulator interface by calling the <see cref="InputMessageDispatcher"/> to simulate Mouse gestures.
    /// </summary>
    internal static class MouseSimulator {

        //
        private const int MOUSE_WHEEL_CLICK_SIZE = 120;

        /// <summary>
        /// Sends the list of <see cref="Input"/> messages using the <see cref="InputMessageDispatcher"/> instance.
        /// </summary>
        /// <param name="inputList">The <see cref="System.Array"/> of <see cref="Input"/> messages to send.</param>
        private static void SendSimulatedInput(Input[] inputList)
        {
            InputMessageDispatcher.DispatchInput(inputList);
        }

        /// <summary>
        /// Simulates mouse movement by the specified distance measured as a delta from the current mouse location in pixels.
        /// </summary>
        /// <param name="pixelDeltaX">The distance in pixels to move the mouse horizontally.</param>
        /// <param name="pixelDeltaY">The distance in pixels to move the mouse vertically.</param>
        internal static void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            var inputList = new InputBuilder().AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulates a mouse left button down gesture.
        /// </summary>
        internal static void LeftButtonDown()
        {
            var inputList = new InputBuilder().AddMouseButtonDown(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulates a mouse left button up gesture.
        /// </summary>
        internal static void LeftButtonUp()
        {
            var inputList = new InputBuilder().AddMouseButtonUp(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulates a mouse right button down gesture.
        /// </summary>
        internal static void RightButtonDown()
        {
            var inputList = new InputBuilder().AddMouseButtonDown(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulates a mouse right button up gesture.
        /// </summary>
        internal static void RightButtonUp()
        {
            var inputList = new InputBuilder().AddMouseButtonUp(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Simulates mouse vertical wheel scroll gesture.
        /// </summary>
        /// <param name="scrollAmountInClicks">The amount to scroll in clicks. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user.</param>
        internal static void VerticalScroll(int scrollAmountInClicks)
        {
            var inputList = new InputBuilder().AddMouseVerticalWheelScroll(scrollAmountInClicks * MOUSE_WHEEL_CLICK_SIZE).ToArray();
            SendSimulatedInput(inputList);
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        internal static void Sleep(int millsecondsTimeout)
        {
            Thread.Sleep(millsecondsTimeout);
        }
    }
}
