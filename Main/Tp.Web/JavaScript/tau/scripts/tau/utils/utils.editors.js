define(["require","tau/utils/utils.htmlConverter","tau/const/entityType.names","tau/utils/utils.constants"],function(t){var e=t("tau/utils/utils.htmlConverter"),n=t("tau/const/entityType.names"),u=t("tau/utils/utils.constants").get("editorType");return{detectEditorType:function(t,i,s){var r=t.DefaultRichEditor||u.HTML,a=r;return r!==u.MARKDOWN||e.isMarkdown(i)||(a=u.HTML),s===n.REQUEST&&(a=u.HTML),a}}});