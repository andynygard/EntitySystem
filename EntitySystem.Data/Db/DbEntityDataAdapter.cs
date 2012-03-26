namespace EntitySystem.Data.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Reflection;
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

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Get the level id
                            int dbLevelId = this.GetLevelId(connection, levelNum);

                            // Clear the level
                            this.ClearLevel(connection, dbLevelId);

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
                                    dbEntityId = this.CreateEntityInLevel(connection, dbLevelId);
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
                                    dbComponentId = this.CreateComponent(connection, component.GetType().ToString());
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
                                    dbEntityComponentId = this.CreateEntityComponent(connection, dbEntityId, dbComponentId);
                                    if (addedEntityComponents.ContainsKey(dbEntityId))
                                    {
                                        addedEntityComponents[dbEntityId].Add(dbComponentId, dbEntityComponentId);
                                    }
                                    else
                                    {
                                        addedEntityComponents.Add(
                                            dbEntityId,
                                            new Dictionary<int, int>() { { dbComponentId, dbEntityComponentId } });
                                    }
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

        #region Database Methods

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
        /// Get the database id for the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>The database id for the level.</returns>
        private int GetLevelId(DbConnection connection, int levelNum)
        {
            using (DbCommand command = ESCommand.GetLevelId(connection, levelNum))
            {
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    throw new ApplicationException(
                        string.Format("Level {0} does not exist in the database.", levelNum));
                }

                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Clear all data in the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="dbLevelId">The database id for the level.</param>
        private void ClearLevel(DbConnection connection, int dbLevelId)
        {
            using (DbCommand command = ESCommand.ClearLevel(connection, dbLevelId))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Create an entity in the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="dbLevelId">The database id for the level.</param>
        /// <returns>The database id for the entity.</returns>
        private int CreateEntityInLevel(DbConnection connection, int dbLevelId)
        {
            int dbEntityId;
            using (DbCommand command = ESCommand.CreateEntity(connection))
            {
                dbEntityId = Convert.ToInt32(command.ExecuteScalar());
            }

            // Add this entity to the level
            using (DbCommand command = ESCommand.AddLevelEntity(connection, dbLevelId, dbEntityId))
            {
                command.ExecuteNonQuery();
            }

            return dbEntityId;
        }

        /// <summary>
        /// Create a component.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="classname">The component classname.</param>
        /// <returns>The database id for the component.</returns>
        private int CreateComponent(DbConnection connection, string classname)
        {
            using (DbCommand command = ESCommand.CreateComponent(connection, classname))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Create an entity-component.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="dbEntityId">The database id for the entity.</param>
        /// <param name="dbComponentId">The database id for the component.</param>
        /// <returns>The database id for the entity-component.</returns>
        private int CreateEntityComponent(DbConnection connection, int dbEntityId, int dbComponentId)
        {
            using (DbCommand command = ESCommand.CreateEntityComponent(connection, dbEntityId, dbComponentId))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Add the data for the given entity-component to the database.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityComponentId">The entity-component id in the database.</param>
        /// <param name="component">The component whose data will be added.</param>
        private void AddEntityComponentData(DbConnection connection, int entityComponentId, IComponent component)
        {
            // Get the public gettable properties
            PropertyInfo[] properties = component.GetType().GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty);

            // Add each property
            foreach (PropertyInfo property in properties)
            {
                // Check that the property is valid
                if (!this.CanSerializeProperty(property))
                {
                    continue;
                }

                // Get the property value
                object value = property.GetValue(component, null);

                // Add the data
                if (!property.PropertyType.IsArray)
                {
                    // This is not an array so simply add the data row
                    string valueStr = value != null ? value.ToString() : null;
                    using (DbCommand command = ESCommand.CreateEntityComponentData(
                        connection,
                        entityComponentId,
                        property.Name,
                        valueStr,
                        property.PropertyType.ToString()))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Get the array info
                    string dataType = property.PropertyType.GetElementType().ToString();
                    int length = value != null ? (value as Array).Length : 0;

                    // Create the array reference for the property
                    int dbArrayId;
                    using (DbCommand command = ESCommand.CreateEntityComponentArray(
                        connection,
                        entityComponentId,
                        property.Name,
                        length,
                        dataType))
                    {
                        dbArrayId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Now add each array element
                    if (value != null)
                    {
                        var valueArray = (Array)value;
                        for (int i = 0; i < valueArray.Length; i++)
                        {
                            object itemValue = valueArray.GetValue(i);
                            string itemValueStr = itemValue != null ? itemValue.ToString() : null;
                            using (DbCommand command = ESCommand.CreateEntityComponentArrayData(
                                connection,
                                dbArrayId,
                                i,
                                itemValueStr))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get a value indicating whether the given property can be serialized.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the property can be serialized.</returns>
        private bool CanSerializeProperty(PropertyInfo property)
        {
            // Check if this property has the ESIgnoreData attribute
            foreach (Attribute attribute in property.GetCustomAttributes(true))
            {
                if (attribute is ESIgnoreData)
                {
                    return false;
                }
            }

            // Check if the data type can be serialized
            return this.CanSerializeType(property.PropertyType);
        }

        /// <summary>
        /// Get a value indicating whether the given type can be serialized.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the type can be serialized.</returns>
        private bool CanSerializeType(Type type)
        {
            if (type.IsArray)
            {
                // Check if the array's elements can be serialized
                return this.CanSerializeType(type.GetElementType());
            }
            else
            {
                // The type can be serialized if it is not an object type
                return Type.GetTypeCode(type) != TypeCode.Object;
            }
        }

        #endregion
    }
}