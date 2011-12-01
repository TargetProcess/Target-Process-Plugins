require(["MashupManager/MashupManagerEditor"],
    function (editor) {
    	(function () {
    		module('check bugzilla repository', {
    			setup: function () {
    				this.managerEditor = new editor({placeholder: $('<div></div>')});
    			},

    			teardown: function () {

    			}
    		});

    		test('first test', function () {
    			ok(this.managerEditor != null, 'manager editor loaded');
    		});
    	})();
    });