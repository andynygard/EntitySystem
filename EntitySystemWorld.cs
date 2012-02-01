namespace EntitySystem
{
    /// <summary>
    /// Represents the game world.
    /// </summary>
    public class EntitySystemWorld
    {
        /// <summary>
        /// Initializes a new instance of the EntitySystemWorld class.
        /// </summary>
        public EntitySystemWorld()
        {
            this.EntityManager = new EntityManager();
            this.UpdateSystemManager = new SystemManager();
            this.DrawSystemManager = new SystemManager();
        }

        /// <summary>
        /// Gets the world's entity manager.
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Gets the world's manager for update systems.
        /// </summary>
        public SystemManager UpdateSystemManager { get; private set; }

        /// <summary>
        /// Gets the world's manager for draw systems.
        /// </summary>
        public SystemManager DrawSystemManager { get; private set; }

        /// <summary>
        /// Update the world state.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last step.</param>
        public void Step(int delta)
        {
            // Process the update-related systems
            this.UpdateSystemManager.ProcessSystems(delta);
        }

        /// <summary>
        /// Draw the world state.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last draw.</param>
        public void Draw(int delta)
        {
            // Process the draw-related systems
            this.DrawSystemManager.ProcessSystems(delta);
        }
    }
}