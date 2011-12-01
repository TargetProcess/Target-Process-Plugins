 


Ext.ns('Tp.controls.grid');
Tp.controls.grid.SummaryGroupingView = Ext.extend(Ext.grid.GroupingView, {

	groupChanging: "groupChanging",
	
    doRenderEnd: function(cs, rs, ds, startRow, colCount, stripe) { return ""; },

    isEndingPresent: false,

    doRender: function(cs, rs, ds, startRow, colCount, stripe) {
        var markup = Tp.controls.grid.SummaryGroupingView.superclass.doRender.call(this, cs, rs, ds, startRow, colCount, stripe);
        var ending = this.doRenderEnd(cs, rs, ds, startRow, colCount, stripe);

        this.isEndingPresent = false;
        if (ending == "")
            return markup;

        if (ending == null)
            return markup;

        if (ending == undefined)
            return markup;

        markup += ending;
        isEndingPresent = true;

        return markup;
    },

	onShowGroupsClick: function() {
		this.fireEvent(this.groupChanging);
		Tp.controls.grid.SummaryGroupingView.superclass.onShowGroupsClick.apply(this, arguments);
	},

    //TO-DO: this override is rude, we need to discover more robust way 
    getGroups: function() {

        // means that there is no summary rows
        if (this.mainBody.dom.childNodes.length <= 1)
            return Tp.controls.grid.SummaryGroupingView.superclass.getGroups.call(this);

        var lastNode = this.mainBody.dom.childNodes[this.mainBody.dom.childNodes.length - 1];
        if (lastNode.childNodes.length > 1)
            return Tp.controls.grid.SummaryGroupingView.superclass.getGroups.call(this);

        //at this line it means that last row is summary row so exclude from the groups

        var arr = [];
        for (var i = 0; i < this.mainBody.dom.childNodes.length - 1; i++) {
            arr.push(this.mainBody.dom.childNodes[i]);
        }
        return arr;
    }
});     