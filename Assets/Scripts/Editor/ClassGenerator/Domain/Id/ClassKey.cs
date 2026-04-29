using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Editor.ClassGenerator
{
    /// <summary>
    /// クラスを一意に識別できるKey
    /// </summary>
    internal sealed record ClassKey
    {
        public string Id { get; init; }
        internal ComponentRoleType ComponentRoleType { get; init; }

        public ClassKey(string id, ComponentRoleType componentRoleType)
        {
            Id = id;
            ComponentRoleType = componentRoleType;
        }

        public bool Equals(ClassKey other)
        {
            if (other == null)
            {
                return false;
            }
            return Id == other.Id &&  ComponentRoleType == other.ComponentRoleType;;
        }
    }
}