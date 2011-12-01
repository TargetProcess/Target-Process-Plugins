var tests = tests || {};
tests.testRunner = function(config) {
    this.config = config;
};

tests.testRunner.prototype = {
    run:function() {
        var self = this;

        $(".progress-bar").progressbar({
            min:0,
            value:0,
            max:self.config.paths.length
        });

        var $frame = $('#' + self.config.frameId);
        self.config.$frame = $frame;
        $frame.bind('load', function () {
            self.listenFrame($frame);
        });

        self.startNextTest();
    },

    listenFrame:function(frame) {
        var self = this;

        $(frame[0].contentDocument).ready(function() {
            var QUnit = frame[0].contentWindow.QUnit;
            if (!QUnit) {
                self.incrimentProgressbar();
                self.startNextTest();
                return;
            }
            var oldLog = QUnit.log;
            var oldModuleDone = QUnit.moduleDone;
            var oldModuleStart = QUnit.moduleStart;

            var oldTestStart = QUnit.testStart;
            var oldTestDone = QUnit.testDone;

            var oldDone = QUnit.done;

            QUnit.log = function(result, message) {
                oldLog.apply(this, arguments);
                self.log.apply(self, arguments)
            };


            QUnit.moduleStart = function() {
                oldModuleStart.apply(this, arguments);
                self.moduleStart.apply(self, arguments);
            };

            QUnit.moduleDone = function() {
                oldModuleDone.apply(this, arguments);
                self.moduleDone.apply(self, arguments);
            };

            QUnit.testStart = function() {
                oldTestStart.apply(this, arguments);
                self.testStart.apply(self, arguments);
            };

            QUnit.testDone = function() {
                oldTestDone.apply(this, arguments);
                self.testDone.apply(self, arguments);
            };

            QUnit.done = function() {
                oldDone.apply(this, arguments);
                self.done.apply(self, arguments);
            };
        });
    },

    moduleStart:function(name) {
        var path = this.config.paths[this.currentTestIndex];
        var moduleHtml = [
            '<div class="module">',
            '   <div class="result"></div>',
            '   <div class="module-name"><a href="',path,'"  target="_blank" ><strong>',name,'</strong></a></div>',
            '   <div class="module-tests"></div>',
            '</div>'].join('');
        this.currentModule = $(moduleHtml);
        $('.modules').append(this.currentModule);
    },

    log:function(result, message) {
        var clsName = result ? 'success' : 'failed';
        $log = $(['<li class="',clsName,'">',message,'</li>'].join(''))
        this.currentTest.find('.test-log').append($log);
    },

    testStart:function(name) {
        var path = this.config.paths[this.currentTestIndex];

        this.currentTest = $([
            '<div class="test">',
            '   <div class="test-result"></div>',
            '   <div class="collapse expand"></div>',
            '   <div class="test-name"><a target="_blank" href="',path,'?',name,'"> ',name,'</a></div>',
            '   <div class="test-log-container" style="display:none;"><ol class="test-log"></ol></div>',
            '</div>'].join(''));
        this.currentModule.find('.module-tests').append(this.currentTest);

        this.currentTest.find('.expand').click(function() {
            $(this).toggleClass('expand');
            $(this).parent().find('.test-log-container').toggle();
        });
    },

    testDone:function(name, failures, total) {
        if (failures > 0) {
            this.currentTest.addClass('failed-test');
            $(".progress-bar").find('.ui-progressbar-value').css('background-color', '#EE5757');
        }
        else {
            this.currentTest.addClass('success-test');
        }

    },

    incrimentProgressbar:function () {
        var $bar = $(".progress-bar");
        var value = $bar.progressbar('option', 'value');
        $bar.progressbar('option', 'value', value + 1);
    },
    done:function() {

        this.incrimentProgressbar();
        this.startNextTest();
    },

    moduleDone:function(name, failures, total) {
        if (!this.currentModule) {
            return;
        }

        if (failures > 0) {
            this.currentModule.addClass('failed-module');
        }
        else {
            this.currentModule.addClass('success-module');
        }

        delete this.currentModule;
    },

    startNextTest:function() {
        var self = this;

        if (!self.hasOwnProperty('currentTestIndex')) {
            self.currentTestIndex = 0;
        }
        else {
            self.currentTestIndex = self.currentTestIndex + 1;
        }

        if (self.currentTestIndex >= self.config.paths.length) {
            return;
        }

        var path = self.config.paths[self.currentTestIndex];
        var src = [$.trim(path), '#', (+(new Date()))].join('');

        self.config.$frame.attr('src', src);
    }

};