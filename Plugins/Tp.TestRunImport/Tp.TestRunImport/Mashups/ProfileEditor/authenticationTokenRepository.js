tau.mashups.addModule("TestRunImport/authenticationTokenRepository", function () {
	function authenticationTokenRepository(config) {
		this._create(config);
	};

	authenticationTokenRepository.prototype = {
		_seleniumUrlBase: '/api/v1/Authentication.asmx?id={UserId}&format=json',

		_create: function(config) {
		},

		getAuthenticationToken: function(userId, success) {
			$.getJSON(new Tp.WebServiceURL(this._seleniumUrlBase.replace( /{UserId}/g , userId)).url, success);
		}
	};
	return authenticationTokenRepository;
});
