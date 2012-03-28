namespace EntitySystem
{
    using EntitySystem.Component;

    /// <summary>
    /// An Entity related event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="entity">The entity.</param>
    public delegate void EntityEvent(object sender, Entity entity);

    /// <summary>
    /// A component related event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="component">The component.</param>
    public delegate void EntityComponentEvent(object sender, Entity entity, IComponent component);
}