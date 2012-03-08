namespace EntitySystem.Data
{
    using EntitySystem.Entity;

    /// <summary>
    /// This interface serves a bridge between the entity system and a data source for loading and saving level data.
    /// </summary>
    public interface IEntityDataAdapter
    {
        /// <summary>
        /// Gets or sets the IEntityTransformer that is responsible for transforming entities into a serializable or
        /// deserializable state.
        /// </summary>
        IEntityTransformer Transformer { get; set; }

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        LevelInfo[] GetLevels();

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        bool LoadLevel(EntityManager entityManager, int levelNum);

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        bool SaveLevel(EntityManager entityManager, int levelNum);
    }
}