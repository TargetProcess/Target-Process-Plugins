define(["tau/core/tau","tests/common/modelConfig","tests/context/progressBar/progressBarModelEvents","tests/common/expector/expector","tau/factories/factory.progressBar","tests/common/testData","tau/services/service.rest","tau/core/repository","tau/store/store","tau/configurator"],function(a,b,c,d,e,f,g,h,i,j){function k(){this.eventsArguments={},j.clear()}k.prototype={initData:function(){this.context=b.createForBug(15),this.initialData=f.getTestDataForProgressBar()},initMockControls:function(){this.mockControl=new MockControl,this.restServiceMock=this.mockControl.createMock(g),this.repository=new h({service:this.restServiceMock}),this.repository.registerData(this.initialData),this.repositoryMock=this.mockControl.createMock(this.repository),j.setProxy(this.repositoryMock)},createModel:function(){this.model=e.createModel(this.context)},initialize:function(){this.initData(),this.initMockControls(),this.createModel()},initializeModel:function(){var a=this.context.assignable.id,b=["id","effortCompleted","effortToDo","timeSpent","timeRemain",Like.is({entityType:["id","name"]}),Like.is({entityState:["id","name"]})],c=this.repository,d=[Similar.to({name:"get",type:this.initialData.__type,config:Like.is({id:a,fields:b}),callbacks:TypeOf.isA(Object)})];this.repositoryMock.expects().execute(d).andStub(function(){c.execute.apply(c,arguments)}),this.model.initialize()},expects:function(){this._expects||(this._expects=new d(this));return this._expects},events:function(){this._events||(this._events=new c(this));return this._events},verify:function(){this.events().verify(),this.mockControl.verify()},reset:function(){this.events().reset(),this.mockControl.reset()},destroy:function(){}};return k})