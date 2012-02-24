namespace EntitySystem.Data
{
    using System.Collections.Generic;
    using EntitySystem.Entity;

    /// <summary>
    /// This interface serves a bridge between an EntityManager and a data source for loading and saving level data.
    /// </summary>
    public interface IEntitySystemDataAdapter
    {
        /// <summary>
        /// Get the list of levels.
        /// </summary>
        /// <returns>A dictionary containing the level numbers and names.</returns>
        Dictionary<int, string> GetLevels();

        /// <summary>
        /// Load the level with the given sequence number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        bool LoadLevel(EntityManager entityManager, int levelNum);

        /// <summary>
        /// Save the level with the given sequence number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        bool SaveLevel(EntityManager entityManager, int levelNum);
    }
}