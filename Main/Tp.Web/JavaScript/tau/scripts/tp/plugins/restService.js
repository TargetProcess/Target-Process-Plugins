define(["libs/jquery/jquery"],function(a){function b(){this._ctor()}b.prototype={_requestUrlBase:"/api/v1/",_ctor:function(){},_getUrl:function(a){return(new Tp.WebServiceURL(this._requestUrlBase+a)).url},getUsers:function(b){a.getJSON(this._getUrl("Users.asmx?include=[Id,FirstName,LastName,Role,IsActive,Email,Login]&Take=1000"),b)},getAvatarUrl:function(a){var b=new Tp.WebServiceURL("/avatar.ashx");b.setArgumentValue("size",24),b.setArgumentValue("UserId",a),b.setArgumentValue("tick",0);return b.toString()}};return b})