namespace Tmobis.Import.Excel;

using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public abstract class BaseExcelProcessor : IDisposable
{
    private bool disposed;
    protected IWorkbook? Workbook { get; set; }
    protected ISheet? Sheet { get; set; }
    protected IRow? Row { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void OpenWorkbook(Stream data)
    {
        Workbook = new XSSFWorkbook(data);
    }

    protected virtual void OpenSheet(int sheetIndex)
    {
        Sheet = Workbook?.GetSheetAt(sheetIndex) ?? throw new NullReferenceException(nameof(Workbook));
    }

    protected virtual void ProcessSheet(int startRow)
    {
        var sheetRowNum = Sheet?.LastRowNum ?? throw new NullReferenceException(nameof(Sheet));
        for (int rowIndex = startRow; rowIndex <= sheetRowNum; rowIndex++)
        {
            Row = Sheet.GetRow(rowIndex) ?? Sheet.CreateRow(rowIndex);
            if (Row == null)
            {
                continue;
            }

            if (!NeedToContinueProcessing(Row))
            {
                break;
            }

            ProcessRow(Row);
        }
    }

    protected virtual bool NeedToContinueProcessing(IRow row)
    {
        return Row?.Cells.Count > 0;
    }

    protected abstract void ProcessRow(IRow row);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
              Workbook?.Close();
            }

            disposed = true;
        }
    }
}
