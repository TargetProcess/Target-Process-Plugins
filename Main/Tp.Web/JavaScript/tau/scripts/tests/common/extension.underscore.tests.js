define(["Underscore","tau/extensions/extension.underscore"],function(a){module("[extension.underscore]"),test("complexKey",function(){var b={a:234,nest:{x:342,subnest:{d:23}}};equals(a.complexKey(b,"a"),234),equals(a.complexKey(b,"nest.x"),342),equals(a.complexKey(b,"nest.subnest.d"),23),equals(a.complexKey(b,"ne"),undefined),equals(a.complexKey(b,"nest.zzz"),undefined)}),test("jsonSelect",function(){var b={a:234,nest:{x:342,subnest:{d:23},a:888}};deepEqual(a.jsonSelect(b,".x"),[342]),deepEqual(a.jsonSelect(b,".a"),[234,888])})})