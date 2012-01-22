namespace EntitySystem
{
    /// <summary>
    /// An entity in the Entity System.
    /// <para />
    /// An entity represents one game object in the game world. The aspects (or 'behaviour') of the entity is decorated
    /// via the EntityManager. The entity is a loose representation of the game object, as the Entity class itself
    /// contains no data or methods - it is simply a label.
    /// </summary>
    public sealed class Entity
    {
        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        /// <param name="id">The unique id of this entity.</param>
        public Entity(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the entity's unique id.
        /// </summary>
        public int Id { get; private set; }
    }
}