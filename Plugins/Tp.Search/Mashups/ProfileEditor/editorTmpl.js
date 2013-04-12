tau.mashups.addModule("tau/mashups/TpSearch/ProfileEditor/editorTmpl",
    '<div>' +
        '	<div class="task-creator-settings">' +
        '		<div class="pad-box">' +
        '			<p class="label">Search Plugin Profile</p>' +
        '			<p class="note"><span class="small">The plugin for search.</span></p>' +
        '			<p><input id=enabled-for-tp2 type=checkbox {{if Settings.EnabledForTp2 }}checked=checked {{/if}} /> <label for=enabled-for-tp2>Enable for TP2</label></p>' +
        '			<p class="warning">Works only in modern browsers</p>' +
        '		</div>' +
        '	</div>' +
        '	<div class="controls-block">' +
        '	</div>' +
        '</div>');