define(["jQuery","Underscore","libs/d3/d3","tp/reports/chart.layout","tp/reports/relationsNetwork/chart.network.layout","tp/reports/chart.behaviour"],function($,_,d3,layout1,layout,BaseChart){return BaseChart.extend({init:function(config){this.handlers=this.initHandlers(config.handlers||[]),this.cardTemplate=config.templateFactory.get("boardplus.card.skeleton")},nodes:function(nodes,context){var self=this,placeholder=$(context.placeholder).closest("div")[0];d3.select(placeholder).selectAll(".tau-chart-box").style("display","block");var boxes=d3.select(placeholder).selectAll(".tau-chart-box").data(nodes,function(node){return node.id});return boxes.enter().append("div"),boxes.attr("class",function(d){return"tau-chart-box i-trigger-set-focus zoom-level-"+context.scales.zoom(d.nodeType)}).html(function(d){return $("<div></div>").append(self.cardTemplate.bind({},{data:d})).html()}),boxes.exit().remove(),boxes},links:function(links,context){var placeholder=d3.select(context.placeholder),relations=placeholder.selectAll(".link").data(links);return relations.enter().append("line").attr("data-entity-id",function(d){return d.id}).attr("class",function(d){return"link "+context.scales.relationType(d.type)}).attr("marker-end",function(d){return"url(#"+context.scales.relationType(d.type)+")"}),relations.exit().remove(),relations},addTransparentClickHandler:function(context){var handler=d3.select(context.placeholder).append("rect").attr("width",context.plot.width).attr("class","transparent i-trigger-reset-focus");this._subscribe(handler,["click"],context)},resetFocus:function(context){d3.select(context.placeholder.parentNode.parentNode.parentNode).classed("tau-chart-focus",!1).selectAll(".tau-chart-focused").classed("tau-chart-focused",!1)},data:function(nodes,context){function $placeholder(){return $(context.placeholder).closest("div")}var self=this,width=context.plot.width,height=context.plot.height;this.addTransparentClickHandler(context),this.resetFocus(context);var links=_.chain(nodes).map(function(node){return node.links}).flatten(!1).unique().value();nodes.forEach(function(node){node.size=context.scales.size(node.nodeType)});var d3Nodes=this.nodes(nodes,context),d3Links=this.links(links,context),move=function(animate){return animate?function(){d3Nodes.transition().call(layout.nodes(graph.scales)),d3Links.transition().call(layout.links(graph.scales))}:function(){d3Nodes.call(layout.nodes(graph.scales)),d3Links.call(layout.links(graph.scales))}},end=function(){height=Math.max(height,graph.height()),$placeholder().height(height),d3.select(context.placeholder.parentNode).style("height",height+"px"),d3.select(context.placeholder).style("height",height+"px"),d3.select(context.placeholder).select(".transparent").attr("height",height),nodes.forEach(function(node){node.fixed=node.nodeType=="primary"}),move(!1)(),$(self).trigger("afterRender")},graph=layout1.graph().width(width).links(links).nodes(nodes).end(function(){end(),end=move(!0)}).dragmove(function(node){function limit(x,lower,upper){return Math.min(Math.max(x,lower),upper)}var x=limit(graph.scales.x(node.x),node.size[0]/2,width-node.size[0]/2-12),y=limit(graph.scales.y(node.y),node.size[1]/2,height-node.size[1]/2);node.x=graph.scales.x.invert(x),node.y=graph.scales.y.invert(y),move(!1)()});d3Nodes.call(graph.drag),d3.select(context.placeholder.parentNode).select("defs").selectAll("marker").data(context.scales.relationType.range()).enter().append("marker").attr("id",String).attr("viewBox","0 -7 14 14").attr("refX",14).attr("markerUnits","userSpaceOnUse").attr("markerWidth",7).attr("markerHeight",7).attr("orient","auto").append("path").attr("class",String).attr("stroke-width","1").attr("d","M0,-7L14,0L0,7"),this._subscribe(d3Nodes,["click","dblclick"],context),this._subscribe(graph.drag,["dragstart.behavior"],context),graph.doLayout()}})})