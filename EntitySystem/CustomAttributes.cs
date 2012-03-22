namespace EntitySystem
{
    using System;

    /// <summary>
    /// Indicates that this property should be ignored when serializing the Entity System.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ESIgnoreData : Attribute
    {
    }
}