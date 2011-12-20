tau.mashups
	//.addDependency("tau/configurator")
	.addDependency("libs/jquery/jquery")
	.addMashup(function (configurator) {

		var intervalID = setInterval(function () {
			var pluginsBlocks = $('div.plugin-block');
			if (pluginsBlocks.length === 0) {
				return;
			}

			var mashupBlock = $.grep(pluginsBlocks, function (element) {
				return $(element).find('h4._pluginName').text() == 'Mashup Manager';
			});

			$(mashupBlock).find('li.delete').remove();
			$(mashupBlock).find('button:contains("Add Profile")').remove();

			clearInterval(intervalID);
		}, 200);

		//		configurator.getGlobalBus().on('afterRender', function (evtArgs) {
		//			if (evtArgs.caller.name === 'pluginsList') {
		//				var pluginElement = evtArgs.data.element;
		//
		//				var pluginsBlocks = pluginElement.find('div.plugin-block');
		//				var mashupBlock = $.grep(pluginsBlocks, function (element) {
		//					return $(element).find('h4._pluginName').text() == 'Mashup Manager';
		//				});
		//
		//				$(mashupBlock).find('li.delete').remove();
		//				$(mashupBlock).find('button:contains("Add Profile")').remove();
		//			}
		//		});
	});