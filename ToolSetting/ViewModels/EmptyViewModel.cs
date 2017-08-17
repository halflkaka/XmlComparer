using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace XmlCompare.ToolSetting.ViewModels
{
    [Export(typeof(EmptyViewModel))]
    public class EmptyViewModel : BindableBase
    {
        public EmptyViewModel()
        {

        }
    }
}
