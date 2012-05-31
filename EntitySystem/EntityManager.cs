namespace EntitySystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EntitySystem.Component;

    /// <summary>
    /// Maintains a reference to the components in the Entity System, providing access to these.
    /// <para />
    /// This class is not thread safe.
    /// </summary>
    public class EntityManager :
        IEnumerable<KeyValuePair<Entity, IComponent>>
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
        private Dictionary<Type, Dictionary<Entity, IComponent>> componentsByType;

        /// <summary>
        /// A list of the existing entities such that a new unique entity id can quickly be created.
        /// </summary>
        private HashSet<int> existingEntityIds;

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
            this.componentsByType = new Dictionary<Type, Dictionary<Entity, IComponent>>();
            this.existingEntityIds = new HashSet<int>();
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
        public event EntityComponentEvent ComponentRemoving;

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <returns>The new entity.</returns>
        public Entity CreateEntity()
        {
            Entity entity;

            lock (this.newEntityLock)
            {
                // Generate the entity
                entity = this.GenerateNewEntity();

                // Retain the id to prevent it from being used again
                this.existingEntityIds.Add(entity.Id);
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
        public void RemoveEntity(Entity entity)
        {
            // Get the list of components with this entity
            var componentsToRemove = new List<Dictionary<Entity, IComponent>>();

            lock (this.newEntityLock)
            {
                // Remove this entity from the internal list
                this.existingEntityIds.Remove(entity.Id);

                // Fire removal event for each component
                foreach (Dictionary<Entity, IComponent> componentsByEntity in this.componentsByType.Values)
                {
                    if (componentsByEntity.ContainsKey(entity))
                    {
                        // Add the component to the removal list
                        componentsToRemove.Add(componentsByEntity);

                        // Fire event
                        if (this.ComponentRemoving != null)
                        {
                            this.ComponentRemoving(this, entity, componentsByEntity[entity]);
                        }
                    }
                }

                // Now actually remove the components
                foreach (Dictionary<Entity, IComponent> componentsByEntity in componentsToRemove)
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
        public void AddComponent(Entity entity, IComponent component)
        {
            // Get the components dictionary for this component type
            Dictionary<Entity, IComponent> componentsByEntity;
            if (!this.componentsByType.TryGetValue(component.GetType(), out componentsByEntity))
            {
                componentsByEntity = new Dictionary<Entity, IComponent>();
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
        public void RemoveComponent(Entity entity, IComponent component)
        {
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(component.GetType(), out componentsByEntity))
            {
                // Remove the component
                componentsByEntity.Remove(entity);

                // Fire event
                if (this.ComponentRemoving != null)
                {
                    this.ComponentRemoving(this, entity, component);
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
        public IComponent GetComponent(Entity entity, Type componentType)
        {
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(componentType, out componentsByEntity))
            {
                IComponent component;
                if (componentsByEntity.TryGetValue(entity, out component))
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
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(componentType, out componentsByEntity))
            {
                return componentsByEntity.Values;
            }

            return new IComponent[0];
        }

        /// <summary>
        /// Gets all the entities that have a component of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>A collection of entities.</returns>
        public IEnumerable<Entity> GetEntitiesWithComponent(Type componentType)
        {
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(componentType, out componentsByEntity))
            {
                return componentsByEntity.Keys;
            }

            return new Entity[0];
        }

        /// <summary>
        /// Gets the first component of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>The first component; Null if no entities exist with the given component type.</returns>
        public IComponent GetFirstComponent(Type componentType)
        {
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(componentType, out componentsByEntity))
            {
                if (componentsByEntity.Count > 0)
                {
                    return componentsByEntity.First().Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first entity with a component of the given type.
        /// </summary>
        /// <param name="componentType">The type of component.</param>
        /// <returns>The first entity; Null if no entities exist with the given component type.</returns>
        public Entity GetFirstEntityWithComponent(Type componentType)
        {
            Dictionary<Entity, IComponent> componentsByEntity;
            if (this.componentsByType.TryGetValue(componentType, out componentsByEntity))
            {
                if (componentsByEntity.Count > 0)
                {
                    return componentsByEntity.First().Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Get an enumerator that iterates through all entity-components that are in the entity system.
        /// <para />
        /// Note: This is not ordered by entity.
        /// </summary>
        /// <returns>An enumerator object.</returns>
        public IEnumerator<KeyValuePair<Entity, IComponent>> GetEnumerator()
        {
            return new EntityComponentEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator that iterates through all entity-components that are in the entity system.
        /// <para />
        /// Note: This is not ordered by entity.
        /// </summary>
        /// <returns>An enumerator object.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate a new entity id that isn't being used.
        /// </summary>
        /// <returns>An entity id.</returns>
        private Entity GenerateNewEntity()
        {
            lock (this.newEntityLock)
            {
                if (this.lowestUnnasignedEntity < int.MaxValue)
                {
                    return new Entity(this.lowestUnnasignedEntity++);
                }
                else
                {
                    // Take the first free entity
                    for (int i = MinEntityId; i < int.MaxValue; i++)
                    {
                        if (!this.existingEntityIds.Contains(i))
                        {
                            return new Entity(i);
                        }
                    }

                    throw new ApplicationException("The maximum number of entities has been reached.");
                }
            }
        }

        #endregion

        /// <summary>
        /// Iterates over all entity-component pairs in an EntityManager.
        /// </summary>
        private class EntityComponentEnumerator :
            IEnumerator<KeyValuePair<Entity, IComponent>>
        {
            /// <summary>
            /// The enumerator for the top-level dictionary in the entity manager for which the enumerator of each
            /// component dictionary (set to currentEnumerator) is iterated over.
            /// </summary>
            private IEnumerator<Dictionary<Entity, IComponent>> topEnumerator;

            /// <summary>
            /// The enumerator for the dictionary at the current element of topEnumerator.
            /// </summary>
            private IEnumerator<KeyValuePair<Entity, IComponent>> currentEnumerator;

            /// <summary>
            /// Initializes a new instance of the EntityComponentEnumerator class.
            /// </summary>
            /// <param name="entityManager">The entity manager to iterate over.</param>
            public EntityComponentEnumerator(EntityManager entityManager)
            {
                this.topEnumerator = entityManager.componentsByType.Values.GetEnumerator();
                this.Reset();
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public KeyValuePair<Entity, IComponent> Current
            {
                get
                {
                    if (this.currentEnumerator == null)
                    {
                        throw new InvalidOperationException(
                            "Enumerator is before the first or after the last element in the collection.");
                    }

                    return this.currentEnumerator.Current;
                }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element.</returns>
            public bool MoveNext()
            {
                // If the current enumerator is null then this is either the first step or we reached the end of the
                // *previous* currentEnumerator
                if (this.currentEnumerator == null)
                {
                    // Move topEnumerator to the next element
                    if (!this.topEnumerator.MoveNext())
                    {
                        // There are no more elements
                        return false;
                    }

                    // Set currentEnumerator to topEnumerator's current element's enumerator
                    this.currentEnumerator = this.topEnumerator.Current.GetEnumerator();
                }

                // Move to the next element in the current enumerator
                if (this.currentEnumerator.MoveNext())
                {
                    return true;
                }
                else
                {
                    // There are no more elements in the current enumerator, so recursively call this method to move to
                    // the next element in the top enumerator
                    this.currentEnumerator = null;
                    return this.MoveNext();
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                this.topEnumerator.Reset();
                this.currentEnumerator = null;
            }

            /// <summary>
            /// Dispose the enumerator.
            /// </summary>
            public void Dispose()
            {
                this.topEnumerator.Dispose();

                if (this.currentEnumerator != null)
                {
                    this.currentEnumerator.Dispose();
                }
            }
        }
    }
}