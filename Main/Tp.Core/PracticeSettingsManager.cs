using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tp.Core
{
    public enum PlanningUnit
    {
        [Description("h")] Hour,
        [Description("pt")] Point,
    }

    public enum PracticeSettings
    {
        None = 0,
        IsStoryEffortEqualsSumTasksEffort = 1,
        IsCloseAssignableIfZeroTimeRemaining = 2,
        PlanningUnit = 3,
        IsTimeDescriptionRequired = 4,
        IsTimeDescriptionEnabled = 5,
        IsCreateRequestAutomatically = 6,
        RequestProjectID = 7,
        IsActorsSelectionAvailable = 8,
        AreAllTasksClosedWhenUserStoryIsClosed = 9
    }

    public class PracticeSetting
    {
        public PracticeSetting()
        {
        }

        public PracticeSetting(PracticeSettings _name, object _value)
        {
            Name = _name;
            Value = _value;
        }

        public PracticeSettings Name { get; set; }

        public object Value { get; set; }
    }

    public static class PracticeSettingsConverter
    {
        private static readonly XmlSerializer CachedXmlSerializer = new XmlSerializer(typeof(List<PracticeSetting>));

        public static IEnumerable<PracticeSetting> Parse(string s)
        {
            if (s.IsNullOrEmpty())
            {
                return new List<PracticeSetting>();
            }
            using (var reader = new StringReader(s))
            {
                return (List<PracticeSetting>) CachedXmlSerializer.Deserialize(reader);
            }
        }

        public static string ConvertToString(IEnumerable<PracticeSetting> practiceSettings)
        {
            var buf = new StringBuilder();
            using (TextWriter writer = new StringWriter(buf))
            {
                CachedXmlSerializer.Serialize(writer, practiceSettings.ToList());
            }
            return buf.ToString();
        }
    }
}
