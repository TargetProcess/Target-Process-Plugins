CodeMirror.defineMode("markdown",function(a,b){function C(a,b,c){c=c||x;return function(d,e){while(!d.eol()){var f=d.next();f==="\\"&&d.next();if(f===b){e.inline=e.f=c;return a}}return a}}function B(a,b){a.eatSpace(),a.match(/^[^\s]+/,!0),b.f=b.inline=x;return j}function A(a,b){if(a.match(/^[^\]]*\]:/,!0)){b.f=B;return i}return t(a,b,x)}function z(a,b){a.eatSpace();var c=a.next();return c==="("||c==="["?t(a,b,C(j,c==="("?")":"]")):"error"}function y(a,b){while(!a.eol()){var c=a.next();c==="\\"&&a.next();if(c==="]"){b.inline=b.f=z;return i}}return i}function x(a,b){function c(){return b.strong?b.em?m:l:b.em?k:null}if(a.match(s,!0))return c();var d=a.next();if(d==="\\"){a.next();return c()}if(d==="`")return t(a,b,C(e,"`"));if(d==="<")return t(a,b,C(i,">"));if(d==="[")return t(a,b,y);var f=c();if(d==="*"||d==="_")return a.eat(d)?(b.strong=!b.strong)?c():f:(b.em=!b.em)?c():f;return c()}function w(a,b){var d=c.token(a,b.htmlState);a.eol()&&!b.htmlState.context&&(b.block=v);return d}function v(a,b){if(a.match(r)){a.skipToEnd();return e}if(a.eatSpace())return null;if(a.peek()==="#"||a.match(q)){a.skipToEnd();return d}if(a.eat(">")){b.indentation++;return f}if(a.peek()==="<")return u(a,b,w);if(a.peek()==="[")return t(a,b,A);if(n.test(a.peek())){var c=new RegExp("(?:s*["+a.peek()+"]){3,}$");if(a.match(c,!0))return h}var i;if(i=a.match(o,!0)||a.match(p,!0)){b.indentation+=i[0].length;return g}return t(a,b,b.inline)}function u(a,b,c){b.f=b.block=c;return c(a,b)}function t(a,b,c){b.f=b.inline=c;return c(a,b)}var c=CodeMirror.getMode(a,{name:"xml",htmlMode:!0}),d="header",e="code",f="quote",g="list",h="hr",i="linktext",j="linkhref",k="em",l="strong",m="emstrong",n=/^[*-=_]/,o=/^[*-+]\s+/,p=/^[0-9]\.\s+/,q=/^(?:\={3,}|-{3,})$/,r=/^(k:\t|\s{4,})/,s=/^[^\[*_\\<>`]+/;return{startState:function(){return{f:v,block:v,htmlState:c.startState(),indentation:0,inline:x,em:!1,strong:!1}},copyState:function(a){return{f:a.f,block:a.block,htmlState:CodeMirror.copyState(c,a.htmlState),indentation:a.indentation,inline:a.inline,em:a.em,strong:a.strong}},token:function(a,b){if(a.sol()){b.f=b.block;var c=b.indentation,d=0;while(c>0)if(a.eat(" "))c--,d++;else if(c>=4&&a.eat("\t"))c-=4,d+=4;else break;b.indentation=d;if(d>0)return null}return b.f(a,b)}}}),CodeMirror.defineMIME("text/x-markdown","markdown")