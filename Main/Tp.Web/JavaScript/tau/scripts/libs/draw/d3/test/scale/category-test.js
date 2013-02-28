function category(a,b){return{"is an ordinal scale":function(){var c=a(),d=c.range();assert.lengthOf(c.domain(),0),assert.lengthOf(c.range(),b),assert.equal(c(1),d[0]),assert.equal(c(2),d[1]),assert.equal(c(1),d[0]);var e=c.copy();assert.deepEqual(e.domain(),c.domain()),assert.deepEqual(e.range(),c.range()),c.domain(d3.range(b));for(var f=0;f<b;++f)assert.equal(c(f+b),c(f));assert.equal(e(1),d[0]),assert.equal(e(2),d[1])},"each instance is isolated":function(){var b=a(),c=a(),d=b.range();assert.equal(b(1),d[0]),assert.equal(c(2),d[0]),assert.equal(c(1),d[1]),assert.equal(b(1),d[0])},"contains the expected number of values in the range":function(){var c=a();assert.lengthOf(c.range(),b)},"each range value is distinct":function(){var b={},c=0,d=a();d.range().forEach(function(a){a in b||(b[a]=++c)}),assert.equal(c,d.range().length)},"each range value is a hexadecimal color":function(){var b=a();b.range().forEach(function(a){assert.match(a,/#[0-9a-f]{6}/),a=d3.rgb(a),assert.isFalse(isNaN(a.r)),assert.isFalse(isNaN(a.g)),assert.isFalse(isNaN(a.b))})},"no range values are very dark or very light":function(){var b=a();b.range().forEach(function(a){var b=d3.hsl(a);assert.isTrue(b.l>=.34,"expected "+a+" to be lighter (l = "+b.l+")"),assert.isTrue(b.l<=.89,"expected "+a+" to be darker (l = "+b.l+")")})}}}require("../env");var vows=require("vows"),assert=require("assert"),suite=vows.describe("d3.scale.category");suite.addBatch({category10:category(d3.scale.category10,10),category20:category(d3.scale.category20,20),category20b:category(d3.scale.category20b,20),category20c:category(d3.scale.category20c,20)}),suite.export(module)