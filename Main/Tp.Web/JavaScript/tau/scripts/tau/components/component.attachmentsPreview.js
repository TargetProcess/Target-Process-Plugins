define(["tau/components/component.creator","tau/models/model.attachments","tau/ui/templates/attachmentsPreview/ui.template.attachmentsPreview","tau/ui/extensions/attachmentsPreview/ui.extension.attachmentsPreview","tau/ui/extensions/attachmentsPreview/ui.extension.attachmentsPreview.animations"],function(a,b,c,d,e){return{create:function(f){var g=function(a){return(a.mimeType||"").indexOf("image/")==0},h={ModelType:b,template:c,extensions:[d,e]};f.filter=f.filter||g;return a.create(h,f)}}})