tau.mashups.addModule("TestRunImport/userRepository", function () {
	function userRepository(config) {
		this._create(config);
	};

	userRepository.prototype = {
		_create: function(config) {
			this._restService = config.RestService;
		},

		getUserById: function(userId, success) {
			var service = this._restService;
			service.getUserById(userId, success);
		},

		getLoggedUser: function(success) {
			var service = this._restService;
			service.getLoggedUser(success);
		}
	};
	return userRepository;
});
