define(["tau/core/tau"],function(a){function b(b){var c=b.get(0).childNodes,d=[];for(var e=0,f=c.length;e<f;e++)c[e].nodeType==3&&d.push(c[e].nodeValue);return a.concat.apply(window,d)}return{checkAssignments:function(a,b){var c=a.find("nobr");equal(c.length,b.groups.length,"Count of assignments is valid");for(var d=0;d<c.length;d++){var e=[],f=b.groups[d].users,g=c.eq(d);if(f.length>0){for(var h=0;h<f.length;h++)e.push(f[h].name);equal(g.text(),[b.groups[d].role.name," ",e.join(" | ")].join(""),"Role name is valid")}else equal(g.find(".unassigned-link").text(),"Unassigned"),equal(g.text(),[b.groups[d].role.name," ","Unassigned"].join(""),"Role name is valid");d<c.length-1&&ok(c.next().is("br"),"Break line was added")}},getDOMStructure:function(a){var c={};c.tagName=a.get(0).tagName.toLowerCase();var d=a.get(0).attributes;if(d&&d.length>0){c.attributes={};for(var e=0,f=d.length;e<f;e++)c.attributes[d[e].name]=d[e].value}var g=b(a).trim();g&&g.length>0&&(c.text=g);var h=a.children(),i=arguments.callee;if(h.length>0){var j=[];h.each(function(a,b){j.push(i($(b)))}),c.children=j}return c}}})