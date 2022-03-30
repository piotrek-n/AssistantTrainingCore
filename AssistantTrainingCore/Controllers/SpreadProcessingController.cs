using AssistantTrainingCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsm;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Csv;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Txt;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace AssistantTrainingCore.Controllers;

/// <summary>
///     https://demos.telerik.com/aspnet-core/spreadprocessing/generate-documents
/// </summary>
public class SpreadProcessingController : Controller
{
    private readonly IReportsRepository _reportsRepository;
    
    public SpreadProcessingController(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    [HttpPost]
    public ActionResult GenerateDocument(string fileFormat)
    {
        IWorkbookFormatProvider fileFormatProvider = null;
        var document = CreateWorkbook(fileFormat);
        var mimeType = string.Empty;
        var fileDownloadName = "{0}.{1}";


        switch (fileFormat)
        {
            case "xlsx":
                fileFormatProvider = new XlsxFormatProvider();
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                break;
            case "csv":
                fileFormatProvider = new CsvFormatProvider();
                mimeType = "text/csv";
                break;
            case "txt":
                fileFormatProvider = new TxtFormatProvider();
                mimeType = "text/plain";
                break;
            case "xlsm":
                fileFormatProvider = new XlsmFormatProvider();
                mimeType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                break;
            default:
                fileFormatProvider = new XlsxFormatProvider();
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                break;
        }

        fileDownloadName = string.Format(fileDownloadName, $"Raport {fileFormat}", "xlsx");

        byte[] renderedBytes = null;

        using (var ms = new MemoryStream())
        {
            fileFormatProvider.Export(document, ms);
            renderedBytes = ms.ToArray();
        }

        return File(renderedBytes, mimeType, fileDownloadName);
    }


    #region File Generation

    private static readonly int IndexColumnQuantity = 1;
    private static readonly int IndexColumnUnitPrice = 2;
    private static readonly int IndexColumnSubTotal = 3;
    private static readonly int IndexRowItemStart = 1;

    private Workbook CreateWorkbook(string fileFormat)
    {
        var currentRow = 0;
        var workbook = new Workbook();
        workbook.Sheets.Add(SheetType.Worksheet);
        var worksheet = workbook.ActiveWorksheet;
        
        switch (fileFormat)
        {
            case "SZKOLENIA":
                var incompleteTrainingResult = _reportsRepository.IncompleteTrainingResult();

                foreach (var product in incompleteTrainingResult)
                {
                    worksheet.Cells[currentRow, 0].SetValue(product.DT_RowId);
                    worksheet.Cells[currentRow, IndexColumnQuantity].SetValue(product.InstructionNumber);
                    worksheet.Cells[currentRow, IndexColumnUnitPrice].SetValue(product.TrainingNumber);

                    currentRow++;
                }

                worksheet.Columns[0].SetWidth(new ColumnWidth(300, true));
                worksheet.Columns[IndexColumnUnitPrice].SetWidth(new ColumnWidth(120, true));
                worksheet.Columns[IndexColumnSubTotal].ExpandToFitNumberValuesWidth();

                return workbook;
            case "INSTRUKCJE":

                var instructionsWithoutTrainingResult = _reportsRepository.InstructionsWithoutTrainingResult();


                foreach (var product in instructionsWithoutTrainingResult)
                {
                    worksheet.Cells[currentRow, 0].SetValue(product.DT_RowId);
                    worksheet.Cells[currentRow, IndexColumnQuantity].SetValue(product.Number);
                    worksheet.Cells[currentRow, IndexColumnUnitPrice].SetValue(product.Version);

                    currentRow++;
                }

                worksheet.Columns[0].SetWidth(new ColumnWidth(300, true));
                worksheet.Columns[IndexColumnUnitPrice].SetWidth(new ColumnWidth(120, true));
                worksheet.Columns[IndexColumnSubTotal].ExpandToFitNumberValuesWidth();

                return workbook;
            case "PRACOWNICY":
                var workersWithoutTrainingResult = _reportsRepository.WorkersWithoutTrainingResult();


                foreach (var product in workersWithoutTrainingResult)
                {
                    worksheet.Cells[currentRow, 0].SetValue(product.DT_RowId);
                    worksheet.Cells[currentRow, IndexColumnQuantity].SetValue(product.Name);
                    worksheet.Cells[currentRow, IndexColumnUnitPrice].SetValue(product.Number);
                    worksheet.Cells[currentRow, IndexColumnSubTotal].SetValue(product.Version);

                    currentRow++;
                }

                worksheet.Columns[0].SetWidth(new ColumnWidth(300, true));
                worksheet.Columns[IndexColumnUnitPrice].SetWidth(new ColumnWidth(120, true));
                worksheet.Columns[IndexColumnSubTotal].ExpandToFitNumberValuesWidth();
                return workbook;
            default:
                return workbook;
        }
    }

    #endregion
}