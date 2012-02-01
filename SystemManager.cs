namespace EntitySystem
{
    using System.Collections.Generic;

    /// <summary>
    /// Manages a set of ISystem instances in the Entity System.
    /// </summary>
    public class SystemManager
    {
        /// <summary>
        /// The systems being processed by this manager.
        /// </summary>
        private HashSet<ISystem> systems;

        /// <summary>
        /// Initializes a new instance of the SystemManager class.
        /// </summary>
        public SystemManager()
        {
            this.systems = new HashSet<ISystem>();
        }

        /// <summary>
        /// Perform processing for all managed systems.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public void ProcessSystems(int delta)
        {
            foreach (ISystem system in this.systems)
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
            this.systems.Add(system);
        }

        /// <summary>
        /// Remove a system.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        /// <returns>True if the system was removed.</returns>
        public bool RemoveSystem(ISystem system)
        {
            return this.systems.Remove(system);
        }

        /// <summary>
        /// Determine whether the given system is managed by this SystemManager instance.
        /// </summary>
        /// <param name="system">The system.</param>
        /// <returns>True if the system is managed by this SystemManager instance.</returns>
        public bool ManagesSystem(ISystem system)
        {
            return this.systems.Contains(system);
        }
    }
}