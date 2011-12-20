tau.mashups
	.addDependency("Bugzilla/UsersPopoverWidget")
	.addDependency("Bugzilla/UserRepository")
	.addDependency("libs/jquery/jquery")
	.addDependency("Bugzilla/ui.widgets")
	.addModule("Bugzilla/UserMappingEditor", function (usersPopoverWidget, userRepository) {
	    function userMappingEditor(config) {
	        this._create(config);
	    };

	    userMappingEditor.prototype = {
	        template: null,
	        placeholder: null,
	        rendered: null,
	        lines: null,
	        userMappingEditorTemplate: '<div class="pad-box">\n    <h3 class="h3"> Users Mapping</h3>\n    <p class="label"><span name="UserMappingErrorLabel" class="error" /></p>\n     <p class="note"><span class="small">Specify Bugzilla-to-TargetProcess users mapping by username (Ex: TeddyBear <span class="mapping-notes-chain">&nbsp;</span> Teddy Bear). You can set up a "catch-all" user. Let\'s say, Jerry Mouse will be a catch-all user. The mapping should look like * <span class="mapping-notes-chain">&nbsp;</span> Jerry Mouse. It means that all users from Bugzilla will be mapped to Jerry Mouse. </span>\n    </p>\n    <ul class="users-block">\n        <li><p class="label pt-10"> Bugzilla User</p></li>\n        <li class="chain-no"></li>\n        <li><p class="label pt-10"> Targetprocess User</p></li>\n    </ul>\n    <div class=\'users-blocks\'></div>\n</div>\n<div class="separator"/>\n<div class="p-15">\n    <a href="#" class="add-more" id="addMoreUsers">Add more users</a>\n</div>\n<div class="separator"/>\n<div class="userMappingError" name="userMappingError"/>\n  \n<div class="separator" style="display:none"></div>\n\n<div class="check-block" name="automapping" style="display:none">\n    <p class="result" style="display: none;"></p>\n    <a href="" class="button" style="display:none">Automapping</a>\n    <span class="preloader" style="display:none"></span>\n</div>',

	        _create: function (config) {
	            this.placeholder = config.placeholder;
	            this.model = config.model;
	            this.usersPopoverWidget = usersPopoverWidget;
	            this.userRepository = new userRepository();
	        },

	        render: function () {
	            this.lines = [];
	            this.placeholder.html('');

	            this.rendered = $.tmpl(this.userMappingEditorTemplate, {});

	            this._renderUserMappingLines(this.model.Settings.UserMapping || []);
	            this._renderUserMappingLines($.repeat({ Key: '', Value: { Name: '', Id: -1} }, 3));

	            this.rendered.find('#addMoreUsers').click($.proxy(this._addMoreUsers, this));

	            this.automappingBtn = this.rendered.find('#automapping');
	            this.automappingBtn.click($.proxy(this._onAutomapping, this));

	            this.rendered.appendTo(this.placeholder);
	        },

	        renderUserAutocomplete: function (projectId) {
	            var that = this;

	            function bindToUserMapping(handler) {
	                var selector = 'input.tpuser';

	                handler(that.placeholder.find(selector));

	                $(selector).live('mappingadded', function () {
	                    handler($(this));
	                });
	            }

	            function usersLoaded(data) {
	                bindToUserMapping(function (elements) {
	                    new that.usersPopoverWidget({ elements: elements, source: data });
	                });

	                bindToUserMapping(function (elements) {
	                    elements.synchronizeUser({ source: data });
	                });
	            }

	            this.userRepository.getUsers(projectId, usersLoaded);
	        },

	        _onAutomapping: function (e) {

	        },

	        _addMoreUsers: function () {
	            this._renderUserMappingLines($.repeat({ Key: '', Value: { Name: '', Id: -1} }, 3));
	            return false;
	        },

	        _renderUserMappingLines: function (userMappings) {
	            var userMappingLineEditors = $(userMappings).map(function () {
	                return new userMappingLine({ model: this });
	            });

	            var lines = this.lines;
	            var container = this.rendered.find('.users-blocks');

	            userMappingLineEditors.each(function () {
	                lines.push(this);
	                this.render(container);
	            });
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
	            this.placeholder.find('.tpuser').tpUserInput('validate');
	        }
	    };

	    function userMappingLine(config) {
	        this._create(config);
	    }

	    userMappingLine.prototype = {
	        userMappingLineTemplate: '<ul class="users-block">\n    <li><input type="text" value="${Key}"  class="input bugzillauser"/></li>\n    <li class="chain"></li>\n    <li><input type="text" value="${Value.Name}" class="input tpuser" userId="${Value.Id}"/></li>\n</ul>',
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
	                    Key: this.dom.find('.bugzillauser').val(),
	                    Value: {
	                        Name: this.dom.find('.tpuser').val(),
	                        Id: this.dom.find('.tpuser').attr('userId')
	                    }
	                };
	            }
	        }
	    };
	    return userMappingEditor;
	});
