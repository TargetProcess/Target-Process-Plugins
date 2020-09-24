tau.mashups
    .addDependency("tp/menu/AdminMenuManager")
    .addDependency("tau/configurator")
    .addDependency("tp3/api/settings/v1")
    .addDependency("tau-intl")
    .addDependency("jQuery")
    .addMashup(function(menuManager, configurator, settingsApi, intl, $) {
        settingsApi.getMenu().addItem({
            id: 'mashups',
            groupId: 'configuration',
            text: intl.formatMessage('Mashups'),
            url: configurator.getApplicationPath() + '/Admin/EditProfileExt.aspx?PluginName=Mashup+Manager&hideLastAction=1&Placeholders=profileeditormashupmanager',
            isVisible: function() {
                return configurator.getLoggedUser().isAdministrator;
            },
            order: 50
        });
    });
