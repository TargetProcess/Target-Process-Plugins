tau.mashups
	.addDependency("TestRunImport/restService")
	.addDependency("TestRunImport/userRepository")
	.addDependency("TestRunImport/authenticationTokenRepository")
	.addDependency("libs/jquery/jquery")
	.addDependency("libs/jquery/jquery.tmpl")
	.addModule("TestRunImport/seleniumUrlEditor", function (restService, userRepository, authenticationTokenRepository) {
	    function seleniumUrlEditor(config) {
	        this._create(config);
	    }

	    seleniumUrlEditor.prototype = {
	        pluginName: null,
	        profileNameInput: null,
	        profileGetter: null,
	        commandGateway: null,
	        preloader: null,
	        rendered: null,
	        seleniumUrlEditorTemplate:
				'<div>' +
				'	<div class="pb-5">' +
				'		<p class="label pt-5">Authentication User&nbsp;<span class="error" name="AuthTokenUserIdErrorLabel"></span></p>' +
				'		<p class="note"><span class="small">The user that will be used for authentication when posting the results of running the tests.</span></p>' +
				'		<input type="text" value="" name="AuthTokenUserId" class="input tpuser" disabled="" userId="${AuthUserId}" style="width: 275px;" />' +
				'	</div>' +
				'	<div class="separator"></div>' +
				'	<div class="pt-5">' +
				'		<p class="note"><span class="small">Selenium \'resultsUrl\'. The remote url the results of running Selenium tests  will be posted to.</span></p>' +
				'		<p class="pb-5">' +
				'			<input type="text" id="resultsUrl" class="input" name="RemoteResultsUrl" value="${RemoteResultsUrl}" readonly="readonly" style="width: 100%;" />' +
				'		</p>' +
				'		<p class="message-error pb-10" style="display: none;">Login failed. You have entered incorrect or non-existent login.</p>' +
				'		<a href="javascript:void(0);" id="generateUrl" class="tau-btn">Generate Selenium URL</a><span class="preloader" style="display:none"></span>' +
				'	</div>' +
				'</div>',

	        _create: function (config) {
	            this.pluginName = config.pluginName;
	            this.profileNameInput = config.profileNameInput;
	            this.profileGetter = config.profileGetter;
	            this.commandGateway = config.commandGateway;
	            this.seleniumUrl = config.seleniumUrl;
	            this.repository = new authenticationTokenRepository({});
	        },

	        render: function (profile) {
	            var r = this.rendered = $.tmpl(this.seleniumUrlEditorTemplate, profile);
	            r.find('a#generateUrl').click($.proxy(this._generateUrl, this));
	            this.preloader = r.find('span.preloader');

	            function bindUser(user) {
	                var selector = 'input.tpuser';
	                r.find(selector).attr('userId', user.Id).val(user.Name);
	            }

	            var userId = r.find('input.tpuser').attr('userId');
	            var userRepo = new userRepository({ RestService: new restService() });
	            if (!isNaN(userId) && userId > 0) {
	                userRepo.getUserById(userId, $.proxy(function (u) {
	                    if (u.Id > 0 && u.IsActive) {
	                        bindUser(u);
	                    } else {
	                        userRepo.getLoggedUser($.proxy(function (d) {
	                            if (d.Id > 0) {
	                                bindUser(d);
	                            }
	                            r.find('*[name=AuthTokenUserIdErrorLabel]').html('Current Selenium \'resultsUrl\' should be re-generated for new user ' + d.Name);
	                            r.find('input#resultsUrl').addClass('error');
	                            r.find('input#resultsUrl').val('');
	                        }, this));
	                    }
	                }, this));
	            } else {
	                userRepo.getLoggedUser($.proxy(function (d) {
	                    if (d.Id > 0 && d.IsActive) {
	                        bindUser(d);
	                    }
	                    if (!this.profileNameInput.enabled()) {
	                        r.find('input#resultsUrl').addClass('error');
	                    }
	                }, this));
	            }
	        },

	        getUserId: function () {
	            return this.rendered.find('input.tpuser').attr('userId');
	        },

	        getSeleniumUrl: function () {
	            return this.rendered.find('input#resultsUrl').val();
	        },

	        _generateUrl: function (e) {
	            e.preventDefault();

	            var btn = $(e.target);
	            if (!btn.enabled()) {
	                return;
	            }
	            btn.enabled(false);
	            this.preloader.show();
	            var profile = this.profileGetter();
	            this.rendered.find('input#resultsUrl').val('');
	            var onEndRequest = $.proxy(function (d) {
	                if (d.length > 0) {
	                    this.rendered.trigger('showerrors', [d]);
	                    this.preloader.hide();
	                    btn.enabled(true);
	                } else {
	                    this.repository.getAuthenticationToken(profile.AuthTokenUserId, $.proxy(function (data) {
	                        this.rendered.trigger('showerrors', [{}]);
	                        var l = document.location;
	                        var url = '/api/v1/Plugins.asmx/{PluginName}/Profiles/{ProfileName}/Commands/SeleniumResults?token={Token}';
	                        var uri = new Tp.WebServiceURL(url.replace(/{PluginName}/g, this.pluginName)
									.replace(/{ProfileName}/g, this.profileNameInput.val() != null ? this.profileNameInput.val().trim() : '')
									.replace(/{Token}/g, data.Token)).url;
	                        this.rendered.find('input#resultsUrl').removeClass('error').val(encodeURI(l.protocol + '//' + l.host + uri));
	                        this.preloader.hide();
	                        btn.enabled(true);
	                    }, this));
	                }
	            }, this);
	            this.commandGateway.execute('ValidateProfileForSeleniumUrl', profile, onEndRequest, onEndRequest, onEndRequest);
	        }
	    };
	    return seleniumUrlEditor;
	});
