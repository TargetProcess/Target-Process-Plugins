using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Mercurial.XmlSerializationTypes;

namespace Mercurial
{
    /// <summary>
    /// This class implements a basic XML-based changeset parser, that parses changeset information
    /// as reported by the Mercurial command line client, in XML format.
    /// </summary>
    public static class ChangesetXmlParser
    {
        /// <summary>
        /// Parse the given XML lazily and return a collection of <see cref="Changeset"/>
        /// objects for the information contained in it, in the order the changesets
        /// appear in the xml.
        /// </summary>
        /// <param name="xml">
        /// The XML to parse.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Changeset"/> objects.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <para>An unknown path action character was detected in the log output.</para>
        /// <para>- or -</para>
        /// <para>The XML content was not legal according to the expected format.</para>
        /// </exception>
        public static IEnumerable<Changeset> LazyParse(string xml)
        {
            if (StringEx.IsNullOrWhiteSpace(xml))
                yield break;

            var serializer = new XmlSerializer(typeof(LogEntryNode));
            foreach (string entryXml in LazyExtractChangesetXmlPieces(xml))
            {
                var entry = (LogEntryNode)serializer.Deserialize(new StringReader(entryXml));
                var changeset = new Changeset
                {
                    Timestamp = entry.Timestamp,
                    AuthorName = (entry.Author ?? new LogEntryAuthorNode()).Name,
                    AuthorEmailAddress = (entry.Author ?? new LogEntryAuthorNode()).Email,
                    CommitMessage = entry.CommitMessage ?? string.Empty,
                    Branch = entry.Branch ?? "default",
                    Hash = entry.Hash,
                    RevisionNumber = entry.Revision,
                    Revision = RevSpec.Single(entry.Hash),
                    Tags = entry.Tags.Select(t => t.Name).ToArray(),
                };

                switch (entry.Parents.Count)
                {
                    case 2:
                        changeset.RightParentHash = entry.Parents[1].Hash;
                        changeset.RightParentRevision = entry.Parents[1].Revision;
                        goto case 1;

                    case 1:
                        changeset.LeftParentHash = entry.Parents[0].Hash;
                        changeset.LeftParentRevision = entry.Parents[0].Revision;
                        break;

                    case 0:
                        changeset.LeftParentRevision = changeset.RevisionNumber - 1;
                        break;
                }

                foreach (LogEntryPathNode action in entry.PathActions)
                {
                    var pathAction = new ChangesetPathAction
                    {
                        Path = action.Path,
                    };
                    switch (action.Action)
                    {
                        case "M":
                            pathAction.Action = ChangesetPathActionType.Modify;
                            break;

                        case "A":
                            pathAction.Action = ChangesetPathActionType.Add;
                            LogEntryCopyNode copySource = entry.Copies.Where(c => c.Destination == action.Path).FirstOrDefault();
                            if (copySource != null)
                                pathAction.Source = copySource.Source;
                            break;

                        case "R":
                            pathAction.Action = ChangesetPathActionType.Remove;
                            break;

                        default:
                            throw new InvalidOperationException("Unknown path action: " + action.Action);
                    }
                    changeset.PathActions.Add(pathAction);
                }

                yield return changeset;
            }
        }

        /// <summary>
        /// Lazily extract all the &lt;logentry&gt;...&lt;/logentry&gt; xml pieces
        /// from the changeset xml log.
        /// </summary>
        /// <param name="xml">
        /// The xml to extract the log entry xml pieces from.
        /// </param>
        /// <returns>
        /// A collection of strings containing xml, one string per
        /// &lt;logentry&gt;...&lt;/logentry&gt;.
        /// </returns>
        private static IEnumerable<string> LazyExtractChangesetXmlPieces(string xml)
        {
            if (StringEx.IsNullOrWhiteSpace(xml))
                yield break;

            using (var reader = new StringReader(xml))
            {
                string line = reader.ReadLine();
                if (line == null)
                    throw new InvalidOperationException("Invalid XML content");
                if (line.StartsWith("<?xml "))
                    line = reader.ReadLine();
                if (line != "<log>")
                    throw new InvalidOperationException("Invalid XML content");
                var entryXml = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "</log>")
                        yield break;
                    entryXml.AppendLine(line);
                    if (line == "</logentry>")
                    {
                        yield return entryXml.ToString();
                        entryXml.Length = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Parse the given XML and return <see cref="Changeset"/> objects for the information
        /// contained in it, ordered in descending order by the revision numbers (ie.
        /// latest changeset first.)
        /// </summary>
        /// <param name="xml">
        /// The XML to parse.
        /// </param>
        /// <returns>
        /// An array of <see cref="Changeset"/> objects, or an empty array if no
        /// changeset is present (empty string most likely.)
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// An unknown path action character was detected in the log output.
        /// </exception>
        public static Changeset[] Parse(string xml)
        {
            return (from changeset in LazyParse(xml)
                    orderby changeset.RevisionNumber descending
                    select changeset).ToArray();
        }
    }
}