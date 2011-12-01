define(["tau/services/service.rest","tau/core/header"],function(a,b){var c=function(){module("[rest service]",{setup:function(){var c={types:{person:{name:"person",fields:["name"],resource:"persons",refs:{contacts:{list:!0,name:"contact",fields:["email"]}}},contact:{name:"contact",fields:["email",b.ref("owner")],resource:"contacts",refs:{owner:{name:"person"}}}}};this.rest=new a(c)},teardown:function(){delete this.rest}}),test("forming url for native fields",function(){var a=this.rest.getUrl({type:"person",config:{id:5,fields:["id","name"]}});equal(a,"/persons.asmx/5?skip=0&take=999&include=[id,name]","url formed correctly")}),test("forming url for native fields not type provided",function(){var a=this.rest.getUrl({type:"bukas",config:{id:5,fields:["id","name"]}});equal(a,"/bukas.asmx/5?skip=0&take=999&include=[id,name]","url formed correctly")}),test("forming url for ref fields",function(){var a=this.rest.getUrl({type:"person",config:{id:5,fields:["id","name",{owner:["id","email",{role:["id","isDefault"]}]}]}});equal(a,"/persons.asmx/5?skip=0&take=999&include=[id,name,owner[id,email,role[id,isDefault]]]","url formed correctly")}),test("forming url for list",function(){var a=this.rest.getUrl({type:"person",config:{id:5,fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/5?skip=0&take=999&include=[id,contacts[email]]","url formed correctly")}),test("forming url for special nested resource",function(){var a=this.rest.getUrl({type:"person",config:{id:5,nested:!0,fields:["id",{contacts:["id","email"]}]}});equal(a,"/persons.asmx/5/contacts?skip=0&take=999&include=[id,email]","url formed correctly")}),test("forming url for find with simple fields: eq",function(){var a=this.rest.getUrl({type:"person",config:{$query:{name:"Vasya",lastName:"Pupkin"},fields:["id",{contacts:["email"]}]}});equal(a,'/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(name eq "Vasya") and (lastName eq "Pupkin")',"url formed correctly")}),test("forming url for find with simple fields: eq number",function(){var a=this.rest.getUrl({type:"person",config:{$query:{order:1},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(order eq 1)","url formed correctly")}),test("forming url for find with simple fields: eq number array",function(){var a=this.rest.getUrl({type:"person",config:{$query:{order:[1,2]},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(order eq 1) and (order eq 2)","url formed correctly")}),test("forming url for find with simple fields: gt number array",function(){var a=this.rest.getUrl({type:"person",config:{$query:{order:{$gt:[1,2]}},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(order gt 1) and (order gt 2)","url formed correctly")}),test("forming url for find with simple fields: eq date",function(){var a=this.rest.getUrl({type:"person",config:{$query:{order:new Date("9 Oct 2011")},fields:["id",{contacts:["email"]}]}});ok(a.indexOf('/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(order eq "2011-10-08')>=0,"url formed correctly")}),test("forming url for find with simple fields: gt and lt",function(){var a=this.rest.getUrl({type:"person",config:{$query:{order:{$gt:1,$lt:2}},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(order gt 1) and (order lt 2)","url formed correctly")}),test("forming url for find with simple fields: eq array",function(){var a=this.rest.getUrl({type:"person",config:{$query:{name:["Vasya","Pupkin"]},fields:["id",{contacts:["email"]}]}});equal(a,'/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(name eq "Vasya") and (name eq "Pupkin")',"url formed correctly")}),test("forming url for find with complex fields: eq",function(){var a=this.rest.getUrl({type:"person",config:{$query:{contact:{email:"Vasya"},lastName:"Pupkin"},fields:["id",{contacts:["email"]}]}});equal(a,'/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(contact.email eq "Vasya") and (lastName eq "Pupkin")',"url formed correctly")}),test("forming url for find with complex fields: isNull",function(){var a=this.rest.getUrl({type:"person",config:{$query:{contact:{email:null}},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(contact.email is null)","url formed correctly")}),test("forming url for find with complex fields: in",function(){var a=this.rest.getUrl({type:"person",config:{$query:{contact:{order:{$in:[1,2,3]}}},fields:["id",{contacts:["email"]}]}});equal(a,"/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(contact.order in (1,2,3))","url formed correctly")}),test("forming url for find with simple fields: contains with array",function(){var a=this.rest.getUrl({type:"person",config:{$query:{name:{$contains:["Vasya","Petya"]},lastName:{$contains:"Pupkin"}},fields:["id",{contacts:["email"]}]}});equal(a,'/persons.asmx/?skip=0&take=999&include=[id,contacts[email]]&where=(name contains "Vasya") and (name contains "Petya") and (lastName contains "Pupkin")',"url formed correctly")})};return{run:c}})