define(["require","tau/core/injector","Underscore"],function(e){var r=e("tau/core/injector"),o=e("Underscore");return r.defineModule(["boardMenuViewsMenuService"],function(e){return{getMenuItemClassName:o.constant("i-role-menu-action-favorites"),getContent:function(e){return e.board.isFavorite?"Remove from Favorites":"Add to Favorites"},shouldRender:function(e){var r=e.group;return r.isFavorite?r.getIsUngroupedGroup():!0},actionHandler:function(r){var o={isFavorite:r.board.isFavorite},t={isFavorite:!o.isFavorite};taus.track({action:"toggle view favorite",isFavorite:t.isFavorite,tags:["viewsMenu"]}),e.viewsApi.updateViewMenuItem(r.board.boardId,t,o)}}})});