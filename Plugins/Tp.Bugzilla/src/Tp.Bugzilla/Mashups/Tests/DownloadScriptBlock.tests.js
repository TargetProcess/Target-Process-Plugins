require(["Bugzilla/DownloadScriptBlock"],
    function (DownloadScriptBlock) {
        module('test tp2.cgi download', {
            setup:function () {
                this.placeholder = $('<div class="additionalInfo"></div>');
                this.versions = ['3.4', '3.6', '4.0'];
                this.downloadScriptBlock = new DownloadScriptBlock({placeholder:this.placeholder, mashupPath:''});
                var that = this;
                this.downloadScriptBlock.commandGateway.execute = function (command, data, success) {
                    success(that.versions);
                }
            },

            teardown:function () {
            }
        });

        test('can be opened and valid links displayed', function () {
            this.downloadScriptBlock.open();
            stop();
            var that = this;
            setTimeout(function () {
                notEqual(that.placeholder.html(), '', 'block has been rendered');
                equal(that.placeholder.find('a.scriptUrl').length, that.versions.length, 'links to all versions displayed');
                $(this.versions).each(function () {
                    var link = that.placeholder.find('a.scriptUrl[href^="Scripts/' + this + '/tp2"]');
                    equal(link.length, 1, 'link for version ' + this + ' rendered');
                });
                start();
            }, 1000);
        });

        test('should be closed after close clicked', function () {
            this.downloadScriptBlock.open();
            var that = this;
            stop();
            setTimeout(function () {
                that.placeholder.find('a.close').click();
                setTimeout(function () {
                    equal(that.placeholder.html(), '', 'block has been closed');
                    start();
                }, 1000);
            }, 1000);
        });
    }
);
