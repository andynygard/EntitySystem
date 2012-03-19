namespace EntitySystem.Data.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using EntitySystem.Component;
    using EntitySystem.Entity;

    /// <summary>
    /// Serves a bridge between the entity system and a database for loading and saving level data.
    /// </summary>
    public class DbEntityDataAdapter : EntityDataAdapter
    {
        #region Constants

        /// <summary>
        /// A list of data types that 
        /// </summary>
        private readonly Type[] ValidDataTypes = new Type[] { };

        #endregion

        #region Private Variables

        /// <summary>
        /// The database provider factory.
        /// </summary>
        private DbProviderFactory dbFactory;

        /// <summary>
        /// The database connection string.
        /// </summary>
        private string connectionString;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DbEntityDataAdapter class.
        /// </summary>
        /// <param name="dbFactory">The database provider factory.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="transformer">The entity system transformer.</param>
        public DbEntityDataAdapter(
            DbProviderFactory dbFactory,
            string connectionString,
            IEntityTransformer transformer)
        {
            this.dbFactory = dbFactory;
            this.connectionString = connectionString;
            this.Transformer = transformer;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        public override LevelInfo[] GetLevels()
        {
            var levels = new List<LevelInfo>();

            using (DbConnection connection = this.CreateConnection())
            {
                connection.Open();

                using (DbCommand command = ESCommand.GetLevelInfo(connection))
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var level = new LevelInfo(
                                Convert.ToInt32(reader["Number"]),
                                Convert.ToString(reader["Name"]),
                                Convert.ToString(reader["Description"]));

                            levels.Add(level);
                        }
                    }
                }
            }

            return levels.ToArray();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        protected override bool DoLoadLevel(EntityManager entityManager, int levelNum)
        {
            return false;
        }

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        protected override bool DoSaveLevel(EntityManager entityManager, int levelNum)
        {
            try
            {
                using (DbConnection connection = this.CreateConnection())
                {
                    connection.Open();

                    // Perform this in a transaction
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Clear the level
                            using (DbCommand command = ESCommand.ClearLevel(connection, levelNum))
                            {
                                command.ExecuteNonQuery();
                            }

                            // Key = local entity; Value = db entity id
                            var addedEntities = new Dictionary<int, int>();
                            var addedComponents = new Dictionary<IComponent, int>();
                            var addedEntityComponents = new Dictionary<int, Dictionary<int, int>>();

                            // Iterate over all entity-components in the entity manager
                            foreach (KeyValuePair<int, IComponent> kvp in entityManager)
                            {
                                int entity = kvp.Key;
                                IComponent component = kvp.Value;

                                // Get the database entity id
                                int dbEntityId;
                                if (addedEntities.ContainsKey(entity))
                                {
                                    dbEntityId = addedEntities[entity];
                                }
                                else
                                {
                                    // The entity doesn't exist so create it
                                    using (DbCommand command = ESCommand.CreateEntity(connection))
                                    {
                                        dbEntityId = Convert.ToInt32(command.ExecuteScalar());
                                    }

                                    addedEntities.Add(entity, dbEntityId);
                                }

                                // Get the database component id
                                int dbComponentId;
                                if (addedComponents.ContainsKey(component))
                                {
                                    dbComponentId = addedComponents[component];
                                }
                                else
                                {
                                    // The component doesn't exist so create it
                                    using (DbCommand command =
                                        ESCommand.CreateComponent(connection, component.GetType().ToString()))
                                    {
                                        dbComponentId = Convert.ToInt32(command.ExecuteScalar());
                                    }

                                    addedComponents.Add(component, dbComponentId);
                                }

                                // Get the database entity-component id
                                int dbEntityComponentId;
                                if (addedEntityComponents.ContainsKey(dbEntityId) &&
                                    addedEntityComponents[dbEntityId].ContainsKey(dbComponentId))
                                {
                                    dbEntityComponentId = addedEntityComponents[dbEntityId][dbComponentId];
                                }
                                else
                                {
                                    // The entity-component doesn't exist so create it
                                    using (DbCommand command =
                                        ESCommand.CreateEntityComponent(connection, dbEntityId, dbComponentId))
                                    {
                                        dbEntityComponentId = Convert.ToInt32(command.ExecuteScalar());
                                    }

                                    if (!addedEntityComponents.ContainsKey(dbEntityId))
                                    {
                                        addedEntityComponents.Add(dbEntityId, new Dictionary<int, int>());
                                    }

                                    addedEntityComponents[dbEntityId].Add(dbComponentId, dbEntityComponentId);
                                }

                                // Add the data for this component
                                this.AddEntityComponentData(connection, dbEntityComponentId, component);
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction and throw the exception
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                // TODO: Log exception
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a database connection.
        /// </summary>
        /// <returns>A database connection.</returns>
        private DbConnection CreateConnection()
        {
            DbConnection connection = this.dbFactory.CreateConnection();
            connection.ConnectionString = this.connectionString;
            return connection;
        }

        /// <summary>
        /// Add the data for the given entity-component to the database.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityComponentId">The entity-component id in the database.</param>
        /// <param name="component">The component whose data will be added.</param>
        private void AddEntityComponentData(DbConnection connection, int entityComponentId, IComponent component)
        {
            // TODO
        }

        #endregion
    }
}