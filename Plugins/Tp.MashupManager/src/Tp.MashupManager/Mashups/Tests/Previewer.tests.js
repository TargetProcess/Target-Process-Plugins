require(["MashupManager/Previewer"],
    function (previewer) {
    	(function () {
    		module('check url for preview is valid', {
    			setup: function () {
    				this.previewer = new previewer({});

    				this.endsWith = function(str, suffix) {
    					return str.indexOf(suffix, str.length - suffix.length) !== -1;
    				};
    			},

    			teardown: function () {

    			}
    		});

    		test('check preview is valid for footer placeholder', function () {
    			var url = this.previewer._getUrl('footerplaceholder');

    			ok(this.endsWith(url, '/Default.aspx'), 'default page should be opened, have "' + url + '" as url, expected "/Default.aspx"');
    		});

    		test('check preview is valid for placeholder contained footerplaceholder', function () {
    			var url = this.previewer._getUrl('Projects, footerplaceholder');

    			ok(this.endsWith(url, '/Default.aspx'), 'default page should be opened, have "' + url + '" as url, expected "/Default.aspx"');
    		});

    		test('check preview is valid for complex placeholder', function () {
    			var url = this.previewer._getUrl('User_List, Projects');

    			ok(this.endsWith(url, 'User/List.aspx'), 'first page should be opened, have "' + url + '" as url, expected "User/List.aspx"');
    		});
    	})();
    });