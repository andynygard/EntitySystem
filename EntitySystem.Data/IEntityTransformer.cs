namespace EntitySystem.Data
{
    using EntitySystem.Entity;

    /// <summary>
    /// Responsible for transforming entities into a serializable or deserializable state. The reason for this is that
    /// the component structure of entities can be different at runtime compared to when they are stored as data (as
    /// some components only exist to facilitate the initialisation of entities).
    /// <para />
    /// The class implementing this interface is responsible for providing the ability to transform entites from their
    /// live runtime state to their stored state and vice versa.
    /// </summary>
    public interface IEntityTransformer
    {
        /// <summary>
        /// Transform the entities in the given EntityManager into a state that is ready for serialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformForSerialization(EntityManager entityManager);

        /// <summary>
        /// Transform the entities in the given EntityManager into a state that is ready for deserialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformForDeserialization(EntityManager entityManager);
    }
}