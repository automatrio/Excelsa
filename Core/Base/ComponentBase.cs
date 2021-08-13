using Excelsa.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Excelsa.Core.Base
{
    public abstract class ComponentBase : ExcelsaDSL
    {
        protected bool testFail = true;
        protected StringBuilder internalLog = new StringBuilder();
        protected readonly SpecificationBase specification;
        private readonly string testeName;
        private readonly string errorMessage;


        public List<TestResult> TestResultLog { get; set; } = new List<TestResult>();
        protected virtual bool DynamicElement { get; set; } = false;
        protected virtual string ScrollIntoViewScript { get; set; } = "return;";
        protected virtual string ScrollIntoViewFallbackScript { get; set; } = "return;";

        public virtual event EventHandler PageNotFound;

        public ComponentBase
            (
            string testeName,
            string errorMessage,          
            SpecificationBase specification
            )
        {
            this.testeName = testeName;
            this.errorMessage = errorMessage;
            this.specification = specification;
            GeneralExtensions.OnLogAppended += RegisterLogWarningOrError;
        }

        public void SetLog()
        {
            TestResultLog.Clear();
            internalLog.Append($@"
                    <div class='expandable COMPONENT_RESULT'>
                        <span>
                            {this.testeName}
                        </span>
                    </div>");

        }

        public void Execute()
        {
            internalLog.Append("<div class='expandable-content'>");

            try
            {
                NavigateToComponent();
                MainFlow();
            }
            catch(Exception ex)
            {
                internalLog.AppendError($"Selenium exception: {ex.Message}").Break();
                internalLog.AppendError($"{this.errorMessage}");
            }

            internalLog.Append("</div>");
        }

        public abstract void MainFlow();

        public void End()
        {
            internalLog.Replace("COMPONENT_RESULT", GetPageStyle());
        }

        public string GetLog()
        {
            return internalLog.ToString();
        }

        private string GetPageStyle()
        {
            int warningCount = 0;
            int errorCount = 0;

            foreach(var result in TestResultLog)
            {
                switch(result)
                {
                    case TestResult.Warning:
                        warningCount++;
                        break;
                    case TestResult.Error:
                        errorCount++;
                        break;
                    default:
                        break;
                }
            }

            if(errorCount > 0)
            {
                return "error";
            }
            if (warningCount > 0 && errorCount == 0)
            {
                return "warning";
            }
            else
            {
                return "success";
            }
        }

        public void RegisterLogWarningOrError(object sender, LogEventArgs args)
        {
            switch (args.Type)
            {
                case LogEntryType.Error:
                    this.TestResultLog.Add(TestResult.Error);
                    return;
                case LogEntryType.Warning:
                    this.TestResultLog.Add(TestResult.Warning);
                    return;
                default:
                    return;
            }
        }
        public void NavigateToComponent()
        {
            if (DynamicElement == true)
            {
                DynamicElement = false;
                return;
            }

            try
            {
                WebDriver.Driver.Scripts().ExecuteScript(ScrollIntoViewScript);
            }
            catch
            {
                try
                {
                    WebDriver.Driver.Scripts().ExecuteScript(ScrollIntoViewFallbackScript);
                }
                catch (Exception ex)
                {
                    internalLog.AppendError(ex.Message).Break();
                    internalLog.AppendError("The component could not be found on this page.");
                    throw;
                }
            }
        }

        protected void OnPageNotFound()
        {
            this.PageNotFound?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum TestResult
    {
        Success,
        Warning,
        Error
    }
}
 
