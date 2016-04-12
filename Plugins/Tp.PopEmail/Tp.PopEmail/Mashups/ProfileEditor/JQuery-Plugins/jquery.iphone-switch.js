tau.mashups
    .addDependency("libs/jquery/jquery")
    .addModule("emailIntegration/jquery/iphone-switch",

function($) {

    /************************************************ 
    *  jQuery iphoneSwitch plugin                   *
    *                                               *
    *  Author: Daniel LaBare                        *
    *  Date:   2/4/2008                             *
    ************************************************/

    $.fn.iphoneSwitch = function (start_state, switched_on_callback, switched_off_callback, options) {
        // define default settings
        var iphoneSwitchClass = function ($element, start_state, switched_on_callback, switched_off_callback, options) {
            var container, image, self = this, $this = this.$element = $element;

            if (!$this.data('state')) {
                $this.data('state', start_state == 'on' ? 'on' : 'off');
            };

            // make the container
            container = $('<div class="iphone_switch_container" style="height:' + settings.switch_height + 'px; width:' + settings.switch_width + 'px; position: relative; overflow: hidden"></div>');

            // make the switch image based on starting state
            image = $('<img class="iphone_switch" style="height:' + settings.switch_height + 'px; width:' + settings.switch_width + 'px; background-image:url(' + settings.switch_path + '); background-repeat:none; background-position:' + (self.getState() == 'on' ? 0 : -32) + 'px" src="' + (self.getState() == 'on' ? settings.switch_on_container_path : settings.switch_off_container_path) + '" />');

            // insert into placeholder
            $this.html($(container).html($(image)));

            $this.mouseover(function () {
                $this.css("cursor", settings.mouse_over);
            });

            $this.mouseout(function () {
                $this.css("background", settings.mouse_out);
            });

            // click handling
            $this.click(function () {
                var isOn = self.getState() == 'on';
                $this.find('.iphone_switch').animate({ backgroundPosition: isOn ? -32 : 0 }, 'fast', function () {
                    $(this).attr('src', isOn ? settings.switch_off_container_path : settings.switch_on_container_path);
                    isOn ? switched_off_callback(this) : switched_on_callback(this);
                });
                $this.data('state', state = isOn ? 'off' : 'on');
            });
        };

        iphoneSwitchClass.prototype = {
            getState: function () { return this.$element.data('state') }
        };

        var settings = {
            mouse_over: 'pointer',
            mouse_out: 'default',
            switch_on_container_path: 'iphone_switch_container_on.png',
            switch_off_container_path: 'iphone_switch_container_off.png',
            switch_path: 'iphone_switch.png',
            switch_height: 25,
            switch_width: 65
        };

        if (options) {
            $.extend(settings, options);
        }

        // create the switch
        return this.each(function () {
            var $this = $(this);
            var callback = start_state;
            if ($.isFunction(callback)) {
                var instance = $this.data('_iphoneSwitcher');
                callback.call(instance);
            }
            else {
                var iphoneSwitchInstance = new iphoneSwitchClass($this, start_state, switched_on_callback, switched_off_callback, options);
                $this.data('_iphoneSwitcher', iphoneSwitchInstance);
            }
        });
    }
}
);