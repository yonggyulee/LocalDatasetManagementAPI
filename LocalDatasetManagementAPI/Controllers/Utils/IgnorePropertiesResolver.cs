using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LocalDatasetManagementAPI.Controllers.Utils
{
    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private HashSet<string> _propsToIgnores;

        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            _propsToIgnores = new HashSet<string>(propNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (_propsToIgnores.Contains(property.PropertyName))
            {
                property.ShouldSerialize = (x) => { return false; };
            }

            return property;
        }
    }
}
