using BIMiconToolbar.Helpers.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMiconToolbar.NumberByPick
{
    public class NumberByPickModel : ModelBase
    {
        public string Prefix { get; set; }
        public double StartNumber { get; set; }
    }
}
