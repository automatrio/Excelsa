
using Excelsa.Core.Base;


namespace Excelsa.Domain.Components
{
    class MonthlyIncomeChartComponent : ComponentBase
    {
        private static readonly string path = "";

        protected override string ScrollIntoViewScript { get; set; } = "";
        protected override string ScrollIntoViewFallbackScript { get; set; } = "";

        public MonthlyIncomeChartComponent(
            SpecificationBase specification)
            : base (
            "Validating the component MonthlyIncomeChart",
            "The component MonthlyIncomeChart could not be validated.",
            specification
            )
        { }

        public override void MainFlow()
        {
            
        }
    }
}
