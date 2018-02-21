tau.mashups
	.addDependency("tp/plugins/restService")
	.addDependency("tp/bus")
	.addDependency("libs/jquery/jquery")
	.addDependency("tp/plugins/vcs/ui.widgets")
	.addModule("Tfs/EntityMappingEditor", function (restService, Bus) {


	    function EntityMappingEditor(config) {
	        this._create(config);
	    }

	    EntityMappingEditor.prototype = {
	        template: null,
	        placeHolder: null,
	        rendered: null,
	        lines: null,
	        entities: null,
	        entityMappingEditorTemplate:
	            '<div class="pad-box">' +
    	            '<p class="error-message pb10" id="entityMappingError" style="display:none"><span></span>' +
            	    '</p>' +
                	'<span name="entity-mapping"/>' +
                    '<p class="note">' +
                        '<span class="small">Select pairs of entities here (Example: Work Items entity -> TP entity)</span>' +
                    '</p>' +
                    '<div class="pt-10 automapping" style="display: none">' +
                        '<a href="" class="button tau-btn">Run Auto-mapping</a>' +
                        '<span class="preloader" style="display:none"></span>' +
                    '</div>' +
                    '<div class="automapping" style="display: none">' +
                        '<div class="automapping-result" style="display:none">' +
                            '<p class="warning-message"/>' +
                            '<div class="separator"/>' +
                        '</div>' +
                    '</div>' +
                    '<ul class="entities-block">' +
                        '<li><p class="label pt-10"> Work Items Entities </p></li>' +
                        '<li class="chain-no"></li>' +
                        '<li><p class="label pt-10"> TargetProcess Entities</p></li>' +
                    '</ul>' +
                    '<div class=\'entities-blocks\'></div>' +
                '</div>' +
                '<div class="p-15">' +
                    '<a href="#" class="add-more" id="addMoreEntities">Add more entities</a>' +
                '</div>',

	        _create: function (config) {
	            this.placeHolder = config.placeHolder;
	            this.model = config.model;
	            this.controller = config.controller;
	            Bus.subscribe("TfsEntityMappingEditor", {
	                onProfileSaveSucceed: $.proxy(this._onSaveSucceed, this)
	            }, true);
	        },

	        _onSaveSucceed: function () {
	            var tfsSelectElements = this.placeHolder.find('.tfs-entity');
	            var tpSelectElements = this.placeHolder.find('.tp-entity');

	            for (var i = 0; i < tfsSelectElements.length; i++) {
	                if (tfsSelectElements[i].selectedIndex == 0 || tpSelectElements[i].selectedIndex == 0)
	                    continue;

	                tfsSelectElements[i].disabled = true;
	                tpSelectElements[i].disabled = true;
	            }
	        },

	        onProjectMappingChanged: function (profileConfig) {
	            var configToSerialize = profileConfig;
	            var workItemsEntities = null;
	            var tpProject = null;
	            var that = this;

	            $.ajax({
	                type: 'POST',
	                url: new Tp.WebServiceURL('/api/v1/Plugins/TFS/Commands/GetWorkItemsEntities').url,
	                dataType: 'json',
	                processData: false,
	                contentType: 'application/json',
	                data: JSON.stringify(configToSerialize)
	            }).done(function (data) { workItemsEntities = data; that._entitiesLoaded(workItemsEntities, tpProject); });

	            var tpProjectId = profileConfig.Settings.ProjectsMapping[0].Value.Id;
	            var uri = new Tp.WebServiceURL('/api/v1/projects/' + tpProjectId + '/?include=[Process[Practices]]').url;
	            $.ajax({
	                url: uri,
	                type: 'GET',
	                dataType: "json"
	            }).done(function (data) { tpProject = data; that._entitiesLoaded(workItemsEntities, tpProject); });
	        },

	        _entitiesLoaded: function (workItemsEntities, tpProject) {
	            if (workItemsEntities == null || tpProject == null)
	                return;

	            var tpEntities = ['Feature', 'User Story', 'Bug', 'Request'];
	            var entitiesForCheck = [{ Id: 3, Name: 'Bug' }, { Id: 4, Name: 'Feature' }, { Id: 8, Name: 'Request'}];
	            $.each(
                    entitiesForCheck,
                    function (index, value) {
                        for (var i = 0; i < tpProject.Process.Practices.Items.length; i++)
                            if (tpProject.Process.Practices.Items[i].Id == value.Id)
                                return;

                        tpEntities.splice(tpEntities.indexOf(value.Name), 1);
                    });

	            this._renderEmpty();

	            this.entities = { WorkItemsEntities: workItemsEntities, TpEntities: tpEntities };

	            this._renderEntityMappingModel(this.entities);
	            this._completeRendering();
	        },

	        render: function () {
	            this._renderEmpty();

	            if (this.controller.isEditMode())
	                this.onProjectMappingChanged(this.model);
	            else
	                this._renderEntityMappingModel(null);

	            this._completeRendering();
	        },

	        _renderEmpty: function () {
	            this.lines = [];
	            this.placeHolder.html('');

	            this.rendered = $.tmpl(this.entityMappingEditorTemplate, {});
	        },

	        _renderEntityMappingModel: function () {
	            if (this.entities)
	                this._renderEntityMappingLines({ Entities: this.entities, Mapping: this.model.Settings.EntityMapping });
	            else
	                this._addMoreEntities();
	        },

	        _completeRendering: function () {
	            this.rendered.appendTo(this.placeHolder);

	            this.rendered.find('#addMoreEntities').click($.proxy(this._addMoreEntities, this));
	            this.progressIndicator = this.rendered.find('span.preloader');
	            this.rendered.find(".automapping .button").click($.proxy(this._onAutomapping, this));

	            this.rendered.find('#howItWorksLink').click(function () {
	                $('#howItWorksDescription').animate({ opacity: 'toggle', height: 'toggle' }, 'slow');
	            });
	        },

	        _addMoreEntities: function () {
	            this._renderEntityMappingLines({ Entities: this.entities, Mapping: null });
	            return false;
	        },

	        _renderEntityMappingLines: function (data) {
	            var entityMappingLineEditors = null;
	            var that = this;

	            if (data.Mapping && data.Mapping.length > 0) {
	                entityMappingLineEditors = $(data.Mapping).map(
                        function () { return new EntityMappingLine({ model: that, data: data, mappingPair: this }) });
	            }
	            else {
	                var filteredData = data;

	                if (this.entities) {
	                    var notMappedTfsEntities = this.getNotMappedTfsEntities();

	                    if (!notMappedTfsEntities || notMappedTfsEntities.length == 0)
	                        return;

	                    filteredData.Entities = { WorkItemsEntities: notMappedTfsEntities, TpEntities: data.Entities.TpEntities };
	                }

	                entityMappingLineEditors = [new EntityMappingLine({ model: this, data: filteredData, mappingPair: null })];
	            }

	            var container = this.rendered.find('.entities-blocks');

	            $.each(entityMappingLineEditors, $.proxy(function (key, value) {
	                this.lines.push(value);
	                value.render(container);
	            }, this));
	        },

	        onTfsSelectedEntityChanged: function (tfsSelect) {
	            var tfsControls = this.placeHolder.find('.tfs-entity');
	            var value = $(tfsSelect).find("option:selected")[0].text;

	            for (var i = 0; i < tfsControls.length; i++) {
	                if (tfsControls[i].disabled || tfsControls[i] == tfsSelect)
	                    continue;

	                for (var j = 0; j < tfsControls[i].length; j++) {

	                    if (tfsControls[i].options[j].text == value) {
	                        tfsControls[i].remove(j);
	                        break;
	                    }
	                }
	            }
	        },

	        getNotMappedTfsEntities: function () {
	            var notMappedTfsEntities = this.entities.WorkItemsEntities.slice();
	            var tfsControls = this.placeHolder.find('.tfs-entity');

	            for (var i = 0; i < tfsControls.length; i++) {
	                if (tfsControls[i].selectedIndex < 1)
	                    continue;

	                var value = $(tfsControls[i]).find("option:selected")[0].text;
	                if ($.inArray(value, notMappedTfsEntities) != -1)
	                    notMappedTfsEntities.splice(notMappedTfsEntities.indexOf(value), 1);
	            }

	            return notMappedTfsEntities;
	        },

	        getEntityMappings: function () {
	            this.dom = $.tmpl(this.entityMappingEditorTemplate);

	            var tfsControls = this.placeHolder.find('.tfs-entity');
	            var tpControls = this.placeHolder.find('.tp-entity');
	            var entitiesMapping = [];
	            for (var i = 0; i < tfsControls.length; i++) {
	                var tfsSelect = tfsControls[i];
	                var tpSelect = tpControls[i];

	                if (tfsSelect.selectedIndex < 1 || tpSelect.selectedIndex < 1)
	                    continue;

	                entitiesMapping.push(
                    {
                        First: tfsSelect.options[tfsSelect.selectedIndex].text,
                        Second: tpSelect.options[tpSelect.selectedIndex].text
                    });
	            }

	            return entitiesMapping;
	        },

	        clearErrors: function () {
	            this.placeHolder.find('#entityMappingError').hide().find('span').text('');
	        },

	        isValidToSave: function () {
	            return this.getEntityMappings().length > 0;
	        },

	        clientValidate: function () {
	            if (!this.isValidToSave()) {
	                this.placeHolder.find('#entityMappingError').show().find('span').text('At least entity mapping pair must be set');
	            }
	        }
	    };

	    function EntityMappingLine(config) {
	        this._create(config);
	    }

	    EntityMappingLine.prototype = {
	        entityMappingLineTemplate:
	            '<ul class="entities-block">' +
    	            '<li><select class="select tfs-entity"></select></li>' +
        	        '<li class="chain"></li>' +
           	        '<li><select class="select tp-entity"/></li>' +
                '</ul>',
	        removeIcon: $('<li class="remove-mapping"><a title="Remove mapping" class="remove" href="#"></a></li>'),

	        _create: function (config) {
	            this.model = config.model;
	            this.entities = config.data;
	            this.mappingPair = config.mappingPair;
	        },

	        render: function (container) {
	            this.dom = $.tmpl(this.entityMappingLineTemplate)
					.hover($.proxy(this.showRemoveIcon, this), $.proxy(this.hideRemoveIcon, this));

	            var tfsSelect = this.dom.find('.tfs-entity')[0];
	            var tpSelect = this.dom.find('.tp-entity')[0];

	            if (this.mappingPair) {
	                this.insertElements(tfsSelect, [this.mappingPair.First], true);
	                this.insertElements(tpSelect, [this.mappingPair.Second], false);
	            }
	            else if (this.entities.Entities) {
	                this.insertElements(tfsSelect, this.entities.Entities.WorkItemsEntities, true);
	                this.insertElements(tpSelect, this.entities.Entities.TpEntities, false);
	            }

	            if (tfsSelect.selectedIndex > 0 && tpSelect.selectedIndex > 0) {
	                tfsSelect.disabled = true;
	                tpSelect.disabled = true;
	            }
	            else {
	                var that = this;
	                tfsSelect.onchange = function () { that.model.onTfsSelectedEntityChanged(tfsSelect); };
	            }

	            this.dom.editorAnimation();
	            this.dom.editorAnimation('appendTo', container);
	        },

	        insertElements: function (selectElement, entities, isFirst) {
	            var that = this;

	            var defaultOption = document.createElement('option');
	            defaultOption.text = '- Select entity -';
	            defaultOption.selected = true;
	            selectElement.add(defaultOption);

	            $.each(
                    entities,
                    function (index, value) {
                        var option = document.createElement('option');
                        option.text = value;

                        if (that.mappingPair) {
                            var selectedValue = isFirst ? that.mappingPair.First : that.mappingPair.Second;
                            if (option.text == selectedValue)
                                option.selected = true;
                        }

                        selectElement.add(option);
                    });
	        },

	        remove: function (e) {
	            e.preventDefault();
	            this.dom.editorAnimation('remove');
	            this.dom = null;
	        },

	        showRemoveIcon: function () {
	            var tfsSelect = this.dom.find('.tfs-entity')[0];
	            var tpSelect = this.dom.find('.tp-entity')[0];

	            if (tfsSelect.disabled && tpSelect.disabled)
	                return;

	            this.removeIcon.find('a').unbind('click');
	            this.removeIcon.find('a').click($.proxy(this.remove, this));
	            this.removeIcon.appendTo(this.dom);
	        },

	        hideRemoveIcon: function () {

	            var removeElement = this.dom.find('.remove-mapping');

	            if (removeElement)
	                removeElement.remove();
	        }
	    };
	    return EntityMappingEditor;
	});
