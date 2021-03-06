﻿namespace EntitySystem.Data
{
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
        /// Transform the entities in the given EntityManager into the gameplay-ready state.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformPostLoad(EntityManager entityManager);

        /// <summary>
        /// Transform the entities in the given EntityManager into the storage-ready state.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        void TransformPreSave(EntityManager entityManager);
    }
}