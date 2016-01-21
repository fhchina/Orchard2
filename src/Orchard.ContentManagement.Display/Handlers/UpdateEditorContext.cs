﻿using Orchard.DisplayManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Display.Handlers
{
    public class UpdateEditorContext : BuildEditorContext
    {
        public UpdateEditorContext(IShape model, IContent content, string groupId, IShapeFactory shapeFactory)
            : base(model, content, groupId, shapeFactory)
        {
        }
    }
}