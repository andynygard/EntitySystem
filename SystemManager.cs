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
        public void ProcessSystems()
        {
            foreach (ISystem system in this.systems)
            {
                if (system.Enabled)
                {
                    system.Process();
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
        public void RemoveSystem(ISystem system)
        {
            if (this.systems.Contains(system))
            {
                this.systems.Remove(system);
            }
        }
    }
}