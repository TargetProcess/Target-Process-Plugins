define(["tau/components/component.creator","tau/models/tagsCloud/model.tagsCloud","tau/ui/extensions/tagsCloud/ui.extension.tagsCloud.editable","tau/ui/extensions/tagsCloud/ui.extension.tagsCloud.filter","tau/ui/templates/tagsCloud/ui.template.tagsCloud"],function(Creator,Model,ExtensionEditable,ExtensionFilter,Template){return{create:function(config){var creatorConfig={extensions:[Model,ExtensionEditable,ExtensionFilter],template:Template};return Creator.create(creatorConfig,config)}}})