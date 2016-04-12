tau.mashups
	.addDependency("libs/jquery/jquery")
	.addModule("TestRunImport/restService", function () {
		function restService() {
			this._ctor();
		}

		restService.prototype = {
			_requestUrlBase: '/api/v1/',

			_ctor: function () {
			},

			_getUrl: function (entityName) {
				return new Tp.WebServiceURL(this._requestUrlBase + entityName).url;
			},

			getUserById: function (userId, success) {
				$.ajax({
					url: this._getUrl('Users.asmx/{UserId}?include=[Id,FirstName,LastName,IsActive,IsAdministrator]'.replace(/{UserId}/g, userId)),
					dataType: 'json',
					success: function (d) {
						success({ Id: d.Id, Name: d.FirstName + ' ' + d.LastName, IsActive: d.IsActive, IsAdministrator: d.IsAdministrator });
					},
					error: function (xhr) {
						if (xhr.status == 404) {
							success({ Id: -1, Name: '', IsActive: false, IsAdministrator: false });
						}
					}
				});
			},

			getLoggedUser: function (success) {
				$.ajax({
					url: this._getUrl('Context.asmx'),
					dataType: 'json',
					success: function (d) {
						success({ Id: d.LoggedUser.Id, Name: d.LoggedUser.FirstName + ' ' + d.LoggedUser.LastName, IsActive: d.LoggedUser.IsActive, IsAdministrator: d.LoggedUser.IsAdministrator });
					}
				});
			}
		};
		return restService;
	});