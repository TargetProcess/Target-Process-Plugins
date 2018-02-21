using System;

namespace Tp.Core.Features
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ClientFeatureAttribute : OverrideableByMashupAttribute
    {
        public string ClientFeatureName { get; set; }

        public ClientFeatureAttribute(string clientFeatureName)
        {
            if (string.IsNullOrEmpty(clientFeatureName))
            {
                throw new ArgumentNullException(nameof(clientFeatureName));
            }

            ClientFeatureName = clientFeatureName;
        }
    }
}
