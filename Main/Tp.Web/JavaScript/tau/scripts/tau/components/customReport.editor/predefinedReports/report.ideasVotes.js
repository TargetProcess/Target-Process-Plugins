define({name:"Ideas Votes",description:"Ideas by Priority and Votes count",type:"scatterplot",classNames:["scatterplot2-request","scatterplot2-stable"],tags:["Help desk"],reportSettings:{dataSource:{source:{items:[{id:"request"}],filter:"?RequestType is 'Idea'"},dimensions:[{id:"count",model:"Requesters.Count()"},{id:"priority",model:"Priority.Name"},{id:"entityType",model:"entityType.Name"}],global:{}},report:{color:"entityType",x:["count"],y:["priority"]}}});