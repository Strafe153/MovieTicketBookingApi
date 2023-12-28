using Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text.Json;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;

namespace Core.Shared.ContractResolvers;

public class IgnoreIdContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName == JsonNamingPolicy.CamelCase.ConvertName(nameof(User.Id)))
        {
            property.ShouldSerialize = instance => false;
        }

        return property;
    }
}
