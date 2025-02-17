using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Jarvis.Shared.Utility {
    public static class ExcelCreation {
        public static Stream WriteExcelFile(Object objectlist)
        {
            List<string> ColumnsToIgnore = new List<string>();
            ColumnsToIgnore.Add("pm_trigger_id");
            ColumnsToIgnore.Add("service_dealer_id");
            ColumnsToIgnore.Add("trigger_id");
            ColumnsToIgnore.Add("notification_type_id");
            // Lets converts our object data to Datatable for a simplified logic.
            // Datatable is most easy way to deal with complex datatypes for easy reading and formatting. 
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(objectlist), (typeof(DataTable)));
            MemoryStream memoryStream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                Row headerRow = new Row();

                List<String> columns = new List<string>();
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    if (ColumnsToIgnore.Contains(column.ColumnName))
                    {
                        continue;
                    }
                    columns.Add(column.ColumnName);

                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    var updatedColumnaName = ChangeColumnName(column.ColumnName);
                    cell.CellValue = new CellValue(updatedColumnaName);
                    headerRow.AppendChild(cell);
                }
                headerRow.CustomFormat = true;
                headerRow.CustomHeight = true;
                headerRow.Height = 20;
                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        var cellValue = dsrow[col].ToString();
                        cellValue = cellValue == "False" ? "No" : cellValue == "True" ? "Yes" : cellValue;
                        cell.CellValue = new CellValue(cellValue);
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookPart.Workbook.Save();

                // close the document and flush to stream
                document.Close();

                // rewind the memory stream
                memoryStream.Seek(0, SeekOrigin.Begin);

                // return the file stream
                return memoryStream;
                //return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            }
        }

        public static string ChangeColumnName(string s)
        {
            var updatedColumnName = String.Join(" ", s.Replace("_", " ").Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList().Select(c => c.Substring(0, 1).ToUpper() + c.Substring(1).ToLower()));
            return updatedColumnName.Replace("Pm ", "PM ");
        }
    }
}
