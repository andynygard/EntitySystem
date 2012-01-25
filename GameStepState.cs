namespace EntitySystem
{
    using System;

    /// <summary>
    /// A reference to the delta value of the current game step.
    /// </summary>
    public class GameStepState
    {
        /// <summary>
        /// The last time that the game step took place.
        /// </summary>
        private DateTime lastStepTime;

        /// <summary>
        /// Initializes a new instance of the GameStepState class.
        /// </summary>
        public GameStepState()
        {
            this.Delta = 0;
            this.lastStepTime = DateTime.Now;
        }

        /// <summary>
        /// Gets the number of milliseconds since the the last game step.
        /// </summary>
        public int Delta { get; private set; }

        /// <summary>
        /// Update the delta value for a new game step.
        /// </summary>
        public void Step()
        {
            this.Delta = (int)DateTime.Now.Subtract(lastStepTime).TotalMilliseconds;
            this.lastStepTime = DateTime.Now;
        }
    }
}