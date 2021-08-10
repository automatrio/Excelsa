using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excelsa.Services
{
    class ImportExcelService
    {
        private readonly string _pageTemplatePath;
        private readonly string _pageDestinationPath;
        private readonly List<string> _availableComponents;

        public ImportExcelService(string pageTemplatePath, string pageDestinationPath, List<string> availableComponents)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            _pageTemplatePath = pageTemplatePath;
            _pageDestinationPath = pageDestinationPath;
            _availableComponents = availableComponents;
        }

        public IEnumerable<Dictionary<string, object>> ImportExcelToDictionary(string filePath)
        {
            var model = new List<Dictionary<string, object>>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[1];
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                for (var row = 2; row <= rowCount; row++)
                {
                    var item = new Dictionary<string, object>();

                    for (var col = 2; col <= colCount; col++)
                    {
                        if (worksheet.Cells[1, col].Value != null)
                        {
                            var fieldName = worksheet.Cells[1, col].Value.ToString();
                            var fieldValue = worksheet.Cells[row, col].Value ?? "";
                            item.Add(fieldName, fieldValue);
                        }
                    }
                    model.Add(item);
                }
                return model;
            }
        }

        public void MapDictionaryToPage(IEnumerable<Dictionary<string, object>> mappedTable)
        {
            // To be expanded
        }
    }
}
