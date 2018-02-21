tau.mashups
    .addDependency("tp/gateway")
    .addDependency("libs/jquery/jquery")
    .addMashup(function (commandGateway, $, config) {

        var initConvertLinks = function() {

            var bugzillaHeader = $('span._pluginName:contains("Bugzilla Integration")');

            var tableParent = null;
            var bzParent = bugzillaHeader.parent();
            while (bzParent.length > 0 && tableParent == null) {
                if (bzParent.is('table') && bzParent.hasClass('generalTable')) {
                    tableParent = bzParent;
                }
                else {
                    bzParent = bzParent.parent();
                }
            }

			if (tableParent != null) {
				var profilesDiv = tableParent.next();
				var profilesTable = profilesDiv.find('table._pluginProfiles');

				var gateway = commandGateway;

				var executeConvert = function(tr, profileName) {
					var success = function() {
						$('<tr>\n    <td colspan="5">\n        <div class="mt-5 purchaseNotify" style="width:650px;">\n            Profile conversion to new Bugzilla plugin has started. It may take several minutes. \n            Use Ctrl+F5 to reload the page. You will then see the profile name in the Bugzilla section (New Plugins). \n        </div>\n    </td>\n</tr>')
							.insertAfter(tr);
					};
					var fail = function() {
						$('<tr>\n    <td colspan="5">\n        <div class="mt-5 pluginsNotify" style="width:650px;">\n            There\'s an unknown exception. Check the plugin log for details.\n        </div>\n    </td>\n</tr>')
							.insertAfter(tr);
					};

					return function() {
						var btn = $(this);
						btn.unbind('click');
						btn.attr('disabled', 'disabled');

						var url = new Tp.WebServiceURL('/api/v1/Plugins.asmx/Bugzilla/Commands/ConvertProfile').url;
						gateway.postJSON(url, {ProfileName: profileName}, success, fail, fail);
					}
				}

				$.each(profilesTable.find('tr'), function(i, el) {
					var td = $(el).find('td').eq(0);
					var profileNameSpan = td.find('span').find('span');
					var profileName = profileNameSpan.text();

					var converting = '_converting...';
					if(profileName.indexOf(converting) != -1){
						var cleanProfileName = profileName.replace(converting, '');
						profileNameSpan.text('');
						profileNameSpan.text(cleanProfileName);
						profileNameSpan.append('<span style="color:red;font-weight:bold;">'+ converting +'</span>');
					}

					var convertLink = $('<a class="tau-btn tau-primary" title="Convert to new profile" href="#" onclick="event.preventDefault();">Convert to new profile</a>');
					td.prepend(convertLink);
					convertLink.click(executeConvert($(el), profileName));
				});
			}
        };

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(initConvertLinks);

        initConvertLinks();
    });
