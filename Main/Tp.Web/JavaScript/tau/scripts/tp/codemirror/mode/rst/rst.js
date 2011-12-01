CodeMirror.defineMode("rst",function(a,b){function D(a,b,c){if(a.eol()||a.eatSpace()){a.skipToEnd();return c}e(b,a);return null}function C(a,b){if(!h)return D(a,b,"verbatim");if(a.sol()){a.eatSpace()||e(b,a);return null}return h.token(a,b.ctx.local)}function B(a,b){return D(a,b,"comment")}function A(a,b){var c="body";if(!b.ctx.start||a.sol())return D(a,b,c);a.skipToEnd(),d(b);return c}function z(a,b){var d=null;if(a.match(k))d="directive";else if(a.match(l))d="hyperlink";else if(a.match(m))d="footnote";else if(a.match(n))d="citation";else{a.eatSpace();if(a.eol()){e(b,a);return null}a.skipToEnd(),c(b,B);return"comment"}c(b,A,{start:!0});return d}function y(a,b){function g(a){b.ctx.prev=a;return f}var d=a.next(),f=b.ctx.token;if(d!=b.ctx.ch)return g(d);if(/\s/.test(b.ctx.prev))return g(d);if(b.ctx.wide){d=a.next();if(d!=b.ctx.ch)return g(d)}if(!a.eol()&&!t.test(a.peek())){b.ctx.wide&&a.backUp(1);return g(d)}c(b,x),e(b,d);return f}function x(a,b){function n(b){return a.match(b)&&l(/\W/)&&m(/\W/)}function m(b){return a.eol()||a.match(b,!1)}function l(a){return f||!b.ctx.back||a.test(b.ctx.back)}var d,f,g;if(a.eat(/\\/)){d=a.next(),e(b,d);return null}f=a.sol();if(f&&(d=a.eat(j))){for(g=0;a.eat(d);g++);if(g>=3&&a.match(/^\s*$/)){e(b,null);return"section"}a.backUp(g+1)}if(f&&a.match(q)){a.eol()||c(b,z);return"directive-marker"}if(a.match(r)){if(!h)c(b,C);else{var k=h;c(b,C,{mode:k,local:k.startState()})}return"verbatim-marker"}if(f&&a.match(w,!1)){if(!i){c(b,C);return"verbatim-marker"}var k=i;c(b,C,{mode:k,local:k.startState()});return null}if(f&&(a.match(u)||a.match(v))){e(b,a);return"list"}if(n(o)){e(b,a);return"footnote"}if(n(p)){e(b,a);return"citation"}d=a.next();if(l(s)){if(!(d!==":"&&d!=="|"||!a.eat(/\S/))){var t;d===":"?t="role":t="replacement",c(b,y,{ch:d,wide:!1,prev:null,token:t});return t}if(d==="*"||d==="`"){var x=d,A=!1;d=a.next(),d==x&&(A=!0,d=a.next());if(d&&!/\s/.test(d)){var t;x==="*"?t=A?"strong":"emphasis":t=A?"inline":"interpreted",c(b,y,{ch:x,wide:A,prev:null,token:t});return t}}}e(b,d);return null}function g(b){return f(b)?CodeMirror.getMode(a,b):null}function f(a){if(a){var b=CodeMirror.listModes();for(var c in b)if(b[c]==a)return!0}return!1}function e(a,b){if(b&&typeof b!="string"){var d=b.current();b=d[d.length-1]}c(a,x,{back:b})}function d(a,b){a.ctx=b||{}}function c(a,b,c){a.fn=b,d(a,c)}var h=g(b.verbatim),i=g("python"),j=/^[!"#$%&'()*+,-./:;<=>?@[\\\]^_`{|}~]/,k=/^\s*\w([-:.\w]*\w)?::(\s|$)/,l=/^\s*_[\w-]+:(\s|$)/,m=/^\s*\[(\d+|#)\](\s|$)/,n=/^\s*\[[A-Za-z][\w-]*\](\s|$)/,o=/^\[(\d+|#)\]_/,p=/^\[[A-Za-z][\w-]*\]_/,q=/^\.\.(\s|$)/,r=/^::\s*$/,s=/^[-\s"([{</:]/,t=/^[-\s`'")\]}>/:.,;!?\\_]/,u=/^\s*((\d+|[A-Za-z#])[.)]|\((\d+|[A-Z-a-z#])\))\s/,v=/^\s*[-\+\*]\s/,w=/^\s+(>>>|In \[\d+\]:)\s/;return{startState:function(){return{fn:x,ctx:{}}},copyState:function(a){return{fn:a.fn,ctx:a.ctx}},token:function(a,b){var c=b.fn(a,b);return c}}}),CodeMirror.defineMIME("text/x-rst","rst")