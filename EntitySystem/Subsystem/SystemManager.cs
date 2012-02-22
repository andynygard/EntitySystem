namespace EntitySystem.Subsystem
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Manages a set of ISystem instances in the Entity System.
    /// </summary>
    public class SystemManager
    {
        /// <summary>
        /// The systems being processed by this manager. Key = Type, Value = ISystem.
        /// </summary>
        private OrderedDictionary systems;

        /// <summary>
        /// Initializes a new instance of the SystemManager class.
        /// </summary>
        public SystemManager()
        {
            this.systems = new OrderedDictionary();
        }

        /// <summary>
        /// Perform processing for all managed systems.
        /// <para />
        /// Note: Systems are processed in the same order in which they are added.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public void ProcessSystems(int delta)
        {
            foreach (ISystem system in this.systems.Values)
            {
                if (system.Enabled)
                {
                    system.Process(delta);
                }
            }
        }

        /// <summary>
        /// Add a system.
        /// </summary>
        /// <param name="system">The system to add.</param>
        public void AddSystem(ISystem system)
        {
            this.systems.Add(system.GetType(), system);
        }

        /// <summary>
        /// Remove a system.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        public void RemoveSystem(ISystem system)
        {
            this.systems.Remove(system.GetType());
        }

        /// <summary>
        /// Get the system of the given type.
        /// </summary>
        /// <param name="systemType">The system type.</param>
        /// <returns>The ISystem instance; null if there is no managed system of this type.</returns>
        public ISystem GetSystem(Type systemType)
        {
            if (this.systems.Contains(systemType))
            {
                return (ISystem)this.systems[systemType];
            }
            else
            {
                return null;
            }
        }
    }
}