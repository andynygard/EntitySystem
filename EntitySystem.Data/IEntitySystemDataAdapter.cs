namespace EntitySystem.Data
{
    using EntitySystem.Entity;

    /// <summary>
    /// This interface serves a bridge between an EntityManager and a data source for loading and saving level data.
    /// </summary>
    public interface IEntitySystemDataAdapter
    {
        /// <summary>
        /// Gets the entity system transformer that is responsible for transforming an EntityManager into a serializable
        /// or deserializable state.
        /// </summary>
        IEntitySystemTransformer Transformer { get; set; }

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        LevelInfo[] GetLevels();

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        bool LoadLevel(EntityManager entityManager, int levelNum);

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        bool SaveLevel(EntityManager entityManager, int levelNum);
    }
}