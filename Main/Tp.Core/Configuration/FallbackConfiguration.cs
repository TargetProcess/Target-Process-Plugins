using System.IO;

namespace Tp.Core.Configuration
{
	public class FallbackConfiguration : IConfigurationReader
	{
		private readonly IConfigurationReader _reader;

		public FallbackConfiguration(string fileName)
		{
			var defaultPath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			var filePath = Path.Combine(defaultPath, fileName);

			if (System.IO.File.Exists(filePath))
				_reader = new CustomConfigurationFileReader(filePath);
			else
				_reader = (IConfigurationReader) DefaultConfiguration.Instance;
		}

		public IConfiguration Config
		{
			get { return _reader.Config; }
		}

		public static IConfiguration File(string fileName)
		{
			return new FallbackConfiguration(fileName).Config;
		}
	}
}
