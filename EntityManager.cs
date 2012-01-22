namespace EntitySystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a reference to the components in the Entity System, providing access to these.
    /// </summary>
    public class EntityManager
    {
        #region Constants

        /// <summary>
        /// The minimum assignable entity id.
        /// </summary>
        private const int MinEntityId = 1;

        /// <summary>
        /// Locking object to prevent two entities being created with the same id.
        /// </summary>
        private readonly object newEntityLock = new object();

        #endregion

        #region Private Variables

        /// <summary>
        /// Mapping of component type to each entity that uses that component and the component instance itself.
        /// </summary>
        private Dictionary<Type, Dictionary<int, IComponent>> componentMap;

        /// <summary>
        /// A list of the existing entities such that a new unique entity id can quickly be created.
        /// </summary>
        private List<int> existingEntities;

        /// <summary>
        /// The lowest unassigned entity id.
        /// </summary>
        private int lowestUnnasignedEntity;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EntityManager class.
        /// </summary>
        public EntityManager()
        {
            this.componentMap = new Dictionary<Type, Dictionary<int, IComponent>>();
            this.existingEntities = new List<int>();
            this.lowestUnnasignedEntity = MinEntityId;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <returns>The new entity.</returns>
        public int CreateEntity()
        {
            int entity = this.GenerateNewEntityId();

            // Retain the id to prevent it from being used again
            this.existingEntities.Add(entity);

            return entity;
        }

        /// <summary>
        /// Remove all references to the given entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemoveEntity(int entity)
        {
            // Remove this entity from the internal list
            lock (this.newEntityLock)
            {
                this.existingEntities.Remove(entity);
            }

            // Remove any references to this identity from the component map
            foreach (Dictionary<int, IComponent> entityComponent in this.componentMap.Values)
            {
                entityComponent.Remove(entity);
            }
        }

        /// <summary>
        /// Get the component of the given type for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="componentType">The component type.</param>
        /// <returns>The component instance; null if the entity does not have the given component.</returns>
        public IComponent GetComponent(int entity, Type componentType)
        {
            // Check that the given type is valid
            if (componentType.GetInterface("IComponent") == null)
            {
                throw new ArgumentException("Type parameter must implement the IComponent interface.", "componentType");
            }

            IComponent component = null;

            if (this.componentMap.ContainsKey(componentType))
            {
                Dictionary<int, IComponent> entities = this.componentMap[componentType];
                if (entities.ContainsKey(entity))
                {
                    component = entities[entity];
                }
            }

            return component;
        }

        /// <summary>
        /// Get all of the components of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>An array of the components.</returns>
        public IComponent[] GetComponents(Type componentType)
        {
            // Check that the given type is valid
            if (componentType.GetInterface("IComponent") == null)
            {
                throw new ArgumentException("Type parameter must implement the IComponent interface.", "componentType");
            }

            IComponent[] components;

            if (this.componentMap.ContainsKey(componentType))
            {
                ICollection<IComponent> componentCollection = this.componentMap[componentType].Values;
                components = new IComponent[componentCollection.Count];
                componentCollection.CopyTo(components, 0);
            }
            else
            {
                components = new IComponent[0];
            }

            return components;
        }

        /// <summary>
        /// Gets all the entities that have a component of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>An array of entities.</returns>
        public int[] GetEntitiesWithComponent(Type componentType)
        {
            // Check that the given type is valid
            if (componentType.GetInterface("IComponent") == null)
            {
                throw new ArgumentException("Type parameter must implement the IComponent interface.", "componentType");
            }

            int[] entities;

            if (this.componentMap.ContainsKey(componentType))
            {
                ICollection<int> entityCollection = this.componentMap[componentType].Keys;
                entities = new int[entityCollection.Count];
                entityCollection.CopyTo(entities, 0);
            }
            else
            {
                entities = new int[0];
            }

            return entities;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate a new entity id that isn't being used.
        /// </summary>
        /// <returns>An entity id.</returns>
        private int GenerateNewEntityId()
        {
            lock (this.newEntityLock)
            {
                if (this.lowestUnnasignedEntity < int.MaxValue)
                {
                    return this.lowestUnnasignedEntity++;
                }
                else
                {
                    // Take the first free entity
                    for (int i = MinEntityId; i < int.MaxValue; i++)
                    {
                        if (!this.existingEntities.Contains(i))
                        {
                            return i;
                        }
                    }

                    throw new ApplicationException("The maximum number of entities has been reached.");
                }
            }
        }

        #endregion
    }
}