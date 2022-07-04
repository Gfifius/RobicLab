using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace RL
{
    class ExcelGenerator
    {

        public byte[] Generate(ReportForm report, int columnX, int columnY)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            var sheet = package.Workbook.Worksheets // создание страницы
                .Add("Эксперимент");

            sheet.Cells["B2"].Value = report.Head.NameExp; // 
            int column = report.Column;
            int row = report.Row;
            //columnX = 2;
            //columnY = 6;
            sheet.Cells[3, 2, 3, 2 + column].LoadFromArrays(new object[][] {report.Head.Params });

            for( int i = 0; i < row; i++)
            {
                for(int j = 0; j < column; j++)
                {
                    sheet.Cells[4+i, 2 +j].Value = report.Experiment.ParamsD[i,j];
                }
            }
            sheet.Cells[3, 2, 3 + row, 2 + column].AutoFitColumns(); // выравнивание в рамках таблицы
            sheet.Cells[3, 2, 3, 2 + column].Style.Font.Bold = true;  // жирный шрифт

            ///*  Границы таблицы  */
            sheet.Cells[3, 2, 3 + row, 2 + column-1].Style.Border.BorderAround(ExcelBorderStyle.Double); // двойная линия вокруг всей таблицы
            sheet.Cells[3, 2, 3, 2 + column-1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;// одинарная вокруг шапки 

            ///*  График  */
            var capitalizationChart = sheet.Drawings.AddChart("FindingsChart", OfficeOpenXml.Drawing.Chart.eChartType.Line); // создаем график
            capitalizationChart.Title.Text = "Результаты эксперимента"; // имя графика
            capitalizationChart.SetPosition(2, 0, column + 2, 0);  // левый верхний угол. строка, оффсет, столбец, оффсет
            capitalizationChart.SetSize(800, 400);
            var capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(sheet.Cells[4, columnY, 4+row-1, columnY], sheet.Cells[4, columnX, 4 + row - 1, columnX])); // данные
           // capitalizationData.Header = report.Company.Currency; // подпись графика


            //sheet.Cells[2, 3].Value = report.Company.Name;  // ячейка С2
            //sheet.Cells["B3"].Value = "Location:";
            //sheet.Cells["C3"].Value = $"{report.Company.Address}, " +
            //                          $"{report.Company.City}, " +
            //                          $"{report.Company.Country}";
            //sheet.Cells["B4"].Value = "Sector:";
            //sheet.Cells["C4"].Value = report.Company.Sector;
            //sheet.Cells["B5"].Value = report.Company.Description;

            //sheet.Cells[8, 2, 8, 4].LoadFromArrays(new object[][] { new[] { "Capitalization", "SharePrice", "Date" } });
            ////var row = 9;
            ////var column = 2;
            //foreach (var item in report.History)
            //{
            //    sheet.Cells[row, column].Value = item.Capitalization;
            //    sheet.Cells[row, column + 1].Value = item.SharePrice;
            //    sheet.Cells[row, column + 2].Value = item.Date;
            //    row++;
            //}

            //sheet.Cells[1, 1, row, column + 2].AutoFitColumns(); // выравнивание в рамках таблицы
            //sheet.Column(2).Width = 14;  // ширина столбцов
            //sheet.Column(3).Width = 12;

            //sheet.Cells[9, 4, 9 + report.History.Length, 4].Style.Numberformat.Format = "yyyy";  // формат данных
            //sheet.Cells[9, 2, 9 + report.History.Length, 2].Style.Numberformat.Format = "### ### ### ##0"; // формат данных, разделяет разряды пробелами
            ///* Выравнивание */
            //sheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //sheet.Cells[8, 3, 8 + report.History.Length, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //sheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ///*  Жирный шрифт  */
            //sheet.Cells[8, 2, 8, 4].Style.Font.Bold = true;
            //sheet.Cells["B2:C4"].Style.Font.Bold = true;
            ///*  Границы таблицы  */
            //sheet.Cells[8, 2, 8 + report.History.Length, 4].Style.Border.BorderAround(ExcelBorderStyle.Double); // двойная линия вокруг всей таблицы
            //sheet.Cells[8, 2, 8, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;// одинарная вокруг шапки 
            ///*  График  */
            //var capitalizationChart = sheet.Drawings.AddChart("FindingsChart", OfficeOpenXml.Drawing.Chart.eChartType.Line); // создаем график
            //capitalizationChart.Title.Text = "Capitalization"; // имя графика
            //capitalizationChart.SetPosition(7, 0, 5, 0);  // левый верхний угол. строка, оффсет, столбец, оффсет
            //capitalizationChart.SetSize(800, 400);
            //var capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(sheet.Cells["B9:B28"], sheet.Cells["D9:D28"])); // данные
            //capitalizationData.Header = report.Company.Currency; // подпись графика

            //sheet.Protection.IsProtected = true; // защита от редактирования
            return package.GetAsByteArray();
        }


    }
}
