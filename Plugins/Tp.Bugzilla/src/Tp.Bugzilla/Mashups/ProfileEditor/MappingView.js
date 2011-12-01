tau.mashups
    .addDependency("Bugzilla/MappingTemplates")
    .addDependency("libs/jquery/jquery")
    .addDependency("Bugzilla/ui.widgets")
    .addModule("Bugzilla/MappingView", function (mappingTemplate, $) {
        function mappingView(config) {
            this._create(config);
        };

        mappingView.prototype = {
            template: null,
            placeholder: null,
            rendered: null,
            lines: null,
            key: null,

            _create: function (config) {
                this.placeholder = config.placeholder;
                this.key = config.key;
                this.mappingTemplate = mappingTemplate[config.mappingTemplate];
                this.mappingConfig = {
                    Key: config.key,
                    Caption: config.Caption,
                    Description: config.Description,
                    KeyName: config.KeyName,
                    ValueName: config.ValueName
                };
                this.onAutomap = config.onAutomap;
            },
            
            initialize: function () {
                this.placeholder.find('#automap').click(this.onAutomap);
            },

            render: function (selector) {
                var that = this;
                return function (data) {
                    that.rendered = $.tmpl(that.mappingTemplate.mappingTemplateName, that.mappingConfig);
                    that._renderMappingLines(data || []);
                    that.placeholder.find(selector).html('');
                    that.rendered.appendTo(that.placeholder.find(selector));
                };
            },

            _renderMappingLines: function (mappings) {
                this.lines = [];
                var container = this.rendered.find('.mappings-blocks');
                container.html('');

                var that = this;
                var mappingLineEditors = $(mappings).map(function () {
                    return new mappingLine({ model: this, mappingTemplate: that.mappingTemplate });
                });

                var lines = this.lines;

                mappingLineEditors.each(function () {
                    lines.push(this);
                    this.render(container);
                });
            },

            getMappings: function () {
                var mappings = $(this.lines).map(function () {
                    return this.getMapping();
                });

                return $.grep(mappings, function (mapping) {
                    return (mapping).Key != '' && (mapping).Value != '';
                });
            }
        };

        function mappingLine(config) {
            this._create(config);
        }

        mappingLine.prototype = {
            _create: function (config) {
                this.model = config.model;
                this.mappingTemplate = config.mappingTemplate;
            },

            render: function (container) {
                this.dom = $.tmpl(this.mappingTemplate.lineTemplateName, this.model);

                this.dom.editorAnimation();
                this.dom.editorAnimation('appendTo', container);
            },

            getMapping: function () {
                if (this.dom) {
                    var valueElement = this.dom.find('.tpvalue');
                    var valueId = valueElement.val();
                    var value = valueId == 0
                        ? {}
                        : { Id: valueId, Name: valueElement.find('option:selected').text() };

                    return {
                        Key: this.dom.find('.externalvalue').attr('value'),
                        Value: value
                    };
                }
            }
        };

        return mappingView;
    });
