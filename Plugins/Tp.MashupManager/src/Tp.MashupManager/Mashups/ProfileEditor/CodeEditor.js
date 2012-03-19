tau.mashups
    .addDependency("libs/jquery/jquery")
    .addDependency("tp/codemirror/lib/codemirror")
    .addDependency("tp/codemirror/mode/javascript/javascript")
    .addCSS("../../tau/scripts/tp/codemirror/lib/codemirror.css")
    .addCSS("../../tau/scripts/tp/codemirror/theme/default.css")
    .addModule("MashupManager/CodeEditor", function ($, CodeMirror) {
        function codeEditor(config) {
            this._create(config);
        };

        codeEditor.prototype = {
            _create: function(config){
                this.textAreaSelector = config.textAreaSelector;
                this.placeholder = config.placeholder;
            },

            initialize: function(){
                this.scriptEditor = CodeMirror.fromTextArea(this.placeholder.find('#script')[0], {
                    lineNumbers: true,
                    matchBrackets: true
                });

                this._getCodeEditorBlock().addClass('mashup-code-editor');
                this._getCodeMirrorBlock().addClass('mashup-code-block');
                this._getCodeGutterBlock().addClass('mashup-code-gutter');

                if (navigator.appName == "Microsoft Internet Explorer") {
                	this._getCodeMirrorBlock().height(500);
                	
                	this._getCodeEditorBlock().height(500);
                	this._getCodeEditorBlock().css('overflow-y', 'auto');
                }

                $(window).resize($.proxy(this._setCodeBlockWidth, this));
                this._setCodeBlockWidth();

                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_pageLoaded($.proxy(this._setCodeBlockWidth, this));

                $('#lnkHideReport').click($.proxy(this._setCodeBlockWidth, this));
                $('#lnkShowReport').click($.proxy(this._setCodeBlockWidth, this));
                $('#rptLinksClosed').click($.proxy(this._setCodeBlockWidth, this));

                this.scriptEditor.refresh();
            },

            getValue: function(){
                return this.scriptEditor.getValue();
            },

            _getCodeMirrorBlock: function(){
                return this.placeholder.find('div.CodeMirror');
            },

            _getCodeEditorBlock: function(){
                return this.placeholder.find('div.CodeMirror-scroll');
            },

            _getCodeGutterBlock: function(){
                return this.placeholder.find('div.CodeMirror-gutter');
            },

            _setCodeBlockWidth: function(){
               var etalonLeft = this.placeholder.find(('p.label:contains("Code")')).position().left;

                var screenWidth = $(window).width();
                var calculatedWidth = screenWidth - etalonLeft;

                $('#mashupDetails').width(calculatedWidth - 20);
                this._getCodeEditorBlock().width(calculatedWidth - 40);
                this._getCodeMirrorBlock().width(calculatedWidth - 40);
                
            }
        };
        return codeEditor;
    });
