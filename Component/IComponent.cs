namespace EntitySystem.Component
{
    /// <summary>
    /// A component in the Entity system.
    /// <para />
    /// A component represents one aspect of an entity in the game world. It may only contain data members and cannot
    /// overlap the responsibility of another component (eg. there can only be one component which maintain's an
    /// entity's x/y position).
    /// </summary>
    public interface IComponent
    {
    }
}