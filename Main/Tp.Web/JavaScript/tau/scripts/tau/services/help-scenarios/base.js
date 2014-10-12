define(["jQuery","Underscore"],function(e,t){function o(n,r,a){r=r||e.Deferred();var i=t.isFunction(n)?n.call(null):e(n);return i.length&&e.contains(document.documentElement,i[0])&&(a||i.is(":visible"))?r.resolve(i):"pending"===r.state()&&t.delay(o,250,n,r,a),r}var n=function(e){var t=e.$holder.tauBubble("getContent");return t.on("click",".i-role-next",e.resolve),e},r=function(e,t,o){e.descriptor=setTimeout(function(){t(),null!==e.descriptor&&r(e,t,o)},o)},a=function(e){clearTimeout(e.descriptor),e.descriptor=null},i=function(e){var t=e.offset();return t.w=e[0].clientWidth,t.h=e[0].clientHeight,t},s=e("<div></div>").appendTo("body"),c={descriptor:null},u=function(e,t){e.addClass("tau-help-attention");var o=['<div style="display: block;" class="i-role-help-drop-target tau-bubble tau-warning-bubble i-state_visible i-role-taububble-ignore">','   <div class="tau-bubble__inner" role="content"></div>',"</div>"].join(""),n={alignTo:e,content:"<p>"+t+"</p>",zIndex:10001,template:o,showOnCreation:!0,className:"tau-warning-bubble",showEvent:null,hideEvent:null,mode:"bubble",onPositionConfig:function(e){e.my="center top",e.at="center center"},onShow:function(t){var o=i(e.closest(".i-role-grid")),n=i(e),r=t.height(),a=o.top+o.h-n.top;if(r>a)t.hide();else{var s=n.top+(a-r)/2;t.css("top",s)}},stackName:"help-flow-drop-target",dontCloseSelectors:["body"]};s.tauBubble(n);var a=function(){s.tauBubble("show")};r(c,a,500)},l=function(e){a(c),s.tauBubble("hide"),e.removeClass("tau-help-attention")},d=function(e){return".tau-teams-projects-manager-wrapper .tau-teams-projects-manager-holder "+e},p=function(e,t){e.scenarioContext.set("project-created",t)},m=function(e){return e.scenarioContext.get("project-created")},f=function(e){t.defer(e.resolve)},v=function(o){return function(){var n=e(".tau-teams-projects-manager-wrapper .i-role-section-projects .i-role-item"),r=n.filter(function(){return t.trim(e(this).text())===o});r.is(":visible")&&r.addClass("tau-active-item");var a=r.find(".i-role-selecttrigger");return 0===a.length?e():a.eq(0)}},b={liveSelect:o,complete:n,resolveCtx:f,markDropTarget:u,unMarkDropTarget:l,fieldInCtxAddProject:d,selectorOnVisibleBubble:function(e){return".i-role-bubble.i-state_visible "+e},hideTooltipOnCardDnD:function(t){var o=t.$node;o.on("mousedown",function(){o.addClass("i-role-help-disable-tooltip"),e("body").one("mouseup",function(){o.removeClass("i-role-help-disable-tooltip")})})},extractJustAddedEntityId:function(t){var o=e(t).data("id");return parseInt(o)},stepVideoIntro:function(t){var o,n;return{custom:!0,target:function(){return o=e.Deferred(),n=e.Deferred(),require(["tau/components/component.video-intro.popup"],function(e){e.create({url:t.url,showNextButton:!0,destroy:o,render:n})}),n},complete:function(){return o}}},stepContextPromptToSeeExistingProjectsAndTeams:function(){return{message:["<p>Open the Teams and Projects menu.</p>"].join(""),target:function(){return o(".tau-context-filter")},complete:function(t){return e(".tau-bubble-board.tau-teams-projects-manager-wrapper").is(":visible")?f(t):t.$node.one("click",t.resolve),t}}},stepContextExplainProjects:function(e){return e=e?" "+e+" ":" ",{message:["<p>Select which Project you want to see on the"+e+"board. This is your current project context. The project context can be a combination of teams-projects.</p>","<p>You can change it at any time. We will limit our context to just one project for now.</p>",'<p class="tau-help-continue"><button class="i-role-next tau-btn tau-primary">Continue</button></p>'].join(""),rollbackDepth:2,target:function(){var e=".tau-teams-projects-manager-wrapper .i-role-section-projects";return o(e)},complete:function(e){var t=e.$holder.tauBubble("getContent");return t.on("click",".i-role-next",e.resolve),e}}},stepContextApplyExistingProject:function(t){return{message:["<p>Click to show the work from your chosen project context on the current board.</p>"].join(""),rollbackDepth:3,target:function(e){p(e,t);var n=v(t);return o(n)},complete:function(e){return e.$node.one("click",e.resolve),e},teardown:function(){var t=e(".tau-teams-projects-manager-wrapper .i-role-section-projects .i-role-item.tau-active-item");t.removeClass("tau-active-item")}}},stepContextPromptToAddNewProject:function(){return{message:"Click to add a new project.",rollbackDepth:2,target:function(){var e=".tau-teams-projects-manager-wrapper .i-role-section-projects .i-role-formtrigger:visible";return o(e)},complete:function(e){return e.$node.hasClass("tau-active")?f(e):e.$node.one("click",e.resolve),e}}},stepContextTypeProjectName:function(){return{message:"Enter project name.",rollbackDepth:3,target:function(){var e=d(".tau-new-entity-info .tau-new-entity-name");return o(e)},complete:function(e){var t=e.$node.val()||"";return t.length>0?f(e):e.$node.one("keypress",e.resolve),e}}},stepContextSelectProcess:function(e){return{message:"Select <strong>"+e+"</strong> process.",rollbackDepth:4,target:function(){var e=d(".tau-new-entity-process .tau-select");return o(e)},complete:function(t){var o=t.$node.find("option:selected").text();return o.toLowerCase()===e.toLowerCase()?f(t):t.$node.on("change",function(){var o=t.$node.find("option:selected").text();o.toLowerCase()===e.toLowerCase()&&t.resolve()}),t}}},stepContextInvitePeopleToProject:function(){return{message:["<p>Invite some people who will work on this project. Type their emails (optional).</p>",'<p class="tau-help-continue"><button class="i-role-next tau-btn tau-primary">Continue</button></p>'].join(""),rollbackDepth:5,target:function(){var e=d(".tau-invite-widget .tau-invite-field");return o(e)},complete:function(t){t.step.scenarioContext.set("isProjectAlreadyCreated",!1);var o=d(".tau-teams-projects-manager .i-role-form");e(o).one("entity.add.completed",function(){t.step.scenarioContext.set("isProjectAlreadyCreated",!0),t.resolve()});var n=t.$node.find(".i-role-member").text();if(n.length>0)f(t);else{var r=t.$holder.tauBubble("getContent");r.on("click",".i-role-next",t.resolve)}return t}}},stepContextCreateProject:function(){return{message:"Click here to create a project.",rollbackDepth:6,target:function(t){var n=t.scenarioContext.get("isProjectAlreadyCreated"),r=e.Deferred();if(n)r.resolve(e("<div></div>"));else{var a=d(".tau-new-entity-form .tau-success");r=o(a)}return r},complete:function(t){var o=t.step.scenarioContext.get("isProjectAlreadyCreated");if(o)p(t.step,e(d(".tau-new-entity-info .tau-new-entity-name")).val()),f(t);else{var n=d(".tau-teams-projects-manager .i-role-form");e(n).one("entity.add.completed",function(){p(t.step,e(d(".tau-new-entity-info .tau-new-entity-name")).val()),t.resolve()})}return t}}},stepContextHighlightCreatedProject:function(){return{message:"The project you've created was added to the list of projects.",target:function(e){var n=".tau-teams-projects-manager-wrapper .i-role-section-projects .i-role-item.tau-added";return o(n).then(function(o){return p(e,t.trim(o.text())),o})},complete:function(e){return t.delay(e.resolve,3e3),e}}},stepContextApplyCreatedProject:function(){return{message:["<p>Click to apply the selected context to the current board.</p>",'<p class="tau-help-apply-project"></p>'].join(""),target:function(e){var t=m(e),n=v(t);return o(n)},complete:function(e){return e.$node.one("click",e.resolve),e},teardown:function(){var t=e(".tau-teams-projects-manager-wrapper .i-role-section-projects .i-role-item.tau-active-item");t.removeClass("tau-active-item")}}},stepContextCloseContextMenu:function(){return{message:"Click to close this menu.",offset:10,target:function(){return o(".tau-context-filter")},complete:function(t){var o=e(".tau-teams-projects-manager-wrapper").is(":visible");return o?t.$node.on("click",t.resolve):f(t),t}}},stepOpenBoard:function(n){var r=n.boardName.toLowerCase(),a=-1===r.indexOf("board")?" board":"",i=n.message?n.message:"Open the "+n.boardName+a+".",s=function(){var t=this,o=t.boardName,n=e('.tau-app-secondary-pane .tau-boardselector__item.i-role-item:contains("'+o+'")');return n.filter(function(){return e(this).text()===o}).eq(0)},c=function(){var t=this,o=t.boardName,n=e('.tau-app-secondary-pane .tau-boardselector__item.i-role-item.tau-boardselector__item_active_true:contains("'+o+'")');return n.filter(function(){return e(this).text()===o}).eq(0)};return{message:i,boardName:n.boardName,target:function(n){var r=e.Deferred(),a=t.bind(s,n),i=a();return i.size()>0?r.resolve(i):n.configurator.getHelpScenarioService().createBoardFromTemplate(this.boardName).done(function(){t.delay(o,1500,a,r)}),r},complete:function(n){if(n.$node.hasClass("tau-boardselector__item_active_true"))t.delay(n.resolve,2e3);else{var r=t.bind(c,n.step),a=e.Deferred();a.done(function(){t.delay(n.resolve,2e3)}),o(r,a)}return n}}},stepSelectYourProject:function(n,r){var a={message:"Select the project you've just created.",target:function(){return o(n)},complete:function(t){var o=m(t.step),n=t.$node.find("option:selected").text();return t.$node.is(":disabled")||n.toLowerCase()===o.toLowerCase()?f(t):(t.$node.find("option").each(function(){var t=e(this),n=t.text();n.toLowerCase()===o.toLowerCase()&&t.html("<b>"+n+"</b>")}),t.$node.on("change",function(){var e=t.$node.find("option:selected").text();e.toLowerCase()===o.toLowerCase()&&t.resolve()})),t}};return t.extend(a,r)},stepSelectToday:function(e,n){var r={message:"<p>Let's assume that it starts <b>today</b>. You can change the start date later.</p>",target:function(){return o(e)},complete:function(e){return e.$node.on("change",function(){var t=e.$node.val(),o=t.replace(/-/g," "),n=new Date(Date.parse(o));(new Date).toDateString()===n.toDateString()&&e.resolve()}),e}};return t.extend(r,n)},stepClickOrEscToClose:function(n){var r={message:"Click to close this screen.",direction:"none",targetSelector:".tau-cover-view_page .close:visible",target:function(){return o(this.targetSelector)},complete:function(o){return window.scrollTo(0,0),o.$node.on("click",o.resolve),o.escHandler=t.bind(function(e){27===e.keyCode&&this.resolve()},o),e(document).on("keydown",o.escHandler),o},teardown:function(t){e(document).off("keydown",t.escHandler)}};return t.extend(r,n)},stepClickForQuickAdd:function(n){var i={quickAddRoleClass:"i-role-ch-quickadd-target",target:function(n){var a=this.quickAddRoleClass,i=e(this.todoCellSelector);e(".i-role-board-body").find("."+a).not(i).addClass(a+"-help-depressed").removeClass(a);var s=t.bind(function(){var t=e(this.todoCellSelector);t.find(" .tau-quick-add .tau-add").is(":visible")||t.mouseenter()},this),c={descriptor:null};r(c,s,250),n.scenarioContext.set("quick-add-inductor-process",c);var u=this.todoCellSelector+" .tau-quick-add .tau-add";return o(u)},complete:function(e){var t=e.$holder.tauBubble("getContent");return t.on("click",".i-role-next",e.resolve),e.$node.off("click",e.resolve).one("click",e.resolve),e},teardown:function(t){var o=t.step.scenarioContext.get("quick-add-inductor-process");a(o);var n=this.quickAddRoleClass,r=n+"-help-depressed";e(".i-role-board-body").find("."+r).addClass(n).removeClass(r)}};return t.extend(i,n)},stepFinish:function(e){e=e||{};var n=e.processName?" "+e.processName+" ":" ",r={message:["<p>Thanks! You're done with the"+n+"tour now.</p>","<p>One important note before you move on:</p>","<p>As you create and remove boards from your screen, you simply change how you see one and the same data set.</p>","<p>You do not actually remove or delete anything, all your information is safe. So, don't be scared to experiment.</p>","<p>Try various mixes of vertical and horizontal lanes or use the predefined templates to get a clear view of what you want to focus on.</p>","<p>Play with the boards!</p>",'<p class="tau-help-continue"><button class="i-role-got-it tau-btn tau-primary">Got it, thanks</button></p>'].join(""),target:function(){return o(".tau-app-secondary-pane .tau-add-board-btn")},complete:function(e){var t=e.$holder.tauBubble("getContent");return e.$node.click(e.resolve),t.on("click",".i-role-got-it",e.resolve),e}};return t.extend(r,e)},flowUserGuide:{name:"User Guide",steps:[{message:["<p>Read User Guide to learn more about concepts and User Interface.</p>",'<p class="tau-help-continue"><button class="i-role-next tau-btn tau-primary">OK! I\'m done</button></p>'].join(""),direction:"none",target:function(){e(".tau-flow-info-panel").tauBubble("hide"),e(".tau-flow-info").removeClass("tau-flow-started tau-flow-info");var t=".tau-main-menu .tau-menu-item.tau-menu-items-help";return o(t)},complete:function(e){e.$node.one("click",e.resolve);var t=e.$holder.tauBubble("getContent");return t.on("click",".i-role-next",e.resolve),e}}]}};return b.flowIntroVideo={name:"Watch video",steps:[b.stepVideoIntro({url:"//www.youtube.com/embed/Lxl2sTEyYaE"})]},b});