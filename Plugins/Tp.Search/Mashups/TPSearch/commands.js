tau.mashups
.addDependency('app.path')
.addDependency('libs/jquery/jquery')
.addModule('tau/mashups/TPSearch/Commands', function (appPath, $) {
	function commands() {
	}

	commands.prototype = {
		search: function (searchData, success, fail) {
			$.ajax({
				url: appPath.get() + '/api/v1/Plugins.asmx/Searcher/Commands/Search',
				data: JSON.stringify(searchData),
				success: success,
				error: fail,
				dataType: 'json',
				type: 'POST'
			});
		},

		getEntity: function (id, success, fail) {
			$.ajax({
				url: appPath.get() + '/api/v1/Generals/' + id + '?Include=[Name,Description,EntityType]',
				success: function (result) {
					success(result);
				},

				error: function (e, r, t) {
					fail(e, r, t);
				},
				dataType: 'json',
				type: 'GET'
			});
		},

		getComment: function (id, success, fail) {
			$.ajax({
				url: appPath.get() + '/api/v1/Comments/' + id + '?Include=[Description,General[Name,EntityType]]',
				success: function (result) {
					success(result);
				},

				error: function (e, r, t) {
					fail(e, r, t);
				},
				dataType: 'json',
				type: 'GET'
			});
		}
	};

	return new commands();
});