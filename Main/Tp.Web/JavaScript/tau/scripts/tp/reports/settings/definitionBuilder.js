define(["Underscore"],function(_){return{getWithLeadAndCycleTime:function(data,acid){return{base64:!0,take:1e3,where:null,definition:{global:{acid:acid},x:{id:"entitystate",filter:"?IsFinal is True",ordering:null},y:{id:"startdate",filter:data.time.startDateFilter,ordering:null},cells:{items:_.map(data.entities,function(item){return{id:item.id,data:"{id,name,entityType.name as type,endDate,leadTime,cycleTime}",filter:null,ordering:null}}),filter:data.filter},user:{cardFilter:"?EndDate is not None"}}}}}})