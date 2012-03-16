namespace EntitySystem.Data.Db
{
    using System.Data.Common;

    /// <summary>
    /// Creates the entity system database commands.
    /// </summary>
    public static class ESCommand
    {
        #region Read Commands

        #region Entity and Component

        /// <summary>
        /// Get the data for all entity-components in the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <returns>The command object.</returns>
        public static DbCommand GetLevelEntityComponentData(DbConnection connection, int levelNumber)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT
                    ec.EntityId
                    ,c.Classname
                    ,ecd.EntityComponentId
                    ,ecd.Property
                    ,ecd.Value
                    ,ecd.DataType
                FROM
                    Level l
                    INNER JOIN Level_Entity le
                        ON l.Id = le.LevelId
                    INNER JOIN EntityComponent ec
                        ON le.EntityId = ec.EntityId
                    INNER JOIN Component c
                        ON ec.ComponentId = c.Id
                    LEFT JOIN EntityComponent_Data ecd
                        ON ec.Id = ecd.EntityComponentId
                WHERE
                    l.Number = @LevelNumber
                ORDER BY
                    ec.EntityId
                    ,ecd.EntityComponentId
                    ,ecd.Property";

            // Add parameters
            command.Parameters[command.Parameters.Add(levelNumber)].ParameterName = "LevelNumber";

            return command;
        }

        /// <summary>
        /// Get the array data for all entity-components in the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <returns>The command object.</returns>
        public static DbCommand GetLevelEntityComponentArrayData(DbConnection connection, int levelNumber)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT
                    ec.EntityId
                    ,c.Classname
                    ,eca.EntityComponentId
                    ,eca.Property
                    ,a.Length
                    ,a.DataType
                    ,ad.[Index]
                    ,ad.Value
                FROM
                    Level l
                    INNER JOIN Level_Entity le
                        ON l.Id = le.LevelId
                    INNER JOIN EntityComponent ec
                        ON le.EntityId = ec.EntityId
                    INNER JOIN Component c
                        ON ec.ComponentId = c.Id
                    LEFT JOIN EntityComponent_Array eca
                        ON ec.Id = eca.EntityComponentId
                    LEFT JOIN Array a
                        ON eca.ArrayId = a.Id
                    LEFT JOIN ArrayData ad
                        ON a.Id = ad.ArrayId
                WHERE
                    l.Number = @LevelNumber
                ORDER BY
                    ec.EntityId
                    ,eca.EntityComponentId
                    ,eca.Property
                    ,ad.[Index]";

            // Add parameters
            command.Parameters[command.Parameters.Add(levelNumber)].ParameterName = "LevelNumber";

            return command;
        }

        #endregion

        #region Level

        /// <summary>
        /// Create a command to get the available level information.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>The command object.</returns>
        public static DbCommand GetLevelInfo(DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT
                    Number    
                    ,Name
                    ,Description
                FROM
                    Level
                ORDER BY
                    Number";

            return command;
        }

        #endregion

        #region Saved Game

        #endregion

        #endregion

        #region Write Commands

        #region Entity and Component

        /// <summary>
        /// Create a command to create an entity and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntity(DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Entity (Id)
                VALUES (NULL)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'Entity'";

            return command;
        }

        /// <summary>
        /// Create a command to create an entity template and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="name">The template name.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntityTemplate(DbConnection connection, int entityId, string name)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO EntityTemplate (Id, EntityId, Name)
                VALUES (NULL, @EntityId, @Name)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'EntityTemplate'";

            // Add parameters
            command.Parameters[command.Parameters.Add(entityId)].ParameterName = "EntityId";
            command.Parameters[command.Parameters.Add(name)].ParameterName = "Name";

            return command;
        }

        /// <summary>
        /// Create a command to create a component and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="classname">The component classname.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateComponent(DbConnection connection, string classname)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Component (Id, Classname)
                VALUES (NULL, @Classname)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'Component'";

            // Add parameters
            command.Parameters[command.Parameters.Add(classname)].ParameterName = "Classname";

            return command;
        }

        /// <summary>
        /// Create a command to create an entity-component mapping and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="componentId">The component id.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntityComponent(DbConnection connection, int entityId, int componentId)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO EntityComponent (Id, EntityId, ComponentId)
                VALUES (NULL, @EntityId, @ComponentId)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'EntityComponent'";

            // Add parameters
            command.Parameters[command.Parameters.Add(entityId)].ParameterName = "EntityId";
            command.Parameters[command.Parameters.Add(componentId)].ParameterName = "ComponentId";

            return command;
        }

        /// <summary>
        /// Create a command to set data for an entity-component property.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityComponentId">The entity-component mapping id.</param>
        /// <param name="property">The name of the property on the component.</param>
        /// <param name="value">The value of the property on the component.</param>
        /// <param name="dataType">The data type of the property on the component.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntityComponentData(
            DbConnection connection,
            int entityComponentId,
            string property,
            string value,
            string dataType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO EntityComponent_Data (EntityComponentId, Property, Value, DataType)
                VALUES (@EntityComponentId, @Property, @Value, @DataType)";

            // Add parameters
            command.Parameters[command.Parameters.Add(entityComponentId)].ParameterName = "EntityComponentId";
            command.Parameters[command.Parameters.Add(property)].ParameterName = "Property";
            command.Parameters[command.Parameters.Add(value)].ParameterName = "Value";
            command.Parameters[command.Parameters.Add(dataType)].ParameterName = "DataType";

            return command;
        }

        /// <summary>
        /// Create a command to create an array for an entity-component property and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="entityComponentId">The entity-component mapping id.</param>
        /// <param name="property">The name of the property on the component.</param>
        /// <param name="length">The array length.</param>
        /// <param name="dataType">The data type of the array.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntityComponentArray(
            DbConnection connection,
            int entityComponentId,
            string property,
            int length,
            string dataType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Array (Id, Length, DataType)
                VALUES (NULL, @Length, @DataType)
                ;
                INSERT INTO EntityComponent_Array (EntityComponentId, Property, ArrayId)
                SELECT
                    @EntityComponentId
                    ,@Property
                    seq AS ArrayId
                FROM
                    SQLITE_SEQUENCE
                WHERE
                    name = 'Array'
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'Array'";

            // Add parameters
            command.Parameters[command.Parameters.Add(entityComponentId)].ParameterName = "EntityComponentId";
            command.Parameters[command.Parameters.Add(property)].ParameterName = "Property";
            command.Parameters[command.Parameters.Add(length)].ParameterName = "Length";
            command.Parameters[command.Parameters.Add(dataType)].ParameterName = "DataType";

            return command;
        }

        /// <summary>
        /// Create a command to set the data of an entity-component's array at the given index.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="arrayId">The array id.</param>
        /// <param name="index">The array index of the item to add.</param>
        /// <param name="value">The value.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateEntityComponentArrayData(
            DbConnection connection,
            int arrayId,
            int index,
            string value)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO ArrayData (ArrayId, Index, Value)
                VALUES (@ArrayId, @Index, @Value)";

            // Add parameters
            command.Parameters[command.Parameters.Add(arrayId)].ParameterName = "ArrayId";
            command.Parameters[command.Parameters.Add(index)].ParameterName = "Index";
            command.Parameters[command.Parameters.Add(value)].ParameterName = "Value";

            return command;
        }

        #endregion

        #region Level

        /// <summary>
        /// Create a command to create a level with the given details and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="number">The level number.</param>
        /// <param name="name">The level name.</param>
        /// <param name="description">The level description.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateLevel(DbConnection connection, int number, string name, string description)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Level (Id, Number, Name, Description)
                VALUES (NULL, @Number, @Name, @Description)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'Level'";

            // Add parameters
            command.Parameters[command.Parameters.Add(number)].ParameterName = "Number";
            command.Parameters[command.Parameters.Add(name)].ParameterName = "Name";
            command.Parameters[command.Parameters.Add(description)].ParameterName = "Description";

            return command;
        }

        /// <summary>
        /// Create a command to add an entity to a level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="levelId">The level id.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>The command object.</returns>
        public static DbCommand AddLevelEntity(DbConnection connection, int levelId, int entityId)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Level_Entity (LevelId, EntityId)
                VALUES (@LevelId, @EntityId)";

            // Add parameters
            command.Parameters[command.Parameters.Add(levelId)].ParameterName = "LevelId";
            command.Parameters[command.Parameters.Add(entityId)].ParameterName = "EntityId";

            return command;
        }

        #endregion

        #region Saved Game

        /// <summary>
        /// Create a command to create a saved game with the given details and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="name">The saved game name.</param>
        /// <param name="levelId">The level id of the game being saved.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateSavedGame(DbConnection connection, string name, int levelId)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO SavedGame (Id, Name, LevelId)
                VALUES (NULL, @Name, @LevelId)
                ;
                SELECT seq AS Id FROM SQLITE_SEQUENCE WHERE name = 'Level'";

            // Add parameters
            command.Parameters[command.Parameters.Add(name)].ParameterName = "Name";
            command.Parameters[command.Parameters.Add(levelId)].ParameterName = "LevelId";

            return command;
        }

        /// <summary>
        /// Create a command to add an entity to a saved game.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="savedGameId">The saved game id.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns>The command object.</returns>
        public static DbCommand AddSavedGameEntity(DbConnection connection, int savedGameId, int entityId)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO SavedGame_Entity (LevelId, EntityId)
                VALUES (@SavedGameId, @EntityId)";

            // Add parameters
            command.Parameters[command.Parameters.Add(savedGameId)].ParameterName = "SavedGameId";
            command.Parameters[command.Parameters.Add(entityId)].ParameterName = "EntityId";

            return command;
        }

        #endregion

        #endregion
    }
}