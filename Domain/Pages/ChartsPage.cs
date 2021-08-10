using Excelsa.Core.Base;
using Excelsa.Domain.Components;
using System.Collections.Generic;
using System.Text;

namespace Excelsa.Domain.Pages
{
    class ChartsPage : PageBase
    {
        public override string Id { get; set; } = "1_1";
        public override string PageName { get; set; } = "Charts";

        public override StringBuilder TestLog { get; set; }

        public override List<ComponentBase> Components { get; set; } = new List<ComponentBase>()
        {

        };
    }
}
