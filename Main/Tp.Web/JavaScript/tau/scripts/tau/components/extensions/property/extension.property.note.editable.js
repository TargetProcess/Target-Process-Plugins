define(["Underscore","jQuery","tau/components/extensions/property/extension.lightweight.richeditor.base","tau/components/extensions/property/note.hint.definitions","tau/utils/utils.htmlConverter"],function(t,e,i,n,o){var s="note-editor",r=i.extend({currentHintDefinition:n.DEFAULT_HINT,noteRoleSelector:".i-role-property","bus afterRender":function(t,e){this._super(t,e),this.$element=e.element,this.$editor=this._initializeEditor(this.$element),this.initialNote=t.data.data.value,this.initialNote||this._toggleNoteHint(this.$editor,!0)},"bus afterSave":function(t,e){var i=this._getCkEditor(this.$editor),n=e.data.comment;0==n.length&&this._toggleNoteHint(this.$editor,!0),i.resetDirty()},"bus note.hint.changed":function(t,e){var i=e.newHintDefinition;i!=this.currentHintDefinition&&this._validateNoteHint(i)&&this._changeNoteHint(this.$editor,i)},_getComponentSelector:function(){return this.noteRoleSelector},_bindHandlers:function(t){this._super(t);var e=this._getEditor(t);e.on("blur."+s,this._onFocusOut.bind(this,e)),e.on("focus."+s,this._onFocusIn.bind(this,e))},_unbindHandlers:function(t){this._super(t);var e=this._getEditor(t);e.off(s)},_validateNoteHint:function(e){return t.contains(t.values(n),e)},_save:function(t){var e=this._getCkEditor(t),i=this._getNote();e.checkDirty()?this.fire("saveNote",{comment:i}):0==i.length&&this._toggleNoteHint(t,!0),this._toggleFullPreviewMode(this.$element,!0)},_onFocusIn:function(t){this._toggleNoteHint(t,!1),this._toggleFullPreviewMode(this.$element,!1)},_onFocusOut:function(t){var e=this._getCkEditor(t);e.checkDirty()&&this.fire("model.note.changed",{comment:this._getNote()})},_onCKEditorBlur:function(t){this._save(t)},_getNote:function(){var t=this._getCkEditor(this.$editor);if(t){var e=t.getData()||"";return o.fromHtmlToSource(e.trim())}return this.initialNote},_changeNoteHint:function(t,e){this._toggleNoteHint(t,!1),this.currentHintDefinition=e;var i=this._getNote();0==i.length&&this._toggleNoteHint(t,!0)},_toggleNoteHint:function(t,e){t.toggleClass(this.currentHintDefinition.className,e)
}});return r});