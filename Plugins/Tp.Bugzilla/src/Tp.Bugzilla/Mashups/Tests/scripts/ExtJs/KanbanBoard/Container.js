Ext.ns('Tp.controls.kanbanboard');

/**
* A container which displays Kanban board items such as <em>Tasks</em> and <em>Bugs</em>
* and allows dragging and dropping them onto another item container, such as swimlane or backlog.
*/
Tp.controls.kanbanboard.Container = Ext.extend(Ext.Container, {
	autoEl: {
		tag: 'div',
		cls: 'kanban-dropzone'
	},

	defaultProxyPos: -1,
	currentProxyPos: -1,
	controller: null,
	sortable: true,
	sortOn: true,

	constructor: function (config) {
		if (!config.controller) {
			throw new Error('Controller not specified');
		}

		config = Ext.apply({
			listeners: {
				afterrender: {
					fn: function (container) {
						container.el.on('scroll', this.onScroll, this);
					},
					scope: this
				}
			}
		}, config);

		Tp.controls.kanbanboard.Container.superclass.constructor.call(this, config);
		this.addEvents({ "begindrag": true, "enddrag": true, "enterdrag": true });
	},

	onScroll: function (e, d, o) {
		var t = this['tButton'], b = this['bButton'];
		if (this.canSort()) {
			if (t) {
				if (d.scrollTop > 0) {
					if (!t.isVisible()) {
						t.show('display');
					}
				} else {
					t.hide('display');
				}
			}
			if (b) {
				if (d.scrollHeight - d.clientHeight > d.scrollTop) {
					if (!b.isVisible()) {
						b.show('display');
					}
				} else {
					b.hide('display');
				}
			}
		}
	},

	highlighDisabled: function (item, entity) {
	},

	highlightItemsCount: function () {
	},

	allowDrop: function (item, entity) {
		return false;
	},

	onDrop: function (dd, e, data) {
	},

	isProxyInDefaultPos: function () {
		return this.defaultProxyPos === this.currentProxyPos;
	},

	onBeforeNotifyValidDrop: function (dd, e, data) {
		Ext.fly(data.ddel).setOpacity(0.2);
		if (this.canSort()) {
			Ext.fly(data.ddel).insertBefore(this.hole).removeClass('x-hide-' + data.item.hideMode);
		} else {
			Ext.fly(this.el).appendChild(data.ddel).removeClass('x-hide-' + data.item.hideMode);
		}
		if (!this.dragZone.dragging) {
			data.item.ownerCt.removeScrollButtons();
			data.item.ownerCt.removeProxy();
		}
		this.removeScrollButtons();
		this.removeProxy();
	},

	onStartDrag: function (x, y) {
		if (this.dragZone.dragging && this.dragZone.dragData) {
			var p = this.createProxy(this.dragZone, this.dragZone.dragData, x, y);
			if (p) {
				this.dragZone.dragData.item.hide();
			}
		}
	},

	removeProxy: function () {
		if (this.hole) {
			Ext.fly(this.hole).remove();
			delete this.hole;
			this.hole = null;
		}
	},

	createProxy: function (dd, data, x, y) {
		this.removeProxy();
		if (this.canSort()) {
			if (typeof x == 'object') {
				y = x[1];
				x = x[0];
			}
			var ddel = Array.findOne(dd.proxy.getGhost().dom.children, function (el) {
				return el.id == data.ddel.id;
			});
			var c = {
				cls: ddel.className + ' kanbanboard-dropdown-placeholder',
				style: 'height:' + ddel.clientHeight + 'px',
				tag: 'div'
			};
			if (this.dragZone.dragging) {
				this.defaultProxyPos = this.currentProxyPos = this.items.indexOf(data.item);
				this.hole = Ext.DomHelper.insertBefore(data.item.el, c);
			} else {
				this.defaultProxyPos = this.currentProxyPos = -1;
				if (this.items.length == 0) {
					this.hole = this.el.insertFirst(c);
					this.currentProxyPos = 0;
				} else {
					var i = this.itemFromPos(x, y);
					if (i) {
						if (i.el.getY() + i.el.getHeight() < y) {
							this.hole = Ext.DomHelper.insertAfter(i.el, c);
							this.currentProxyPos = this.items.length;
						} else {
							this.hole = Ext.DomHelper.insertBefore(i.el, c);
							this.currentProxyPos = this.items.indexOf(i);
						}
					} else {
						this.hole = Ext.DomHelper.insertAfter(this.items.itemAt(this.items.length - 1).el, c);
						this.currentProxyPos = this.items.length;
					}
				}
			}
		}
		return this.hole;
	},

	removeScrollButtons: function () {
		Array.forEach(['tButton', 'bButton'], function (name) {
			if (this[name]) {
				/*Array.forEach(Ext.EventManager.getListeners(this[name], 'mouseleave') || [], function (f) {
				if (typeof f[0] === 'function') {
				f[0].call();
				}
				}, this);*/
				this[name].remove();
				delete this[name];
				this[name] = null;
			}
		}, this);
	},

	createScrollButtons: function () {
		if (this.canSort() && this.el.isScrollable()) {
			this.removeScrollButtons();
			var viewSize = this.el.getViewSize(), d = this.el.dom;
			var autoScrollTask = new Ext.util.DelayedTask(this.scrollItems, this);
			Array.forEach([{ name: 'tButton', func: this.scrollUp }, { name: 'bButton', func: this.scrollDown}], function (obj) {
				var name = obj.name, func = obj.func;
				this[name] = this.el[name === 'tButton' ? 'insertFirst' : 'createChild'].call(this.el, {
					style: 'display:none; height:30px; width:' + (d.scrollWidth - 10) + 'px; bottom: ' + ((this.controller.uxKanbanBoardPanel.el.getRegion().bottom - this.el.getRegion().bottom) + (name === 'tButton' ? (viewSize.height - 30) : 0)) + 'px',
					cls: 'kanbanboard-button-placeholder ' + (name === 'tButton' ? 'top' : 'bottom'),
					tag: 'div'
				});
				if (name === 'tButton' && d.scrollTop > 0) {
					this[name].show('display');
				}
				if (name === 'bButton' && d.scrollHeight - d.clientHeight > d.scrollTop) {
					this[name].show('display');
				}
				this[name].on('mouseenter', function () { autoScrollTask.delay(50, null, null, [autoScrollTask, func, 10]); });
				this[name].on('mouseleave', function () { autoScrollTask.cancel(); });
				this[name].on('mouseup', function () { autoScrollTask.cancel(); });
			}, this);
		}
	},

	scrollUp: function (element, step) {
		if (element.dom.scrollTop > 0) {
			element.scroll('up', step);
			return true;
		}
		return false;
	},

	scrollDown: function (element, step) {
		var d = element.dom;
		if (d.scrollHeight - d.clientHeight > d.scrollTop) {
			this.el.scroll('down', step);
			return true;
		}
		return false;
	},

	scrollItems: function (task, func, step) {
		if (func.call(this, this.el, step)) {
			task.delay(50, null, null, [task, func, step]);
		}
		else {
			task.cancel();
		}
	},

	itemFromPos: function (x, y) {
		var res;
		this.items.each(function (i) {
			if (i.isVisible()) {
				if (res == null) {
					res = i;
				} else {
					res = Math.abs(i.el.getY() - y) < Math.abs(res.el.getY() - y) ? i : res;
				}
			}
		}, this);
		return res;
	},

	proxyPrev: function () {
		var prev = this.items.itemAt(this.currentProxyPos - (!this.dragZone.dragging || this.defaultProxyPos > this.currentProxyPos ? 1 : 0));
		return prev && !prev.isVisible() ? prev.prevVisible() : prev;
	},

	proxyNext: function () {
		var next = this.items.itemAt(this.currentProxyPos + (!this.dragZone.dragging || this.defaultProxyPos > this.currentProxyPos ? 0 : 1));
		return next && !next.isVisible() ? next.nextVisible() : next;
	},

	enableSorting: function (isEnabled) {
		this.sortOn = isEnabled;
	},

	canSort: function () {
		return this.sortable && this.sortOn;
	},

	onEnterDropTarget: function (dd, e, data, dropAllowed) {
		if (this.canSort() && dropAllowed === Ext.dd.DropTarget.prototype.dropAllowed) {
			if (!this.dragZone.dragging) {
				this.createProxy(dd, data, e.getXY());
			}
			this.createScrollButtons();
		}
	},

	onOverDropTarget: function (dd, e, data, dropAllowed) {
		if (this.canSort() && dropAllowed === Ext.dd.DropTarget.prototype.dropAllowed) {
			this.moveProxy(dd, e, data, e.getXY());
		}
	},

	onOutDropTarget: function (dd, e, data) {
		if (this.dragZone.dragging) {
			this.moveProxyToDefault(dd, e, data);
		} else {
			this.removeProxy();
		}
		this.removeScrollButtons();
	},

	onEndDrag: function (data, e) {
	},

	isCommentRequired: function (data) {
		return false;
	},

	onNotifyDropTarget: function (dd, e, data) {
		if (this.dragZone.dragging && (!this.canSort() || this.isProxyInDefaultPos())) {
			return false;
		}
		if (this.isCommentRequired(data)) {
			var commentDialog = new Tp.controls.CommentDialog({
				listeners: {
					submit: {
						fn: function (d) {
							this.onBeforeNotifyValidDrop(dd, e, data);
							this.onDrop(dd, e, data, d.commentText.getValue() || '', this.canSort() ? this.proxyPrev() : undefined, this.canSort() ? this.proxyNext() : undefined);
						},
						scope: this
					},
					cancel: {
						fn: function () {
							dd.onInvalidDrop(this.dropTarget, e, this.dropTarget.id);
							this.el.removeClass(this.dropTarget.overClass);
							this.removeScrollButtons();
							this.removeProxy();
						},
						scope: this
					}
				}
			});
			commentDialog.show();
		} else {
			this.onBeforeNotifyValidDrop(dd, e, data);
			this.onDrop(dd, e, data, '', this.canSort() ? this.proxyPrev() : undefined, this.canSort() ? this.proxyNext() : undefined);
		}
		return true;
	},

	afterRender: function () {
		Tp.controls.kanbanboard.Container.superclass.afterRender.call(this);
		this.el.unselectable();
		this.initDragAndDrop();
	},

	moveProxy: function (dd, e, data, x, y) {
		if (this.canSort() && this.hole) {
			if (typeof x == 'object') {
				y = x[1];
				x = x[0];
			}
			var pReg = Ext.fly(this.hole).getRegion();
			if (pReg.top <= y && pReg.bottom >= y) {
				return;
			}
			var col = this.items, d = this.defaultProxyPos, c, prev = this.proxyPrev(), next = this.proxyNext();
			if (prev && (y <= prev.el.getY())) {
				var p;
				while (prev) {
					p = prev.prevVisible();
					if (y <= prev.el.getY() && (p == null || y > p.el.getY())) {
						this.hole = Ext.fly(this.hole).parent().dom.removeChild(this.hole);
						Ext.fly(this.hole).insertBefore(prev.el);
						c = col.indexOf(prev);
						if (this.dragZone.dragging && d < c) {
							c--;
						}
						if (this.dragZone.dragging && this.defaultProxyPos < this.currentProxyPos) {
							var ps = this.items.itemAt(c);
							if (!ps.isVisible() && this.defaultProxyPos > this.items.indexOf(ps.prevVisible())) {
								c = this.defaultProxyPos;
							}
						}
						this.currentProxyPos = c;
						if (this.dragZone.dragging) {
							this.el[this.isProxyInDefaultPos() ? 'removeClass' : 'addClass'].call(this.el, this.dropTarget.overClass);
						}
						this.fireEvent('positionchanged', this, data.item, data.entity, this.hole, c);
						break;
					}
					prev = p;
				}
			}
			if (next && (y >= next.el.getY())) {
				var n;
				while (next) {
					n = next.nextVisible();
					if (y >= next.el.getY() && (n == null || y < n.el.getY())) {
						this.hole = Ext.fly(this.hole).parent().dom.removeChild(this.hole);
						Ext.fly(this.hole).insertAfter(next.el);
						c = col.indexOf(next);
						if (!this.dragZone.dragging || d > c) {
							c++;
						}
						if (this.dragZone.dragging && this.defaultProxyPos > this.currentProxyPos) {
							var ns = this.items.itemAt(c);
							if (!ns.isVisible() && this.defaultProxyPos < this.items.indexOf(ns.nextVisible())) {
								c = this.defaultProxyPos;
							}
						}
						this.currentProxyPos = c;
						if (this.dragZone.dragging) {
							this.el[this.isProxyInDefaultPos() ? 'removeClass' : 'addClass'].call(this.el, this.dropTarget.overClass);
						}
						this.fireEvent('positionchanged', this, data.item, data.entity, this.dom, c);
						break;
					}
					next = n;
				}
			}
		}
	},

	moveProxyToDefault: function (dd, e, data) {
		if (this.dragZone.dragging && this.canSort() && !this.isProxyInDefaultPos()) {
			var step = this.currentProxyPos > this.defaultProxyPos ? 1 : -1, pos = this.defaultProxyPos, it;
			while (pos != this.currentProxyPos) {
				pos += step;
				it = this.items.items[pos];
				if (it && it.isVisible()) {
					this.moveProxy(dd, e, data, it.getPosition());
					break;
				}
			}
		}
	},

	initDragAndDrop: function () {
		var allowDrop = this.allowDrop.createDelegate(this);
		var self = this;

		this.dragZone = new Ext.dd.DragZone(this.getEl(), {
			ddGroup: 'kanbanboard',
			scroll: false,

			getDragData: function (e) {
				//do not init drag in case of url 
				if (Ext.fly(e.getTarget()).dom.tagName.toUpperCase() == 'A') {
					return null;
				}

				Ext.WindowMgr.bringToFront(this.proxy);

				// Find Ext.Element which is being dragged.
				var targetEl = e.getTarget('div.kanban-item', 10, true);
				if (targetEl) {
					// Convert dragged Ext.Element to Tp.controls.kanbanboard.Item which encapsulates it.
					var itemCmp = Ext.ComponentMgr.get(targetEl.id);
					return {
						ddel: itemCmp.el.dom, // dd proxy
						item: itemCmp, // the component being dragged
						entity: itemCmp.entity // an entity attached to the component being dragged
					};
				}
				return null;
			},

			onStartDrag: function (x, y) {
				self.onStartDrag(x, y);
			},

			onDragDrop: function (e, id) {
				var target = this.cachedTarget || Ext.dd.DragDropMgr.getDDById(id);
				if (this.beforeDragDrop(target, e, id) !== false) {
					if (target.isNotifyTarget) {
						if (!target.notifyDrop(this, e, this.dragData)) {
							this.onInvalidDrop(target, e, id);
						}
					} else {
						this.onValidDrop(target, e, id);
					}
					if (this.afterDragDrop) {
						this.afterDragDrop(target, e, id);
					}
				}
				delete this.cachedTarget;
			},

			onBeforeDrag: function (data, e) {
				if (!self.controller.enabled) {
					return false;
				}
				self.fireEvent('begindrag', self, data.item, data.entity);
				return true;
			},

			onEndDrag: function (data, e) {
				self.onEndDrag(data, e);
				self.fireEvent('enddrag', self, data.item, data.entity);
			},

			beforeInvalidDrop: function (target, e, id) {
				Ext.dd.DragZone.prototype.beforeInvalidDrop.call(this, target, e, id);
				this.dragData.item.show();
				self.removeScrollButtons();
				self.removeProxy();
				return false;
			},

			onValidDrop: function (target, e, id) {
				if (this.afterValidDrop) {
					this.afterValidDrop(target, e, id);
				}
			}
		});

		this.dropTarget = new Ext.dd.DropTarget(this.getEl(), {
			ddGroup: 'kanbanboard',
			overClass: 'kanban-itemcontainer-dd-over',

			notifyEnter: function (dd, e, data) {
				var enabled = this.verifyDropEnabled(dd, e, data);
				self.fireEvent('enterdrag', self, data.item, data.entity);
				self.onEnterDropTarget(dd, e, data, enabled);
				if (self.dragZone.dragging && (!self.canSort() || self.isProxyInDefaultPos())) {
					enabled = Ext.dd.DropTarget.prototype.dropNotAllowed;
				} else {
					Ext.dd.DropTarget.prototype.notifyEnter.call(this, dd, e, data);
				}
				return enabled;
			},

			notifyOver: function (dd, e, data) {
				var enabled = this.verifyDropEnabled(dd, e, data);
				self.onOverDropTarget(dd, e, data, enabled);
				if (self.dragZone.dragging && (!self.canSort() || self.isProxyInDefaultPos())) {
					enabled = Ext.dd.DropTarget.prototype.dropNotAllowed;
				}
				Ext.dd.DropTarget.prototype.notifyOver.call(this, dd, e, data);
				return enabled;
			},

			notifyOut: function (dd, e, data) {
				self.onOutDropTarget(dd, e, data);
				Ext.dd.DropTarget.prototype.notifyOut.call(this, dd, e, data);
			},

			notifyDrop: function (dd, e, data) {
				if (allowDrop(data.item, data.entity)) {
					return self.onNotifyDropTarget(dd, e, data);
				}
				return false;
			},

			verifyDropEnabled: function (dd, e, data) {
				if (!self.controller.enabled) {
					return Ext.dd.DropTarget.prototype.dropNotAllowed;
				}
				if (allowDrop(data.item, data.entity)) {
					return Ext.dd.DropTarget.prototype.dropAllowed;
				}
				else {
					return Ext.dd.DropTarget.prototype.dropNotAllowed;
				}
			}
		});
	},

	sort: function () {
		var comparator = this.comparator.createDelegate(this);
		this.items.sort('ASC', function (a, b) {
			if (a.entity != null && b.entity != null) {
				return comparator(a.entity, b.entity);
			}
			return 0;
		});
	},

	comparator: function (a, b) {
		throw new Error('Not implemented.');
	},

	repaint: function () {
		// this.doLayout(); won't work - ???
		var last;
		this.items.each(function (i) {
			if (i.rendered) {
				if (last != null) {
					i.el.insertAfter(last.el);
				}
				last = i;
			}
		}, this);
	}
});
