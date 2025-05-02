using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using MyOffice;
using NPOI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
//using MyUI;
//using H_Pannel_lib;
using HIS_DB_Lib;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DB2VM_API.Controller._API_EXCEL下載
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class transactions : ControllerBase
    {
        private static string API_Server = "http://127.0.0.1:4433";
        [HttpPost]
        public string get_datas_sheet([FromBody] returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "get_datas_sheet";
            try
            {
                List<sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet(API_Server);

                if (returnData.ValueAry == null)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 無傳入資料";
                    return returnData.JsonSerializationt(true);
                }
                if (returnData.ValueAry.Count != 5)
                {
                    returnData.Code = -200;
                    returnData.Result = $"returnData.ValueAry 內容應為[藥碼][起始時間][結束時間][ServerName1,ServerName2][ServerType1,ServerType2]";
                    return returnData.JsonSerializationt(true);
                }
                string[] 藥碼Ary = returnData.ValueAry[0].Split(",");
                string 起始時間 = returnData.ValueAry[1];
                string 結束時間 = returnData.ValueAry[2];
                string serverName = returnData.ValueAry[3];
                string serverType = returnData.ValueAry[4];

                string[] ServerNames = serverName.Split(',');
                string[] ServerTypes = serverType.Split(',');
                if (藥碼Ary.Length == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"[藥碼] 欄位異常 ,請用','分隔需搜尋藥碼";
                    return returnData.JsonSerializationt(true);

                }
                if (起始時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"[起始時間] 為非法格式";
                    return returnData.JsonSerializationt(true);
                }
                if (結束時間.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"[結束時間] 為非法格式";
                    return returnData.JsonSerializationt(true);
                }
                if (ServerNames.Length != ServerTypes.Length)
                {
                    returnData.Code = -200;
                    returnData.Result = $"ServerNames及ServerTypes長度不同";
                    return returnData.JsonSerializationt(true);
                }
                DateTime dateTime_st = 起始時間.StringToDateTime();
                DateTime dateTime_end = 結束時間.StringToDateTime();
                //sys_serverSettingClass sys_serverSettingClasses_med = sys_serverSettingClasses.MyFind("Main", "網頁", "藥檔資料")[0];

                
                List<medClass> medClasses = medClass.get_med_cloud(API_Server);

                List<SheetClass> sheetClasses = new List<SheetClass>();
                List<List<transactionsClass>> list_transactionsClasses = new List<List<transactionsClass>>();
                藥碼Ary = 藥碼Ary.Distinct().ToArray();

                for (int k = 0; k < 藥碼Ary.Length; k++)
                {
                    string 藥碼 = 藥碼Ary[k];
                    //returnData returnData_get_datas_by_code = new returnData();
                    //returnData_get_datas_by_code.ValueAry.Add(藥碼Ary[k]);
                    //returnData_get_datas_by_code.ValueAry.Add(serverName);
                    //returnData_get_datas_by_code.ValueAry.Add(serverType);
                    //string json_get_datas_by_code = get_datas_by_code(returnData_get_datas_by_code);
                    //returnData_get_datas_by_code = json_get_datas_by_code.JsonDeserializet<returnData>();
                    //if (returnData_get_datas_by_code.Code != 200)
                    //{
                    //    return returnData_get_datas_by_code.JsonSerializationt(true);
                    //}

                    List<transactionsClass> transactionsClasses = transactionsClass.get_datas_by_code(API_Server, 藥碼, serverName, serverType);

                    transactionsClasses = (from temp in transactionsClasses
                                           where temp.操作時間.StringToDateTime() >= dateTime_st
                                           where temp.操作時間.StringToDateTime() <= dateTime_end
                                           select temp).ToList();

                    List<medClass> medClasses_buf = new List<medClass>();

                    medClasses_buf = (from value in medClasses
                                      where value.藥品碼.ToUpper() == 藥碼.ToUpper()
                                      select value).ToList();
                    if (medClasses_buf.Count == 0)
                    {
                        if (returnData.Value.StringIsEmpty())
                        {
                            return null;
                        }
                    }

                    //string loadText = Basic.MyFileStream.LoadFileAllText(@"./excel_emg_tradding.txt", "utf-8");
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "excel_emg_tradding.xlsx");
                    //string loadText = LoadFileAllText(path, "utf-8");
                    //string loadText = MyOffice.ExcelClass.NPOI_LoadSheetsToJson(path);
                    string loadText = MyOffice.ExcelClass.NPOI_LoadSheetsToJson_new(path);



                    Console.WriteLine($"取得creats {myTimerBasic.ToString()}");
                    int row_max = 60000;
                    List<SheetClass> sheetClasslist = loadText.JsonDeserializet<List<SheetClass>>();
                    SheetClass sheetClass = sheetClasslist[0];
                    returnData.Data = sheetClass;
                    Logger.Log("sheetClass", $"{returnData.JsonSerializationt(true)}");
                    //SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();

                    int 消耗量 = 0;
                    int NumOfRow = -1;
                    for (int i = 0; i < transactionsClasses.Count; i++)
                    {
                        if (NumOfRow >= row_max || NumOfRow == -1)
                        {
                            //sheetClass = loadText.JsonDeserializet<SheetClass>();
                            sheetClass.Name = $"{藥碼}";
                            //sheetClass.ReplaceCell(1, 1, $"{medClasses_buf[0].藥品名稱}");
                            //sheetClass.ReplaceCell(1, 3, $"{medClasses_buf[0].藥品學名}");
                            //sheetClass.ReplaceCell(1, 5, $"{medClasses_buf[0].藥品許可證號}");
                            //sheetClass.ReplaceCell(2, 1, $"{medClasses_buf[0].管制級別}");
                            //sheetClass.ReplaceCell(2, 3, $"{medClasses_buf[0].廠牌}");
                            //sheetClass.ReplaceCell(2, 5, $"{medClasses_buf[0].包裝單位}");
                            sheetClass.Rows[1].Cell[1].Text = $"{medClasses_buf[0].藥品名稱}";
                            sheetClass.Rows[1].Cell[5].Text = $"{medClasses_buf[0].藥品學名}";
                            sheetClass.Rows[1].Cell[11].Text = $"{medClasses_buf[0].藥品許可證號}";
                            sheetClass.Rows[2].Cell[1].Text = $"{medClasses_buf[0].管制級別}";
                            sheetClass.Rows[2].Cell[3].Text = $"{medClasses_buf[0].廠牌}";
                            sheetClass.Rows[2].Cell[11].Text = $"{medClasses_buf[0].包裝單位}";

                           
                            foreach (var item in sheetClass.CellValues)
                            {
                                if (item.Text == "藥名") item.Text = $"{medClasses_buf[0].藥品名稱}";
                                if (item.Text == "藥品學名") item.Text = $"{medClasses_buf[0].藥品學名}";
                                if (item.Text == "X") item.Text = $"{medClasses_buf[0].藥品許可證號}";
                                if (item.Text == "1") item.Text = $"{medClasses_buf[0].管制級別}";
                                if (item.Text == "廠商") item.Text = $"{medClasses_buf[0].廠牌}";
                                if (item.Text == "ml") item.Text = $"{medClasses_buf[0].包裝單位}";

                            }
                            //sheetClass.ReplaceCell(1, 10, $"{起始時間}");

                            //sheetClass.ReplaceCell(2, 10, $"{結束時間}");

                            sheetClasses.Add(sheetClass);
                            NumOfRow = 0;
                        }

                        //消耗量 += transactionsClasses[i].交易量.StringToInt32();
                        //sheetClass.AddNewCell_Webapi(NumOfRow + 4, 0, $"{i + 1}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        (string 效期, string 批號) = spliteNote(transactionsClasses[i].備註);
                        string 交易量 = transactionsClasses[i].交易量;
                        string 支出數 = "";
                        string 收入數 = "";
                        if (交易量.Contains("-"))
                        {
                            支出數 = 交易量.Replace("-","");
                        }
                        else
                        {
                            收入數 = 交易量;
                        }
                        //sheetClass.AddNewCell_Webapi(1, 2, $"", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Right, NPOI.SS.UserModel.VerticalAlignment.Top, NPOI.SS.UserModel.BorderStyle.Thin);

                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 0, $"{transactionsClasses[i].操作時間}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 1, $"{transactionsClasses[i].收支原因}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 2, $"{收入數}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 3, $"{批號}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 4, $"{支出數}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 5, $"{transactionsClasses[i].結存量}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 6, $"{transactionsClasses[i].病歷號}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 7, $"{transactionsClasses[i].病人姓名}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 8, $"{transactionsClasses[i].開方時間}", "微軟正黑體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 9, $"{transactionsClasses[i].領藥號}", "微軟正黑體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 10, $"{transactionsClasses[i].操作人}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 11, $"{transactionsClasses[i].藥師證字號}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Center, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(NumOfRow + 4, 12, $"{transactionsClasses[i].備註}", "標楷體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);

                        //sheetClass.AddNewCell_Webapi(NumOfRow + 5, 2, $"{transactionsClasses[i].床號}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        //sheetClass.AddNewCell_Webapi(NumOfRow + 5, 3, $"{transactionsClasses[i].類別}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        NumOfRow++;
                    }
                    //for (int i = 0; i < sheetClasses.Count; i++)
                    //{
                    //    sheetClasses[i].ReplaceCell(1, 5, $"{消耗量}");
                    //}

                }

                returnData.Code = 200;
                returnData.Result = "Sheet取得成功!";
                returnData.Data = sheetClasses;
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = e.Message;
                return returnData.JsonSerializationt();
            }

        }
        /// <summary>
        /// 取得收支結存報表(Excel)(多台合併)
        /// </summary>
        /// <remarks>
        ///  --------------------------------------------<br/> 
        /// 以下為範例JSON範例
        /// <code>
        ///   {
        ///     "Data": 
        ///     {
        ///        
        ///     },
        ///     "ValueAry" : 
        ///     [
        ///       "藥碼1,藥碼2,藥碼",
        ///       "起始時間",
        ///       "結束時間",
        ///       "口服1,口服2",
        ///       "調劑台,調劑台"
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="returnData">共用傳遞資料結構</param>
        /// <returns>[returnData.Data]為交易紀錄結構</returns>
        [Route("download_cdmis_datas_excel")]
        [HttpPost]
        public async Task<ActionResult> download_cdmis_datas_excel([FromBody] returnData returnData)
        {
            try
            {
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                myTimerBasic.StartTickTime(50000);

                returnData = get_datas_sheet(returnData).JsonDeserializet<returnData>();
                if (returnData.Code != 200)
                {
                    return null;
                }
                string jsondata = returnData.Data.JsonSerializationt();

                List<SheetClass> sheetClasses = jsondata.JsonDeserializet<List<SheetClass>>();

                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";

                byte[] excelData = sheetClasses.NPOI_GetBytes(Excel_Type.xlsx);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xlsx_command, $"{DateTime.Now.ToDateString("-")}_收支結存簿冊.xlsx"));
            }
            catch
            {
                return null;
            }

        }
        private(string 效期, string 批號) spliteNote(string note)
        {
            var match = Regex.Match(note, @"\[效期\]:(?<exp>[^,\[\]]+),\[批號\]:(?<lot>[^,\[\]]+)");
            string 效期 = "";
            string 批號 = "";
            if (match.Success)
            {
                效期 = match.Groups["exp"].Value;
                批號 = match.Groups["lot"].Value;
            }
            效期 = $"[效期]:{效期}";
            return (效期, 批號);
        }
        //private string LoadFileAllText(string filename, string endcoding)
        //{
        //    string text = "";
        //    if (!IsFileNameExists(filename)) return "";
        //    try
        //    {
        //        StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding(endcoding), false);
        //        while (reader.Peek() > 0)
        //        {
        //            text = reader.ReadToEnd();
        //        }
        //        reader.Close();
        //        reader.Dispose();
        //    }
        //    catch
        //    {
        //        return "";
        //    }

        //    return text;

        //}
        //private bool IsFileNameExists(string FullFileName)
        //{
        //    return System.IO.File.Exists(FullFileName);
        //}

    }
}
