using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Excelsa.Services
{
    class CodeGeneratorService
    {
        public async Task<bool> GenerateComponent(
            string templatePath,
            string destinationPath,
            List<object> values
            )
        {
            var componentConfig = new ComponentCodeConfiguration();

            return await Generate(
                values[0] + "Component",
                templatePath,
                destinationPath,
                values,
                componentConfig);
        }

        public async Task<bool> GeneratePage(
            string templatePath,
            string internalTemplatePath,
            string destinationPath,
            List<object> values
            )
        {
            var pageConfig = new PageCodeConfiguration();

            Func<string, Task<StringBuilder>> replaceInternalTemplate = async value => {
                var template = new StringBuilder();
                using (var reader = new StreamReader(internalTemplatePath))
                {
                    template.Append(
                        await reader.ReadToEndAsync()
                        );
                }
                template.Replace("#COMPONENT_NAME#", value);

                return template;
            };

            return await Generate(
                values[0] + "Page",
                templatePath,
                destinationPath,
                values,
                pageConfig,
                replaceInternalTemplate);
        }

        public async Task<bool> GenerateTest(
            string templatePath,
            string internalTemplatePath,
            string destinationPath,
            List<object> values
            )
        {
            var testConfig = new TestCodeConfiguration();

            Func<string, Task<StringBuilder>> replaceInternalTemplate = async value => {
                var template = new StringBuilder();
                using (var reader = new StreamReader(internalTemplatePath))
                {
                    template.Append(
                        await reader.ReadToEndAsync()
                        );
                }
                template.Replace("#PAGE_NAME#", value);

                return template;
            };

            return await Generate(
                values[0] + "Test",
                templatePath,
                destinationPath,
                values,
                testConfig,
                replaceInternalTemplate);
        }

        private static async Task<bool> Generate(
            string filename,
            string templatePath,
            [Optional] string destinationPath,
            List<object> values,
            ICodeConfiguration config,
            [Optional] Func<string, Task<StringBuilder>> AddInternalTemplate
            )
        {
            var template = new StringBuilder();

            try
            {
                string filePath = Path.Combine(destinationPath, filename + ".cs");

                using (var writer = new StreamWriter(filePath))
                {
                    using (var reader = new StreamReader(templatePath))
                    {
                        template.Append(
                            await reader.ReadToEndAsync()
                            );
                    }

                    for (int i = 0; i < config.CodeConfigurations.Count; i++)
                    {
                        var type = config.CodeConfigurations[i].ReplaceValueType;

                        if (type == typeof(string) || type == typeof(int))
                        {
                            template.Replace(
                                config.CodeConfigurations[i].Placeholder,
                                values[i].ToString()
                                );
                        }
                        else if (type == typeof(List<string>))
                        {
                            var list = values[i] as IEnumerable<string>;
                            var compoundValue = new StringBuilder();

                            foreach(var value in list)
                            {
                                if(AddInternalTemplate != null)
                                {
                                    compoundValue.Append(await AddInternalTemplate(value));
                                }
                            }

                            template.Replace(
                                config.CodeConfigurations[i].Placeholder,
                                compoundValue.ToString()
                                );
                        }
                        

                    }

                    await writer.WriteAsync(template.ToString());
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public struct Configuration
    {
        public Configuration(
            string placeholder,
            Type replaceValueType
            )
        {
            Placeholder = placeholder;
            ReplaceValueType = replaceValueType;
        }

        public string Placeholder { get; }
        public Type ReplaceValueType { get; set;  }
    }

    internal interface ICodeConfiguration
    {
        List<Configuration> CodeConfigurations { get; }
    }

    internal class ComponentCodeConfiguration : ICodeConfiguration
    {
        public List<Configuration> CodeConfigurations { get; } = new List<Configuration>()
        {
            new Configuration("#COMPONENT_NAME#", typeof(string))
        };
    }

    internal class PageCodeConfiguration : ICodeConfiguration
    {
        public List<Configuration> CodeConfigurations { get; } = new List<Configuration>()
        {
            new Configuration("#PAGE_NAME#", typeof(string)),
            new Configuration("#PAGE_ID#", typeof(int)),
            new Configuration("#LIST_OF_COMPONENTS#", typeof(List<string>))
        };
    }

    internal class TestCodeConfiguration : ICodeConfiguration
    {
        public List<Configuration> CodeConfigurations { get; } = new List<Configuration>()
        {
            new Configuration("#TEST_NAME#", typeof(string)),
            new Configuration("#LIST_OF_PAGES#", typeof(List<string>))
        };
    }
}
