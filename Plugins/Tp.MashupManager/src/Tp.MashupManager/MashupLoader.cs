using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.MashupManager
{
    public class MashupLoader : IMashupLoader
    {
        private readonly Func<string, bool> _allowedMashupFilePredicate;

        public MashupLoader(Maybe<IReadOnlyCollection<string>> allowedMashupFileExtensions)
        {
            _allowedMashupFilePredicate = allowedMashupFileExtensions
                .Select<IReadOnlyCollection<string>, Func<string, bool>>(
                    extensions => file => extensions.Contains(Path.GetExtension(file)))
                .GetOrDefault(_ => true);
        }

        public Mashup Load(string mashupFolderPath, string mashupName)
        {
            if (!Directory.Exists(mashupFolderPath))
            {
                return null;
            }

            var configFiles = Directory.GetFiles(mashupFolderPath, "*.cfg");
            var mashupConfig = new MashupConfig(configFiles.SelectMany(File.ReadAllLines));

            var files = Directory
                .GetFiles(mashupFolderPath, "*", SearchOption.AllDirectories)
                .Where(f => !configFiles.Contains(f) && _allowedMashupFilePredicate(f));

            var mashupFiles = files.Select(f => new MashupFile
            {
                FileName = f.GetRelativePathTo(mashupFolderPath),
                Content = File.ReadAllText(f)
            }).ToList();

            var mashup = new Mashup(mashupFiles)
            {
                Name = mashupName,
                Placeholders = string.Join(",", mashupConfig.Placeholders),
                MashupMetaInfo = mashupConfig.MashupMetaInfo
            };

            return mashup;
        }
    }
}
