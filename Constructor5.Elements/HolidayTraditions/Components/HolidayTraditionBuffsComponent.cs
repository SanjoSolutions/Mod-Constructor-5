﻿using Constructor5.Base.ElementSystem;
using Constructor5.Base.ExportSystem.AutoTuners;
using Constructor5.UI.AutoGeneratedEditors;
using Constructor5.Core;
using System;

namespace Constructor5.Elements.HolidayTraditions.Components
{
    [XmlSerializerExtraType]
    public class HolidayTraditionBuffsComponent : HolidayTraditionComponent
    {
        public override string ComponentLabel => "Holiday Buffs";

        [AutoEditorReferenceList("Buff", ShowCreateButton = true)]
        public ReferenceList Buffs { get; set; } = new ReferenceList();

        protected internal override void OnExport(HolidayTraditionExportContext context) => AutoTunerInvoker.Invoke(this, context.Tuning);
    }
}
