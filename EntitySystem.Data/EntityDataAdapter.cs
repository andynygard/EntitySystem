namespace EntitySystem.Data
{
    using System;
    using System.Data.Common;
    using EntitySystem.Entity;

    /// <summary>
    /// Serves a bridge between the entity system and a data source for loading and saving level data.
    /// </summary>
    public class EntityDataAdapter : IEntityDataAdapter
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// Initializes a new instance of the EntityDataAdapter class.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="transformer">The entity system transformer.</param>
        public EntityDataAdapter(DbConnection connection, IEntityTransformer transformer)
        {
            this.Transformer = transformer;
            this.connection = connection;
        }

        /// <summary>
        /// Gets or sets the IEntityTransformer that is responsible for transforming entities into a serializable or
        /// deserializable state.
        /// </summary>
        public IEntityTransformer Transformer { get; set; }

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
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        public bool LoadLevel(EntityManager entityManager, int levelNum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        public bool SaveLevel(EntityManager entityManager, int levelNum)
        {
            throw new NotImplementedException();
        }
    }
}