namespace EntitySystem
{
    /// <summary>
    /// A system in the Entity System. A system is responsible for processing the component(s) of a particular aspect
    /// of the game.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Gets a value indicating whether this system should be processed for update.
        /// </summary>
        bool UpdateEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this system should be processed for draw.
        /// </summary>
        bool DrawEnabled { get; }

        /// <summary>
        /// Gets the state info for the current game step.
        /// </summary>
        GameStepState GameStepState { get; }

        /// <summary>
        /// Perform update processing.
        /// </summary>
        void ProcessUpdate();

        /// <summary>
        /// Perform draw processing.
        /// </summary>
        void ProcessDraw();
    }
}