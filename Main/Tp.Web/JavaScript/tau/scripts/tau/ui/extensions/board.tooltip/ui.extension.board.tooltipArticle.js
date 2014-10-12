define(["Underscore","jQuery","tau/core/extension.base"],function(t,e,o){e.widget("ui.tauBubbleArticle",e.ui.tauBubble,{});var i="http://guide.targetprocess.com/teams-and-projects.html#visibility",r=t.map([{id:"board.add",html:"<p>Create new Views – just choose what you want to see and how you want to see it.</p><p>Play with the Views!</p>",url:"http://guide.targetprocess.com/boards/boards-overview.html"},{id:"project.formAdd.selectProcess",html:"<p>You can use our default process or create your own with custom workflows and practices.</p>",url:i},{id:"project.formAdd.assignPeople",html:"<p>People can be included into both teams and projects so that you can vary work visibility.</p>",url:i},{id:"project.formAdd.assignTeams",html:"<p>You can have one team that works on several projects or one large project with several teams. Any mix of teams and projects, no matter how crazy, is supported.</p>",url:i},{id:"team.formAdd.assignPeople",html:"<p>People can be included into both teams and projects so that you can vary work visibility.</p>",url:i},{id:"team.formAdd.assignProjects",html:"<p>You can have one team that works on several projects or one large project with several teams. Any mix of teams and projects, no matter how crazy, is supported.</p>",url:i},{id:"board.templates",html:"<p>Easily create Views from predefined templates.</p>",url:"http://guide.targetprocess.com/boards/templates.html"},{id:"board.editor",html:"<p>On a View you can see different data as cards, organize your selected cards by common properties, and apply filters to cards and lanes.</p>",url:"http://guide.targetprocess.com/boards/boards-how-to.html",linkText:"See how to setup a View"},{id:"board.editor.access",html:"<p>Anyone can create Views that are private or shared with their team/project, but only administrators can create public Views.</p>",url:"http://guide.targetprocess.com/boards/access.html"},{id:"board.editor.prioritization",html:"<p>By default you should hold the Shift key to prioritize cards. While holding Shift, simply drag a card and move it into the desired position.</p><p>Remember that you can always change the prioritization mode for the View.</p>",url:"http://guide.targetprocess.com/boards/sort.html"},{id:"board.editor.selectCards",html:"<p>You can display various entities on Views. Entities with common properties can be displayed at the same time.</p>",url:"http://guide.targetprocess.com/boards/cards.html"},{id:"board.editor.selectLanes",html:"<p>Organize your selected items by common properties. You can use different properties (even custom fields) as your horizontal and vertical lanes.</p>",url:"http://guide.targetprocess.com/boards/lanes.html",linkText:"View the full list"},{id:"board.editor.selectListLevels",html:"<p>Organize your selected items by common properties. You can use different properties (even custom fields) as your 1st and 2nd levels.</p>",url:"http://guide.targetprocess.com/boards/lists.html"},{id:"board.editor.selectListCards",html:"<p>You can display various entities on Views. Entities with common properties can be displayed at the same time.</p>",url:"http://guide.targetprocess.com/boards/lists.html"},{id:"timeline.options.plannedInPast",html:"<p>This checkbox shows/hides planned dates for Done cards.</p>",url:"https://guide.targetprocess.com/boards/getting-started-with-the-timeline-view.html#planned-time"},{id:"timeline.options.showForecast",html:"<p>This checkbox shows/hides Forecast for cards.</p>",url:"https://guide.targetprocess.com/boards/getting-started-with-the-timeline-view.html#forecast-time"},{id:"board.editor.filter",html:["<p>","<strong>Basic mode.</strong> Just type keywords and filter cards by name. You can filter lanes (columns and rows) this way, as well.","</p>","<p>",'<strong>Advanced mode.</strong> These filters are much more powerful. Start off with "?" and from there you can create a complex query to use as a filter.',"</p>"].join(""),url:"http://guide.targetprocess.com/filters.html"},{id:"board.editor.saveContext",html:'<p>Specify a Projects-Teams context for&nbsp;this&nbsp;View, or go with "not specified" context.</p>',url:"https://guide.targetprocess.com/boards/context.html"}],function(e){return t.defaults(e,{url:"http://guide.targetprocess.com",linkText:"Read more&hellip;"})});return o.extend({destroy:function(){this._super()},"bus afterRender":function(){var o=t.bind(function(t){var o=e(t.currentTarget);o.data("ui-tauBubbleArticle")||this._initTooltip(o)},this);e(document).on("mouseenter.tooltipArticle",".i-role-tooltipArticle",o),this.on("destroy",function(){e(document).off("mouseenter.tooltipArticle",".i-role-tooltipArticle",o)})},_initTooltip:function(e){var o=e.data("articleId"),i=t.findWhere(r,{id:o});i&&e.tauBubbleArticle({content:i.html+'<a target="_blank" href="'+i.url+'">'+i.linkText+"</a>",zIndex:99999,template:['<div class="tau-bubble-board">','<div class="tau-bubble-board__arrow" role="arrow"></div>','<div class="tau-bubble-board__inner tau-container" role="content"></div>',"</div>"].join(""),className:"tau-bubble-tooltipArticle",showEvent:"click",hideEvent:"click",stackName:"tooltipArticle",delay:500,showOnCreation:!1,onShow:function(){this.$target.toggleClass("tau-help_state_active",!0)},onHide:function(){this.$target.toggleClass("tau-help_state_active",!1)},onPositionConfig:"board.add"==i.id?this._tooltipPositionConfigRight:this._tooltipPositionConfigTop,onArrowPositionConfig:this._tooltipArrowPositionConfigTop})},_tooltipPositionConfigTop:function(t){t.at="left-8 top",t.my="left-50 bottom"},_tooltipArrowPositionConfigTop:function(t){return"center top"==t.at?t.at="left+10 top":"center bottom"==t.at&&(t.at="left+10 bottom"),t},_tooltipPositionConfigRight:function(t){t.at="right center",t.my="left center"}})});