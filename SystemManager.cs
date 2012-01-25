namespace EntitySystem
{
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

        /// <summary>
        /// Initializes a new instance of the SystemManager class.
        /// </summary>
        public SystemManager()
        {
            this.updateSystems = new HashSet<ISystem>();
            this.drawSystems = new HashSet<ISystem>();
        }

        /// <summary>
        /// Perform update processing for all systems that are registered for it.
        /// </summary>
        public void ProcessUpdateSystems()
        {
            foreach (ISystem system in this.updateSystems)
            {
                if (system.UpdateEnabled)
                {
                    system.ProcessUpdate();
                }
            }
        }

        /// <summary>
        /// Perform draw processing for all systems that are registered for it.
        /// </summary>
        public void ProcessDrawSystems()
        {
            foreach (ISystem system in this.drawSystems)
            {
                if (system.DrawEnabled)
                {
                    system.ProcessDraw();
                }
            }
        }

        /// <summary>
        /// Add a system for update processing.
        /// </summary>
        /// <param name="system">The system to add.</param>
        public void AddUpdateSystem(ISystem system)
        {
            this.updateSystems.Add(system);
        }

        /// <summary>
        /// Add a system for draw processing.
        /// </summary>
        /// <param name="system">The system to add.</param>
        public void AddDrawSystem(ISystem system)
        {
            this.drawSystems.Add(system);
        }

        /// <summary>
        /// Remove a system from update processing.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        public void RemoveUpdateSystem(ISystem system)
        {
            if (this.updateSystems.Contains(system))
            {
                this.updateSystems.Remove(system);
            }
        }
        /// <summary>
        /// Remove a system from draw processing.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        public void RemoveDrawSystem(ISystem system)
        {
            if (this.drawSystems.Contains(system))
            {
                this.drawSystems.Remove(system);
            }
        }
    }
}