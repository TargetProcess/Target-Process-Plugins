define(["tau/components/component.creator","tau/models/board.context.filter/model.board.context.filter.data","tau/models/board.context.filter/model.board.context.filter.add.project","tau/models/board.context.filter/model.board.context.filter.add.team","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.main","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.filter","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.selection","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.add.project","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.add.team","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.members-widget","tau/ui/extensions/board.context.filter/ui.extension.board.context.filter.viewer","tau/components/extensions/error/extension.errorBar","tau/ui/templates/board.context.filter/ui.template.board.context.filter"],function(ComponentCreator,ModelMain,ModelAddProject,ModelAddTeam,ExtensionMain,ExtensionFilter,ExtensionSelection,ExtensionAddProject,ExtensionAddTeam,ExtensionMembers,ExtensionViewer,ExtensionError,Template){return{create:function(config){config["queue.bus"]=!0;var creatorConfig={"queue.bus":!0,extensions:[ModelMain,ModelAddProject,ModelAddTeam,ExtensionMain,ExtensionFilter,ExtensionSelection,ExtensionAddProject,ExtensionAddTeam,ExtensionMembers,ExtensionViewer,ExtensionError],template:Template};return ComponentCreator.create(creatorConfig,config)}}})