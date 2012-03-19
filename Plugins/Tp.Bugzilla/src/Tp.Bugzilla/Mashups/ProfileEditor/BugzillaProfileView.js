tau.mashups
    .addDependency("Bugzilla/BugzillaProfileTemplate")
    .addDependency("Bugzilla/ConnectionChecker")
    .addDependency("tp/bus")
    .addDependency("tp/plugins/profileControlsBlock")
    .addDependency("Bugzilla/DownloadScriptBlock")
    .addDependency("libs/jquery/jquery")
    .addDependency("Bugzilla/jquery.utils")
    .addDependency("libs/jquery/jquery.tmpl")
    .addModule("Bugzilla/BugzillaProfileView", function (template, connectionChecker, bus, profileControlsBlock, DownloadScriptBlock, $) {
        function bugzillaProfileView(config) {
            this._ctor(config);
        }

        bugzillaProfileView.prototype = {
            placeholder: null,
            saveBtn: null,

            _ctor: function (config) {
                this.placeholder = config.placeholder;
                this.template = template;
                this.isEditMode = config.isEditMode;
                this.onProjectChanged = config.onProjectChanged;
                this.bus = bus;
                this.mashupPath = config.mashupPath;

                this.connectionChecker = new connectionChecker({
                    placeholder: this.placeholder,
                    loaderSelector: 'span#checkConnectionPreloader',
                    profileRetriever: config.profileRetriever
                });

                this.bus.subscribe('BugzillaProfileView', {
                    onProfileSaveSucceed: $.proxy(this._onSaveSucceed, this)
                }, true);
            },

            render: function (profile, isEditMode) {
                var rendered = $.tmpl(this.template, profile);

                rendered.find('#name').enabled(!isEditMode);
                rendered.find('#project-note').click(function(){
                    rendered.find('#project-info').slideToggle();
                })

                if (isEditMode) {
                    rendered.find('#project').enabled(false);
                }
                else {
                    rendered.find('#project').change($.proxy(function () {
                        this.onProjectChanged(this.getProfile());
                    }, this));
                }

                this.placeholder.html('');
                rendered.appendTo(this.placeholder);

                this._setSelectedProject(profile);
                this._setFocus();

                this.connectionChecker.initialize();
                var that = this;
                rendered.find('#checkConnection').click(function(e) {
                    e.preventDefault();
                    if (that.downloadScriptBlock) {
                        rendered.find('._downloadScriptBlock').click();
                    }
                    that.connectionChecker.checkConnection(null, $.proxy(that.onCheckConnectionFailure, that));
                });

                this.downloadScriptBlock = new DownloadScriptBlock({placeholder: this.placeholder.find('.additionalInfo'), mashupPath: this.mashupPath});

                new profileControlsBlock({ placeholder: rendered }).render();
            },

            onCheckConnectionFailure: function(data) {
                if ($(data).filter(
                    function(index) {
                        return this.AdditionalInfo && (this.AdditionalInfo == 'TpCgiNotFound' || this.AdditionalInfo == 'InvalidTpCgiVersion');
                    }).length > 0) {
                    rendered.find('._downloadScriptBlock').click();
                }
            },

            //region getProfile

            getProfile: function () {
                var project = this.getProject();
                if (project == null)
                    project = 0;

                return {
                    Name: this._find('#name').val(),
                    Settings: {
                        Login: this._find('#login').val(),
                        Password: this._find('#password').val(),
                        Url: this._find('#url').val(),
                        Project: project,
                        SavedSearches: this._find('#savedSearches').val()
                    }
                };
            },

            //region getProject

            getProject: function () {
                return this._find('#project').val();
            },

            //region save

            _onSaveSucceed: function () {
                this.placeholder.find('#name').enabled(false);
                this.placeholder.find('#project').enabled(false);
            },

            _setSelectedProject: function (profile) {
                if (profile.Name) {
                    this._find("#project").val(profile.Settings.Project);
                }
            },

            _setFocus: function () {
                var nameInput = this.placeholder.find('#name');
                if (this.isEditMode) {
                    this.placeholder.find('#savedSearches').focus();
                    nameInput.enabled(false);
                }
                else {
                    nameInput.focus();
                }
            },

            _find: function (selector) {
                return this.placeholder.find(selector);
            }
        };

        return bugzillaProfileView;
    });
