require(["tp/plugins/userRepository"],function(a){(function(){module("user repository test",{setup:function(){this.restService=new restServiceMock,this.userRepository=new a({restService:this.restService})}}),test("should not return inactive users",function(){var a=function(a){equal(a.length,1)};this.userRepository.getUsers(a)})})()})