define(["Underscore","jQuery","tau/configurator","tau/core/profile.methods","tests/common/service.mock","tests/performance/profile.tree.visualizer","tau/components/component.list.editable","tests/performance/list.datapoint","tau/models/dataProviders/model.provider.items.tasks_bugs","tau/models/dataProviders/model.provider.groups.tasks_bugs","tau/ui/templates/list_/grid.entity/ui.template.list.grid.entity","tau/components/component.title","tau/components/component.property.severity","tau/components/component.property.priority","tau/components/component.property.entityState","tau/components/component.property.effort","tau/components/component.progressBar.small","tau/components/component.assignmentsList","tau/components/component.entity.actions","tau/components/component.property.release",,"tau/components/component.property.iteration",,"tau/components/component.property.build",,"tau/components/component.property.build.testPlanRun",,"tau/components/component.property.lastStatus",,"tau/components/component.property.lastRunDate.list",,"tau/components/component.steps",,"tau/components/component.success",,"tau/components/component.menu.testPlanRun"],function(_,$,configurator,profiler,ServiceMock,profileVisualizer,componentList,dataPoint,BugsTasksItemsDataProvider,BugsTasksGroupsDataProvider){function startCallback(){$('<span>Amount: </span><input type="text" id="amount" value="100" />').appendTo("#toolbarContainer"),$("<button>Initialize</button>").click(testInitialize()).appendTo("#toolbarContainer");var $thead=$("<thead></thead>").append("<th>#</th>").append("<th>Prepare data</th>").append("<th>Register data</th>").append("<th>Create comp</th>").append("<th>Initialize</th>"),$table=$('<table id="result" border="1" cellpadding="1" cellspacing="1" width="30%"></table>').append($thead);$table.appendTo("#resultsContainer");var $visualRes=$('<div id="visualResult"></div>');$visualRes.appendTo("#resultsContainer")}function registerData(configurator,testData){configurator.setService(new ServiceMock);var repository=configurator.getStore().config.proxy;repository.registerData(testData),repository.markRecordSetAsCompleteLoaded("severity"),repository.markRecordSetAsCompleteLoaded("entityState"),repository.markRecordSetAsCompleteLoaded("priority"),repository.markRecordSetAsCompleteLoaded("process"),repository.markRecordSetAsCompleteLoaded("project")}function startTest(itemsCount){var points={};points.start=+(new Date);var entity={id:370,entityType:{name:"userStory"}},context={applicationContext:{selectedProjects:[{id:1}],processes:[{id:1,name:"Kanban",terms:[{name:"Bug",value:"Issue"},{name:"Bugs",value:"Issues"}],practices:[{name:"Planning",effortPoints:"Hour",isStoryEffortEqualsSumTasksEffort:!1},{name:"Time Tracking",isCloseAssignableIfZeroTimeRemaining:!1,isTimeDescriptionFieldVisible:!0,isTimeDescriptionRequired:!0,isRequiredShowRoleDropDown:!1},{name:"Bug Tracking"},{name:"Requirements"},{name:"Test Cases"},{name:"Source Control"},{name:"Help Desk"},{name:"Iterations"}]}]},entity:entity,assignable:entity,general:entity,getTimeTrackingPolicies:function(){return{disableSpentRemain:!0}},isPracticeAvailable:function(){return!0},getEffortPoint:function(){return"h"},getTerms:function(){return[]}},config={context:context,itemsDataProvider:BugsTasksItemsDataProvider,groupsDataProvider:BugsTasksGroupsDataProvider,groupBy:"entityState.name",multiprojects:!0,views:[{type:"grid.entity",group:{dataIndex:"name"}}]},testData=dataPoint.prepareTestData(itemsCount);points.prepareTestData=+(new Date),registerData(configurator,testData),points.registerData=+(new Date);var instance=componentList.create(config);return points.createComponent=+(new Date),instance.initialize(),points.initialize=+(new Date),{points:points}}function testInitialize(){return function(){configurator.clear();var itemsCount=$("#amount").val(),results=startTest(itemsCount),r=profiler.accumulate.result();profileVisualizer.build(r).appendTo("#resultsContainer");var points=results.points;$("<tr></tr>").append("<td>1</td>").append("<td>"+(points.prepareTestData-points.start)+"</td>").append("<td>"+(points.registerData-points.prepareTestData)+"</td>").append("<td>"+(points.createComponent-points.registerData)+"</td>").append("<td>"+(points.initialize-points.createComponent)+"</td>").appendTo("#result")}}return{run:startCallback}})