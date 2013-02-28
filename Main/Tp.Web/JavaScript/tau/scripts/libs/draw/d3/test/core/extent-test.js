require("../env");var vows=require("vows"),assert=require("assert"),suite=vows.describe("d3.extent");suite.addBatch({extent:{topic:function(){return d3.extent},"returns the numeric extent for numbers":function(a){assert.deepEqual(a([1]),[1,1]),assert.deepEqual(a([5,1,2,3,4]),[1,5]),assert.deepEqual(a([20,3]),[3,20]),assert.deepEqual(a([3,20]),[3,20])},"returns the lexicographic extent for strings":function(a){assert.deepEqual(a(["c","a","b"]),["a","c"]),assert.deepEqual(a(["20","3"]),["20","3"]),assert.deepEqual(a(["3","20"]),["20","3"])},"ignores null, undefined and NaN":function(a){assert.deepEqual(a([NaN,1,2,3,4,5]),[1,5]),assert.deepEqual(a([1,2,3,4,5,NaN]),[1,5]),assert.deepEqual(a([10,null,3,undefined,5,NaN]),[3,10]),assert.deepEqual(a([-1,null,-3,undefined,-5,NaN]),[-5,-1])},"compares heterogenous types as numbers":function(a){assert.deepEqual(a([20,"3"]),["3",20]),assert.deepEqual(a(["20",3]),[3,"20"]),assert.deepEqual(a([3,"20"]),[3,"20"]),assert.deepEqual(a(["3",20]),["3",20])},"returns undefined for empty array":function(a){assert.deepEqual(a([]),[undefined,undefined]),assert.deepEqual(a([null]),[undefined,undefined]),assert.deepEqual(a([undefined]),[undefined,undefined]),assert.deepEqual(a([NaN]),[undefined,undefined]),assert.deepEqual(a([NaN,NaN]),[undefined,undefined])},"applies the optional accessor function exactly once":function(a){var b=10;assert.deepEqual(d3.extent([0,1,2,3],function(){return++b}),[11,14])}}}),suite.export(module)