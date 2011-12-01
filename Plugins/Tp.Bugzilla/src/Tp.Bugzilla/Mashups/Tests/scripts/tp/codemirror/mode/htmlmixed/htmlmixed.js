CodeMirror.defineMode("htmlmixed",function(a,b){function i(a,b){if(a.match(/^<\/\s*style\s*>/i,!1)){b.token=f,b.localState=null,b.mode="html";return f(a,b)}return g(a,/<\/\s*style\s*>/,e.token(a,b.localState))}function h(a,b){if(a.match(/^<\/\s*script\s*>/i,!1)){b.token=f,b.curState=null,b.mode="html";return f(a,b)}return g(a,/<\/\s*script\s*>/,d.token(a,b.localState))}function g(a,b,c){var d=a.current(),e=d.search(b);e>-1&&a.backUp(d.length-e);return c}function f(a,b){var f=c.token(a,b.htmlState);f=="tag"&&a.current()==">"&&b.htmlState.context&&(/^script$/i.test(b.htmlState.context.tagName)?(b.token=h,b.localState=d.startState(c.indent(b.htmlState,"")),b.mode="javascript"):/^style$/i.test(b.htmlState.context.tagName)&&(b.token=i,b.localState=e.startState(c.indent(b.htmlState,"")),b.mode="css"));return f}var c=CodeMirror.getMode(a,{name:"xml",htmlMode:!0}),d=CodeMirror.getMode(a,"javascript"),e=CodeMirror.getMode(a,"css");return{startState:function(){var a=c.startState();return{token:f,localState:null,mode:"html",htmlState:a}},copyState:function(a){if(a.localState)var b=CodeMirror.copyState(a.token==i?e:d,a.localState);return{token:a.token,localState:b,mode:a.mode,htmlState:CodeMirror.copyState(c,a.htmlState)}},token:function(a,b){return b.token(a,b)},indent:function(a,b){return a.token==f||/^\s*<\//.test(b)?c.indent(a.htmlState,b):a.token==h?d.indent(a.localState,b):e.indent(a.localState,b)},compareStates:function(a,b){return c.compareStates(a.htmlState,b.htmlState)},electricChars:"/{}:"}}),CodeMirror.defineMIME("text/html","htmlmixed")