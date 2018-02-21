tau.mashups
    .addModule('tau/mashup.manager/services/service.mashup.manager.messages', function() {
        var RELOAD = 'Please reload the page (press F5 / Cmd+R) in order to apply the changes.';

        var ServiceMashupManagerMessages = {
            TIMEOUT: 10000,
            SAVED_MESSAGE: 'Mashup has been saved successfully. ' + RELOAD,
            DELETED_MESSAGE: 'Mashup has been deleted successfully. ' + RELOAD,
            INSTALLED_MESSAGE: 'Mashup has been installed successfully. ' + RELOAD
        };

        return ServiceMashupManagerMessages;
    });
