define(["tau/core/class","tests/integration/storeWrapper","libs/json2"],function(Class,wrapper){return Class.extend({init:function(a,b){this.givenEntity(wrapper.entity),this.givenType(a),this.description=b||null,this.name=a,this.fields=[],this.element=$("<div class='integration-test not-executed' style='display: none;'></div>").appendTo("body");return this},givenEntity:function(a){this.entity=a;return this},givenType:function(a){_.extend(this.entity,{type:a||null});return this},askAsNestedList:function(){this.nested=!0;return this},whenAskFields:function(a){this.fields=a;return this},getDescription:function(){this.fields&&this.fields.length>0&&!this.description&&(this.description=" data validation by fields: "+JSON.stringify(this.fields));return this.description||"basic data validation"},markAsExecuted:function(){this.element.removeClass("not-executed");if($(".not-executed").length===0){var a=$("<div class='all-done'/>");a.appendTo("body"),$(".fail>strong>.test-name").each(function(b,c){a.append("<br />"),a.append($(c).html())})}},done:function(){this.then();return this},then:function(fnAsserts){var self=this;self.fnAsserts=fnAsserts||function(){};var store=wrapper.getStore();store.get(self.entity.type,{id:self.entity.id,fields:self.fields,nested:self.nested},{success:function(result){var data=result.data;module("["+self.name+"]"),test(self.getDescription(),function(){self.fnAsserts.call(self,data,result.command);if(self.fields&&self.fields.length>0){var arr=eval(result.command.arrStr);arr.length>0&&equals(arr.length,self.fields.length,"response "+result.command.arrStr+" length equals to "+self.fields.length)}}),self.markAsExecuted()},failure:function(a){test("failures discovered for '"+self.getDescription()+"'",function(){throw"Error: "+a.data.status}),self.markAsExecuted()}}).done();return this}})})