define(["libs/jquery/jquery"],function(a){var b=function(){var a={},b=function(b,c,d){var e=b||!1,f=c||[];if(!!e)for(var g in a)if(typeof a[g]["on"+e]=="function")try{var h=d||a[g];a[g]["on"+e].apply(h,f)}catch(i){}},c=function(b,c,e){if(b in a)if(e)d(b);else throw new Error("Mediator name conflict: "+b);a[b]=c},d=function(b){b in a&&delete a[b]};return{name:"Bus",publish:b,subscribe:c,unsubscribe:d}};return b()})