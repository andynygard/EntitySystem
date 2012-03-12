namespace EntitySystem.Data
{
    /// <summary>
    /// Describes a level.
    /// </summary>
    public class LevelInfo
    {
        /// <summary>
        /// Initializes a new instance of the LevelInfo class.
        /// </summary>
        /// <param name="number">The level number.</param>
        /// <param name="name">The level name.</param>
        /// <param name="description">The level description.</param>
        public LevelInfo(int number, string name, string description)
        {
            this.Number = number;
            this.Name = name;
            this.Description = description;
        }

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