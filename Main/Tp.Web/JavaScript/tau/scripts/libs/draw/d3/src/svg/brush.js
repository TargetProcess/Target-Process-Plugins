d3.svg.brush=function(){function g(a){a.each(function(){var a=d3.select(this),e=a.selectAll(".background").data([0]),f=a.selectAll(".extent").data([0]),l=a.selectAll(".resize").data(d,String),m;a.style("pointer-events","all").on("mousedown.brush",k).on("touchstart.brush",k),e.enter().append("rect").attr("class","background").style("visibility","hidden").style("cursor","crosshair"),f.enter().append("rect").attr("class","extent").style("cursor","move"),l.enter().append("g").attr("class",function(a){return"resize "+a}).style("cursor",function(a){return d3_svg_brushCursor[a]}).append("rect").attr("x",function(a){return/[ew]$/.test(a)?-3:null}).attr("y",function(a){return/^[ns]/.test(a)?-3:null}).attr("width",6).attr("height",6).style("visibility","hidden"),l.style("display",g.empty()?"none":null),l.exit().remove(),b&&(m=d3_scaleRange(b),e.attr("x",m[0]).attr("width",m[1]-m[0]),i(a)),c&&(m=d3_scaleRange(c),e.attr("y",m[0]).attr("height",m[1]-m[0]),j(a)),h(a)})}function h(a){a.selectAll(".resize").attr("transform",function(a){return"translate("+e[+/e$/.test(a)][0]+","+e[+/^s/.test(a)][1]+")"})}function i(a){a.select(".extent").attr("x",e[0][0]),a.selectAll(".extent,.n>rect,.s>rect").attr("width",e[1][0]-e[0][0])}function j(a){a.select(".extent").attr("y",e[0][1]),a.selectAll(".extent,.e>rect,.w>rect").attr("height",e[1][1]-e[0][1])}function k(){function z(){var a=d3.event.changedTouches;return a?d3.touches(d,a)[0]:d3.mouse(d)}function A(){d3.event.keyCode==32&&(q||(r=null,s[0]-=e[1][0],s[1]-=e[1][1],q=2),d3_eventCancel())}function B(){d3.event.keyCode==32&&q==2&&(s[0]+=e[1][0],s[1]+=e[1][1],q=0,d3_eventCancel())}function C(){var a=z(),d=!1;t&&(a[0]+=t[0],a[1]+=t[1]),q||(d3.event.altKey?(r||(r=[(e[0][0]+e[1][0])/2,(e[0][1]+e[1][1])/2]),s[0]=e[+(a[0]<r[0])][0],s[1]=e[+(a[1]<r[1])][1]):r=null),o&&D(a,b,0)&&(i(m),d=!0),p&&D(a,c,1)&&(j(m),d=!0),d&&(h(m),l({type:"brush",mode:q?"move":"resize"}))}function D(a,b,c){var d=d3_scaleRange(b),g=d[0],h=d[1],i=s[c],j=e[1][c]-e[0][c],k,l;q&&(g-=i,h-=j+i),k=Math.max(g,Math.min(h,a[c])),q?l=(k+=i)+j:(r&&(i=Math.max(g,Math.min(h,2*r[c]-k))),i<k?(l=k,k=i):l=i);if(e[0][c]!==k||e[1][c]!==l)return f=null,e[0][c]=k,e[1][c]=l,!0}function E(){C(),m.style("pointer-events","all").selectAll(".resize").style("display",g.empty()?"none":null),d3.select("body").style("cursor",null),u.on("mousemove.brush",null).on("mouseup.brush",null).on("touchmove.brush",null).on("touchend.brush",null).on("keydown.brush",null).on("keyup.brush",null),l({type:"brushend"}),d3_eventCancel()}var d=this,k=d3.select(d3.event.target),l=a.of(d,arguments),m=d3.select(d),n=k.datum(),o=!/^(n|s)$/.test(n)&&b,p=!/^(e|w)$/.test(n)&&c,q=k.classed("extent"),r,s=z(),t,u=d3.select(window).on("mousemove.brush",C).on("mouseup.brush",E).on("touchmove.brush",C).on("touchend.brush",E).on("keydown.brush",A).on("keyup.brush",B);if(q)s[0]=e[0][0]-s[0],s[1]=e[0][1]-s[1];else if(n){var v=+/w$/.test(n),w=+/^n/.test(n);t=[e[1-v][0]-s[0],e[1-w][1]-s[1]],s[0]=e[v][0],s[1]=e[w][1]}else d3.event.altKey&&(r=s.slice());m.style("pointer-events","none").selectAll(".resize").style("display",null),d3.select("body").style("cursor",k.style("cursor")),l({type:"brushstart"}),C(),d3_eventCancel()}var a=d3_eventDispatch(g,"brushstart","brush","brushend"),b=null,c=null,d=d3_svg_brushResizes[0],e=[[0,0],[0,0]],f;return g.x=function(a){return arguments.length?(b=a,d=d3_svg_brushResizes[!b<<1|!c],g):b},g.y=function(a){return arguments.length?(c=a,d=d3_svg_brushResizes[!b<<1|!c],g):c},g.extent=function(a){var d,h,i,j,k;return arguments.length?(f=[[0,0],[0,0]],b&&(d=a[0],h=a[1],c&&(d=d[0],h=h[0]),f[0][0]=d,f[1][0]=h,b.invert&&(d=b(d),h=b(h)),h<d&&(k=d,d=h,h=k),e[0][0]=d|0,e[1][0]=h|0),c&&(i=a[0],j=a[1],b&&(i=i[1],j=j[1]),f[0][1]=i,f[1][1]=j,c.invert&&(i=c(i),j=c(j)),j<i&&(k=i,i=j,j=k),e[0][1]=i|0,e[1][1]=j|0),g):(a=f||e,b&&(d=a[0][0],h=a[1][0],f||(d=e[0][0],h=e[1][0],b.invert&&(d=b.invert(d),h=b.invert(h)),h<d&&(k=d,d=h,h=k))),c&&(i=a[0][1],j=a[1][1],f||(i=e[0][1],j=e[1][1],c.invert&&(i=c.invert(i),j=c.invert(j)),j<i&&(k=i,i=j,j=k))),b&&c?[[d,i],[h,j]]:b?[d,h]:c&&[i,j])},g.clear=function(){return f=null,e[0][0]=e[0][1]=e[1][0]=e[1][1]=0,g},g.empty=function(){return b&&e[0][0]===e[1][0]||c&&e[0][1]===e[1][1]},d3.rebind(g,a,"on")};var d3_svg_brushCursor={n:"ns-resize",e:"ew-resize",s:"ns-resize",w:"ew-resize",nw:"nwse-resize",ne:"nesw-resize",se:"nwse-resize",sw:"nesw-resize"},d3_svg_brushResizes=[["n","e","s","w","nw","ne","se","sw"],["e","w"],["n","s"],[]]