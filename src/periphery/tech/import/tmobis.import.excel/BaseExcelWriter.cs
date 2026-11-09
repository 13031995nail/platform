namespace Tmobis.Import.Excel;

using System;
using System.IO;
using NPOI.SS.UserModel;

public abstract class BaseExcelWriter : BaseExcelProcessor
{
    public virtual byte[] Process(MemoryStream data)
    {
        OpenWorkbook(data);
        OpenSheet(0);
        ProcessSheet(0);
        return GetProcessedFile();
    }

    public byte[] GetProcessedFile()
    {
        using var outputStream = new MemoryStream();
        Workbook?.Write(outputStream);
        return outputStream.ToArray();
    }

    protected void SetCellValue<T>(IRow row, int columnIndex, T value)
      where T : IConvertible
    {
        var cell = row.GetCell(columnIndex) ?? row.CreateCell(columnIndex);
        if (value != null)
        {
            cell.SetCellValue(value.ToString());
        }
        else
        {
            cell.SetCellValue(string.Empty);
        }
    }
}
