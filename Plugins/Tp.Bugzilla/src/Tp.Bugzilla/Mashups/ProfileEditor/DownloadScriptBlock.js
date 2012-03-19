tau.mashups
    .addDependency('tp/plugins/commandGateway')
    .addDependency('libs/jquery/jquery')
    .addDependency('libs/jquery/jquery.tmpl')
    .addModule('Bugzilla/DownloadScriptBlock', function (CommandGateway, $) {
        function DownloadScriptBlock(config) {
            this._ctor(config);
        }

        DownloadScriptBlock.prototype = {
            template: '<div class="downloadScript">\n    {{each versions}}\n        <a class="scriptUrl" href="${url}">v.${version}</a><br>\n    {{/each}}\n    <p class="note pt-5"><span class="small">Note: Download and extract tp2.cgi file to Bugzilla root folder</span></p>\n</div>',
            block: null,

            _ctor:function (config) {
                this.placeholder = config.placeholder;
                this.mashupPath = config.mashupPath;
                this.commandGateway = new CommandGateway();
                this.getSupportedVersions($.proxy(this.render, this));
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