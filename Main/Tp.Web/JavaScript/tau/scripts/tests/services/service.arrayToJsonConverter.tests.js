define(["tau/services/service.arrayToJsonConverter"],function(a){var b=function(){module("[converter]",{setup:function(){this.converter=a},tearDown:function(){delete this.converter}}),test("simple converting",function(){var a=this.converter.convert({header:["id","name"]},[5,"Oleg"]);deepEqual(a,{id:5,name:"Oleg"},"JSON is formed correctly")}),test("one-to-one converting",function(){var a=this.converter.convert({header:["id","name",{creator:["id","login"]}]},[5,"Oleg",[1,"admin"]]);deepEqual(a,{id:5,name:"Oleg",creator:{id:1,login:"admin"}},"JSON is formed correctly")}),test("one-to-one converting when null is coming",function(){var a=this.converter.convert({header:["id","name",{creator:["id","login"]}]},[5,"Oleg",null]);deepEqual(a,{id:5,name:"Oleg",creator:null},"JSON is formed correctly")}),test("inner list converting",function(){var a=this.converter.convert({header:["id","name",{contacts:["id","company"],list:!0}]},[5,"Oleg",[[1,"Sony"],[2,"Bravia"]]]);deepEqual(a,{id:5,name:"Oleg",contacts:[{id:1,company:"Sony"},{id:2,company:"Bravia"}]},"JSON is formed correctly")})};return{run:b}})