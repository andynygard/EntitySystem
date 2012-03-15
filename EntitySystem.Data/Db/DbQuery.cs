namespace EntitySystem.Data.Db
{
    /// <summary>
    /// Defines the queries used for an entity system database.
    /// </summary>
    public static class DbQuery
    {
        /// <summary>
        /// Query to fetch the available levels.
        /// </summary>
        public const string GetLevels = @"
            SELECT
                Number    
                ,Name
                ,Description
            FROM
                Level
            ORDER BY
                Number";

        /// <summary>
        /// Query to fetch the level data.
        /// </summary>
        public const string GetLevelData = @"
            ";

        /// <summary>
        /// Query to create a new entity id and select its Id.
        /// </summary>
        public const string CreateEntity = @"
            INSERT INTO Entity (Id)
            VALUES (NULL)
            ;
            SELECT
                seq AS Id
            FROM
                SQLITE_SEQUENCE    
            WHERE
                 name = 'Entity'";

        /// <summary>
        /// Query to create a new component and select its id.
        /// </summary>
        public const string CreateComponent = @"
            INSERT INTO Component (Id, Classname)
            VALUES (NULL, @Classname)
            ;
            SELECT
                seq AS Id
            FROM
                SQLITE_SEQUENCE    
            WHERE
                 name = 'Component'
";

        /// <summary>
        /// Query parameter.
        /// </summary>
        public const string ParamLevelNumber = "LevelNumber";
    }
}