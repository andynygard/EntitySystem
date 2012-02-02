namespace EntitySystem.Subsystem
{
    using EntitySystem.Entity;

    /// <summary>
    /// A base abstract implementation of the ISystem interface.
    /// </summary>
    public abstract class BaseSystem : ISystem
    {
        /// <summary>
        /// Initializes a new instance of the BaseSystem class.
        /// </summary>
        /// <param name="entityManager">The EntityManager for the world that this system belongs to.</param>
        public BaseSystem(EntityManager entityManager)
        {
            this.Enabled = true;
            this.EntityManager = entityManager;
        }

        /// <summary>
        /// Gets a value indicating whether this system should be processed.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets the EntityManager for the world that this system belongs to.
        /// </summary>
        protected EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Perform the system's processing.
        /// </summary>
        /// <param name="delta">The number of milliseconds since the last processing occurred.</param>
        public abstract void Process(int delta);
    }
}