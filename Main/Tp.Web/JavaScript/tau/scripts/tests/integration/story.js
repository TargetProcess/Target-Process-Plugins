define(["tests/integration/spec"],function(a){(new a("userStory")).whenAskFields(["id","name"]).then(function(a,b){equals(a.id,this.entity.id,"id of context equals to entity id"),equals(a.name,"welcome to REST world!","name is returned correctly")}),(new a("userStory","bugs")).whenAskFields([{bugs:[]}]).done(),(new a("userStory","bugs nested")).whenAskFields([{bugs:["id","name"]}]).askAsNestedList().done(),(new a("userStory","tasks")).whenAskFields([{tasks:[]}]).done(),(new a("userStory","tasks nested")).whenAskFields([{tasks:[]}]).askAsNestedList().done(),(new a("userStory","requests")).whenAskFields([{requests:[]}]).done(),(new a("userStory","requests nested")).whenAskFields([{requests:[]}]).askAsNestedList().done(),(new a("userStory","times")).whenAskFields([{times:[]}]).done(),(new a("userStory","times nested")).whenAskFields([{times:[]}]).askAsNestedList().done();var b=["id","name","tasks-count","bugs-count","testCases-count","tags","description","attachments-count","comments-count","effortCompleted","effortToDo","timeSpent","timeRemain",{entityType:["id","name"]},{entityState:["id","name"]},"effort","tasks-effort-sum","tasks-effortToDo-sum",{owner:["id","firstName","lastName"]},{roleEfforts:["id","effort","effortToDo",{role:["id","name"]}],list:!0},{assignments:["id",{role:["id","name"]},{generalUser:["id","firstName","lastName"]}],list:!0},"requests-count",{priority:["id","name"]},{project:["id","name"]},{release:["id","name"]},{iteration:["id","name"]}];(new a("userStory","all fields requested on start")).whenAskFields(b).done();return{}})