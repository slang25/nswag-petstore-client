using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NSwag.PetStore.Client
{
    internal static class PetStoreSerializerSettings
    {
        public static JsonSerializerSettings TransformSettings(JsonSerializerSettings settings)
        {
            settings.DefaultValueHandling = DefaultValueHandling.Populate;
            settings.ContractResolver = new NullToEmptyListResolver();
            return settings;
        }
    }

    internal sealed class NullToEmptyListResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var propType = property.PropertyType;
            if (propType.IsGenericType && 
                propType.GetGenericTypeDefinition() == typeof(ICollection<>))
                property.NullValueHandling = NullValueHandling.Include;
            return property;
        }
        
        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            var provider = base.CreateMemberValueProvider(member);

            if (member.MemberType == MemberTypes.Property)
            {
                var propType = ((PropertyInfo)member).PropertyType;
                if (propType.IsGenericType && 
                    propType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return new EmptyListValueProvider(provider, propType);
                }
            }

            return provider;
        }

        class EmptyListValueProvider : IValueProvider
        {
            readonly IValueProvider innerProvider;
            readonly object defaultValue;

            public EmptyListValueProvider(IValueProvider innerProvider, Type listType)
            {
                this.innerProvider = innerProvider;
                defaultValue = Array.CreateInstance(listType.GetGenericArguments()[0], 0);
            }

            public void SetValue(object target, object value) => innerProvider.SetValue(target, value ?? defaultValue);
            public object GetValue(object target) => innerProvider.GetValue(target) ?? defaultValue;
        }
    }
}