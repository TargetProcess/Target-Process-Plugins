Ext.ns('Tp.controls');

Tp.controls.DateField = Ext.extend(Ext.form.DateField, {
	onTriggerClick: function () {
		if (this.disabled) {
			return;
		}
		if (this.menu == null) {
			this.menu = new Ext.menu.DateMenu({
				hideOnClick: false,
				focusOnSelect: false
			});
		}
		this.onFocus();
		Ext.apply(this.menu.picker, {
			minDate: this.minValue,
			maxDate: this.maxValue,
			disabledDatesRE: this.disabledDatesRE,
			disabledDatesText: this.disabledDatesText,
			disabledDays: this.disabledDays,
			disabledDaysText: this.disabledDaysText,
			format: this.format,
			showToday: this.showToday,
			minText: String.format(this.minText, this.formatDate(this.minValue)),
			maxText: String.format(this.maxText, this.formatDate(this.maxValue))
		});

		this.menu.picker.setValue(this.getValue() || new Date());
		var menu = this.menu;
		if (Ext.isIE) {
			this.menu.on('show', function (ev) {
				var pickerEl = menu.picker.getEl();
				menu.el.setWidth(pickerEl.getWidth());
			});
		}
		this.menu.show(this.positionEl, this.rightAlignCalendar ? 'tr-br?' : 'tl-bl?');
		this.menuEvents('on');
	}
});

Ext.reg('tpdatefield', Tp.controls.DateField);