tau.mashups
    .addDependency('libs/jquery/jquery')
    .addDependency('tau/configurator')
    .addMashup(function($, configurator) {
        var intervalId;

        function fixPluginLayout() {
            var pluginsBlocks = $('div.plugin-block');
            if (pluginsBlocks.length === 0) {
                return;
            }

            var mashupBlock = $(
                $.grep(pluginsBlocks, function(element) {
                    return $(element).find('h4._pluginName').text() === 'Search';
                })
            );

            if (configurator.getFeaturesService().isEnabled('search')) {
                mashupBlock.find('li.delete').remove();
                mashupBlock.find('div.separator').remove();

                var profileNameLabel = mashupBlock.find('.name.tau-profileName');
                if (profileNameLabel.length !== 0) {
                    mashupBlock.find('div.pt-5').remove();
                }
            } else {
                mashupBlock.closest('.plugin-block').remove();
            }

            clearInterval(intervalId);
        }

        intervalId = setInterval(fixPluginLayout, 100);
        fixPluginLayout();
    });
