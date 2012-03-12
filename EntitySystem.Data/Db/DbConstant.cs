namespace EntitySystem.Data.Db
{
    /// <summary>
    /// Defines constants used for an entity system database.
    /// </summary>
    public static class DbConstant
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
        /// Query parameter.
        /// </summary>
        public const string ParamLevelNumber = "LevelNumber";
    }
}