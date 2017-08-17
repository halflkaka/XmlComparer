using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSettingFactorty))]
    public class CXmlSettingFactImp : IXmlSettingFactorty
    {
        public IXmlSettingCollection ReadSettingCollection(string filePath)
        {

            if (!File.Exists(filePath))
            {
                CXmlSettingCollectionImp tmp = new CXmlSettingCollectionImp();
                WriteSettingCollection(tmp, filePath);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            try
            {
                IXmlSettingCollection result = (CXmlSettingCollectionImp)formatter.Deserialize(fs);
                fs.Close();
                return result as IXmlSettingCollection;
            }
            catch (Exception e)
            {
                fs.Close();
                throw e;
            }
        }
        public void WriteSettingCollection(IXmlSettingCollection instance, string filePath)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(fs, (CXmlSettingCollectionImp)instance);
                fs.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IXmlSetting InitFromXmlFile(string xmlType, string xmlFilePath, string userId)
        {
            CXmlSettingImp neoXmlSetting = new CXmlSettingImp();
            neoXmlSetting.file_type = xmlType;
            neoXmlSetting.update_time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            neoXmlSetting.updated_by = userId;
            //neoXmlSetting.GetChildRelation(xmlFilePath);
            //neoXmlSetting.GetParentRelation(xmlFilePath);

            XmlDocument NodeSettingDoc = new XmlDocument();
            try
            {
                NodeSettingDoc.Load(xmlFilePath);
                XmlNode root = NodeSettingDoc.DocumentElement;
                XmlNodeList listNode = root.SelectNodes("//*");

                List<string> lstDisctinctNodeNames = (from XmlNode tempNode in listNode select tempNode.Name).Distinct().ToList();
                foreach (string strNodeName in lstDisctinctNodeNames)
                {
                    CXmlSettingNodeImp neoNode = new CXmlSettingNodeImp();
                    neoNode.name = strNodeName;
                    neoNode.compare_flag = true;

                    List<string> lstDisctinctAttributeNames = new List<string>();
                    List<XmlAttributeCollection> lstAttrSet = (from XmlNode tmpXmlNode in listNode
                                                               where tmpXmlNode.Name == strNodeName
                                                               select tmpXmlNode.Attributes).ToList();
                    foreach (XmlAttributeCollection tempAttrSet in lstAttrSet)
                    {
                        foreach (XmlNode tempAttr in tempAttrSet)
                        {
                            lstDisctinctAttributeNames.Add(tempAttr.Name);
                        }
                    }
                    lstDisctinctAttributeNames = lstDisctinctAttributeNames.Distinct().ToList();


                    foreach (string strAttrName in lstDisctinctAttributeNames)
                    {
                        CXmlSettingAttrImp neoAttr = new CXmlSettingAttrImp();
                        neoAttr.name = strAttrName;
                        neoAttr.identifier_flag = false;
                        neoAttr.compare_flag = true;
                        neoNode.AppendAttribute(neoAttr);
                    }
                    neoXmlSetting.AppendNode(neoNode);
                }
                neoXmlSetting.GetParentRelation(xmlFilePath);
                neoXmlSetting.GetChildRelation(xmlFilePath);
                return neoXmlSetting;
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}
