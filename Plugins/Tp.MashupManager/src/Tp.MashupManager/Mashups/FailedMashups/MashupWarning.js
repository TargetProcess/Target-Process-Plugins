tau.mashups
    .addDependency("jQuery")
    .addDependency("Underscore")
    .addDependency("tp/mashups/failed")
    .addMashup(function ($, _, failedMashups) {

        if (!loggedUser || !loggedUser.isAdministrator) {
            return;
        }

        if (_.size(failedMashups) > 0) {
            var values = _(failedMashups).keys().join("<br/>");

            var message = "<div style='padding: 20px; font-size: 1.2em;'>Looks like there're  <span style='color:red'>syntax errors in some mashups</span>. The following javascript sources are not loaded:"
                + "<div style='font-family: Courier New; padding: 10px;'>" + values + "</div>"
                + "<a href='?debug=1'>Switch</a> into the debug mode and check browser errors</div>";

            var $bubble = $('<li><div class="impBadge" style="width: 16px; opacity: 0.8; line-height: 16px; color:white;'
                + 'font-size:1.1em; font-weight:bold; text-align: center; cursor: pointer;'
                + 'height: 16px; border-radius: 17px;'
                + '-moz-box-shadow: inset 0 1px 1px 0 #830000;-webkit-box-shadow: inset 0 1px 1px 0 #830000;'
                + 'top:3px;right:3px; background-color:#9e0b0f;text-shadow: 0 1px 1px #830000;">!</div></li>');

            $bubble.tauBubble({
                content: message,
                zIndex: 9999
            });

            $('.user-sub').prepend($bubble);

        }
    });