using System;
using Shared.DependencyContext;
using UnityEngine;

namespace Shared.DI
{
    [Serializable]
    public class ParentDependencyContextReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        public string TypeName;

        [NonSerialized]
        public DependencyContextBase Object;

        public Type Type { get; private set; }

        ParentDependencyContextReference(Type type)
        {
            Type = type;
            TypeName = type.FullName;
            Object = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            TypeName = Type?.FullName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(TypeName))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type = assembly.GetType(TypeName);
                    if (Type != null)
                        break;
                }
            }
        }

        public static ParentDependencyContextReference Create<T>() where T : DependencyContextBase
        {
            return new ParentDependencyContextReference(typeof(T));
        }
    }
}

