tau.mashups
    .addDependency("tp/plugins/bugzilla/bugsRepository")
    .addDependency("BugzillaListDetails/BugIconTemplate")
    .addDependency("BugzillaListDetails/InfoBubble")
    .addDependency("tp/plugins/pluginsRepository")
    .addDependency("libs/jquery/jquery")
    .addDependency("libs/jquery/jquery.tmpl")
    .addModule("BugzillaListDetails/IconViewer", function (bugzillaRepository, template, infoBubble, pluginsRepository) {
        function bugzillaIconViewer(config) {
            this.thIdNumber = this._getThIdNumber();
            this.bugzillaRepository = bugzillaRepository;
            this.infoBubble = new infoBubble();
            this.template = template;
            this.firstInitialization = true;
            this.drawingInProgress = false;
            this.pluginsRepository = pluginsRepository;

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_pageLoaded($.proxy(this.drawBugIcons, this));
        };

        bugzillaIconViewer.prototype = {

            _getThIdNumber: function () {
                var allThs = $('table[class="generalTable"] th');
                var result = -1;

                $.each(allThs, function (index, th) {
                    if ($(th).find('a:contains("ID")').length > 0) {
                        result = index + 1;
                    }
                });

                return result;
            },

            drawBugIcons: function (sender, args) {
                var that = this;
                var bugzillaStartedAndHasAtLeastOneProfile = function (callback) {
                    that.pluginsRepository.pluginStartedAndHasAtLeastOneProfile('Bugzilla', callback);
                };

                if (!args) {
                    bugzillaStartedAndHasAtLeastOneProfile($.proxy(this._drawBugIcons, this));
                    this.firstInitialization = false;
                    return;
                }
                
                var updatePanels = $.grep(args.get_panelsUpdated(), function (element) { return element.id.contains('updatePanelGrid'); });
                if (!this.drawingInProgress && (this.firstInitialization || updatePanels.length > 0)) {
                    this.drawingInProgress = true;
                    bugzillaStartedAndHasAtLeastOneProfile($.proxy(this._drawBugIcons, this));
                    this.firstInitialization = false;
                }
            },

            _drawBugIcons: function () {
                var bugsOnPage = this._getBugsOnPage();
                this.bugzillaRepository.getBugs(bugsOnPage, $.proxy(this._onGetBugzillaBugSuccess, this));
            },

            _getBugsOnPage: function () {
                var idTds = $('table[class="generalTable"]>tbody>tr>td:nth-child(' + this.thIdNumber + ')');
                var ids = new Array();

                $.each(idTds, function (index1, td) {
                    var id = parseInt($(td).text().trim());
                    if (isNaN(id) === false) {
                        ids.push(id);
                    }
                });

                return ids;
            },

            _onGetBugzillaBugSuccess: function (filteredBugs) {
                var context = this;
                var tds = $('table[class="generalTable"] tr td:nth-child(' + context.thIdNumber + ')');

                $.each(filteredBugs, function (index, bug) {
                    var td = $.grep(tds, function (element) {
                        var id = parseInt($(element).text());
                        return id == bug.TpId;
                    })[0];

                    var icon = $.tmpl(context.template, { id: bug.TpId });

                    icon.appendTo($(td));
                    icon.find('a').click($.proxy(context._toggleBubble, context));
                });

                this.drawingInProgress = false;
            },

            _toggleBubble: function (source) {
                var bugId = parseInt(source.currentTarget.attributes["bugId"].value);
                var bugInfo = this.bugzillaRepository.getBugInfo(bugId);
                this.infoBubble.toggleBubble(source, bugInfo);
            }
        };

        return bugzillaIconViewer;
    })
