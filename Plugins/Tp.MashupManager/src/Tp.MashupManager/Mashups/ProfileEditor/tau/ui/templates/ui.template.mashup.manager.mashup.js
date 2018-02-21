/*eslint max-len: [1, 210, 4]*/
/*jshint maxlen: 210*/
tau.mashups
    .addDependency('tau/core/templates-factory')
    .addDependency('tau/mashup.manager/ui/templates/ui.template.mashup.manager.mashup.metainfo')
    .addModule('tau/mashup.manager/ui/templates/ui.template.mashup.manager.mashup', function(templates) {
        var config = {
            name: 'mashup.manager.mashup',
            engine: 'jqote2',
            markup: [
                '<% var mashupMetaInfo = this.mashupMetaInfo; %>',
                '<div class="mashup-details i-role-mashupDetails">',

                '<div class="p-10">',

                '<p class="label right i-role-mashupMetaInfo">',
                '<%= fn.sub("mashup.manager.mashup.metainfo", this) %>',
                '</p>',

                '<h3 class="h3">Mashup</h3>',
                '<p class="label">',
                '<span class="small">',
                'Create your custom mashup<span class="library-link i-role-libraryLink">, or use any from ',
                '<a class="note" href="#mashup.manager=library" title="Targetprocess Mashup library">Targetprocess Mashup Library</a></span>.',
                '</span>',
                '<span class="small"> <a class="note" target="_blank" href="https://dev.targetprocess.com/docs/mashups-overview" title="More on mashups">More on mashups.</a></span>',
                '</p>',

                '<p class="label pt-10">Name&nbsp;<span class="error" name="NameErrorLabel"></span></p>',
                '<p class="label"><span class="small">Give a unique name to your mashup</span></p>',
                '<input type="text" name="Name" value="<%! this.name || \'\' %>" class="input mashup-details__input i-role-mashupName" />',

                '<p class="label pt-10">',
                '<span class="small">',
                '<label><input type="radio" name="IsEnabled" value="true" class="i-role-mashupIsEnabled" <%= mashupMetaInfo.isEnabled ? \'checked="checked"\' : \'\' %> />Enabled</label>',
                '<label><input type="radio" name="IsEnabled" value="false" class="i-role-mashupIsEnabled" <%= mashupMetaInfo.isEnabled ? \'\' : \'checked="checked"\' %> />Disabled</label>',
                '</span>',
                '</p>',

                '<p class="label pt-10">Placeholder(s)</p>',
                '<p class="label">',
                '<span class="small">',
                'This determines where your mashup will be attached and executed.<br>',
                'Frequently used global placeholders are <b>footerPlaceholder</b>, <b>RestUI_Board</b>, and <b>tp3placeholder</b>. ',
                'These will set your mashup as part of the common UI.<br>',
                'Alternatively, you can use page-specific (local) names to run your mashup on supported pages. ',
                '<a class="note" target="_blank" href="https://dev.targetprocess.com/docs/placeholders">More on placeholders</a><br>',
                '<br>',
                '<a href="javascript:void(0);" tabindex="-1" class="note i-role-placeholdersHelpLink">Quick tips</a>',
                '</span>',

                '<div style="display: none;" class="i-role-placeholdersHelp">',
                '<ul class="pl-15 mt-5 mb-5">',

                '<li class="pb-10">',
                'The rule of thumb is: remove path prefix and .aspx, and put underscore instead of slashes<br>',
                '<table>',
                '<tr>',
                '<td>For instance, page for general settings has this URL:</td>',
                '<td></td>',
                '<td>So, the local placeholder will be</td>',
                '</tr><tr>',
                '<td>https://localhost/Admin/GlobalSetting.aspx</td>',
                '<td>&rarr;</td>',
                '<td><span class="rules-actions">Admin_GlobalSetting</span></td>',
                '</tr>',
                '</table>',
                '</li>',

                '<li class="pb-10">',
                'Use Ctrl+C/Ctrl+V to paste URL and generate placeholder name automatically',
                '</li>',

                '<li class="pb-10">',
                'If you need to attach a mashup to multiple placeholders, use commas to separate their names',
                '</li>',

                '<li class="pb-10">',
                'Placeholder names are case-insensitive',
                '</li>',

                '</ul>',
                '</div>',
                '</p>',

                '<input type="text" name="Placeholders" value="<%! this.placeholders %>" class="input mashup-details__input i-role-placeholders" />',

                '<p class="label pt-10">Code</p>',
                '<p class="label">',
                '<span class="small">JavaScript code of your mashup. ',
                '<a class="note" target="_blank" href="https://dev.targetprocess.com/docs/create-first-mashup" title="More on writing mashups" class="note">More on writing mashups</a></span>',
                '</p>',

                '<div class="mashup-details__code-wrapper"><pre name="Script" class="input i-role-script mashup-details__code-wrapper__pre"><%! this.script %></pre></div>',
                '</div>',

                '<div class="separator"></div>',

                '<div class="p-10">',
                '<p class="error-message" style="display: none;" class="i-role-failedOperation"><span></span></p>',
                '<button class="tau-btn tau-primary i-role-save" onclick="return false;">Save Mashup</button>',
                '</div>',

                '</div>'
            ]
        };

        return templates.register(config);
    });
