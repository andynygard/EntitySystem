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
        internal GameStepState()
        {
            this.Delta = 0;
            this.lastStepTime = DateTime.MinValue;
        }

        /// <summary>
        /// Gets the number of milliseconds since the the last game step.
        /// </summary>
        public int Delta { get; private set; }

        /// <summary>
        /// Update the delta value for a new game step.
        /// </summary>
        internal void Step()
        {
            // Check if this is the first step, in which case Delta will remain at zero.
            if (!this.lastStepTime.Equals(DateTime.MinValue))
            {
                this.Delta = (int)DateTime.Now.Subtract(lastStepTime).TotalMilliseconds;
            }

            this.lastStepTime = DateTime.Now;
        }
    }
}