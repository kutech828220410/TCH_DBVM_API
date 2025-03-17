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
using System.IO;
using System.Text;

namespace DB2VM.Controller
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
    {
    
        static public string API_Server = "http://127.0.0.1:4433";

        [HttpGet]
        public string Get(string Code)
        {
            //if (Code.StringIsEmpty()) return "[]";
            System.Text.StringBuilder soap = new System.Text.StringBuilder();
            soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            soap.Append("<soap:Body>");
            soap.Append("<GetSTK_XML xmlns=\"http://tempuri.org/\">");
            soap.Append($"<prs_stk>{Code}</prs_stk>");
            soap.Append("</GetSTK_XML>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");
            string Xml = Basic.Net.WebServicePost("http://192.168.8.108/Service.asmx?op=GetSTK_XML", soap);
        
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Xml);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsManager.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNodeList drugNodes = xmlDoc.SelectNodes("//data/drug", nsManager);
            List<medClass> medClasses = new List<medClass>();
            if (drugNodes == null || drugNodes.Count == 0)
            {
                return "[]";
            }

            foreach (XmlNode node in drugNodes)
            {
                string 藥品碼 = node.SelectSingleNode("prs_stk")?.InnerText ?? "";
                string 藥品名稱 = node.SelectSingleNode("prs_name")?.InnerText ?? "";
                string 藥品學名 = node.SelectSingleNode("prs_sc_name")?.InnerText ?? "";
                string 管制級別 = node.SelectSingleNode("control_level")?.InnerText ?? "";
                string 圖片網址 = node.SelectSingleNode("url")?.InnerText ?? "";
                string 最小包裝單位 = node.SelectSingleNode("prs_prc_unit")?.InnerText ?? "";
                string 類別 = node.SelectSingleNode("med_type")?.InnerText ?? "";


                // 檢查 MCODE 是否為空
                if (藥品碼.StringIsEmpty()) continue;

                medClass medClass = new medClass
                {
                    藥品碼 = 藥品碼,
                    藥品名稱 = 藥品名稱,
                    藥品學名 = 藥品學名,
                    管制級別 = 管制級別,
                    中西藥 = "西藥",
                    最小包裝單位 = 最小包裝單位,
                    類別 = 類別,
                    圖片網址 = 圖片網址

                };
                if (medClass.管制級別.StringIsEmpty()) medClass.管制級別 = "N";
                medClasses.Add(medClass);

            }


            
            if (medClasses.Count == 0) return "[]";
            returnData returnData_medClass = medClass.add_med_clouds(API_Server, medClasses);
            if (returnData_medClass == null || returnData_medClass.Code != 200)
            {
                returnData_medClass.Data = medClasses;
                return returnData_medClass.JsonSerializationt(true);
            }
            string jsonString = medClasses.JsonSerializationt();
            return jsonString;
        }
       
        
    }


}
