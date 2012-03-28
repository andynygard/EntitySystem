namespace EntitySystem.Data
{
    /// <summary>
    /// This class serves a bridge between the entity system and a data source for loading and saving level data.
    /// </summary>
    public abstract class EntityDataAdapter
    {
        /// <summary>
        /// Gets or sets the IEntityTransformer that is responsible for transforming entities into a serializable or
        /// deserializable state.
        /// </summary>
        public IEntityTransformer Transformer { get; set; }

        /// <summary>
        /// Get the available levels.
        /// </summary>
        /// <returns>An array of level information.</returns>
        public abstract LevelInfo[] GetLevels();

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        public bool LoadLevel(EntityManager entityManager, int levelNum)
        {
            bool success = this.DoLoadLevel(entityManager, levelNum);

            // Perform post-load transformation
            if (success && this.Transformer != null)
            {
                this.Transformer.TransformPostLoad(entityManager);
            }

            return success;
        }

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        public bool SaveLevel(EntityManager entityManager, int levelNum)
        {
            // Perform pre-save transformation
            if (this.Transformer != null)
            {
                this.Transformer.TransformPreSave(entityManager);
            }

            return this.DoSaveLevel(entityManager, levelNum);
        }

        /// <summary>
        /// Load the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager to be populated.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was loaded.</returns>
        protected abstract bool DoLoadLevel(EntityManager entityManager, int levelNum);

        /// <summary>
        /// Save the level with the given level number.
        /// </summary>
        /// <param name="entityManager">The entity manager of the level to be saved.</param>
        /// <param name="levelNum">The level number.</param>
        /// <returns>True if the level was saved.</returns>
        protected abstract bool DoSaveLevel(EntityManager entityManager, int levelNum);
    }
}