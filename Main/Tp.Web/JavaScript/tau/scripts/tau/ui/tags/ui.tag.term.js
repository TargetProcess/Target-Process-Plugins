define(["Underscore","jQuery","tau/core/termProcessor","libs/jquery/jquery.tmpl"],function(a,b,c){b.extend(b.tmpl.tag,{term:{_default:{$1:"'type'",$2:"null"},open:["var $__t = $item;","while($__t.parent && !$__t.$tau){","$__t = $__t.parent","}","_=_.concat($__t.$tau.terms($2)[$1]);"].join("")}});return{register:function(a,b){var d=a.context&&a.context.applicationContext?a.context.applicationContext.processes[0].terms:{};b.$tau=b.$tau||{};var e=new c(d);b.$tau.terms=function(a){return e.getTerms(a)}}}})