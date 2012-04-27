tau.mashups
    .addDependency("tp/menu/AdminMenuManager")
    .addDependency("libs/jquery/jquery")
    .addDependency("libs/jquery/jquery.tmpl")
    .addMashup(function (menuManager) {
        var menu = new menuManager(
            {
            	itemText: 'Mashups',
                itemUrl: '/Admin/EditProfileExt.aspx?PluginName=Mashup+Manager&Placeholders=profileeditormashupmanager',
                identifyUrlPart: 'PluginName=Mashup+Manager',
                classIdentificator: '_mashupsSettingLink'
            }
        );

        menu.addItem();
    })