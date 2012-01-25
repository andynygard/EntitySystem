namespace EntitySystem
{
    /// <summary>
    /// A system in the Entity System. A system is responsible for processing the component(s) of a particular aspect
    /// of the game.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Gets a value indicating whether this system should be processed.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Gets the state info for the current game step.
        /// </summary>
        GameStepState GameStepState { get; }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        void Process();
    }
}