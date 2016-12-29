﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Orchard.ContentFields.Settings;
using Orchard.ContentFields.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Display.ContentDisplay;
using Orchard.ContentManagement.Display.Models;
using Orchard.ContentManagement.Metadata.Models;
using Orchard.DisplayManagement.ModelBinding;
using Orchard.DisplayManagement.Views;
using Orchard.Localization;

namespace Orchard.ContentFields.Fields
{
    public class LinkFieldDisplayDriver : ContentFieldDisplayDriver<LinkField>
    {
        public LinkFieldDisplayDriver(IStringLocalizer<LinkFieldDisplayDriver> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public override IDisplayResult Display(LinkField field, BuildFieldDisplayContext context)
        {
            return Shape<DisplayLinkFieldViewModel>("LinkField", model =>
            {
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            })
            .Location("Content")
            .Location("SummaryAdmin", "");
        }

        public override IDisplayResult Edit(LinkField field, BuildFieldEditorContext context)
        {
            return Shape<EditLinkFieldViewModel>("LinkField_Edit", model =>
            {
                var settings = context.PartFieldDefinition.Settings.ToObject<LinkFieldSettings>();
                model.Value = (field.IsNew()) ? settings.DefaultValue : field.Value;
                model.Text = (field.IsNew()) ? settings.TextDefaultValue : field.Text;
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(LinkField field, IUpdateModel updater, UpdateFieldEditorContext context)
        {
            bool modelUpdated = await updater.TryUpdateModelAsync(field, Prefix, f => f.Value, f => f.Text);

            if (modelUpdated)
            {                
                var settings = context.PartFieldDefinition.Settings.ToObject<LinkFieldSettings>();

                if (settings.Required && String.IsNullOrWhiteSpace(field.Value))
                {
                    updater.ModelState.AddModelError(Prefix, T["The url is required for {0}.", context.PartFieldDefinition.DisplayName()]);
                }
                else if (!string.IsNullOrWhiteSpace(field.Value) && !Uri.IsWellFormedUriString(field.Value, UriKind.RelativeOrAbsolute))
                {
                    updater.ModelState.AddModelError(Prefix, T["{0} is an invalid url.", field.Value]);
                }
                else if (settings.LinkTextMode == LinkTextMode.Required && string.IsNullOrWhiteSpace(field.Text))
                {
                    updater.ModelState.AddModelError(Prefix, T["The link text is required for {0}.", context.PartFieldDefinition.DisplayName()]);
                }
                else if (settings.LinkTextMode == LinkTextMode.Static && string.IsNullOrWhiteSpace(settings.TextDefaultValue))
                {
                    updater.ModelState.AddModelError(Prefix, T["The text default value is required for {0}.", context.PartFieldDefinition.DisplayName()]);
                }
            }

            return Edit(field, context);
        }
    }
}