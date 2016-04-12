Ext.ns('Tp');

Tp.UrlParameterCollection = Ext.extend(Object, {
	query: null,
	_parameters: null,

	constructor: function (query) {
		this.query = query;
		var a = [];
		var b = this.query.split('&');
		var c = '';
		if (b.length < 1) return a;
		for (i = 0; i < b.length; i++) {
			c = b[i].split('=');
			if (c[0].length > 0) {
				a[i] = [c[0], ((c.length == 1) ? c[0] : c[1])];
			}
		}
		this._parameters = a;
	},

	get: function (key) {
		var parameter = this._findParameter(key);
		if (parameter) {
			return parameter[1];
		}
	},
	_findParameter: function (key) {
		for (var i = 0; i < this._parameters.length; i++) {
			if (this._parameters[i][0] == key) {
				return this._parameters[i];
			}
		}
	},

	set: function (key, value) {
		var parameter = this._findParameter(key);
		if (parameter) {
			parameter[1] = value;
			return;
		}
		this._parameters.push([key, value]);
	},

	remove: function (key) {
		for (var i = 0; i < this._parameters.length; i++) {
			if (this._parameters[i][0] == key) {
				this._parameters.splice(i, 1);
			}
		}
	},

	toString: function () {
		if (this._parameters.length == 0) {
			return '';
		}

		var query = '?';
		for (var i = 0; i < this._parameters.length; i++) {
			if (i > 0)
				query += '&';
			query += this._parameters[i][0] + '=' + this._parameters[i][1];
		}

		return query;
	}
});
Tp.URL = Ext.extend(Object, {
	constructor: function (url) {
		if (url.length == 0) eval('throw "Invalid URL [' + url + ']"');
		this.url = url;
		this.port = -1;
		this.query = (this.url.indexOf('?') >= 0) ? this.url.substring(this.url.indexOf('?') + 1) : '';
		if (this.query.indexOf('#') >= 0) this.query = this.query.substring(0, this.query.indexOf('#'));
		this.protocol = '';
		this.host = '';
		this.protocolSep = '';
		this.portSep = '';
		var protocolSepIndex = this.url.indexOf('://');
		if (protocolSepIndex >= 0) {
			this.protocolSep = '://';
			this.protocol = this.url.substring(0, protocolSepIndex).toLowerCase();
			this.host = this.url.substring(protocolSepIndex + 3);
			if (this.host.indexOf('/') >= 0) this.host = this.host.substring(0, this.host.indexOf('/'));
			var atIndex = this.host.indexOf('@');
			if (atIndex >= 0) {
				var credentials = this.host.substring(0, atIndex);
				var colonIndex = credentials.indexOf(':');
				if (colonIndex >= 0) {
					this.username = credentials.substring(0, colonIndex);
					this.password = credentials.substring(colonIndex);
				} else {
					this.username = credentials;
				}
				this.host = this.host.substring(atIndex + 1);
			}
			var portColonIndex = this.host.indexOf(':');
			if (portColonIndex >= 0) {
				this.portSep = ':';
				this.port = this.host.substring(portColonIndex);
				this.host = this.host.substring(0, portColonIndex);
			}
			this.file = this.url.substring(protocolSepIndex + 3);
			this.file = this.file.substring(this.file.indexOf('/'));
		} else {
			this.file = this.url;
		}
		if (this.file.indexOf('?') >= 0) this.file = this.file.substring(0, this.file.indexOf('?'));
		var refSepIndex = url.indexOf('#');
		if (refSepIndex >= 0) {
			this.file = this.file.substring(0, refSepIndex);
			this.reference = this.url.substring(this.url.indexOf('#'));
		} else {
			this.reference = '';
		}
		this.path = this.file;
		if (this.query.length > 0) this.file += '?' + this.query;
		if (this.reference.length > 0) this.file += '#' + this.reference;

		this._parameters = new Tp.UrlParameterCollection(this.query);
	},

	/* Returns the port part of this URL, i.e. '8080' in the url 'http://server:8080/' */
	getPort: function () {
		return this.port;
	},

	/* Returns the query part of this URL, i.e. 'Open' in the url 'http://server/?Open' */
	getQuery: function () {
		return this._parameters.toString();
	},

	/* Returns the protocol of this URL, i.e. 'http' in the url 'http://server/' */
	getProtocol: function () {
		return this.protocol;
	},

	/* Returns the host name of this URL, i.e. 'server.com' in the url 'http://server.com/' */
	getHost: function () {
		return this.host;
	},

	/* Returns the user name part of this URL, i.e. 'joe' in the url 'http://joe@server.com/' */
	getUserName: function () {
		return this.username;
	},

	/* Returns the password part of this url, i.e. 'secret' in the url 'http://joe:secret@server.com/' */
	getPassword: function () {
		return this.password;
	},

	/* Returns the file part of this url, i.e. everything after the host name. */
	getFile: function () {
		return this.file;
	},
	/* Returns the reference of this url, i.e. 'bookmark' in the url 'http://server/file.html#bookmark' */
	getReference: function () {
		return this.reference;
	},

	/* Returns the file path of this url, i.e. '/dir/file.html' in the url 'http://server/dir/file.html' */
	getPath: function () {
		return this.path;
	},

	getAcid: function () {
		return this.getArgumentValue('acid');
	},

	setAcid: function (value) {
		this.setArgumentValue('acid', value);
	},

	setArgumentValue: function (key, value) {
		this._parameters.set(key, value);
	},

	removeArgument: function (key) {
		this._parameters.remove(key);
	},

	/* Returns the FIRST matching value to the specified key in the query.
	If the url has a non-value argument, like 'Open' in '?Open&bla=12', this method
	returns the same as the key: 'Open'...
	The url must be correctly encoded, ampersands must encoded as &amp;
	I.e. returns 'value' if the key is 'key' in the url 'http://server/?Open&amp;key=value' */
	getArgumentValue: function (key) {
		var parameter = this._parameters.get(key);
		if (parameter != null) {
			return parameter.replace(/\+/g, " ");
		}

		return null;
	},

	/* Returns a String representation of this url */
	toString: function () {
		var segments = [this.getProtocol(), this.protocolSep, this.getHost(), this.getPort(), this.getPath(), this.getQuery()];
		var _url = '';
		Ext.each(segments, function () {
			if (this && this != -1) _url += this;
		});
		return _url;
	}
});

Tp.AcidURL = Ext.extend(Tp.URL, {
	constructor: function (url) {
		Tp.AcidURL.superclass.constructor.call(this, url);
	},

	toString: function () {
		var acid = new Tp.URL(document.location.href).getAcid();
		this.setAcid(acid);
		return Tp.AcidURL.superclass.toString.call(this);
	}
});

Tp.WebServiceURL = Ext.extend(Tp.AcidURL, {
	constructor: function (url) {
		Tp.WebServiceURL.superclass.constructor.call(this, appHostAndPath + url);
	}
});