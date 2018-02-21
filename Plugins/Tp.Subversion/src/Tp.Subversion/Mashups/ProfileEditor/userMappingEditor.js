tau.mashups
    .addDependency("tp/bus")
    .addDependency("libs/jquery/jquery")
    .addDependency("tp/plugins/vcs/ui.widgets")
    .addModule("Subversion/UserMappingEditor", function (Bus) {


        function UserMappingEditor(config) {
            this._create(config);
        }

        UserMappingEditor.prototype = {
            template: null,
            placeHolder: null,
            rendered: null,
            lines: null,
            userMappingEditorTemplate: '<div class="pad-box">\n    <p class="label"><span name="UserMappingErrorLabel" class="error" /></p>\n    <span name="user-mapping"/>\n\n    <p class="note"><span class="small">There are two options for mapping users: manual mapping and auto-mapping. Automapping is the preferred option.</span>\n        <a id="howItWorksLink" class="note" style="font-size: 11px;" href="javascript:void(0);">How it works</a>\n\n    <div id="howItWorksDescription" style="display:none">\n        <p class="mt-10 mb-10 rules-block">All Subversion commits over the last month are scanned, identifying commit authors\n            with TargetProcess users by their email, login and first/last name. This might take a while, and mapping\n            progress is shown real-time.</p>\n    </div>\n    <p class="note">\n        <span class="small">For manual mapping enter Subversion author and TP user first name, last name (example: JohnSmith <span class="mapping-notes-chain">&nbsp;</span> John Smith).</span>\n    </p>\n    </p>\n    <div class="pt-10 automapping">\n        <a href="" class="tau-btn">Automap</a>\n        <span class="preloader" style="display:none"></span>\n    </div>\n    <div class="automapping">\n        <div class="automapping-result" style="display:none">\n            <p class="warning-message"/>\n\n            <div class="separator"/>\n        </div>\n    </div>\n    <ul class="users-block">\n        <li><p class="label pt-10"> Subversion User Login </p></li>\n        <li class="chain-no"></li>\n        <li><p class="label pt-10"> TargetProcess User</p></li>\n    </ul>\n    <div class=\'users-blocks\'></div>\n</div>\n<div class="p-15">\n    <a href="#" class="add-more" id="addMoreUsers">Add more users</a>\n</div>',

            _create: function (config) {
                this.placeHolder = config.placeHolder;
                this.model = config.model;
                Bus.subscribe("UserMappingEditor_Automapping", {
                    onAutomapPeopleCommandCompleted: $.proxy(this._onAutomapPeopleCommandCompleted, this)
                }, true);
            },

            _onAutomapPeopleCommandCompleted: function (results) {
                this.model.Settings.UserMapping = results.UserLookups;
                this.render();

                var automappingResult = this.placeHolder.find('.automapping-result');

                var okMessage = automappingResult.find(".warning-message");
                okMessage.text(results.Comment);
                automappingResult.show();
            },

            render: function () {
                this._renderEmpty();

                this._renderUserMappingModel();
                this._completeRendering();
            },

            _renderEmpty: function () {
                this.lines = [];
                this.placeHolder.html('');

                this.rendered = $.tmpl(this.userMappingEditorTemplate, {});
            },

            _renderUserMappingModel: function () {
                this._renderUserMappingLines(this.model.Settings.UserMapping || []);
                this._addMoreUsers();
            },

            _completeRendering: function () {
                this.rendered.appendTo(this.placeHolder);

                this.rendered.find('#addMoreUsers').click($.proxy(this._addMoreUsers, this));
                this.progressIndicator = this.rendered.find('span.preloader');
                this.rendered.find(".automapping .button").click($.proxy(this._onAutomapping, this));

                this.rendered.find('#howItWorksLink').click(function () {
                    $('#howItWorksDescription').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
                });
            },

            _onCheckConnectionInitiated: function () {
                this.progressIndicator.show();
            },

            _onCheckConnectionSuccessful: function (profile) {
                Bus.unsubscribe("UserMappingEditor");
                Bus.publish("AutomapPeopleCommand", [profile.Settings]);
            },

            _onCheckConnectionFailed: function (args) {
                Bus.unsubscribe("UserMappingEditor");
                this.progressIndicator.hide();
            },

            _onAutomapping: function (e) {
                e.preventDefault();

                Bus.subscribe("UserMappingEditor", {
                    onCheckConnectionInitiated: $.proxy(this._onCheckConnectionInitiated, this),
                    onCheckConnectionSuccessful: $.proxy(this._onCheckConnectionSuccessful, this),
                    onCheckConnectionFailed: $.proxy(this._onCheckConnectionFailed, this)
                }, true);

                Bus.publish("CheckConnectionForced");
            },

            _addMoreUsers: function () {
                this._renderUserMappingLines($.repeat({ Key: '', Value: { Name: '', Id: -1} }, 3));
                return false;
            },

            _renderUserMappingLines: function (userMappings) {
                var userMappingLineEditors = $(userMappings).map(function () {
                    return new UserMappingLine({ model: this });
                });

                var container = this.rendered.find('.users-blocks');

                $.each(userMappingLineEditors, $.proxy(function (key, value) {
                    this.lines.push(value);
                    value.render(container);
                }, this));
            },

            getUserMappings: function () {
                var userMappings = $(this.lines).map(function () {
                    return this.getUserMapping();
                });

                var mappings = $.grep(userMappings, function (userMapping) {
                    return userMapping.Key != '' && userMapping.Value != '';
                });

                return mappings;
            },

            clientValidate: function () {
                this.placeHolder.find('.tpuser').tpUserInput('validate');
            },

            getValidationErrorsCount: function () {
                return this.placeHolder.find('.tpuser.error').length;
            }
        };

        function UserMappingLine(config) {
            this._create(config);
        }

        UserMappingLine.prototype = {
            userMappingLineTemplate: '<ul class="users-block">\n    <li><input type="text" value="${Key}"  class="input svnuser"/></li>\n    <li class="chain"></li>\n    <li><input type="text" value="${Value.Name}" class="input tpuser" userId="${Value.Id}"/></li>\n</ul>',
            removeIcon: $('<li class="remove-mapping"><a title="Remove mapping" class="remove" href="#"></a></li>'),

            _create: function (config) {
                this.model = config.model;
            },

            render: function (container) {
                this.dom = $.tmpl(this.userMappingLineTemplate, this.model)
                    .hover($.proxy(this.showRemoveIcon, this), $.proxy(this.hideRemoveIcon, this));
                this.dom.find('.tpuser').tpUserInput();
                this.dom.editorAnimation();
                this.dom.editorAnimation('appendTo', container);
            },

            remove: function (e) {
                e.preventDefault();
                this.dom.editorAnimation('remove');
                this.dom = null;
            },

            showRemoveIcon: function () {
                this.removeIcon.find('a').unbind('click');
                this.removeIcon.find('a').click($.proxy(this.remove, this));
                this.removeIcon.appendTo(this.dom);
            },

            hideRemoveIcon: function () {
                this.dom.find('.remove-mapping').remove();
            },

            getUserMapping: function () {
                if (this.dom) {
                    return {
                        Key: this.dom.find('.svnuser').val(),
                        Value: {
                            Name: this.dom.find('.tpuser').val(),
                            Id: this.dom.find('.tpuser').attr('userId')
                        }
                    };
                }
            }
        };
        return UserMappingEditor;
    });
