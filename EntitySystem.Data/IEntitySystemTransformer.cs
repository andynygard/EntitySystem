namespace EntitySystem.Data
{
    using EntitySystem.Entity;

    /// <summary>
    /// Responsible for transforming an EntityManager into a serializable or deserializable state. The reason for this
    /// is that the structure of the EntityManager can be different at runtime compared to when it is stored as data (as
    /// some components only exist to facilitate the initialisation of the functional components).
    /// <para />
    /// The class implementing this interface is responsible for providing the ability to transform an EntityManager
    /// from its live runtime state to its stored-state and vice versa.
    /// </summary>
    public interface IEntitySystemTransformer
    {
        /// <summary>
        /// Transform the given EntityManager into a state that is ready for serialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformForSerialization(EntityManager entityManager);

        /// <summary>
        /// Transform the given EntityManager into a state that is ready for deserialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformForDeserialization(EntityManager entityManager);
    }
}