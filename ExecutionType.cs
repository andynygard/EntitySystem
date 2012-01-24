namespace EntitySystem
{
    /// <summary>
    /// Indicates the type of execution that a system performs.
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        /// Responsible for updating the game state.
        /// </summary>
        Update,

        /// <summary>
        /// Responsible for drawing.
        /// </summary>
        Draw,

        /// <summary>
        /// Responsible for both updating the game state and drawing.
        /// </summary>
        UpdateAndDraw
    }
}
