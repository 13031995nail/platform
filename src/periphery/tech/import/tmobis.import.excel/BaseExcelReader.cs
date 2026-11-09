namespace Tmobis.Import.Excel;

using System;
using System.IO;
using NPOI.SS.UserModel;

public abstract class BaseExcelReader<TResult> : BaseExcelProcessor
    where TResult : new()
{
    protected TResult? Result { get; set; }

    public virtual TResult Process(MemoryStream data)
    {
        Result = CreateResult();
        OpenWorkbook(data);
        OpenSheet(0);
        ProcessSheet(0);
        return Result;
    }

    protected virtual TResult CreateResult()
    {
        return new TResult();
    }

    protected virtual string GetCellValue(IRow row, int columnIndex)
    {
        var cell = row.GetCell(columnIndex);
        return cell?.ToString()?.Trim() ?? string.Empty;
    }

    protected virtual int GetNextRecord(int startAfterRow)
    {
        var filledRowNum = GetFilledRowNumber(Sheet); 
        for (int i = startAfterRow + 1; i <= filledRowNum; i++)
        {
            var row = Sheet?.GetRow(i);
            if (row?.RowNum > filledRowNum)
            {
                return -1;
            }

            if (row == null)
            {
                continue;
            }

            if (row.Cells.Any(cell => cell != null && !string.IsNullOrWhiteSpace(cell.ToString())))
            {
                return i;
            }
        }

        return -1;
    }

    protected virtual bool AreRequiredColumnsFilled(IRow row, params int[] requiredColumns)
    {
        foreach (int col in requiredColumns)
        {
            var cell = row.GetCell(col);
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
            {
              return false;
            }
        }

        return true;
    }

    protected int GetFilledRowNumber(ISheet? sheet)
    {
        if (sheet == null)
        {
          return -1;
        }

        int lastRow = -1;
        for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null)
            {
                continue;
            }

            bool hasData = row.Cells.Any(cell =>
                cell != null &&
                cell.CellType != CellType.Formula &&
                !string.IsNullOrWhiteSpace(cell.ToString())
            );

            if (hasData)
            {
                lastRow = i;
            }
        }

        return lastRow;
    }
}
