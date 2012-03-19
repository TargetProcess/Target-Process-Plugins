tau.mashups
	.addDependency("libs/jquery/jquery")
	.addMashup(function () {

		var intervalID = setInterval(function () {
			var pluginsBlocks = $('div.plugin-block');
			if (pluginsBlocks.length === 0) {
				return;
			}

			var mashupBlock = $.grep(pluginsBlocks, function (element) {
				return $(element).find('h4._pluginName').text() == 'Mashup Manager';
			});

			$(mashupBlock).find('li.delete').remove();

			clearInterval(intervalID);
		}, 200);
	});