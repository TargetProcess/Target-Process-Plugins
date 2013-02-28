define(["Underscore","tau/core/extension.base","tau/models/tags/tagsProcessor"],function(_,ExtensionBase,TagsProcessor){return ExtensionBase.extend({"bus beforeInit":function(evt,initData){var configurator=initData.config.context.configurator;this.fire("configurator.ready",configurator),this.fire("dataBind",{}),this.fire("tagsProcessor.ready",new TagsProcessor);var options=_.defaults(initData.config.options||{},{groupName:"boardTemplates"});this.fire("options.ready",options)},"bus boardSettings.ready:last + form.submitted":function(evt,bs){var boardSettings=bs.boardSettings;boardSettings.get({fields:["cells","x","y","zoomLevel","viewMode"],callback:_.bind(function(r){this.fire("boardData.ready",r)},this)})},"bus options.ready:last + configurator.ready:last + tagsProcessor.ready:last + boardData.ready + form.submitted":function(evt,options,configurator,tagsProcessor,boardData,formData){var storage=configurator.getRestStorage(),templateData={name:_.asString(formData.name).trim().slice(0,50),description:_.asString(formData.description).trim().slice(0,255),tags:tagsProcessor.asArray(tagsProcessor.asArray(formData.tags).concat(["custom"])),definition:_.pick(boardData,"cells","x","y","zoomLevel","viewMode"),numericPriority:(new Date).getTime()};templateData.name.length?storage.data(options.groupName,undefined,{scope:"Public",publicData:templateData}).done(_.bind(function(res){this.fire("form.processed",res)},this)):this.fire("form.failed",{name:"Should not be empty"})}})})