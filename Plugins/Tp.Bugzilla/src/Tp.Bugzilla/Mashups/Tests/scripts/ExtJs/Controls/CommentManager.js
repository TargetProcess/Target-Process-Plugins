Ext.ns('Tp.controls');

Tp.controls.CommentManager = Ext.extend(Object, {
    richeditorDiv: null,
    newcommentDiv: null,
    saveButton: null,
    cancelButton: null,
    commentIdStateHolder: null,
    richEditField: null,
    currentCommentDiv: null,
    currentContent: null,
    currentCommentId: null,
    txtCommentContentClientID: null,
    hiddenCommentId: null,
    richEditId: null,
    newCommentId: null,
    cancelId: null,

    constructor: function(txtCommentContentClientID, hiddenCommentId, richEditId, newCommentId, okId, cancelId) {
        Tp.util.validateForNulls([txtCommentContentClientID, hiddenCommentId, richEditId, newCommentId, cancelId]);
        this.txtCommentContentClientID = txtCommentContentClientID;
        this.hiddenCommentId = hiddenCommentId;
        this.richEditId = richEditId;
        this.newCommentId = newCommentId;
        this.cancelId = cancelId;
        Ext.get(okId).on({
            'mousedown': this.sync,
            'keydown': this.sync,
            scope: this});
    },

    destroy: function() {
        if (this.richEditField != null)
            this.richEditField.destroy();

        if (this.commentIdStateHolder != null)
            this.commentIdStateHolder.destroy();
    },

    sync: function() {
        this.richEditField.syncValue();
    },

    //rebind controls
    apply: function() {
        this.destroy();
        this.commentIdStateHolder = new Ext.form.Hidden({ applyTo: this.hiddenCommentId });
        this.commentIdStateHolder.setValue(null);
        this.richEditField = new Ext.form.HtmlEditor({
            applyTo: this.txtCommentContentClientID,
            enableFont: false,
            enableColors: false,
            enableAlignments: false,
            enableFontSize: false,
            enableFormat: true,
            width: 600, height: 250
        });
        this.richEditField.setValue("");
        this.richEditField.focus();
        this.richeditorDiv = Ext.get(this.richEditId);
        this.newcommentDiv = Ext.get(this.newCommentId);
        this.cancelButton = Ext.get(this.cancelId);
        this.cancelButton.hide();
        this.currentCommentDiv = null;
        this.currentContent = null;
        this.currentCommentId = null;
        //Fix HTML EDITOR FOR IE7
        this.richEditField.deferFocus();
    },

    /**
     * Paste new content and start edit process
     */
    pasteAndEdit: function(target, content) {
        commentManager.apply();
        commentManager.switchRichEdit(Ext.get(target), content, true, false);
    },

    /**
     * Replace comment content div with rich editor control
     */

    switchRichEdit: function(target, content, show, initFrame) {
        target.appendChild(this.richeditorDiv);

        if (!Ext.isIE && initFrame) {
            this.richEditField.initFrame();
        }
        this.richEditField.setValue(content);
        this.richEditField.focus();
        if (show)
            this.cancelButton.show();
        else
            this.cancelButton.hide();
    },

    beginEditComment: function(currentCommentId) {

        var commentDiv = Ext.get("comment_" + currentCommentId);
        if (commentDiv.contains(this.richeditorDiv)) {
            return;
        }
        var content = commentDiv.dom.innerHTML;
        commentDiv.dom.innerHTML = "";

        //show rich edit for current comment
        this.switchRichEdit(commentDiv, content, true, true);

        //restore value if present for previous comment
        if (this.currentCommentDiv != null) {
            this.currentCommentDiv.dom.innerHTML = this.currentContent;
        }

        //write down current edit state
        this.currentCommentDiv = commentDiv;
        this.currentContent = content;
        this.currentCommentId = currentCommentId;
        this.commentIdStateHolder.setValue(this.currentCommentId);
    },

    /*
     * Replace comment rich editor with static div, move rich editor to the new comment area.
     */
    cancelEditComment: function() {
        //show rich edit for new comment
        this.switchRichEdit(this.newcommentDiv, "", false, true);

        //clean up current edit state
        this.currentCommentId = null;
        this.commentIdStateHolder.setValue(null);
        if (this.currentCommentDiv != null) {
            this.currentCommentDiv.dom.innerHTML = this.currentContent;
            this.currentCommentDiv = null;
        }
    } // end cancelEditComment function
})