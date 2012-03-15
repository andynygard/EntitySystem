namespace EntitySystem.Data.Db
{
    using System.Data.Common;

    /// <summary>
    /// Creates the entity system database commands.
    /// </summary>
    public static class ESCommand
    {
        /// <summary>
        /// Creates a command to get the available level information.
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

        /// <summary>
        /// Creates a command to create a new entity and select its Id.
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
                SELECT
                    seq AS Id
                FROM
                    SQLITE_SEQUENCE    
                WHERE
                     name = 'Entity'";

            return command;
        }

        /// <summary>
        /// Creates a command to create a new component and select its Id.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="classname">The classname of the component.</param>
        /// <returns>The command object.</returns>
        public static DbCommand CreateComponent(DbConnection connection, string classname)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Component (Id, Classname)
                VALUES (NULL, @Classname)
                ;
                SELECT
                    seq AS Id
                FROM
                    SQLITE_SEQUENCE    
                WHERE
                     name = 'Component'";

            // Add parameters
            command.Parameters[command.Parameters.Add(classname)].ParameterName = "Classname";

            return command;
        }

        /// <summary>
        /// Creates a command to create a new entity-component mapping in the entity system and select its Id.
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
                SELECT
                    seq AS Id
                FROM
                    SQLITE_SEQUENCE    
                WHERE
                     name = 'EntityComponent'";

            // Add parameters
            command.Parameters[command.Parameters.Add(entityId)].ParameterName = "EntityId";
            command.Parameters[command.Parameters.Add(componentId)].ParameterName = "ComponentId";

            return command;
        }
    }
}