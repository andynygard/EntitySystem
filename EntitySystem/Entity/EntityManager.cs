namespace EntitySystem.Entity
{
    using System;
    using System.Collections.Generic;
    using EntitySystem.Component;

    /// <summary>
    /// Maintains a reference to the components in the Entity System, providing access to these.
    /// <para />
    /// This class is not thread safe.
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
        private Dictionary<Type, Dictionary<int, IComponent>> componentsByType;

        /// <summary>
        /// A list of the existing entities such that a new unique entity id can quickly be created.
        /// </summary>
        private HashSet<int> existingEntities;

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
            this.componentsByType = new Dictionary<Type, Dictionary<int, IComponent>>();
            this.existingEntities = new HashSet<int>();
            this.lowestUnnasignedEntity = MinEntityId;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when an entity is added to the EntityManager.
        /// </summary>
        public event EntityEvent EntityAdded;

        /// <summary>
        /// Fires when an entity is removed from the EntityManager.
        /// </summary>
        public event EntityEvent EntityRemoved;

        /// <summary>
        /// Fires when a component is added to the EntityManager.
        /// </summary>
        public event EntityComponentEvent ComponentAdded;

        /// <summary>
        /// Fires when a component is about to be removed from the EntityManager.
        /// </summary>
        public event EntityComponentEvent PreComponentRemoved;

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <returns>The new entity.</returns>
        public int CreateEntity()
        {
            int entity;

            lock (this.newEntityLock)
            {
                // Generate the id
                entity = this.GenerateNewEntityId();

                // Retain the id to prevent it from being used again
                this.existingEntities.Add(entity);
            }

            // Fire event
            if (this.EntityAdded != null)
            {
                this.EntityAdded(this, entity);
            }

            return entity;
        }

        /// <summary>
        /// Remove all references to the given entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemoveEntity(int entity)
        {
            // Get the list of components with this entity
            var componentsToRemove = new List<Dictionary<int, IComponent>>();

            lock (this.newEntityLock)
            {
                // Remove this entity from the internal list
                this.existingEntities.Remove(entity);

                // Fire removal event for each component
                foreach (Dictionary<int, IComponent> componentsByEntity in this.componentsByType.Values)
                {
                    if (componentsByEntity.ContainsKey(entity))
                    {
                        // Add the component to the removal list
                        componentsToRemove.Add(componentsByEntity);

                        // Fire event
                        if (this.PreComponentRemoved != null)
                        {
                            this.PreComponentRemoved(this, entity, componentsByEntity[entity]);
                        }
                    }
                }

                // Now actually remove the components
                foreach (Dictionary<int, IComponent> componentsByEntity in componentsToRemove)
                {
                    componentsByEntity.Remove(entity);
                }
            }

            // Fire entity removed event
            if (this.EntityRemoved != null)
            {
                this.EntityRemoved(this, entity);
            }
        }

        /// <summary>
        /// Add a component for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="component">The component to add.</param>
        public void AddComponent(int entity, IComponent component)
        {
            // Get the components dictionary for this component type
            Dictionary<int, IComponent> componentsByEntity;
            if (this.componentsByType.ContainsKey(component.GetType()))
            {
                componentsByEntity = this.componentsByType[component.GetType()];
            }
            else
            {
                componentsByEntity = new Dictionary<int, IComponent>();
                this.componentsByType.Add(component.GetType(), componentsByEntity);
            }

            // Add the component
            componentsByEntity.Add(entity, component);

            // Fire event
            if (this.ComponentAdded != null)
            {
                this.ComponentAdded(this, entity, component);
            }
        }

        /// <summary>
        /// Remove the component for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="component">The component to remove.</param>
        public void RemoveComponent(int entity, IComponent component)
        {
            if (this.componentsByType.ContainsKey(component.GetType()))
            {
                // Remove the component
                Dictionary<int, IComponent> componentsByEntity = this.componentsByType[component.GetType()];
                componentsByEntity.Remove(entity);

                // Fire event
                if (this.PreComponentRemoved != null)
                {
                    this.PreComponentRemoved(this, entity, component);
                }
            }
            else
            {
                throw new ApplicationException("The component " + component + " does not exist for any entities.");
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
            if (this.componentsByType.ContainsKey(componentType))
            {
                Dictionary<int, IComponent> componentsByEntity = this.componentsByType[componentType];
                if (componentsByEntity.ContainsKey(entity))
                {
                    return componentsByEntity[entity];
                }
            }

            return null;
        }

        /// <summary>
        /// Get all of the components of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>A collection of components.</returns>
        public IEnumerable<IComponent> GetComponents(Type componentType)
        {
            if (this.componentsByType.ContainsKey(componentType))
            {
                return this.componentsByType[componentType].Values;
            }
            else
            {
                return new IComponent[0];
            }
        }

        /// <summary>
        /// Gets all the entities that have a component of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>A collection of entities.</returns>
        public IEnumerable<int> GetEntitiesWithComponent(Type componentType)
        {
            if (this.componentsByType.ContainsKey(componentType))
            {
                return this.componentsByType[componentType].Keys;
            }
            else
            {
                return new int[0];
            }
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