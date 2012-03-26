namespace EntitySystem.Data.Db
{
    using System.Data;
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
                    l.Number = @Number
                ORDER BY
                    ec.EntityId
                    ,ecd.EntityComponentId
                    ,ecd.Property";

            // Add parameters
            AddParameter(command, "Number", levelNumber, DbType.Int32);

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
                    l.Number = @Number
                ORDER BY
                    ec.EntityId
                    ,eca.EntityComponentId
                    ,eca.Property
                    ,ad.[Index]";

            // Add parameters
            AddParameter(command, "Number", levelNumber, DbType.Int32);

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
                SELECT MAX(Id) AS Id FROM Entity";

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
                SELECT Id FROM EntityTemplate WHERE EntityId = @EntityId";

            // Add parameters
            AddParameter(command, "EntityId", entityId, DbType.Int32);
            AddParameter(command, "Name", name, DbType.String);

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
                SELECT
                    NULL
                    ,@Classname
                WHERE
                    NOT EXISTS (SELECT 1 FROM Component WHERE Classname = @Classname)
                ;
                SELECT Id FROM Component WHERE Classname = @Classname";

            // Add parameters
            AddParameter(command, "Classname", classname, DbType.String);

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
                SELECT Id FROM EntityComponent WHERE EntityId = @EntityId AND ComponentId = @ComponentId";

            // Add parameters
            AddParameter(command, "EntityId", entityId, DbType.Int32);
            AddParameter(command, "ComponentId", componentId, DbType.Int32);

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
            AddParameter(command, "EntityComponentId", entityComponentId, DbType.Int32);
            AddParameter(command, "Property", property, DbType.String);
            AddParameter(command, "Value", value, DbType.String);
            AddParameter(command, "DataType", dataType, DbType.String);

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
                    ,MAX(Id)
                FROM
                    Array
                WHERE
                    Length = @Length
                    AND DataType = @DataType
                ;
                SELECT MAX(Id) AS Id FROM Array WHERE Length = @Length AND DataType = @DataType";

            // Add parameters
            AddParameter(command, "EntityComponentId", entityComponentId, DbType.Int32);
            AddParameter(command, "Property", property, DbType.String);
            AddParameter(command, "Length", length, DbType.Int32);
            AddParameter(command, "DataType", dataType, DbType.String);

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
                INSERT INTO ArrayData (ArrayId, [Index], Value)
                VALUES (@ArrayId, @Index, @Value)";

            // Add parameters
            AddParameter(command, "ArrayId", arrayId, DbType.Int32);
            AddParameter(command, "Index", index, DbType.Int32);
            AddParameter(command, "Value", value, DbType.String);

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
                SELECT Id FROM Level WHERE Number = @Number";

            // Add parameters
            AddParameter(command, "Number", number, DbType.Int32);
            AddParameter(command, "Name", name, DbType.String);
            AddParameter(command, "Description", description, DbType.String);

            return command;
        }

        /// <summary>
        /// Create a command to get the level id for the given level number.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="number">The level number.</param>
        /// <returns>The command object.</returns>
        public static DbCommand GetLevelId(DbConnection connection, int number)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT
                    Id
                FROM
                    Level
                WHERE
                    Number = @Number";

            // Add parameters
            AddParameter(command, "Number", number, DbType.Int32);

            return command;
        }

        /// <summary>
        /// Create a command to clear the given level.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="levelId">The level id.</param>
        /// <returns>The command object.</returns>
        public static DbCommand ClearLevel(DbConnection connection, int levelId)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM
                    Level_Entity
                WHERE
                    LevelId = @LevelId";

            // Add parameters
            AddParameter(command, "LevelId", levelId, DbType.Int32);

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
            AddParameter(command, "LevelId", levelId, DbType.Int32);
            AddParameter(command, "EntityId", entityId, DbType.Int32);

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
                SELECT Id FROM SavedGame WHERE Name = @Name";

            // Add parameters
            AddParameter(command, "Name", name, DbType.String);
            AddParameter(command, "LevelId", levelId, DbType.Int32);

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
            AddParameter(command, "SavedGameId", savedGameId, DbType.Int32);
            AddParameter(command, "EntityId", entityId, DbType.Int32);

            return command;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Add a parameter to the given command.
        /// </summary>
        /// <param name="command">The command the paramater will be added to.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dataType">The data type of the parameter.</param>
        private static void AddParameter(DbCommand command, string name, object value, DbType dataType)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            param.DbType = dataType;
            command.Parameters.Add(param);
        }

        #endregion
    }
}