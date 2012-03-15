﻿namespace EntitySystem.Data.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using EntitySystem.Entity;

    /// <summary>
    /// Serves a bridge between the entity system and a database for loading and saving level data.
    /// </summary>
    public class DbEntityDataAdapter : EntityDataAdapter
    {
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

                using (DbCommand command = this.CreateCommandGetLevels(connection))
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
            // Populate a data set with the level data
            var dataSet = new EntityDataSet();
            using (DbConnection connection = this.CreateConnection())
            {
                using (DbCommand command = this.CreateCommandLoadLevelData(connection, levelNum))
                {
                    using (DbDataAdapter dataAdapter = this.dbFactory.CreateDataAdapter())
                    {
                        dataAdapter.SelectCommand = command;
                        dataAdapter.Fill(dataSet);
                    }
                }
            }

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
            throw new NotImplementedException();
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
        /// Create a command to get the list of available levels.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <returns>The command object.</returns>
        private DbCommand CreateCommandGetLevels(DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = DbConstant.GetLevels;
            return command;
        }

        /// <summary>
        /// Create a command to load the data of the given level.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <param name="levelNum">The level number to load.</param>
        /// <returns>The command object.</returns>
        private DbCommand CreateCommandLoadLevelData(DbConnection connection, int levelNum)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = DbConstant.GetLevelData;

            // Set the query parameter
            DbParameter param = this.dbFactory.CreateParameter();
            param.ParameterName = DbConstant.ParamLevelNumber;
            param.DbType = DbType.Int32;
            param.Value = levelNum;
            command.Parameters.Add(param);

            return command;
        }

        #endregion
    }
}