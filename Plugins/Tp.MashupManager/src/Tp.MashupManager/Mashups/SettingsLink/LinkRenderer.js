tau.mashups
    .addDependency("tp/menu/AdminMenuManager")
    .addDependency("tau/configurator")
    .addDependency("tp3/api/settings/v1")
    .addDependency("tau-intl")
    .addDependency("jQuery")
    .addMashup(function(menuManager, configurator, settingsApi, intl, $) {
        if (configurator.getFeaturesService().isEnabled('tp3Settings')) {
            settingsApi.getMenu().addItem({
                id: 'mashups',
                groupId: 'integration',
                text: intl.formatMessage('Mashups'),
                url: configurator.getApplicationPath() + '/Admin/EditProfileExt.aspx?PluginName=Mashup+Manager&hideLastAction=1&Placeholders=profileeditormashupmanager',
                isVisible: function() {
                    return configurator.getLoggedUser().isAdministrator;
                },
                order: 200
            });
        } else {
            $(function() {
                var menu = new menuManager(
                    {
                        itemText: 'Mashups',
                        itemUrl: '/Admin/EditProfileExt.aspx?PluginName=Mashup+Manager&Placeholders=profileeditormashupmanager',
                        identifyUrlPart: 'PluginName=Mashup+Manager',
                        classIdentificator: '_mashupsSettingLink'
                    }
                );

                menu.addItem();
            });
        }
    });
