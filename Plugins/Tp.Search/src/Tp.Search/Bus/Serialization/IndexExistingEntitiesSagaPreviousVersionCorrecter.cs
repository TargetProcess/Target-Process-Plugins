using System.Text.RegularExpressions;
using System.Xml.Linq;
using Tp.Integration.Messages.SerializationPatches;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;
using Tp.Search.Bus.Workflow;

namespace Tp.Search.Bus.Serialization
{
    //asc: This class allows to handle already executing build index saga during search plugin version update where IndexExistingEntitiesSagaData was changed
    //Core idea is to build actual type instance with old id from deserialized IndexExistingEntitiesSagaData, where old id is used to complete saga
    //General use case is following:
    //	user start rebuild indexes using search plugin of version 1
    //	search plugin is stopped and updated to version 2
    //  search plugin of version 2 starts to rebuild indexes, found that version 1 started rebuild indexes, clean old sagas and starts to rebuild indexes
    class IndexExistingEntitiesSagaPreviousVersionCorrecter : IPatch
    {
        private static readonly string Sample;
        private const string OldVersionIdGroupName = "IdVal";

        private static readonly string OldVersionIdLocator = string.Format("\"&lt;Id&gt;k__BackingField\":\"(?<{0}>.*?)\"",
            OldVersionIdGroupName);

        private const string SampleIdToBindTemplate = "<Id>{0}</Id>";
        private const string SampleId = "<Id>(.*)</Id>";

        static IndexExistingEntitiesSagaPreviousVersionCorrecter()
        {
            var s = new XmlBlobSerializer();
            XDocument d = s.Serialize(new IndexExistingEntitiesSagaData());
            Sample = d.ToString();
        }

        public bool NeedToApply(string text)
        {
            return text.Contains("<Type>Tp.Search.Bus.Workflow.IndexExistingEntitiesSagaData, Tp.Search</Type>");
        }

        public string Apply(string text)
        {
            Match idLocator = Regex.Match(text, OldVersionIdLocator);
            if (!idLocator.Success)
            {
                return text;
            }
            string idValue = idLocator.Groups[OldVersionIdGroupName].Value;
            string boundSampleId = string.Format(SampleIdToBindTemplate, idValue);
            return Regex.Replace(Sample, SampleId, boundSampleId);
        }
    }
}
