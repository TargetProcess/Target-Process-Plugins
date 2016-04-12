tau.mashups
	.addDependency("libs/jquery/jquery")
	.addMashup(function () {

		var intervalId = setInterval(function () {
			var pluginsBlocks = $('div.plugin-block');
			if (pluginsBlocks.length === 0) {
				return;
			}

			var mashupBlock = $.grep(pluginsBlocks, function (element) {
				return $(element).find('h4._pluginName').text() == 'Search';
			});

			$(mashupBlock).find('li.delete').remove();
			$(mashupBlock).find('div.pt-5').remove();

			clearInterval(intervalId);
		}, 200);
	});