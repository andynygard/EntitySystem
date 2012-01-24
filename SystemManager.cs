namespace EntitySystem
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages all ISystem instances in the Entity System.
    /// </summary>
    public class SystemManager
    {
        /// <summary>
        /// The systems responsible for updating the game state.
        /// </summary>
        private HashSet<ISystem> updateSystems;

        /// <summary>
        /// The systems responsible for drawing.
        /// </summary>
        private HashSet<ISystem> drawSystems;

        private DateTime lastUpdateStep;

        /// <summary>
        /// Initializes a new instance of the SystemManager class.
        /// </summary>
        public SystemManager()
        {
            this.updateSystems = new HashSet<ISystem>();
            this.drawSystems = new HashSet<ISystem>();
        }

        public void ProcessSystems(ExecutionType executionType)
        {
            if (executionType == ExecutionType.Update)
            {
                foreach (ISystem system in this.updateSystems)
                {
                    system.Process();
                }
            }
            else
            {
                foreach (ISystem system in this.drawSystems)
                {
                    system.Process();
                }
            }
        }

        /// <summary>
        /// Add a system.
        /// </summary>
        /// <param name="system">The system to add.</param>
        /// <param name="executionType">The execution type.</param>
        public void AddSystem(ISystem system, ExecutionType executionType)
        {
            if (executionType == ExecutionType.Update)
            {
                this.updateSystems.Add(system);
            }
            else
            {
                this.drawSystems.Add(system);
            }
        }

        /// <summary>
        /// Remove a system.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        public void RemoveSystem(ISystem system)
        {
            if (this.updateSystems.Contains(system))
            {
                this.updateSystems.Remove(system);
            }

            if (this.drawSystems.Contains(system))
            {
                this.drawSystems.Remove(system);
            }
        }
    }
}
