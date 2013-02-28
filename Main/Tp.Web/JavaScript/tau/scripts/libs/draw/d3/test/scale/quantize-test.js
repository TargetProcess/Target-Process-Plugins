require("../env");var vows=require("vows"),assert=require("assert"),suite=vows.describe("d3.scale.quantize");suite.addBatch({quantize:{topic:function(){return d3.scale.quantize},"has the default domain [0, 1]":function(a){var b=a();assert.deepEqual(b.domain(),[0,1]),assert.equal(b(.25),0)},"has the default range [0, 1]":function(a){var b=a();assert.deepEqual(b.range(),[0,1]),assert.equal(b(.75),1)},"maps a number to a discrete value in the range":function(a){var b=a().range([0,1,2]);assert.equal(b(0),0),assert.equal(b(.2),0),assert.equal(b(.4),1),assert.equal(b(.6),1),assert.equal(b(.8),2),assert.equal(b(1),2)},"coerces domain to numbers":function(a){var b=a().domain(["0","100"]);assert.strictEqual(b.domain()[0],0),assert.strictEqual(b.domain()[1],100)},"only considers the extent of the domain":function(a){var b=a().domain([-1,0,100]);assert.deepEqual(b.domain(),[-1,100])},"clamps input values to the domain":function(a){var b={},c={},d={},e=a().range([b,c,d]);assert.equal(e(-0.5),b),assert.equal(e(1.5),d)},"range cardinality determines the degree of quantization":function(a){var b=a();assert.inDelta(b.range(d3.range(0,1.001,.001))(1/3),.333,1e-6),assert.inDelta(b.range(d3.range(0,1.01,.01))(1/3),.33,1e-6),assert.inDelta(b.range(d3.range(0,1.1,.1))(1/3),.3,1e-6),assert.inDelta(b.range(d3.range(0,1.2,.2))(1/3),.4,1e-6),assert.inDelta(b.range(d3.range(0,1.25,.25))(1/3),.25,1e-6),assert.inDelta(b.range(d3.range(0,1.5,.5))(1/3),.5,1e-6),assert.inDelta(b.range(d3.range(1))(1/3),0,1e-6)},"range values are arbitrary":function(a){var b={},c={},d={},e=a().range([b,c,d]);assert.equal(e(0),b),assert.equal(e(.2),b),assert.equal(e(.4),c),assert.equal(e(.6),c),assert.equal(e(.8),d),assert.equal(e(1),d)}}}),suite.export(module)