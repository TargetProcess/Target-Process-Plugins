tau.mashups
	.addModule("TestRunImport/editorTemplate",
			'<div>' +
			'<h2 class="h2">Automatic Test Run Import</h2>' +
			'<p class="note">Imports automatic test run results to TargetProcess from testing framework.</p>' +
			'<div class="n-unit-settings">' +
			'	<div class="pad-box">' +
			'		<p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span></p>' +
			'		<input type="text" class="input" id="name" name="Name" value="${Name}" style="width: 275px;" />' +
			'	</div>' +
			'	<div class="separator"></div>' +
			'	<div class="pad-box">' +
			'		<p class="label">Select Framework&nbsp;<span class="error" name="FrameworkTypeErrorLabel"></span></p>' +
			'		<p class="note"><span class="small">Testing framework results to be binded to the test plan.</span></p>' +
			'		<select class="select" id="frameworkDropDown" name="FrameworkType"><option value="0">- Select Framework -</option><option value="1">NUnit</option><option value="2">JUnit</option><option value="3">Selenium</option></select>' +
			'	</div>' +
			'	<div class="separator"></div>' +
			'	<div class="pad-box">' +
			'		<h3 class="h3">Result File</h3>' +
			'		<div id="pathHolder">' +
			'			<p class="label pt-5">Enter a full path to the test result XML file&nbsp;<span class="error" name="ResultsFilePathErrorLabel"></span></p>' +
			'			<p class="label">' +
			'				<span class="small">&nbsp;Path to the file that contains test results.<br />&nbsp;Ex: C:\\ResultFolder\\results.xml, http://hostname/results.xml, ftp://login:password@hostname/results.xml.</span></p>' +
			'			<table style="border-collapse: collapse;">' +
			'				<tr>' +
			'					<td><input type="text" class="input" id="resultsFilePath" name="ResultsFilePath" value="${Settings.ResultsFilePath}" style="width: 450px;" /></td>' +
			'					<td class="pl-10 pr-5"><div id="switch"></div></td>' +
			'					<td>FTP Passive Mode</td>' +
			'				</tr>' +
			'			</table>' +
			'			<p class="label pt-5">Sync Interval&nbsp;<span class="error" name="SynchronizationIntervalErrorLabel"></span></p>' +
			'			<p class="note"><span class="small">Test results will be imported with defined time interval in hours. Ex: 24.</span></p>' +
			'			<input type="text" class="input" id="syncInterval" name="SynchronizationInterval" style="width: 75px;" value="${Settings.SynchronizationInterval}" />' +
			'		</div>' +
			'	</div>' +
			'</div>' +
			'<div class="n-unit-mapping">' +
			'	<div class="pad-box">' +
			'		<h3 class="h3">Test Plan</h3>' +
			'		<p class="label pt-5">Test Plan Name&nbsp;<span class="error" name="ProjectErrorLabel"></span>&nbsp;<span class="error" name="TestPlanErrorLabel"></span></p>' +
			'		<p class="note"><span class="small">Test automation results bind to the test plan.</span></p>' +
			'		<select class="select" id="projectsDropDown" name="Project">' +
			'			{{each projects}}<option value="${Id}">${Name}</option>{{/each}}' +
			'		</select>' +
			'		<select class="select" id="testPlansDropDown" name="TestPlan">' +
			'			{{each testplans}}<option value="${Id}">${Name}</option>{{/each}}' +
			'		</select>' +
			'	</div>' +
			'	<div class="pad-box">' +
			'		<h3 class="h3">Test Cases</h3>' +
			'		<p class="label pt-5">Mapping Template&nbsp;<span class="error" name="RegExpErrorLabel"></span></p>' +
			'		<p class="note">' +
			'			<span class="small">Use regular expression to find test names. Ex: ^(?&lt;testName&gt;[^_]+)_Test$ will extract test case name from ComplexFeature_Test,<br />_(?&lt;testId&gt;\d+)$ will extract test case id from ComplexFeature_1289.</span>&nbsp;<a id="linkSample" class="note" style="font-size: 11px;" href="javascript:void(0);">Learn more</a></p>' +
			'		<div id="mappingDescription" style="display:none" class="label"><span class="small" style="font-size:12px; color:#333">' +
			'			<br><br><p>You have a set of tests in one of Testing frameworks (NUnit, JUnit, Selenium). To have test results in TargetProcess, it is required to have a set of related test cases. For example, you have 3 tests in NUnit:</p>' +
			'			<br><p style="font-size: 14px"><span class="rules-conditions">AddSimpleUserTest</span></p>' +
			'			<p style="font-size: 14px"><span class="rules-conditions">AddAdminTest</span></p>' +
			'			<p style="font-size: 14px"><span class="rules-conditions">EditAdminTest</span></p>' +
			'			<br><p>You want to run them and have results available in TargetProcess.</p>' +
			'			<br><p>First it is required to map these tests to TargetProcess test cases. There are several options. The simplest option is just have 3 test cases in TargetProcess with the same names or with spaces, like this:</p>' +
			'			<br><table class="help-mapping-block">' +
			'				<tr><td nowrap class="title">NUnit Test</td><td class="title">TargetProcess Test Case</td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddSimpleUserTest</span></td><td><span class="rules-actions">Add Simple User Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddAdminTest</span></td><td><span class="rules-actions">Add Admin Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">EditAdminTest</span></td><td><span class="rules-actions">Edit Admin Test</span></td></tr>' +
			'			</table>' +
			'			<br><br><p class="pt-5">The other option is to add test case id to each test in NUnit. And use regular expression to extract the id from the test name <b>_(?&lt;testId&gt;\d+)$</b></p>' +
			'			<p class="small">If you are not familiar with regular expressions, it may be hard to write correct one. See a few links at the end of the help block.</p>' +
			'			<br><div class="rules-block">[Test]' +
			'				<br><span class="rules-conditions">public void</span> AddSimpleUserTest_90()' +
			'				<br>{' +
			'				<br>&nbsp;&nbsp;&nbsp;Navigation.Login();' +
			'				<br>&nbsp;&nbsp;&nbsp;CurrentEntityOperator.Retriever.SetCurrentEntity(Creator.Project());' +
			'				<br>&nbsp;&nbsp;&nbsp;...' +
			'				<br>}' +
			'			</div><br>' +
			'			<br><table class="help-mapping-block">' +
			'				<tr><td nowrap class="title">NUnit Test</td><td class="title">TargetProcess Test Case</td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddSimpleUserTest_90</span></td><td><span class="rules-actions">Add Simple User Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddAdminTest_91</span></td><td><span class="rules-actions">Add Admin Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">EditAdminTest_95</span></td><td><span class="rules-actions">Edit Admin Test</span></td></tr>' +
			'			</table>' +
			'			<br><br><p>Alternatively you may cut part of the name, for example, remove Key word from test names using regular expression <b>^(?&lt;testName&gt;[^_]+)Key$</b></p>' +
			'			<br><div class="rules-block">[Test]' +
			'				<br><span class="rules-conditions">public void</span> AddSimpleUserTestKey()' +
			'				<br>{' +
			'				<br>&nbsp;&nbsp;&nbsp;Navigation.Login();' +
			'				<br>&nbsp;&nbsp;&nbsp;CurrentEntityOperator.Retriever.SetCurrentEntity(Creator.Project());' +
			'				<br>&nbsp;&nbsp;&nbsp;...' +
			'				<br>}' +
			'			</div><br>' +
			'			<br><table class="help-mapping-block">' +
			'				<tr><td nowrap class="title">NUnit Test</td><td class="title">TargetProcess Test Case</td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddSimpleUserTestKey</span></td><td><span class="rules-actions">Add Simple User Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">AddAdminTestKey</span></td><td><span class="rules-actions">Add Admin Test</span></td></tr>' +
			'				<tr><td nowrap><span class="rules-conditions">EditAdminTestKey</span></td><td><span class="rules-actions">Edit Admin Test</span></td></tr>' +
			'			</table>' +
			'			<br><p>If you do not have test cases, you may add them manually or import from CSV file.</p><br>' +
			'			<p>How to write regular expressions:<br><a href="http://www.radsoftware.com.au/articles/regexlearnsyntax.aspx" target="_blank">Learn Regular Expression (Regex) syntax with C# and .NET</a><br><a href="http://msdn.microsoft.com/en-us/library/aa719739.aspx" target="_blank">.NET Framework Regular Expressions</a></p><br>' +
			'		</span></div>' +
			'		<input type="text" class="input" id="regExp" name="RegExp" value="${Settings.RegExp}" style="width: 100%;" />' +
			'	</div>' +
			'	<div class="pad-box" id="mappingHolder" style="display:none">' +
			'		<h3 class="h3">Mapping Results</h3>' +
			'		<div class="check-mapping-block">' +
			'			<table class="check-mapping-table" id="checkMappingTable"><col width=44><col width=292><col width=292><col width=16>' +
			'				<tr><th></th><th>Test Case</th><th>Unit Test</th><th></th><tr>' +
			'			</table>' +
			'		</div>' +
			'		<p class="pt-10 pb-5"></p>' +
			'	</div>' +
			'	<div class="mapping-block">' +
			'		<p class="label">&nbsp;<span class="error" name="MappingErrorLabel"></span></p>' +
			'       <p class="error-message" id="failedMappingCheck" style="display:none"><span></span></p>' +
			'		<a href="javascript:void(0);" class="check-connection-link" id="checkMapping">Check Mapping</a><span class="preloader" style="display:none"></span>' +
			'	</div>' +
			'</div>' +
			'<div class="controls-block">' +
			'</div>' +
			'</div>');
