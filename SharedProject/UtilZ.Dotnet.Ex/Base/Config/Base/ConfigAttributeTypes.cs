using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    internal class ConfigAttributeTypes
    {
        public Type RootAttributeType { get; private set; } = typeof(ConfigRootAttribute);
        public Type IgnoreAttributeType { get; private set; } = typeof(ConfigIgnoreAttribute);
        public Type CollectionAttributeType { get; private set; } = typeof(ConfigCollectionAttribute);
        public Type ObjectAttributeType { get; private set; } = typeof(ConfigObjectAttribute);
        public Type ItemAttributeType { get; private set; } = typeof(ConfigItemAttribute);
        public Type CustomerAttributeType { get; private set; } = typeof(ConfigCustomerAttribute);
        public Type ConfigCommentAttributeType { get; private set; } = typeof(ConfigCommentAttribute);

        public ConfigAttributeTypes()
        {

        }
    }
}
