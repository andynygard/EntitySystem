namespace EntitySystem
{
    /// <summary>
    /// An entity. This is simply a numerical id, however exists as a class so that type-checks can be made during
    /// serialization.
    /// </summary>
    public sealed class Entity
    {
        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        /// <param name="id">The entity id.</param>
        public Entity(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        public int Id { get; private set; }
    }
}
