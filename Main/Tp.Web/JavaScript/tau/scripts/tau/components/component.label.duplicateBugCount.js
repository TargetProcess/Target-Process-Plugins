define(["Underscore","tau/components/component.label","tau/components/extensions/duplicateBugList/extension.duplicateBugList.count.label.setter","tau/models/model.duplicateBugCountCalculator","tau/models/model.duplicateBugRetriever","tau/models/model.extensions","tau/models/assignmentsList/extension.model.store.operations","tau/core/termProcessor"],function(_,ComponentCollapsible,DuplicateBugCountLabelSetter,DuplicateBugCountCalculator,DuplicateBugRetriever,modelExtensions,StoreOperations,TermProcessor){return{create:function(config){var terms=config.context.applicationContext.processes[0].terms,tp=new TermProcessor(terms);return config=config||{},_.extend(config,{text:"Duplicate "+tp.getTerms("Bug").names,quantityCssClass:config.quantityCssClass||"ui-gray-quantity",extensions:[DuplicateBugCountLabelSetter,DuplicateBugCountCalculator,StoreOperations,DuplicateBugRetriever]}),ComponentCollapsible.create(config)}}})