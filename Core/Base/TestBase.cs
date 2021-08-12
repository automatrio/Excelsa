using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excelsa.Core.Base
{
    public abstract class TestBase : TestFlow
    {
        public abstract List<PageBase> Pages { get; }
        public abstract void ConductTest();
    }
}
