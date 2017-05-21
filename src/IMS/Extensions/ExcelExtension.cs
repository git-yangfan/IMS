using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Table;

public static class ExcelExtension
{
    private static void dataTableToCsv(DataTable table, string file)
    {
        string title = "";
        FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);

        for (int i = 0; i < table.Columns.Count; i++)
        {
            title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格
        }

        title = title.Substring(0, title.Length - 1) + "\n";
        sw.Write(title);

        foreach (DataRow row in table.Rows)
        {
            string line = "";

            for (int i = 0; i < table.Columns.Count; i++)
            {
                line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
            }
            line = line.Substring(0, line.Length - 1) + "\n";
            sw.Write(line);
        }
        sw.Close();
        fs.Close();
    }

    /// <summary>
    /// 将一组对象导出成EXCEL
    /// </summary>
    /// <typeparam name="T">要导出对象的类型</typeparam>
    /// <param name="objList">一组对象</param>
    /// <param name="FileName">导出后的文件名</param>
    /// <param name="columnInfo">列名信息</param>
    public static void ExExcel<T>(List<T> objList, string FileName, Dictionary<string, string> columnInfo)
    {
        ExExcel(objList, FileName, columnInfo, null);
    }

    /// <summary>
    /// 将一组对象导出成EXCEL
    /// </summary>
    /// <typeparam name="T">要导出对象的类型</typeparam>
    /// <param name="objList">一组对象</param>
    /// <param name="FileName">导出后的文件名</param>
    /// <param name="columnInfo">列名信息</param>
    /// <param name="other">追加其他内容</param>
    public static void ExExcel<T>(List<T> objList, string FileName, Dictionary<string, string> columnInfo, string other)
    {
        if (columnInfo.Count == 0) { return; }
        if (objList.Count == 0) { return; }
        //生成EXCEL的HTML
        string excelStr = "";

        Type myType = objList[0].GetType();
        //根据反射从传递进来的属性名信息得到要显示的属性
        List<PropertyInfo> myPro = new List<PropertyInfo>();
        foreach (string cName in columnInfo.Keys)
        {
            PropertyInfo p = myType.GetProperty(cName);
            if (p != null)
            {
                myPro.Add(p);
                excelStr += columnInfo[cName] + "\t";
            }
        }
        //如果没有找到可用的属性则结束
        if (myPro.Count == 0) { return; }
        excelStr += "\n";

        foreach (T obj in objList)
        {
            foreach (PropertyInfo p in myPro)
            {
                excelStr += p.GetValue(obj, null) + "\t";
            }
            excelStr += "\n";
        }
        if (!string.IsNullOrEmpty(other))
        {
            excelStr += other;
        }
        //输出EXCEL
        HttpResponse rs = System.Web.HttpContext.Current.Response;
        rs.Clear();
        rs.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        rs.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(FileName, Encoding.UTF8));
        rs.ContentType = "application/ms-excel";
        rs.Write(excelStr);
        rs.End();
    }

    #region 保存数据列表到Excel（泛型）+void SaveToExcel<T>(IEnumerable<T> data, string FileName, string OpenPassword = "")
    /// <summary>
    /// 保存数据列表到Excel（泛型）
    /// </summary>
    /// <typeparam name="T">集合数据类型</typeparam>
    /// <param name="data">数据列表</param>
    /// <param name="FileName">Excel文件</param>
    /// <param name="OpenPassword">Excel打开密码</param>
    public static void SaveToExcel<T>(IEnumerable<T> data, string FileName, string OpenPassword = "")
    {
        FileInfo file = new FileInfo(FileName);
        try
        {
            using (ExcelPackage ep = new ExcelPackage(file, OpenPassword))
            {
                ExcelWorksheet ws = ep.Workbook.Worksheets.Add(typeof(T).Name);
                ws.Cells["A1"].LoadFromCollection(data, true, TableStyles.Medium10);

                ep.Save(OpenPassword);
            }
        }
        catch (InvalidOperationException ex)
        {
            //Console.WriteLine(ex.Message);
            throw ex;
        }
    }
    #endregion

    #region 从Excel中加载数据（泛型）+IEnumerable<T> LoadFromExcel<T>(string FileName) where T : new()
    /// <summary>
    /// 从Excel中加载数据（泛型）
    /// </summary>
    /// <typeparam name="T">每行数据的类型</typeparam>
    /// <param name="FileName">Excel文件名</param>
    /// <returns>泛型列表</returns>
    public static IEnumerable<T> LoadFromExcel<T>(string FileName) where T : new()
    {
        FileInfo existingFile = new FileInfo(FileName);
        List<T> resultList = new List<T>();
        Dictionary<string, int> dictHeader = new Dictionary<string, int>();

        using (ExcelPackage package = new ExcelPackage(existingFile))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            int colStart = worksheet.Dimension.Start.Column;  //工作区开始列
            int colEnd = worksheet.Dimension.End.Column;       //工作区结束列
            int rowStart = worksheet.Dimension.Start.Row;       //工作区开始行号
            int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号
            //将每列标题添加到字典中
            for (int i = colStart; i <= colEnd; i++)
            {
                dictHeader[worksheet.Cells[rowStart, i].Value.ToString()] = i;
            }
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(typeof(T).GetProperties());
            for (int row = rowStart + 1; row <= rowEnd; row++)
            {
                T result = new T();
                //为对象T的各属性赋值
                foreach (PropertyInfo p in propertyInfoList)
                {
                    if (dictHeader.ContainsKey(p.Name))
                    {
                        try
                        {
                            ExcelRange cell = worksheet.Cells[row, dictHeader[p.Name]]; //与属性名对应的单元格
                            if (cell.Value == null)
                                continue;
                            Type type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                            p.SetValue(result, Convert.ChangeType(cell.Value, type), null);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw ex;
                        }
                    }
                }
                resultList.Add(result);
            }
        }
        return resultList;
    }


    /// <summary>
    /// 从Excel中加载数据（泛型）
    /// </summary>
    /// <typeparam name="T">每行数据的类型</typeparam>
    /// <param name="FileName">Excel文件名</param>
    /// <returns>泛型列表</returns>
    public static IEnumerable<T> LoadFromExcel<T>(FileStream File) where T : new()
    {
        //FileInfo existingFile = new FileInfo(FileName);
        List<T> resultList = new List<T>();
        Dictionary<string, int> dictHeader = new Dictionary<string, int>();

        using (ExcelPackage package = new ExcelPackage(File))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            int colStart = worksheet.Dimension.Start.Column;  //工作区开始列
            int colEnd = worksheet.Dimension.End.Column;       //工作区结束列
            int rowStart = worksheet.Dimension.Start.Row;       //工作区开始行号
            int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号
            //将每列标题添加到字典中
            for (int i = colStart; i <= colEnd; i++)
            {
                dictHeader[worksheet.Cells[rowStart, i].Value.ToString()] = i;
            }
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(typeof(T).GetProperties());
            for (int row = rowStart + 1; row <= rowEnd; row++)
            {
                T result = new T();
                //为对象T的各属性赋值
                foreach (PropertyInfo p in propertyInfoList)
                {
                    if (dictHeader.ContainsKey(p.Name))
                    {
                        try
                        {
                            ExcelRange cell = worksheet.Cells[row, dictHeader[p.Name]]; //与属性名对应的单元格
                            if (cell.Value == null)
                                continue;
                            Type type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                            p.SetValue(result, Convert.ChangeType(cell.Value, type), null);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw ex;
                        }
                    }
                }
                resultList.Add(result);
            }
        }
        return resultList;
    }
   
    
    
    
    #endregion

    /// <summary>
    /// 创造参数对
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comnName"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static KeyValuePair<string, Func<T, string>> CreateKVP<T>(string comnName, Func<T, string> func)
    {
        return new KeyValuePair<string, Func<T, string>>(comnName, func);
    }

    /// <summary>
    /// 向表中添加列
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="columnName"></param>
    public static void AddSheetHeadRange(this ExcelWorksheet sheet, params string[] columnNames)
    {
        for (int i = 0; i < columnNames.Length; i++)
            sheet.Cells[1, i + 1].Value = columnNames[i];
    }

    /// <summary>
    /// 向表中添加行数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sheet"></param>
    /// <param name="listSources"></param>
    /// <param name="values"></param>
    public static void AddSheetRow<T>(this ExcelWorksheet sheet, IList<T> listSources, params KeyValuePair<string, Func<T, string>>[] values)
    {
        if (values != null && values.Length > 0)
        {
            sheet.AddSheetHeadRange(values.Select(item => item.Key).ToArray());
            if (listSources != null && listSources.Count > 0)
            {
                IList<Func<T, string>> listVs = values.Select(item => item.Value).ToList();
                for (int i = 0; i < listSources.Count; i++)
                {
                    for (int j = 0; j < listVs.Count; j++)
                    {
                        sheet.Cells[(i + 2), (j + 1)].Value = listVs[j](listSources[i]);
                    }
                }
            }
        }
    }
}