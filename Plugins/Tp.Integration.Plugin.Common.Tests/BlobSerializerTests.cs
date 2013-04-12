using System.Xml.Linq;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests
{
	[TestFixture]
	public class BlobSerializerTests
	{
		[Test]
		public void DeserializeAfterAdding60CustomFields()
		{
			const string s = @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Integration.Common.ProjectDTO, Tp.Integration.Messages</Type>
  <Version>2</Version>
  <string>{""__type"":""ProjectDTO:#Tp.Integration.Common"",""&lt;ID&gt;k__BackingField"":null,""&lt;Abbreviation&gt;k__BackingField"":""TPW"",""&lt;CompanyID&gt;k__BackingField"":null,""&lt;CreateDate&gt;k__BackingField"":""\/Date(1360821600000+0300)\/"",""&lt;CustomField10&gt;k__BackingField"":null,""&lt;CustomField11&gt;k__BackingField"":null,""&lt;CustomField12&gt;k__BackingField"":null,""&lt;CustomField13&gt;k__BackingField"":null,""&lt;CustomField14&gt;k__BackingField"":null,""&lt;CustomField15&gt;k__BackingField"":null,""&lt;CustomField1&gt;k__BackingField"":null,""&lt;CustomField2&gt;k__BackingField"":null,""&lt;CustomField3&gt;k__BackingField"":null,""&lt;CustomField4&gt;k__BackingField"":null,""&lt;CustomField5&gt;k__BackingField"":null,""&lt;CustomField6&gt;k__BackingField"":null,""&lt;CustomField7&gt;k__BackingField"":null,""&lt;CustomField8&gt;k__BackingField"":null,""&lt;CustomField9&gt;k__BackingField"":null,""&lt;DeleteDate&gt;k__BackingField"":null,""&lt;Description&gt;k__BackingField"":null,""&lt;EndDate&gt;k__BackingField"":null,""&lt;EntityTypeName&gt;k__BackingField"":""Tp.BusinessObjects.Project"",""&lt;InboundMailAutoCheck&gt;k__BackingField"":false,""&lt;InboundMailAutomaticalEmailCheckTime&gt;k__BackingField"":0,""&lt;InboundMailCreateRequests&gt;k__BackingField"":false,""&lt;InboundMailLogin&gt;k__BackingField"":null,""&lt;InboundMailPassword&gt;k__BackingField"":null,""&lt;InboundMailPort&gt;k__BackingField"":110,""&lt;InboundMailProtocol&gt;k__BackingField"":null,""&lt;InboundMailReplyAddress&gt;k__BackingField"":null,""&lt;InboundMailServer&gt;k__BackingField"":null,""&lt;InboundMailUseSSL&gt;k__BackingField"":false,""&lt;IsActive&gt;k__BackingField"":true,""&lt;IsInboundMailEnabled&gt;k__BackingField"":false,""&lt;IsProduct&gt;k__BackingField"":false,""&lt;LastCommentDate&gt;k__BackingField"":null,""&lt;LastCommentUserID&gt;k__BackingField"":null,""&lt;LastEditorID&gt;k__BackingField"":1,""&lt;ModifyDate&gt;k__BackingField"":""\/Date(1365165112000+0300)\/"",""&lt;Name&gt;k__BackingField"":""Tau Product Web Site - Scrum #1"",""&lt;NumericPriority&gt;k__BackingField"":2,""&lt;OwnerID&gt;k__BackingField"":2,""&lt;ParentProjectID&gt;k__BackingField"":null,""&lt;ParentProjectName&gt;k__BackingField"":null,""&lt;ProcessID&gt;k__BackingField"":3,""&lt;ProcessName&gt;k__BackingField"":""Scrum"",""&lt;ProgramOfProjectID&gt;k__BackingField"":1,""&lt;ProgramOfProjectName&gt;k__BackingField"":""tauLine #1"",""&lt;ProjectID&gt;k__BackingField"":13,""&lt;SCConnectionString&gt;k__BackingField"":null,""&lt;SCPassword&gt;k__BackingField"":null,""&lt;SCStartingRevision&gt;k__BackingField"":null,""&lt;SCUser&gt;k__BackingField"":null,""&lt;SourceControlType&gt;k__BackingField"":0,""&lt;StartDate&gt;k__BackingField"":""\/Date(1360821600000+0300)\/""}</string>
</Value>";

			var project = BlobSerializer.Deserialize(XDocument.Parse(s), "Tp.Integration.Common.ProjectDTO, Tp.Integration.Messages") as ProjectDTO;
			project.ID.Should(Be.EqualTo(13));
		}

		[Test]
		public void DeserializeAfterAddingDataMember()
		{
			var s = @"<Value xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>Tp.Integration.Common.UserDTO, Tp.Integration.Messages</Type>
  <Version>2</Version>
  <string>{""__type"":""UserDTO:#Tp.Integration.Common"",""&lt;ID&gt;k__BackingField"":null,""&lt;ActiveDirectoryName&gt;k__BackingField"":null,""&lt;AvailableFrom&gt;k__BackingField"":null,""&lt;AvailableFutureAllocation&gt;k__BackingField"":0,""&lt;AvailableFutureHours&gt;k__BackingField"":0.0000,""&lt;CreateDate&gt;k__BackingField"":""\/Date(1361685600000+0300)\/"",""&lt;CurrentAllocation&gt;k__BackingField"":100,""&lt;CurrentAvailableHours&gt;k__BackingField"":0.0000,""&lt;DefaultRoleName&gt;k__BackingField"":""Support Person"",""&lt;DeleteDate&gt;k__BackingField"":null,""&lt;Email&gt;k__BackingField"":""Tod.Black@targetprocess.com"",""&lt;FirstName&gt;k__BackingField"":""Tod"",""&lt;IsActive&gt;k__BackingField"":false,""&lt;IsAdministrator&gt;k__BackingField"":false,""&lt;IsObserver&gt;k__BackingField"":false,""&lt;LastName&gt;k__BackingField"":""Black"",""&lt;Login&gt;k__BackingField"":""Tod.Black@targetprocess.com"",""&lt;ModifyDate&gt;k__BackingField"":""\/Date(1361685600000+0300)\/"",""&lt;Password&gt;k__BackingField"":""1846175709"",""&lt;RoleID&gt;k__BackingField"":6,""&lt;Skills&gt;k__BackingField"":null,""&lt;UserID&gt;k__BackingField"":4,""&lt;WeeklyAvailableHours&gt;k__BackingField"":40.0000}</string>
</Value>";

			var user = BlobSerializer.Deserialize(XDocument.Parse(s), "Tp.Integration.Common.UserDTO, Tp.Integration.Messages") as UserDTO;
			user.UserID.Should(Be.EqualTo(4));
		}
	}
}
