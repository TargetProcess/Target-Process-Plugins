tau.mashups
    .addModule("emailIntegration/editorTemplate",
'<form method="POST"><h2 class="h2">E-mail Integration</h2>'+
'<p class="note">Retrieves emails from your mail account into internal inbox and creates requests from emails.</p>'+
'<div class="base-block">'+
'	<div class="pad-box">' +
'		<p class="label">Profile Name&nbsp;<span class="error" name="NameErrorLabel"></span><br>' +
'		<span class="small">Once this name is saved, you can not change it.</span>' +
'		<span class="small" style="float: right;">Secure access method</span></p>' +
'		<input id="Name" type="text" name="Name" class="input" style="width: 275px;" value="${Name}" />' +
'		<div class="controls-group main-controls left" style="float: right;">' +
'			<input {{if Settings.SecureAccessMethod === 0}}style="display: none;"{{/if}} id="SignIn" name="SignIn" class="tau-btn tau-btn-big input{{if Settings.SecureAccessMethod && !Settings.OAuthState.IsDeleted}} tau-success{{/if}}" type="button" value="{{if Settings.SecureAccessMethod && !Settings.OAuthState.IsDeleted}}Sign out{{else}}Sign in{{/if}}">' +
'			<select style="float: right;" class="select" id="authDropDown" name="AuthenticationType">' +
'				<option value="">Login &amp; Password</option>' +
'				<option value="1" {{if Settings.SecureAccessMethod === 1}}selected="selected"{{/if}} url="https://accounts.google.com" pop3="https://mail.google.com" imap="https://mail.google.com" scope="email">Google OAuth</option>' +
'				<option value="2" {{if Settings.SecureAccessMethod === 2}}selected="selected"{{/if}} url="https://login.microsoftonline.com/common/v2.0" pop3="https://outlook.office.com/POP.AccessAsUser.All" imap="https://outlook.office.com/IMAP.AccessAsUser.All" scope="email openid offline_access">Outlook OAuth</option>' +
'			</select>' +
'		</div>' +
'	</div>' +
'	<div class="separator"></div>' +
'<div class="pad-box">'+
'<h3 class="h3">Email Settings</h3>' +
'<table>' +
'<tr>' +
'</tr>' +
'<tr>' +
'<td><p class="label">Protocol</p></td>' +
'<td><p class="label">Server&nbsp;<span class="error" name="MailServerErrorLabel"></span></p></td>'+
'<td class="pl-5"><p class="label">Port&nbsp;<span class="error" name="PortErrorLabel"></span></p></td>' +
'<td class="pl-10 pr-5"><p class="label">SSL Mode</p></td>' +
'</tr>'+
'<tr>' +
'<td>' +
'<select class="select" id="Protocol" name="Protocol">' +
'<option value="pop3" {{if Settings.Protocol === "pop3"}}selected="selected"{{/if}}>POP</option>' +
'<option value="imap" {{if Settings.Protocol === "imap"}}selected="selected"{{/if}}>IMAP</option>' +
'</select>' +
'</td>' +
'<td><input type="text" name="MailServer" id="MailServer" class="input" value="${Settings.MailServer}" style="width: 400px;" /></td>'+
'<td class="pl-5"><input type="text" name="Port" id="Port" class="input" value="${Settings.Port}" style="width: 64px;" /></td>'+
'<td class="pl-10 pr-5"><div id="switch"></div></td>'+
'</tr>'+
'</table>'+
'<p class="label pt-10">{{if Settings.SecureAccessMethod}}Client id{{else}}Login{{/if}}&nbsp;<span class="error" name="LoginErrorLabel"></span></p></p>'+
'<input type="text" id="Login" name="Login" class="input" style="width: 275px;" value="${Settings.Login}" />&nbsp;&nbsp;&nbsp;<span style="color: rgba(0, 0, 0, 0.7);">{{if Settings.SecureAccessMethod !== 0}}${Settings.OAuthState.Email}{{/if}}</span>'+
'<p class="label pt-10">{{if Settings.SecureAccessMethod}}Client secret{{else}}Password{{/if}}&nbsp;<span class="error" name="PasswordErrorLabel"></span></p></p>'+
'<input type="password" id="Password" name="Password" class="input" style="width: 275px;" value="${passwordValue}" />' +
'<input type="hidden" id="UsersMigrated" name="UsersMigrated" value="${Settings.UsersMigrated}">' +
'</div>'+
'<div class="check-block">'+
'<p class="message-ok" id="successfulConnection" style="display:none"><span>Connection was established successfully</span></p>' +
'<p class="error-message" id="failedConnection" style="display:none"><span></span></p>' +
'<a href="javascript:void(0);" id="checkConnection" class="check-connection-link">Check Connection</a><span class="preloader" style="display:none"></span>' +
'</div>'+
'</div>'+
'<div class="base-block">'+
'<div class="pad-box">'+
'<p class="label">Rules&nbsp;'+

'<span class="error" name="RulesErrorLabel"></span></p><p class="label"><span class="small">Type the rules which will be applied after an email message arrives here. One rule per line.</span></p>' +
'<p class="label"><span class="small">Please, note: <i>when</i> and <i>then</i> keywords are case-sensitive.</span>' +
'&nbsp;<a id="linkSample" class="note" style="font-size: 11px;" href="javascript:void(0);">Learn more</a></p>' +

'<div id="ruleDescription" style="display:none">' +
'<br><br><p>Each rule has the following format</p>' +
'<br><p style="font-size: 18px">when <span class="rules-conditions">[conditions]</span> then <span class="rules-actions">[actions]</span></p>' +
'<p class="small">Conditions part is optional</p>' +
'<br><br><table class="rules-block">' +
'<tr><td colspan="2"><b>Possible conditions</b></td></tr>' +
'<tr><td nowrap><span class="rules-conditions">subject contains \'keyword1, keyword2, ...\'</span></td><td>actions from "then" part will be performed if ANY of the listed keywords is in the subject</td></tr>' +
'<tr><td nowrap><span class="rules-conditions">company matched to PROJECT ID</span></td><td>actions from "then" part will be executed only if requester and project  mentioned in "when" clause relate to the same company</td></tr>' +
'<tr><td colspan="2"><br><b>Possible actions</b></td></tr>' +
'<tr><td nowrap><span class="rules-actions">attach to project PROJECT ID</span></td><td>email message will be attached to project</td></tr>' +
'<tr><td nowrap><span class="rules-actions">create request in project PROJECT ID</span></td><td>private request will be created from email for a given project</td></tr>' +
'<tr><td nowrap><span class="rules-actions">attach request to team TEAM ID</span></td><td>request created in previous step will be attached to a given team</td></tr>' +
'</table>' +
'<br><br>' +
'<p class="label">Example</p>' +
'Let\'s say, we have 2 projects. "Project1" with id=1 and "Project2" with id=2. Project 2 is assigned to "TargetProcess" company.'+
'<br><br>'+
'<p>We can set the following 3 rules in the profile:</p><br>' +
'<div class="rules-block">when <span class="rules-conditions">subject contains \'Targetprocess, tp\'</span> and <span class="rules-conditions">company matched to project 2</span> then <span class="rules-actions">attach to project 2</span> and <span class="rules-actions">create public request in project 2</span>' +
'<br>when <span class="rules-conditions">company matched to project 2</span> then <span class="rules-actions">attach to project 2</span>' +
'<br>then <span class="rules-actions">attach to project 1</span> and <span class="rules-actions">create private request in project 1</span> and <span class="rules-actions">attach request to team 37</span></div>' +
'<br>'+
'Rules are checked from top to bottom. Let\'s see how these rules are applied to messages.'+
'<ol>'+
'<li>If a message is sent by requester assigned to Targetprocess company and there is either "targetprocess" or "tp" word in the subject then the 1st rule'+
'&nbsp;will match and the message will attach to Project 2, with public request created in this project.</li>'+
'<li>If message is sent by requester from Targetprocess company but there is NO "targetprocess" or "tp" words in the subject, then the 1st rule will be skipped as'+
'&nbsp;there\'s no match and the 2nd rule will be applied, with the email attached to Project 2.</li>'+
'<li>All the other messages not matching to  any of the first 2 rules will be moved to Project 1, with private requests created for Project 1.</li>'+
'</ol><br><br></div>' +

'<textarea name="Rules" id="Rules" class="textarea" rows="10" hint="when subject contains \'KEYWORD1,KEYWORD2\' and company matched to project PROJECT ID then attach to\n project PROJECT ID and create request in project PROJECT ID" style="width: 100%;">{{if Settings.Rules != null}}${unescape(Settings.Rules)}{{/if}}</textarea>' +
'</div>'+
'</div>'+
'<div class="controls-block">' +
'</div>'+
'</form>'
);
