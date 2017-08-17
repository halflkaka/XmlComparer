using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Prism.Events;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;
using System.Windows;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlCompareCore))]
    public class CXmlCompareCoreImp : IXmlCompareCore
    {
        IEventAggregator _ea;
        private Dictionary<string, int> m_DuplicateKeyMemo;
        private IXmlCompareUserControl m_UserControl;
        private IXmlCompareToolSetting m_ToolSetting;
        private IXmlSettingFactorty m_SettingFactory;
       
        #region Private functions for Comparing Xml Files
        private void DomToNode(IXmlSetting currentSetting, XmlNode currentNode, ref CXmlFileNodeImp parent)
        {
            bool NodeCompareFlag = false;
            try
            {
                IXmlSettingNode settingNode = currentSetting.GetNode(currentNode.Name);
                if (settingNode != null)
                {
                    NodeCompareFlag = settingNode.compare_flag;
                }
            }
            catch (Exception e)
            {
                // do nothing
            }

            if (NodeCompareFlag)
            {
                CXmlFileNodeImp neoNode = new CXmlFileNodeImp(currentNode.Name, GetXPath(currentNode));

                string key = "";
                if (GetKeyForNode(currentNode, neoNode.name, currentSetting, out key))
                    neoNode.key = key;
                key = neoNode.name + "|" + neoNode.key;

                if (currentNode.NodeType == XmlNodeType.Text)
                {
                    neoNode.text = currentNode.Value;
                    parent.m_childNodes.Add(neoNode.key, neoNode);
                    neoNode.m_parentNodes.Add(parent.key, parent);
                    return;
                }
                foreach (XmlAttribute xmlAttr in currentNode.Attributes)
                {
                    List<string> listOfAttrName = new List<string>();
                    GetEffectiveAttributeNames(currentNode, neoNode.name, currentSetting, out listOfAttrName);
                    if (listOfAttrName.IndexOf(xmlAttr.Name) >= 0)
                    {
                        neoNode.m_attributes.Add(xmlAttr.Name, xmlAttr.Value);
                    }
                }
                foreach (XmlNode childNode in currentNode.ChildNodes)
                {
                    DomToNode(currentSetting, childNode, ref neoNode);
                }
                if (parent.m_childNodes.ContainsKey(key))
                {
                    // dealing with duplicate key, add numeric suffix to the key
                    if (m_DuplicateKeyMemo.ContainsKey(key))
                    {
                        m_DuplicateKeyMemo[key] = m_DuplicateKeyMemo[key] + 1;
                    }
                    else
                    {
                        m_DuplicateKeyMemo.Add(key, 1);
                    }
                    key = key + "." + m_DuplicateKeyMemo[key].ToString();
                }
                parent.m_childNodes.Add(key, neoNode);
                neoNode.m_parentNodes.Add(key, parent);
            }
        }
        private bool GetKeyForNode(XmlNode currentNode, string name, IXmlSetting currentSetting, out string key)
        {
            bool rc = true;
            key = "";

            try
            {
                IXmlSettingNode settingNode = currentSetting.GetNode(name);
                List<String> lstKeyAttrName = settingNode.identifier().Split(',').ToList<string>();
                if (lstKeyAttrName.Count > 0)
                {
                    List<string> tempKeys = new List<string>();
                    foreach (string attrNameNode in lstKeyAttrName)
                    {
                        string attrValue = currentNode.Attributes[attrNameNode].Value;
                        tempKeys.Add(attrValue);
                    }
                    key = String.Join("|", tempKeys.ToArray());
                }
            }
            catch (Exception e)
            {
                key = "";
                rc = false;
            }
            return rc;

        }

        private bool GetEffectiveAttributeNames(XmlNode currentNode, string name, IXmlSetting currentSetting, out List<string> AttrNameList)
        {
            bool rc = true;
            AttrNameList = new List<string>();
            try
            {
                IXmlSettingNode settingNode = currentSetting.GetNode(name);
                List<String> lstEffectiveAttrName = settingNode.attributesToCompare().Split(',').ToList<string>();
                AttrNameList.AddRange(lstEffectiveAttrName);
            }
            catch (Exception e)
            {
                rc = false;
            }
            return rc;

        }

        private string GetXPath(XmlNode currentNode)
        {
            string path = currentNode.Name;
            if (currentNode.ParentNode != null)
            {
                string tempPath = GetXPath(currentNode.ParentNode);
                if (tempPath != "")
                    path = tempPath + "/" + currentNode.Name;

            }
            return path;
        }

        private bool CompareXml(string fn1, string fn2, string xmlType)
        {
            XmlDocument xmlDoc1, xmlDoc2;
            try
            {
                 xmlDoc1 = new XmlDocument();
                 xmlDoc2 = new XmlDocument();
                 xmlDoc1.Load(fn1);
                 xmlDoc2.Load(fn2); ;
                 CXmlFileNodeImp nd1, nd2;
                 nd1 = new CXmlFileNodeImp(xmlType, fn1);
                 nd2 = new CXmlFileNodeImp(xmlType, fn2);

                 m_DuplicateKeyMemo.Clear();
                 IXmlSetting currentSetting =
                 m_SettingFactory.ReadSettingCollection(m_ToolSetting.GetXmlSettingFilePath().Value)
                     .GetSetting(xmlType);

                 DomToNode(currentSetting, xmlDoc1.DocumentElement, ref nd1);
                 m_DuplicateKeyMemo.Clear();
                 DomToNode(currentSetting, xmlDoc2.DocumentElement, ref nd2);

                 if (nd1 == nd2)
                    return true;
                 else
                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region  private functions to generate reports
        private void GenerateReport(string fn1, string fn2, string xmlType, string outputDir, bool showAlert)
        {
            CLog log = CLog.CreateInstance();
            List<CLogRec> logList = log.GetLogs();
            List<CSummaryRec> logSummary = log.GetSummary();
            string strReportName = "CmpReport_ " +
                                System.IO.Path.GetFileName(fn2).Split('.').First() + "_" +
                                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +
                                ".xlsx";
            string strLogName = "CmpLog_" +
                                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +
                                ".txt";
            if (logList.Count > 0)
            {
                List<IXmlSettingReportDictionary> dicList = new List<IXmlSettingReportDictionary>();
                IXmlSetting currentSetting =
                    m_SettingFactory.ReadSettingCollection(m_ToolSetting.GetXmlSettingFilePath().Value)
                    .GetSetting(xmlType);
                foreach (List<IXmlSettingReportDictionary> tmpList
                    in from IXmlSettingNode tmp
                    in currentSetting.GetNodes
                       select tmp.GetDictionarys)
                {
                    dicList.AddRange(tmpList);
                }

                Microsoft.Office.Interop.Excel.Application objExcel = null;
                Microsoft.Office.Interop.Excel.Workbook objWorkbook = null;
                Microsoft.Office.Interop.Excel.Worksheet objWorksheet = null;
                try
                {
                    objExcel = new Microsoft.Office.Interop.Excel.Application();

                    objExcel.Visible = false;
                    objExcel.SheetsInNewWorkbook = 3;
                    objWorkbook = objExcel.Workbooks.Add();

                    //write report issue dictionary
                    objWorksheet = objWorkbook.Worksheets[1];
                    objWorksheet.Name = "Report Dictionary";

                    string[] aTitle = new[] { "Issue Source",
                                                "Issue Category",
                                                "Issue Instruction" };
                    int intLineNo = 1;
                    List<CXmlSettingRptDicRep> listOfRptDic =
                        (from tmp in dicList
                         select new CXmlSettingRptDicRep
                         {
                             source = tmp.issue_source,
                             category = tmp.issue_category,
                             instruction = tmp.issue_instruction
                         }
                         ).ToList();
                    SaifeiAsm.ExcelHelper.WriteData(objWorksheet,
                        ref intLineNo,
                        SaifeiAsm.ExcelHelper.ObjectParseMethord.UsingFields,
                        listOfRptDic,
                        aTitle);
                    intLineNo = intLineNo + 2;
                    objWorksheet.Cells[intLineNo, 2] = "General Notes:";
                    foreach (string strGeneralNote in currentSetting.GeneralNotes)
                    {
                        objWorksheet.Cells[intLineNo, 3] = strGeneralNote;
                        intLineNo = intLineNo + 1;
                    }
                    //write report summary
                    objWorksheet = objWorkbook.Worksheets[2];
                    objWorksheet.Name = "Report Summary";
                    aTitle = new[] { "Section",
                                         "Attribute Name",
                                         "Count"};
                    intLineNo = 1;
                    SaifeiAsm.ExcelHelper.WriteData<CSummaryRec>(objWorksheet,
                        ref intLineNo,
                        SaifeiAsm.ExcelHelper.ObjectParseMethord.UsingFields,
                        logSummary,
                        aTitle);
                    //write report content
                    objWorksheet = objWorkbook.Worksheets[3];
                    objWorksheet.Name = "Compare Report";
                    aTitle = new[] { "TimeStamp",
                                         "Category",
                                         "Source",
                                         "Indentifier",
                                         "Path",
                                         "Attribute Name",
                                         "Old Attribute Name",
                                         "New Attribute Name" };
                    intLineNo = 1;
                    SaifeiAsm.ExcelHelper.WriteData<CLogRec>(objWorksheet,
                        ref intLineNo,
                        SaifeiAsm.ExcelHelper.ObjectParseMethord.UsingFields,
                        logList,
                        aTitle);
                    objWorkbook.SaveAs(outputDir + "\\" + strReportName);
                    objWorkbook.Close();
                    objExcel.Visible = true;
                    objExcel.Quit();
                    objWorksheet = null;
                    objWorkbook = null;
                    objExcel = null;
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Error happened: '"
                        + e.Message
                        + "' Report could not be generated.");
                }
                finally
                {
                    if (objWorksheet != null)
                        objWorksheet = null;
                    if (objWorkbook != null)
                    {
                        objWorkbook.Close();
                        objWorkbook = null;
                    }
                    if (objExcel != null)
                    {
                        objExcel.Quit();
                        objExcel = null;
                    }
                }
            }

            if (showAlert && logList.Count > 0)
                System.Windows.MessageBox.Show("Compare Report is Generated.\n" +
                "Please Check Folder '" + outputDir + "'.");
            else if (logList.Count > 0)
                RecordTextLog(outputDir + "\\" + strLogName,
                    "Differents is found between '" + fn1 + "' and '" + fn2
                    + "'. Please check compare report " + outputDir + "\\" + strReportName);
            else
                RecordTextLog(outputDir + "\\" + strLogName,
                    "Files '" + fn1 + "' and '" + fn2
                    + "' are identical per compare setting.");

        }
        private void RecordTextLog(string strTextLogFilePath, string strTextToRecord)
        {
            System.IO.StreamWriter textWriter = new StreamWriter(strTextLogFilePath, true);
            textWriter.WriteLine(strTextToRecord);
            textWriter.Flush();
            textWriter.Close();
        }
        #endregion

        [ImportingConstructor]
        public CXmlCompareCoreImp(IXmlCompareUserControl userControl,
            IXmlCompareToolSetting toolSetting,
            IXmlSettingFactorty settingFactory)
        {
            m_DuplicateKeyMemo = new Dictionary<string, int>();
            m_UserControl = userControl;
            m_ToolSetting = toolSetting;
            m_SettingFactory = settingFactory;
        }
       
        public void Compare(string iOriginalPath,
            string iNewPath,
            string iFileType,
            string iOutputPath,
            bool iShowAlert = false)
        {
            CLog log = CLog.CreateInstance();
            log.Flush();
            bool flag = CompareXml(iOriginalPath, iNewPath, iFileType);
            GenerateReport(iOriginalPath, iNewPath, iFileType, iOutputPath, iShowAlert);
        }
        public List<CFilePare> GetFilePare(string OldFileFolderDir, string NewFileFolderDir)
        {
            List<CFilePare> listResult = new List<CFilePare>();
            try
            {
                DirectoryInfo dirOfOldFiles = Directory.CreateDirectory(OldFileFolderDir);
                List<FileInfo> listOfOldFile = (from FileInfo file in dirOfOldFiles.GetFiles("*.xml") select file).ToList();
                DirectoryInfo dirOfNewFiles = Directory.CreateDirectory(NewFileFolderDir);
                List<FileInfo> listOfNewFile = (from FileInfo file in dirOfNewFiles.GetFiles("*.xml") select file).ToList();
                foreach (FileInfo newFile in listOfNewFile)
                {
                    CFilePare tmpFilePare = new CFilePare();
                    tmpFilePare.NewFile = newFile.FullName;
                    foreach (FileInfo oldFile in listOfOldFile)
                    {
                        if (newFile.Name == oldFile.Name)
                            tmpFilePare.OldFile = oldFile.FullName;
                    }
                    listResult.Add(tmpFilePare);
                }
            }
            catch
            {
                //do nothing
            }
            return listResult;
        }

        public bool Initialize(out IXmlSettingCollection xmlSettingCollection)
        {
            bool result = false;
            try
            {
                CUser user = m_UserControl.GetCurrentUser();
                result = m_UserControl.CheckUser(user.Id);
                if (!result)
                {
                    m_UserControl.CreateUser(user.Id, user.Name);
                    result = true;
                }
                xmlSettingCollection = m_SettingFactory.ReadSettingCollection(m_ToolSetting.GetXmlSettingFilePath().Value);
            }
            catch (Exception e)
            {
                result = false;
                xmlSettingCollection = null;
                System.Windows.MessageBox.Show(e.Message);
            }
            return result;
        }


        public void BatchCompare(List<CFilePare> iFilePares, string iFileType, string iOutputPath)
        {
            foreach (CFilePare tmpFilePare in iFilePares)
            {
                this.Compare(tmpFilePare.OldFile, tmpFilePare.NewFile, iFileType, iOutputPath, false);
            }
        }
    }
}
