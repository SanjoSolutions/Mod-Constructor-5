﻿using Constructor5.Core;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Constructor5.Elements.Situations
{
    [XmlSerializerExtraType]
    public abstract class SituationTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public abstract string Label { get; }

        protected internal abstract void OnExport(SituationExportContext context);
    }
}
