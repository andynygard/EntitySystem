namespace EntitySystem.Subsystem
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
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        void Process(int delta);
    }
}