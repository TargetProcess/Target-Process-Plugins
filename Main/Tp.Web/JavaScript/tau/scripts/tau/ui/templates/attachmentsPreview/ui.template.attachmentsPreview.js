define(["tau/core/templates-factory","tau/ui/templates/attachmentsPreview/ui.template.attachmentsPreview.attachmentThumbnail"],function(a){var b={name:"attachments-preview",markup:['<div class="ui-attachments-preview-control">','<div class="toolbar">','<button class="close" type="button">X</button>','<button class="prev" type="button">&larr;</button>','<button class="next" type="button">&rarr;</button>',"</div>",'   <div class="swap-view">','    <div class="image-view">','       <div class="ui-attachment-wait"></div>','       <img class="ui-attachment-view-image" src="${selected.uri}">',"    </div>","   </div>",'   <ul class="thumbnail-nav">','       {{tmpl(items) "attachments-preview-attachment-thumbnail"}}',"   </ul>","</div>"].join(""),dependencies:["attachments-preview-attachment-thumbnail"]};return a.register(b)})