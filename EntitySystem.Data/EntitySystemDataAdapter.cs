namespace EntitySystem.Data
{
    using System;
    using System.Data.Common;
    using EntitySystem.Entity;

    /// <summary>
    /// Serves a bridge between an EntityManager and a data source for loading and saving level data.
    /// </summary>
    public class EntitySystemDataAdapter : IEntitySystemDataAdapter
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// Initializes a new instance of the EntitySystemDataAdapter class.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        public EntitySystemDataAdapter(DbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        public LevelInfo[] GetLevels()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        public bool LoadLevel(EntityManager entityManager, int levelNum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to initialise.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        public bool SaveLevel(EntityManager entityManager, int levelNum)
        {
            throw new NotImplementedException();
        }
    }
}