define([
    'jQuery'
    , 'Underscore'
    , 'tp/codemirror/lib/codemirror'
    , 'tp/codemirror/mode/javascript/javascript'
], function ($, _, CodeMirror) {
        function CodeEditor(config) {
            this.textAreaSelector = config.textAreaSelector;
            this.placeholder = config.placeholder;
            this.initialize();
        };

        CodeEditor.prototype = {
            initialize: function(){
                this.scriptEditor = CodeMirror.fromTextArea(this.placeholder.find(this.textAreaSelector)[0], {
                    lineNumbers: true,
                    matchBrackets: true
                });

                this._getCodeEditorBlock().addClass('mashup-code-editor');
                this._getCodeMirrorBlock().addClass('mashup-code-block');
                this._getCodeGutterBlock().addClass('mashup-code-gutter');

                if (navigator.appName == "Microsoft Internet Explorer") {
                    this._getCodeEditorBlock().height(500);
                    this._getCodeEditorBlock().css('overflow-y', 'auto');
                }

                $(window).resize(_.bind(this._setCodeBlockWidth, this));

                $('#lnkHideReport').click(_.bind(this._setCodeBlockWidth, this));
                $('#lnkShowReport').click(_.bind(this._setCodeBlockWidth, this));
                $('#rptLinksClosed').click(_.bind(this._setCodeBlockWidth, this));

                this._setCodeBlockWidth();

                var refreshInterval = setInterval(_.bind(function(){
                    if ($('.mashup-code-block').is(':visible')){
                        this._setCodeBlockWidth();
                        this.scriptEditor.refresh();
                        clearInterval(refreshInterval)
                    }
                }, this), 100);
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
                this._getCodeEditorBlock().width(calculatedWidth - 40);
            }
        };

        return CodeEditor;
    });
