tau.mashups
    .addDependency('tp/plugins/commandGateway')
    .addDependency('libs/jquery/jquery')
    .addDependency('libs/jquery/jquery.tmpl')
    .addModule('Bugzilla/DownloadScriptBlock', function (CommandGateway, $) {
        function DownloadScriptBlock(config) {
            this._ctor(config);
        }

        DownloadScriptBlock.prototype = {
            template: '<div class="downloadScript info-message" style="display:none;">\n    <p class="pb-5">tp2.cgi scripts for Bugzilla:&nbsp;<a href="#" class="close"></a></p>\n    {{each versions}}\n        <a class="scriptUrl ml-5" href="${url}">v.${version}</a><br>\n    {{/each}}\n    <p class="pt-5">Note: Download and extract tp2.cgi file to Bugzilla root folder</p>\n</div>',
            block: null,

            _ctor:function (config) {
                this.placeholder = config.placeholder;
                this.mashupPath = config.mashupPath;
                this.commandGateway = new CommandGateway();
            },

            open: function(){
                this.getSupportedVersions($.proxy(this.render, this));
            },

            close: function(){
                var that = this;
                if (this.block){
                    this.block.hide('fast', function(){
                        $(that.block).remove();
                    })
                }
            },

            render: function(versions){
                var that = this;
                var data = $(versions).map(function(index, version){
                    return {version:version, url: that.getScriptUrl(version)};
                });
                this.block = $.tmpl(this.template, {versions: data});
                this.placeholder.html('');
                this.block.appendTo(this.placeholder);
                this.block.find('a.scriptUrl').click(function(e){
                    e.preventDefault();
                    window.open($(this).attr('href'));
                });
                this.block.find('a.close').click(function(e){
                    e.preventDefault();
                    that.close();
                });
                this.block.slideDown('fast');
            },

            getSupportedVersions:function (success) {
                this.commandGateway.execute('GetSupportedVersions', null, success);
            },

            getScriptUrl:function (version) {
                return new Tp.URL(this.mashupPath + 'Scripts/' + version + '/tp2.cgi.zip').url;
            }
        };

        return DownloadScriptBlock;
    });