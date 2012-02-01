namespace EntitySystem
{
    /// <summary>
    /// Represents the game world.
    /// </summary>
    public class World
    {
        /// <summary>
        /// Initializes a new instance of the World class.
        /// </summary>
        public World()
        {
            this.EntityManager = new EntityManager();
            this.UpdateSystemManager = new SystemManager();
            this.DrawSystemManager = new SystemManager();
            this.GameStep = new GameStepState();
        }

        /// <summary>
        /// Gets the world's EntityManager.
        /// </summary>
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Gets the world's SystemManager for update systems.
        /// </summary>
        public SystemManager UpdateSystemManager { get; private set; }

        /// <summary>
        /// Gets the world's SystemManager for draw systems.
        /// </summary>
        public SystemManager DrawSystemManager { get; private set; }

        /// <summary>
        /// Gets the world's game step state.
        /// </summary>
        public GameStepState GameStep { get; private set; }

        /// <summary>
        /// Update the world state by one game step.
        /// </summary>
        public void Step()
        {
            // Step forward the game state
            this.GameStep.Step();

            // Process the update-related systems
            this.UpdateSystemManager.ProcessSystems();
        }

        /// <summary>
        /// Draw the world state.
        /// </summary>
        public void Draw()
        {
            // Process the draw-related systems
            this.DrawSystemManager.ProcessSystems();
        }
    }
}