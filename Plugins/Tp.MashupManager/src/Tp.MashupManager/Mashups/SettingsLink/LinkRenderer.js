tau.mashups
    .addDependency("tp/menu/AdminMenuManager")
    .addDependency("jQuery")
    .addMashup(function (menuManager, $) {
        $(function() {
            var menu = new menuManager(
                {
                    itemText: 'Mashups',
                    itemUrl: '/Admin/EditProfileExt.aspx?PluginName=Mashup+Manager&Placeholders=profileeditormashupmanager' + (window.location.href.indexOf('rmnav') == -1 ? '' : '&rmnav=1'),
                    identifyUrlPart: 'PluginName=Mashup+Manager',
                    classIdentificator: '_mashupsSettingLink'
                }
            );

            menu.addItem();
        });
    });