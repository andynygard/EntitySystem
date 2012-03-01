namespace EntitySystem.Data
{
    /// <summary>
    /// Describes a level.
    /// </summary>
    public class LevelInfo
    {
        /// <summary>
        /// Gets the level number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the level name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the level description.
        /// </summary>
        public string Description { get; private set; }
    }
}
