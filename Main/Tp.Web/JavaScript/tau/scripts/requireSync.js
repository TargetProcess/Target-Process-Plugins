var requireSync=function(e,n){requirejs.s.contexts._.nextTick=function(e){e()};var r,o=!1;return requirejs(e,function(e){r=e,o=!0,n.apply(null,arguments)}),requirejs.s.contexts._.nextTick=requirejs.nextTick,!o&&"undefined"!=typeof console&&console.error&&console.error("Unable to load module synchronously, there is either an unloaded dependency or an async loader plugin in use.\nDependencies: "+e),r};