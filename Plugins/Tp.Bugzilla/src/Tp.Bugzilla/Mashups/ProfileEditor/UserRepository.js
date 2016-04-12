tau.mashups
    .addDependency("Bugzilla/RestService")
    .addModule("Bugzilla/UserRepository", function (restService) {
	function userRepository(config) {
		this._create(config);
	};

	userRepository.prototype = {
		_create: function (config) {
			this._restService = new restService();
		},

		getUsers: function (projectId, success) {
			var service = this._restService;

			var handle = function (data) {
				var activeUsers = data.Items;
				var mappingUsers = $(activeUsers).map(function () {
					return {
						Id: this.Id,
						Name: this.FirstName + ' ' + this.LastName,
						Role: this.Role.Name,
						Avatar: service.getAvatarUrl(this.Id)
					};
				});
				success(mappingUsers);
			};

			service.getUsers(projectId, handle);
		}
	};
	return userRepository;
});
