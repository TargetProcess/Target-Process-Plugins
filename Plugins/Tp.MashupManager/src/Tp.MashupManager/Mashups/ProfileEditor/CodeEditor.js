tau.mashups
    .addDependency("libs/jquery/jquery")
    .addDependency("tp/codemirror/proxies/javascript")
    .addCSS("../../tau/scripts/tp/codemirror/src/lib/codemirror.css")
    .addCSS("../../tau/scripts/tp/codemirror/src/theme/default.css")
    .addModule("MashupManager/CodeEditor", function () {
        function codeEditor(config) {
            this._create(config);
        };

        codeEditor.prototype = {
            _create: function(config){
                this.textAreaSelector = config.textAreaSelector;
                this.expandButtonSelector = config.expandButtonSelector;
                this.placeholder = config.placeholder;

                this.maximized = false;
            },

            initialize: function(){
                this.scriptEditor = CodeMirror.fromTextArea(this.placeholder.find('#script')[0], {
                    lineNumbers: true,
                    matchBrackets: true
                });

                this.placeholder.find(this.expandButtonSelector).click($.proxy(this._onFullScreenClick, this));

                this._getCodeEditorBlock().addClass('mashup-code-editor');
                this._getCodeMirrorBlock().addClass('mashup-code-block');
                this._getCodeGutterBlock().addClass('mashup-code-gutter');
            },

            getValue: function(){
                return this.scriptEditor.getValue();
            },

            _onFullScreenClick: function(){
                if(this._maximized) {
                    this._setCodeSize(670,340);
                }
                else {
                    this._setCodeSize(800,800);
                }

                this._maximized = !this._maximized;
            },

            _setCodeSize: function(width, height){
                var mirror = this._getCodeMirrorBlock();
                mirror.width(width);
                mirror.height(height);

                var editor = this._getCodeEditorBlock();
                editor.width(width);
                editor.height(height);

                var gutter = this._getCodeGutterBlock();
                gutter.height(height);
            },

            _getCodeMirrorBlock: function(){
                return this.placeholder.find('div.CodeMirror');
            },

            _getCodeEditorBlock: function(){
                return this.placeholder.find('div.CodeMirror-scroll');
            },

            _getCodeGutterBlock: function(){
                return this.placeholder.find('div.CodeMirror-gutter');
            }
        };
        return codeEditor;
    });
