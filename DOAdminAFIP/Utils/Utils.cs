using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.HSSF.UserModel;
using DOAdminAFIP.Extensions;

namespace DOAdminAFIP
{
    public class Utils
    {
        #region Excel

        public static HSSFWorkbook GenerateWorkbook(IEnumerable<IEnumerable<string>> matrix)
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet();
            int contRow = 0;
            int contCell = 0;

            foreach (var line in matrix)
            {
                var row = sheet.CreateRow(contRow);
                contRow++;
                contCell = 0;

                foreach (var cell in line)
                {
                    var auxcell = row.CreateCell(contCell);
                    auxcell.SetCellValue(cell);
                    contCell++;
                }
            }

            Enumerable.Range(0, sheet.GetRow(0).PhysicalNumberOfCells)
                      .ToList()
                      .ForEach(i => { sheet.AutoSizeColumn(i); GC.Collect(); });

            return workbook;
        }

        #endregion

        #region Split

        public static (string taken, string left) Split(string s, int index)
        {
            var arr = s.ToCharArray();
            return (arr.Take(index).Concat(), arr.Skip(index).Concat());
        }        

        #endregion
    }
}
