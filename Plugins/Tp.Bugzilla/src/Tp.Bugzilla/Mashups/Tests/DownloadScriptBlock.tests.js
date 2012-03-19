require(["Bugzilla/DownloadScriptBlock"],
    function (DownloadScriptBlock) {
        module('test tp2.cgi download', {
            setup:function () {
                this.placeholder = $('<div class="additionalInfo"></div>');
                this.versions = ['3.4', '3.6', '4.0'];
                var that = this;
                require('tp/plugins/commandGateway').prototype.execute =    function (command, data, success) {
                    success(that.versions);
                }
                this.downloadScriptBlock = new DownloadScriptBlock({placeholder:this.placeholder, mashupPath:''});
            },

            teardown:function () {
            }
        });

        test('can be opened and valid links displayed', function () {
            notEqual(this.placeholder.html(), '', 'block has been rendered');
            equal(this.placeholder.find('a.scriptUrl').length, this.versions.length, 'links to all versions displayed');
            var that = this;
            $(this.versions).each(function () {
                var link = that.placeholder.find('a.scriptUrl[href^="Scripts/' + this + '/tp2"]');
                equal(link.length, 1, 'link for version ' + this + ' rendered');
            });
        });
    }
);
