using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using System.Data;
using System.Configuration;
using Basic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using System.Data;
using System.Configuration;
using Basic;
using SQLUI;
using System.Xml;
using HIS_DB_Lib;

namespace DB2VM
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBARController : ControllerBase
    {
        [HttpGet]
        public string Get(string? BarCode)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData returnData = new returnData();
            try
            {
                if (BarCode.StringIsEmpty())
                {
                    returnData.Code = -200;
                    returnData.Result = $"傳入BarCode 空白";
                    return returnData.JsonSerializationt(true);
                }
                if (BarCode.ToUpper() == "test".ToUpper())
                {
                    List<OrderClass> orderClasses = new List<OrderClass>();

                    for (int i = 0; i < 4; i++)
                    {
                        OrderClass orderClass = new OrderClass();
                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.PRI_KEY = $"TEST_{DateTime.Now.ToDateTimeString_6()}";
                        orderClass.藥局代碼 = "OPD";
                        orderClass.藥袋類型 = "";
                        orderClass.領藥號 = $"{DateTime.Now.ToDateTimeTiny(TypeConvert.Enum_Year_Type.Anno_Domini)}";
                        orderClass.病人姓名 = "測試";
                        orderClass.藥袋條碼 = orderClass.PRI_KEY;
                        orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                        orderClass.開方日期 = DateTime.Now.ToDateTimeString_6();

                        if (i == 0) orderClass.藥品碼 = "UNIO04";
                        if (i == 0) orderClass.交易量 = (-3).ToString();
                        if (i == 1) orderClass.藥品碼 = "GLUO04";
                        if (i == 1) orderClass.交易量 = (-28).ToString();
                        if (i == 2) orderClass.藥品碼 = "BIME00";
                        if (i == 2) orderClass.交易量 = (-2).ToString();
                        if (i == 3) orderClass.藥品碼 = "CARE01";
                        if (i == 3) orderClass.交易量 = (-4).ToString();
                        orderClass.狀態 = "未過帳";

                        orderClasses.Add(orderClass);
                    }
                    returnData.Code = 200;
                    returnData.Data = orderClasses;
                    returnData.Result = $"取得測試醫令成功,共{orderClasses.Count}筆資料";
                    returnData.TimeTaken = myTimerBasic.ToString();
                }
                if (BarCode.Length == 9)
                {
                    if (BarCode.Substring(0, 1) == "1" || BarCode.Substring(0, 1) == "4")
                    {
                        System.Text.StringBuilder soap = new System.Text.StringBuilder();
                        soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                        soap.Append("<soap:Body>");
                        soap.Append("<GetCodeOE_XML  xmlns=\"http://tempuri.org/\">");
                        soap.Append($"<print_barcode>{BarCode}</print_barcode>");
                        soap.Append("</GetCodeOE_XML >");
                        soap.Append("</soap:Body>");
                        soap.Append("</soap:Envelope>");
                        string Xml = Basic.Net.WebServicePost("http://192.168.8.108/Service.asmx?op=GetCodeOE_XML", soap);
                        GetCodeOE_XML(Xml, ref returnData); // 警級用藥、首日量、出院帶藥
                        returnData.TimeTaken = myTimerBasic.ToString();
                        return returnData.JsonSerializationt(true);
                    }
                }
                else if (BarCode.Length == 12)
                {
                    System.Text.StringBuilder soap = new System.Text.StringBuilder();
                    soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    soap.Append("<soap:Body>");
                    soap.Append("<GetCodeSH_XML  xmlns=\"http://tempuri.org/\">");
                    soap.Append($"<print_barcode>{BarCode}</print_barcode>");
                    soap.Append("</GetCodeSH_XML >");
                    soap.Append("</soap:Body>");
                    soap.Append("</soap:Envelope>");
                    string Xml = Basic.Net.WebServicePost("http://192.168.8.108/Service.asmx?op=GetCodeSH_XML", soap);
                    GetCodeSH_XML(Xml, ref returnData);
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                else if (BarCode.Length == 8)
                {
                    System.Text.StringBuilder soap = new System.Text.StringBuilder();
                    soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    soap.Append("<soap:Body>");
                    soap.Append("<GetCodeCTLI_XML  xmlns=\"http://tempuri.org/\">");
                    soap.Append($"<print_barcode>{BarCode}</print_barcode>");
                    soap.Append("</GetCodeCTLI_XML >");
                    soap.Append("</soap:Body>");
                    soap.Append("</soap:Envelope>");
                    string Xml = Basic.Net.WebServicePost("http://192.168.8.108/Service.asmx?op=GetCodeCTLI_XML", soap);
                    GetCodeCTLI_XML(Xml, ref returnData);                 

                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                else if (BarCode.Length == 15 || BarCode.Length == 17) 
                {
                    System.Text.StringBuilder soap = new System.Text.StringBuilder();
                    soap.Append("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
                    soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    soap.Append("<soap:Body>");
                    soap.Append("<GetCodeCTLO_XML xmlns=\"http://tempuri.org/\">");
                    soap.Append($"<odr_no>{BarCode}</odr_no>");
                    soap.Append("</GetCodeCTLO_XML >");
                    soap.Append("</soap:Body>");
                    soap.Append("</soap:Envelope>");
                    string Xml = Basic.Net.WebServicePost("http://192.168.8.108/Service.asmx?op==GetCodeCTLO_XML", soap);
                    GetCodeCTLO_XML(Xml, ref returnData);
                    returnData.TimeTaken = myTimerBasic.ToString();
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData.Code = -200;
                    returnData.Result = $"條碼錯誤";
                    return returnData.JsonSerializationt(true);
                }

                string jsonString = returnData.JsonSerializationt(true);
                return jsonString;
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception:{e.Message}";
                return returnData.JsonSerializationt(true);
            }

        }

        [Route("MRN")]
        [HttpPost]
        public string POST_MRN(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "POST_MRN";
            try
            {
                if (returnData.ValueAry.Count < 1)
                {
                    returnData.Code = -200;
                    returnData.Result = $"輸入資料內容錯誤,需為病歷號、(選)[起始時間、結束時間]";
                    return returnData.JsonSerializationt(true);
                }
                List<OrderClass> orderClasses = new List<OrderClass>();
                if (returnData.ValueAry.Count == 3)
                {
                    string patcode = returnData.ValueAry[0];
                    string dt_st = returnData.ValueAry[1];
                    string dt_end = returnData.ValueAry[2];
                    if (dt_st.Check_Date_String() == false || dt_end.Check_Date_String() == false)
                    {
                        returnData.Code = -200;
                        returnData.Result = $"輸入資料日期格式錯誤";
                        return returnData.JsonSerializationt(true);
                    }

                    orderClasses = OrderClass.get_by_PATCODE("http://127.0.0.1:4433", patcode, dt_st.StringToDateTime(), dt_end.StringToDateTime());
                }
                if (returnData.ValueAry.Count == 1)
                {
                    string patcode = returnData.ValueAry[0];
                    orderClasses = OrderClass.get_by_PATCODE("http://127.0.0.1:4433", patcode);
                }

                returnData.Code = 200;
                returnData.TimeTaken = myTimerBasic.ToString();
                returnData.Data = orderClasses;
                returnData.Result = $"取得醫令共{orderClasses.Count}筆資料";
                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {e.Message}";
                return returnData.JsonSerializationt(true);
            }
            finally
            {

            }
        }

        [Route("BAG_NUM")]
        [HttpPost]
        public string POST_BAG_NUM(returnData returnData)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            returnData.Method = "POST_BAG_NUM";
            try
            {
                if (returnData.ValueAry.Count != 2)
                {
                    returnData.Code = -200;
                    returnData.Result = $"輸入資料內容錯誤,需為領藥號、日期";
                    return returnData.JsonSerializationt(true);
                }
                string print_que = returnData.ValueAry[0];
                string print_date = returnData.ValueAry[1];
                if (print_date.Check_Date_String() == false)
                {
                    returnData.Code = -200;
                    returnData.Result = $"輸入資料日期格式錯誤";
                    return returnData.JsonSerializationt(true);
                }
                print_date = print_date.StringToDateTime().ToDateString(TypeConvert.Enum_Year_Type.Republic_of_China, "");

                System.Text.StringBuilder soap = new System.Text.StringBuilder();
                soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                soap.Append("<soap:Body>");
                soap.Append("<GetCodeOE_XML  xmlns=\"http://tempuri.org/\">");
                soap.Append($"<print_que>{print_que}</print_que>");
                soap.Append($"<print_date>{print_date}</print_date>");
                soap.Append("</GetCodeOE_XML >");
                soap.Append("</soap:Body>");
                soap.Append("</soap:Envelope>");
                string Xml = Basic.Net.WebServicePost("http://192.168.163.69/TmhtcAdcWS/Service.asmx?op=GetCodeOE_XML", soap);
                GetCodeOE_XML(Xml, ref returnData);

                if (returnData.Code != 200)
                {
                    soap = new System.Text.StringBuilder();
                    soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    soap.Append("<soap:Body>");
                    soap.Append("<GetCodeSH_XML  xmlns=\"http://tempuri.org/\">");
                    soap.Append($"<print_que>{print_que}</print_que>");
                    soap.Append($"<print_date>{print_date}</print_date>");
                    soap.Append("</GetCodeSH_XML >");
                    soap.Append("</soap:Body>");
                    soap.Append("</soap:Envelope>");
                    Xml = Basic.Net.WebServicePost("http://192.168.163.69/TmhtcAdcWS/Service.asmx?op=GetCodeSH_XML", soap);
                    GetCodeSH_XML(Xml, ref returnData);
                }


                return returnData.JsonSerializationt();
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"Exception : {e.Message}";
                return returnData.JsonSerializationt(true);
            }
            finally
            {

            }
        }

        // 計算字串乘積的函式
        static double CalculateProduct(string input)
        {
            // 使用 Split 方法將字串分割
            string[] parts = input.Split('/');

            // 將分割出的字串轉換成整數
            double num1 = double.Parse(parts[0]);
            double num2 = double.Parse(parts[1]);

            // 返回計算結果
            return (double)num1 / num2;
        }

        public void GetCodeOE_XML(string Xml, ref returnData returnData)
        {
            string[] Node_array_pat = new string[] { "soap:Body", "GetCodeOE_XMLResponse", "GetCodeOE_XMLResult", "data", "pat" };
            string[] Node_array_drug = new string[] { "soap:Body", "GetCodeOE_XMLResponse", "GetCodeOE_XMLResult", "data", "pat", "drug" };
            XmlElement xmlElement = Xml.Xml_GetElement(Node_array_pat);
            if (xmlElement == null)
            {
                returnData.Code = -200;
                returnData.Result = $"HIS查無此藥袋資料";
                return;
            }
            string input_mark = xmlElement.Xml_GetInnerXml("input_mark");  //首日量、緊急用藥:S  出院帶藥:H  UD長期用藥:U  門診:O  急診:E
            string que_no = xmlElement.Xml_GetInnerXml("que_no"); //領藥號
            string que_unit = xmlElement.Xml_GetInnerXml("que_unit"); //護理站
            string patient_no = xmlElement.Xml_GetInnerXml("patient_no"); //病歷號
            string patient_name = xmlElement.Xml_GetInnerXml("patient_name"); //病人姓名
            string patient_sex = xmlElement.Xml_GetInnerXml("patient_sex");  //性別
            string patient_brithday = xmlElement.Xml_GetInnerXml("patient_brithday"); //出生年月
            string ipd_no = xmlElement.Xml_GetInnerXml("ipd_no"); //住院/門診序號
            string ipd_date = xmlElement.Xml_GetInnerXml("ipd_date"); //入院日期
            string odr_no = xmlElement.Xml_GetInnerXml("odr_no");//處方/退藥序號
            string odr_date = xmlElement.Xml_GetInnerXml("odr_date"); //處方/退藥日期
            string odr_dept = xmlElement.Xml_GetInnerXml("odr_dept"); //處方科別
            string odr_cla = xmlElement.Xml_GetInnerXml("odr_cla"); //處方身份
            string dr_name = xmlElement.Xml_GetInnerXml("dr_name"); //醫師姓名
            string prt_date = xmlElement.Xml_GetInnerXml("prt_date"); //藥袋列印日期
            string que_seq = xmlElement.Xml_GetInnerXml("que_seq"); //藥袋流水號

            string odr_bed = xmlElement.Xml_GetInnerXml("odr_bed"); 
            string create_date = xmlElement.Xml_GetInnerXml("create_date");
            string mod_date = xmlElement.Xml_GetInnerXml("mod_date");
            string barcode = xmlElement.Xml_GetInnerXml("barcode");
            string xml_create_time = xmlElement.Xml_GetInnerXml("xml_create_time");

            input_mark = orderType[$"{input_mark}"];
            if (barcode.StringIsEmpty())
            {
                barcode = $"{que_no},{odr_no},{patient_no},{create_date}";
            }

            List<OrderClass> orderClasses = new List<OrderClass>();
            
            orderClasses = OrderClass.get_by_pri_key("http://127.0.0.1:4433", barcode);
            List<OrderClass> orderClasses_buf = new List<OrderClass>();
            List<OrderClass> orderClasses_add = new List<OrderClass>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNodeList drugNodes = xmlDoc.SelectNodes("//drug", nsmgr);
            foreach (XmlNode drugNode in drugNodes)
            {
                string odr_seq = drugNode["odr_seq"]?.InnerText;  //處置流水號
                string prs_id = drugNode["prs_id"]?.InnerText; //處置代碼
                string prs_name = drugNode["prs_name"]?.InnerText;  //處置名稱
                string prs_sc_name = drugNode["prs_sc_name"]?.InnerText;  //學名/品名
                string prs_spec = drugNode["prs_spec"]?.InnerText;  //規格
                string prs_srv_unit = drugNode["prs_srv_unit"]?.InnerText; //服用單位

                string prs_stk = drugNode["prs_stk"]?.InnerText; //庫存對照碼
                string drug_uqty = drugNode["drug_uqty"]?.InnerText; //次劑量
                string drug_qty = drugNode["drug_qty"]?.InnerText; //總藥量
                string drug_day = drugNode["drug_day"]?.InnerText;//使用天數
                string drug_way1 = drugNode["drug_way1"]?.InnerText; //頻率
                string drug_way2 = drugNode["drug_way2"]?.InnerText; //途徑
                string del_mark = drugNode["del_mark"]?.InnerText;  //刪除註記

                if (orderClasses.Count == 0)
                {
                    OrderClass orderClass = new OrderClass();
                    orderClass.GUID = Guid.NewGuid().ToString();
                    orderClass.PRI_KEY = barcode;
                    orderClass.藥局代碼 = "OPD";
                    orderClass.藥袋類型 = input_mark;
                    orderClass.領藥號 = que_no;
                    orderClass.病房 = que_unit;
                    orderClass.病歷號 = patient_no;
                    orderClass.病人姓名 = patient_name;
                    orderClass.住院序號 = ipd_no;
                    orderClass.就醫時間 = ipd_date.StringToAnnoDomini().ToDateTimeString_6();
                    orderClass.醫師代碼 = dr_name;
                    orderClass.藥袋條碼 = barcode;
                    orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                    orderClass.開方日期 = odr_date.StringToAnnoDomini().ToDateTimeString_6(); ;

                    orderClass.批序 = odr_seq;
                    orderClass.藥品碼 = prs_stk;
                    orderClass.藥品名稱 = prs_name;
                    orderClass.劑量單位 = prs_srv_unit;
                    orderClass.天數 = drug_day;
                    orderClass.單次劑量 = drug_uqty;
                    orderClass.頻次 = drug_way1;
                    orderClass.途徑 = drug_way2;
                    orderClass.交易量 = (CalculateProduct(drug_qty) * -1).ToString();
                    orderClass.狀態 = "未過帳";

                    orderClasses_add.Add(orderClass);
                }
                
            }

            if (orderClasses.Count == 0) orderClasses = orderClasses_add;
            List<object[]> list_value_add = orderClasses_add.ClassToSQL<OrderClass, enum_醫囑資料>();
            if (list_value_add.Count > 0) OrderClass.add("http://127.0.0.1:4433", orderClasses_add);

            returnData.Code = 200;
            returnData.Data = orderClasses;
            returnData.Result = $"取得醫令成功,共{orderClasses.Count}筆資料,新增{orderClasses_add.Count}筆資料";

        }
        public void GetCodeSH_XML(string Xml, ref returnData returnData)
        {
            string[] Node_array_pat = new string[] { "soap:Body", "GetCodeSH_XMLResponse", "GetCodeSH_XMLResult", "data", "pat" };
            string[] Node_array_drug = new string[] { "soap:Body", "GetCodeSH_XMLResponse", "GetCodeSH_XMLResult", "data", "pat", "drug" };
            XmlElement xmlElement = Xml.Xml_GetElement(Node_array_pat);
            if (xmlElement == null)
            {
                returnData.Code = -200;
                returnData.Result = $"HIS查無此藥袋資料";
                return;
            }
            string input_mark = xmlElement.Xml_GetInnerXml("input_mark");
            string que_no = xmlElement.Xml_GetInnerXml("que_no");
            string que_unit = xmlElement.Xml_GetInnerXml("que_unit");
            string patient_no = xmlElement.Xml_GetInnerXml("patient_no");
            string patient_name = xmlElement.Xml_GetInnerXml("patient_name");
            string patient_sex = xmlElement.Xml_GetInnerXml("patient_sex");
            string patient_brithday = xmlElement.Xml_GetInnerXml("patient_brithday");
            string ipd_no = xmlElement.Xml_GetInnerXml("ipd_no");
            string odr_bed = xmlElement.Xml_GetInnerXml("odr_bed");
            string ipd_date = xmlElement.Xml_GetInnerXml("ipd_date");
            string odr_no = xmlElement.Xml_GetInnerXml("odr_no");
            string odr_date = xmlElement.Xml_GetInnerXml("odr_date");
            string odr_dept = xmlElement.Xml_GetInnerXml("odr_dept");
            string odr_cla = xmlElement.Xml_GetInnerXml("odr_cla");
            string dr_name = xmlElement.Xml_GetInnerXml("dr_name");
            string prt_date = xmlElement.Xml_GetInnerXml("prt_date");
            string que_seq = xmlElement.Xml_GetInnerXml("que_seq");
            string create_date = xmlElement.Xml_GetInnerXml("create_date");
            string mod_date = xmlElement.Xml_GetInnerXml("mod_date");
            string barcode = xmlElement.Xml_GetInnerXml("barcode");
            string xml_create_time = xmlElement.Xml_GetInnerXml("xml_create_time");

            List<OrderClass> orderClasses = new List<OrderClass>();
            if (barcode.StringIsEmpty())
            {
                barcode = $"{que_no},{odr_no},{patient_no},{create_date}";
            }
            orderClasses = OrderClass.get_by_pri_key("http://127.0.0.1:4433", barcode);
            List<OrderClass> orderClasses_buf = new List<OrderClass>();
            List<OrderClass> orderClasses_add = new List<OrderClass>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNodeList drugNodes = xmlDoc.SelectNodes("//drug", nsmgr);
            foreach (XmlNode drugNode in drugNodes)
            {
                string odr_seq = drugNode["odr_seq"]?.InnerText;
                string prs_id = drugNode["prs_id"]?.InnerText;
                string prs_name = drugNode["prs_name"]?.InnerText;
                string prs_sc_name = drugNode["prs_sc_name"]?.InnerText;
                string prs_spec = drugNode["prs_spec"]?.InnerText;
                string prs_stk = drugNode["prs_stk"]?.InnerText;
                string prs_srv_unit = drugNode["prs_srv_unit"]?.InnerText;
                string drug_uqty = drugNode["drug_uqty"]?.InnerText;
                string drug_qty = drugNode["drug_qty"]?.InnerText;
                string drug_day = drugNode["drug_day"]?.InnerText;
                string drug_way1 = drugNode["drug_way1"]?.InnerText;
                string drug_way2 = drugNode["drug_way2"]?.InnerText;
                string del_mark = drugNode["del_mark"]?.InnerText;

                if (orderClasses.Count == 0)
                {
                    OrderClass orderClass = new OrderClass();
                    orderClass.GUID = Guid.NewGuid().ToString();
                    orderClass.PRI_KEY = barcode;
                    orderClass.藥局代碼 = "OPD";
                    orderClass.藥袋類型 = input_mark;
                    orderClass.領藥號 = que_no;
                    orderClass.住院序號 = ipd_no;
                    orderClass.床號 = odr_bed;
                    orderClass.病房 = que_unit;
                    orderClass.病歷號 = patient_no;
                    orderClass.病人姓名 = patient_name;
                    orderClass.住院序號 = ipd_no;
                    orderClass.就醫時間 = ipd_date.StringToAnnoDomini().ToDateTimeString_6();
                    orderClass.醫師代碼 = dr_name;
                    orderClass.藥袋條碼 = barcode;
                    orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                    orderClass.開方日期 = xml_create_time;

                    orderClass.批序 = odr_seq;
                    orderClass.藥品碼 = prs_stk;
                    orderClass.藥品名稱 = prs_name;
                    orderClass.劑量單位 = prs_srv_unit;
                    orderClass.天數 = drug_day;
                    orderClass.單次劑量 = drug_uqty;
                    orderClass.頻次 = drug_way1;
                    orderClass.途徑 = drug_way2;
                    orderClass.交易量 = (CalculateProduct(drug_qty) * -1).ToString();
                    orderClass.狀態 = "未過帳";

                    orderClasses_add.Add(orderClass);
                }
                else
                {

                }
            }

            if (orderClasses.Count == 0) orderClasses = orderClasses_add;
            List<object[]> list_value_add = orderClasses_add.ClassToSQL<OrderClass, enum_醫囑資料>();
            if (list_value_add.Count > 0)
            {
                OrderClass.add("http://127.0.0.1:4433", orderClasses_add);
            }

            returnData.Code = 200;
            returnData.Data = orderClasses;
            returnData.Result = $"取得醫令成功,共{orderClasses.Count}筆資料,新增{orderClasses_add.Count}筆資料";

        }
        public void GetCodeCTLI_XML(string Xml, ref returnData returnData)
        {
            string[] Node_array_pat = new string[] { "soap:Body", "GetCodeCTLI_XMLResponse", "GetCodeCTLI_XMLResult", "data", "pat" };
            string[] Node_array_drug = new string[] { "soap:Body", "GetCodeCTLI_XMLResponse", "GetCodeCTLI_XMLResult", "data", "pat", "drug" };
            XmlElement xmlElement = Xml.Xml_GetElement(Node_array_pat);
            if (xmlElement == null)
            {
                returnData.Code = -200;
                returnData.Result = $"HIS查無此藥袋資料";
                return;
            }
            string input_mark = xmlElement.Xml_GetInnerXml("input_mark");
            string que_no = xmlElement.Xml_GetInnerXml("que_no");
            string que_unit = xmlElement.Xml_GetInnerXml("que_unit");
            string patient_no = xmlElement.Xml_GetInnerXml("patient_no");
            string patient_name = xmlElement.Xml_GetInnerXml("patient_name");
            string patient_sex = xmlElement.Xml_GetInnerXml("patient_sex");
            string patient_brithday = xmlElement.Xml_GetInnerXml("patient_brithday");
            string ipd_no = xmlElement.Xml_GetInnerXml("ipd_no");
            string odr_bed = xmlElement.Xml_GetInnerXml("odr_bed");
            string ipd_date = xmlElement.Xml_GetInnerXml("ipd_date");
            string odr_no = xmlElement.Xml_GetInnerXml("odr_no");
            string odr_date = xmlElement.Xml_GetInnerXml("odr_date");
            string odr_dept = xmlElement.Xml_GetInnerXml("odr_dept");
            string odr_cla = xmlElement.Xml_GetInnerXml("odr_cla");
            string dr_name = xmlElement.Xml_GetInnerXml("dr_name");
            string prt_date = xmlElement.Xml_GetInnerXml("prt_date");
            string que_seq = xmlElement.Xml_GetInnerXml("que_seq");
            string create_date = xmlElement.Xml_GetInnerXml("create_date");
            string mod_date = xmlElement.Xml_GetInnerXml("mod_date");
            string barcode = xmlElement.Xml_GetInnerXml("barcode");
            string xml_create_time = xmlElement.Xml_GetInnerXml("xml_create_time");

            List<OrderClass> orderClasses = new List<OrderClass>();
            if (barcode.StringIsEmpty())
            {
                barcode = $"{que_no},{odr_no},{patient_no},{create_date}";
            }
            orderClasses = OrderClass.get_by_pri_key("http://127.0.0.1:4433", barcode);
            List<OrderClass> orderClasses_buf = new List<OrderClass>();
            List<OrderClass> orderClasses_add = new List<OrderClass>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNodeList drugNodes = xmlDoc.SelectNodes("//drug", nsmgr);
            foreach (XmlNode drugNode in drugNodes)
            {
                string odr_seq = drugNode["odr_seq"]?.InnerText;
                string prs_id = drugNode["prs_id"]?.InnerText;
                string prs_name = drugNode["prs_name"]?.InnerText;
                string prs_sc_name = drugNode["prs_sc_name"]?.InnerText;
                string prs_spec = drugNode["prs_spec"]?.InnerText;
                string prs_stk = drugNode["prs_stk"]?.InnerText;
                string prs_srv_unit = drugNode["prs_srv_unit"]?.InnerText;
                string drug_uqty = drugNode["drug_uqty"]?.InnerText;
                string drug_qty = drugNode["drug_qty"]?.InnerText;
                string drug_day = drugNode["drug_day"]?.InnerText;
                string drug_way1 = drugNode["drug_way1"]?.InnerText;
                string drug_way2 = drugNode["drug_way2"]?.InnerText;
                string del_mark = drugNode["del_mark"]?.InnerText;

                if (orderClasses.Count == 0)
                {
                    OrderClass orderClass = new OrderClass();
                    orderClass.GUID = Guid.NewGuid().ToString();
                    orderClass.PRI_KEY = barcode;
                    orderClass.藥局代碼 = "OPD";
                    orderClass.藥袋類型 = input_mark;
                    orderClass.領藥號 = que_no;
                    orderClass.住院序號 = ipd_no;
                    orderClass.床號 = odr_bed;
                    orderClass.病房 = que_unit;
                    orderClass.病歷號 = patient_no;
                    orderClass.病人姓名 = patient_name;
                    orderClass.住院序號 = ipd_no;
                    orderClass.就醫時間 = ipd_date.StringToAnnoDomini().ToDateTimeString_6();
                    orderClass.醫師代碼 = dr_name;
                    orderClass.藥袋條碼 = barcode;
                    orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                    orderClass.開方日期 = xml_create_time;

                    orderClass.批序 = odr_seq;
                    orderClass.藥品碼 = prs_stk;
                    orderClass.藥品名稱 = prs_name;
                    orderClass.劑量單位 = prs_srv_unit;
                    orderClass.天數 = drug_day;
                    orderClass.單次劑量 = drug_uqty;
                    orderClass.頻次 = drug_way1;
                    orderClass.途徑 = drug_way2;
                    double temp = CalculateProduct(drug_qty);
                    orderClass.交易量 = (CalculateProduct(drug_qty) * -1).ToString();
                    orderClass.狀態 = "未過帳";

                    orderClasses_add.Add(orderClass);
                }
                else
                {

                }
            }

            if (orderClasses.Count == 0) orderClasses = orderClasses_add;
            List<object[]> list_value_add = orderClasses_add.ClassToSQL<OrderClass, enum_醫囑資料>();
            if (list_value_add.Count > 0)
            {
                OrderClass.add("http://127.0.0.1:4433", orderClasses_add);
            }

            returnData.Code = 200;
            returnData.Data = orderClasses;
            returnData.Result = $"取得醫令成功,共{orderClasses.Count}筆資料,新增{orderClasses_add.Count}筆資料";

        }
        public void GetCodeCTLO_XML(string Xml, ref returnData returnData)
        {
            string[] Node_array_pat = new string[] { "soap:Body", "GetCodeCTLO_XMLResponse", "GetCodeCTLO_XMLResult", "data", "pat" };
            string[] Node_array_drug = new string[] { "soap:Body", "GetCodeCTLO_XMLResponse", "GetCodeCTLO_XMLResult", "data", "pat", "drug" };
            XmlElement xmlElement = Xml.Xml_GetElement(Node_array_pat);
            if (xmlElement == null)
            {
                returnData.Code = -200;
                returnData.Result = $"HIS查無此藥袋資料";
                return;
            }
            string input_mark = xmlElement.Xml_GetInnerXml("input_mark");
            string que_no = xmlElement.Xml_GetInnerXml("que_no");
            string que_unit = xmlElement.Xml_GetInnerXml("que_unit");
            string patient_no = xmlElement.Xml_GetInnerXml("patient_no");
            string patient_name = xmlElement.Xml_GetInnerXml("patient_name");
            string patient_sex = xmlElement.Xml_GetInnerXml("patient_sex");
            string patient_brithday = xmlElement.Xml_GetInnerXml("patient_brithday");
            string ipd_no = xmlElement.Xml_GetInnerXml("ipd_no");
            string odr_bed = xmlElement.Xml_GetInnerXml("odr_bed");
            string ipd_date = xmlElement.Xml_GetInnerXml("ipd_date");
            string odr_no = xmlElement.Xml_GetInnerXml("odr_no");
            string odr_date = xmlElement.Xml_GetInnerXml("odr_date");
            string odr_dept = xmlElement.Xml_GetInnerXml("odr_dept");
            string odr_cla = xmlElement.Xml_GetInnerXml("odr_cla");
            string dr_name = xmlElement.Xml_GetInnerXml("dr_name");
            string prt_date = xmlElement.Xml_GetInnerXml("prt_date");
            string que_seq = xmlElement.Xml_GetInnerXml("que_seq");
            string create_date = xmlElement.Xml_GetInnerXml("create_date");
            string mod_date = xmlElement.Xml_GetInnerXml("mod_date");
            string barcode = xmlElement.Xml_GetInnerXml("barcode");
            string xml_create_time = xmlElement.Xml_GetInnerXml("xml_create_time");

            List<OrderClass> orderClasses = new List<OrderClass>();
            if (barcode.StringIsEmpty())
            {
                barcode = $"{que_no},{odr_no},{patient_no},{create_date}";
            }
            orderClasses = OrderClass.get_by_pri_key("http://127.0.0.1:4433", barcode);
            List<OrderClass> orderClasses_buf = new List<OrderClass>();
            List<OrderClass> orderClasses_add = new List<OrderClass>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNodeList drugNodes = xmlDoc.SelectNodes("//drug", nsmgr);
            foreach (XmlNode drugNode in drugNodes)
            {
                string odr_seq = drugNode["odr_seq"]?.InnerText;
                string prs_id = drugNode["prs_id"]?.InnerText;
                string prs_name = drugNode["prs_name"]?.InnerText;
                string prs_sc_name = drugNode["prs_sc_name"]?.InnerText;
                string prs_spec = drugNode["prs_spec"]?.InnerText;
                string prs_stk = drugNode["prs_stk"]?.InnerText;
                string prs_srv_unit = drugNode["prs_srv_unit"]?.InnerText;
                string drug_uqty = drugNode["drug_uqty"]?.InnerText;
                string drug_qty = drugNode["drug_qty"]?.InnerText;
                string drug_day = drugNode["drug_day"]?.InnerText;
                string drug_way1 = drugNode["drug_way1"]?.InnerText;
                string drug_way2 = drugNode["drug_way2"]?.InnerText;
                string del_mark = drugNode["del_mark"]?.InnerText;

                if (orderClasses.Count == 0)
                {
                    OrderClass orderClass = new OrderClass();
                    orderClass.GUID = Guid.NewGuid().ToString();
                    orderClass.PRI_KEY = barcode;
                    orderClass.藥局代碼 = "OPD";
                    orderClass.藥袋類型 = input_mark;
                    orderClass.領藥號 = que_no;
                    orderClass.住院序號 = ipd_no;
                    orderClass.床號 = odr_bed;
                    orderClass.病房 = que_unit;
                    orderClass.病歷號 = patient_no;
                    orderClass.病人姓名 = patient_name;
                    orderClass.住院序號 = ipd_no;
                    orderClass.就醫時間 = ipd_date.StringToAnnoDomini().ToDateTimeString_6();
                    orderClass.醫師代碼 = dr_name;
                    orderClass.藥袋條碼 = barcode;
                    orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                    orderClass.開方日期 = xml_create_time;

                    orderClass.批序 = odr_seq;
                    orderClass.藥品碼 = prs_stk;
                    orderClass.藥品名稱 = prs_name;
                    orderClass.劑量單位 = prs_srv_unit;
                    orderClass.天數 = drug_day;
                    orderClass.單次劑量 = drug_uqty;
                    orderClass.頻次 = drug_way1;
                    orderClass.途徑 = drug_way2;
                    orderClass.交易量 = (CalculateProduct(drug_qty) * -1).ToString();
                    orderClass.狀態 = "未過帳";

                    orderClasses_add.Add(orderClass);
                }
                else
                {

                }
            }

            if (orderClasses.Count == 0) orderClasses = orderClasses_add;
            List<object[]> list_value_add = orderClasses_add.ClassToSQL<OrderClass, enum_醫囑資料>();
            if (list_value_add.Count > 0)
            {
                OrderClass.add("http://127.0.0.1:4433", orderClasses_add);
            }

            returnData.Code = 200;
            returnData.Data = orderClasses;
            returnData.Result = $"取得醫令成功,共{orderClasses.Count}筆資料,新增{orderClasses_add.Count}筆資料";

        }
        private Dictionary<string, string> orderType = new Dictionary<string, string>
        {
            { "S","STKN"},
            { "H","出院帶藥"},
            { "U","UD長期用藥"},
            { "O","門診"},
            { "E","急診"},
        };
    }
}
