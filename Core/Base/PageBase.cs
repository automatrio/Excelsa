using System;
using System.Collections.Generic;
using System.Text;

namespace Excelsa.Core.Base
{
    public abstract class PageBase : ExcelsaDSL
    {
        abstract public string Id { get; set; }
        abstract public string PageName { get; set; }

        // Add your custom properties here

        virtual public bool IsPageNotFound { get; set; } = false;

        abstract public List<ComponentBase> Components { get; set; }
        abstract public StringBuilder TestLog { get; set; }

        public PageBase()
        {
            foreach(var component in Components)
            {
                component.PageNotFound += OnPageNotFound;
            }
        }

        public void GetTestStyle()
        {
            int amountErrors = 0;
            int amountWarnings = 0;

            foreach(var component in Components)
            {
                if(component.TestResultLog.Contains(TestResult.Error))
                {
                    amountErrors++;
                    
                }
                if (component.TestResultLog.Contains(TestResult.Warning))
                {
                    amountWarnings++;
                }
            }


            if (IsPageNotFound == true)
            {
                TestLog.Replace("RESULT", "not-found");
            }
            else if (amountErrors > 0)
            {
                TestLog.Replace("RESULT", "error");
            }
            else if(amountWarnings > 0 && amountErrors == 0)
            {
                TestLog.Replace("RESULT", "warning");
            }
            else if(amountErrors == 0 && amountWarnings == 0 && IsPageNotFound == false)
            {
                TestLog.Replace("RESULT", "success");
            }
        }

        public void InitializeTest()
        {
            TestLog.AppendLine($@"
                <li class='listlink RESULT' id='{Id}'>
                    {PageName}Page
                </li>
                <li class='test-content' id='test {Id}'>");
        }

        public void EndTest()
        {
            TestLog.AppendLine("</li>");
        }

        public void OnPageNotFound(object sender, EventArgs args)
        {
            IsPageNotFound = true;
        }
    }
}
