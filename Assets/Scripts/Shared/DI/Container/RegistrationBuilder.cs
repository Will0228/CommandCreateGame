using System;
using System.Collections.Generic;

namespace Shared.DI
{
    public sealed class RegistrationBuilder
    {
        private readonly Lifetime _lifetime;
        private readonly Type _implementationType;
        private readonly Dictionary<Type, Value> _registeredTypes = new();
        private readonly RegistrationRegistry _registrationRegistry;

        public RegistrationBuilder(Lifetime lifetime, Type implementationType, Dictionary<Type, Value> registeredTypes, RegistrationRegistry registrationRegistry)
        {
            _lifetime = lifetime;
            _implementationType = implementationType;
            _registeredTypes = registeredTypes;
            _registrationRegistry = registrationRegistry;
        }

        /// <summary>
        /// 実装しているすべてのインターフェースとして登録する
        /// </summary>
        public RegistrationBuilder AsImplementedInterfaces()
        {
            var interfaces = _implementationType.GetInterfaces();
            foreach (var type in interfaces)
            {
                if (_registeredTypes.TryAdd(type, new Value(_lifetime, _implementationType)))
                {
                    continue;
                }
                throw new InvalidOperationException($"すでに{type}が登録されています");
            }
            _registrationRegistry.Register(_implementationType,  _lifetime);
            return this;
        }

        public RegistrationBuilder As<TInterface>()
        {
            if (_registeredTypes.TryAdd(typeof(TInterface), new Value(_lifetime, _implementationType)))
            {
                return this;
            }
            throw new InvalidOperationException($"すでに{typeof(TInterface)}が登録されています");
        }
    }
}