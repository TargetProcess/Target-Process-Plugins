namespace Tp.Core.Configuration
{
    internal interface IConfigurationReader
    {
        /// <summary>
        /// Get the configuration that represents the content of the configuration file
        /// </summary>
        IConfiguration Config { get; }
    }
}
