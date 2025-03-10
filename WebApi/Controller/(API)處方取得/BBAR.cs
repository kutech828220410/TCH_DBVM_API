using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic;
using HIS_DB_Lib;
using SQLUI;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DB2VM_API.Controller._API_處方取得
{
    [Route("api/[controller]")]
    [ApiController]
    public class BBAR : ControllerBase
    {
        static public string API_Server = "http://127.0.0.1:4433";
        //static public string API_Server = "https://www.kutech.tw:4443";

        static public string Server = "127.0.0.1";
        static public string DB = "dbvm_his";
        static public string UserName = "his_user";
        static public string Password = "hson11486";
        static public uint Port = 3306;
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        [HttpGet]
        public string get_order(string? BarCode)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            try
            {
                if (BarCode.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "Barcode空白";
                    return returnData.JsonSerializationt(true);
                }
                //BarCode += "%"; 
                SQLControl sQLControl_med_carInfo = new SQLControl(Server, DB, "yc_pha_order", UserName, Password, Port, SSLMode);
                List<object[]> list_pha_order = sQLControl_med_carInfo.GetRowsByLike(null, enum_yc_pha_order.ST_QRCODE.GetEnumName(), BarCode);
                List<object[]> string_pha_order = new List<object[]>();
                foreach (var order in list_pha_order)
                {
                    object[] stringifiedOrder = Array.ConvertAll(order, item => item?.ToString() ?? string.Empty);
                    string_pha_order.Add(stringifiedOrder);
                }
                List<phaOrderClass> sql_medCar = string_pha_order.SQLToClass<phaOrderClass, enum_yc_pha_order>();
                List<OrderClass> orderClasses = new List<OrderClass>();
                foreach (phaOrderClass phaOrderClass in sql_medCar)
                {
                    if (phaOrderClass.CD_FROM == "O") phaOrderClass.CD_FROM = "門診";
                    if (phaOrderClass.CD_FROM == "E") phaOrderClass.CD_FROM = "急診";
                    if (phaOrderClass.CD_FROM == "I") phaOrderClass.CD_FROM = "住院";
                    if (phaOrderClass.CD_ORDER == "N") phaOrderClass.CD_ORDER = "門急";
                    if (phaOrderClass.CD_ORDER == "B") phaOrderClass.CD_ORDER = "首日量";
                    if (phaOrderClass.CD_ORDER == "S") phaOrderClass.CD_ORDER = "STAT";
                    if (phaOrderClass.CD_ORDER == "M") phaOrderClass.CD_ORDER = "出院帶藥";


                    //if (phaOrderClass.CD_CANCEL == "N") phaOrderClass.CD_FROM = "New";
                    //if (phaOrderClass.CD_CANCEL == "Y") phaOrderClass.CD_FROM = "DC";
                    string dateTime = $"{phaOrderClass.DM_DRUG.Substring(0,4)}-{phaOrderClass.DM_DRUG.Substring(4, 2)}-{phaOrderClass.DM_DRUG.Substring(6, 2)}" +
                        $" {phaOrderClass.DM_DRUG.Substring(8, 2)}:{phaOrderClass.DM_DRUG.Substring(10, 2)}:{phaOrderClass.DM_DRUG.Substring(12, 2)}";
                    if(phaOrderClass.CD_CANCEL == "Y")
                    {

                    }
                    OrderClass orderClass = new OrderClass
                    {
                        PRI_KEY = BarCode,
                        藥袋條碼 = BarCode,
                        開方日期 = dateTime,
                        藥袋類型 = phaOrderClass.CD_ORDER,
                        藥局代碼 = phaOrderClass.CD_FROM,
                        病歷號 = phaOrderClass.ID_PATIENT,
                        領藥號 = phaOrderClass.IT_DRUGNO,
                        病人姓名 = phaOrderClass.NM_PATIENT,
                        藥品碼 = phaOrderClass.ID_DRUG,
                        藥品名稱 = phaOrderClass.NM_DRUG,
                        單次劑量 = phaOrderClass.DB_DOSE,
                        頻次 = phaOrderClass.ST_FREQUENCY,
                        途徑 = phaOrderClass.ST_PATH,
                        交易量 = $"-{phaOrderClass.DB_AMOUNT}",
                        //交易量 = ((phaOrderClass.DB_AMOUNT).ToString(),
                        //批序 = phaOrderClass.DATETIMESEQ,
                        //藥袋類型 = phaOrderClass.CD_CANCEL,
                        //病房 = orderlistClass.病房,
                        床號 = phaOrderClass.ST_BEDNO,
                        狀態 = "未過帳",
                        備註 = ""
                    };
                    string 批序 = phaOrderClass.DATETIMESEQ;
                    string 藥袋狀態 = phaOrderClass.CD_CANCEL;
                    if(藥袋狀態 == "N")
                    {
                        藥袋狀態 = "NEW";
                    }
                    else
                    {
                        藥袋狀態 = "DC";
                    }
      
                    orderClass.批序 = $"{批序}-[{藥袋狀態}]";
                    orderClasses.Add(orderClass);
                }
                


            
                //List<OrderClass> update_OrderClass = OrderClass.update_order_list(API_Server, orderClasses);
                if(orderClasses.Count == 0 )
                {
                    returnData.Code = 200;
                    returnData.Result = $"此條碼無資料";
                    return returnData.JsonSerializationt(true);
                }

                returnData returnData_update_order = OrderClass.update_order_list_new(API_Server, orderClasses);
                if (returnData_update_order.Code != 200) return returnData_update_order.JsonSerializationt(true);
                List<OrderClass> out_OrderClass = returnData_update_order.Data.ObjToClass<List<OrderClass>>();


                returnData.Data = out_OrderClass;
                returnData.Code = 200;
                returnData.Result = $"取得醫令資料{out_OrderClass.Count}筆資料";
                return returnData.JsonSerializationt(true);
            }
            catch(Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception:{ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpGet("date")]
        public string get_order_by_date(string? date)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            try
            {
                if (date.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = "date空白";
                    return returnData.JsonSerializationt(true);
                }
                List<orderlistClass> orderlistClasses = orderlistClass.get_order_by_date(date);
                List<medClass> medClasses = medClass.get_med_cloud(API_Server);
                Dictionary<string, List<medClass>> medClassDict = medClass.CoverToDictionaryByCode(medClasses);
                List<OrderClass> orderClasses = new List<OrderClass>();
                foreach(orderlistClass orderlistClass in orderlistClasses)
                {
                    foreach (var medicationItems in orderlistClass.medicationItems)
                    {
                        medClass targetMed = medClasses.Where(temp => temp.料號 == medicationItems.料號).FirstOrDefault();
                        if (targetMed == null)
                        {
                            targetMed = new medClass();
                            targetMed.藥品碼 = medicationItems.料號;
                        }
                        string 開方日期 = "";
                        //DateTime dd = orderlistClass.orderDate.StringToDateTime();
                        //string sqlFormattedDate = dd.ToString("yyyy-MM-dd HH:mm:ss");
                        if (orderlistClass.開方日期.StringIsEmpty() == false)
                        {
                            開方日期 = orderlistClass.開方日期;
                        }
                        else
                        {
                            開方日期 = orderlistClass.orderDate;
                        }
                        OrderClass orderClass = new OrderClass
                        {
                            PRI_KEY = orderlistClass.藥袋條碼,
                            藥袋條碼 = orderlistClass.藥袋條碼,
                            開方日期 = 開方日期,
                            病歷號 = orderlistClass.病歷號.ToString(),
                            領藥號 = orderlistClass.領藥號.ToString(),
                            病人姓名 = orderlistClass.病人姓名,
                            藥品碼 = targetMed.藥品碼,
                            藥品名稱 = medicationItems.藥品名稱,
                            單次劑量 = medicationItems.單次劑量.ToString(),
                            頻次 = medicationItems.頻次,
                            途徑 = medicationItems.途徑,
                            交易量 = (medicationItems.交易量 * -1).ToString(),
                            批序 = medicationItems.批序.ToString(),
                            藥袋類型 = medicationItems.藥袋類型,
                            病房 = orderlistClass.病房,
                            床號 = orderlistClass.床號,
                            狀態 = "未過帳"
                        };
                        if (orderClass.藥袋類型 == "A") orderClass.藥袋類型 = "New";
                        if (orderClass.藥袋類型 == "D") orderClass.藥袋類型 = "DC";
                        orderClasses.Add(orderClass);
                    }
                }
               
                //medCarInfoClass.update_order_list(API_Server, orderClasses);
                //List<OrderClass> update_OrderClass = medCarInfoClass.update_order_list(API_Server, orderClasses);
                returnData.Data = orderClasses;
                returnData.Code = 200;
                return returnData.JsonSerializationt(true);
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception:{ex.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [HttpPost("get_order")]
        public string get_order([FromBody] returnData returnData)
        {
            List<orderlistClass> orderlistClasses = returnData.Data.ObjToClass<List<orderlistClass>>();
            List<OrderClass> orderClasses = new List<OrderClass>();
            foreach (var orderlistClass in orderlistClasses)
            {
                foreach(var medicationItems in orderlistClass.medicationItems)
                {
                    OrderClass orderClass = new OrderClass
                    {
                        開方日期 = orderlistClass.開方日期.StringToDateTime().ToDateTimeString(),
                        病歷號 = orderlistClass.病歷號.ToString(),
                        藥品碼 = medicationItems.料號,
                        藥品名稱 = medicationItems.藥品名稱,
                        單次劑量 = medicationItems.單次劑量.ToString(),
                        頻次 = medicationItems.頻次,
                        途徑 = medicationItems.途徑,
                        交易量 = medicationItems.交易量.ToString()
                    };
                    orderClasses.Add(orderClass);
                }               
            }
            returnData.Data = orderClasses;
            return returnData.JsonSerializationt(true);
        }
    }

}
