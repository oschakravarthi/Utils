using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Diagnostics.Tracing;
using SubhadraSolutions.Utils.Globalization;
using SubhadraSolutions.Utils.IO;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SubhadraSolutions.Utils.Xml;

public static class XmlHelper
{
    private const int BytesPerMegabyte = 1048576;

    private const int LargeXmlFileSize = 2097152;

    private static readonly Dictionary<string, object> _lock_xmlFiles = new(StringComparer.OrdinalIgnoreCase);

    public static string BeautifyXml(string xml, bool omitXmlHeader = false, bool noNewSurroundingElement = false)
    {
        var name = ActiveCode.Current.Name;
        var empty = string.Empty;
        if (!string.IsNullOrEmpty(xml) && xml.Length > 2097152)
        {
            return $"[Large XML: {xml.Length / 1048576} MB]";
        }

        var xmlDocument = new XmlDocument();
        try
        {
            var xml2 = xml;
            if (!noNewSurroundingElement)
            {
                xml2 = string.Format(CultureInfo.InvariantCulture, "<Update>{0}</Update>", [xml]);
            }

            xmlDocument.LoadXml(xml2);
        }
        catch (XmlException ex)
        {
            TraceLogger.TraceWarning("{0}: Warning: Ignoring {1} from the following XML:\r\n{2}\r\n", name,
                ex.GetType().Name, xml);
            return empty;
        }

        return BeautifyXml(xmlDocument, omitXmlHeader);
    }

    public static string BeautifyXml(XmlDocument doc, bool omitXmlHeader = false)
    {
        var stringBuilder = new StringBuilder("");
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace,
            Encoding = Encoding.Unicode,
            OmitXmlDeclaration = omitXmlHeader
        };
        using (var w = XmlWriter.Create(stringBuilder, xmlWriterSettings))
        {
            doc.Save(w);
        }

        return stringBuilder.ToString();
    }

    public static IEnumerable<XmlElement> ChildElements(this XmlElement walker)
    {
        return walker.ChildNodes.OfType<XmlElement>();
    }

    public static IEnumerable<string> ChildNodeNames(this XmlElement walker)
    {
        return walker.ChildElements().Select(item => item.LocalName);
    }

    public static XElement GetElementByLocalName(this XElement element, string elementName,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (element == null)
        {
            return null;
        }

        return element.Nodes()
            .OfType<XElement>().FirstOrDefault(x => string.Equals(x.Name.LocalName, elementName, comparison));
    }

    public static string GetInnerText(this XElement element)
    {
        if (element == null)
        {
            return null;
        }

        var xText = element.FirstNode as XText;
        var textVal = xText.Value;
        return textVal;
    }

    public static string GetValueFromAttributeOrInnerNode(this XElement element, string name)
    {
        if (element == null)
        {
            return null;
        }

        var attribute = element.Attribute(name);
        if (attribute != null)
        {
            return attribute.Value;
        }

        attribute = element
            .Attributes().FirstOrDefault(x => string.Equals(x.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
        if (attribute != null)
        {
            return attribute.Value;
        }

        var innerNode = GetElementByLocalName(element, name);
        return GetInnerText(innerNode);
    }

    public static XNamespace GetXmlNamespace(XDocument xmlFile)
    {
        return xmlFile?.Root != null
            ? xmlFile.Root
                .Attributes()
                .First(a => a.IsNamespaceDeclaration)
                .Value
            : null;
    }

    public static bool LoadFromEmbeddedResource<TConfigType>(out TConfigType objSettings, string resourceName,
        string resourceNamespace = null, Assembly fromAssembly = null) where TConfigType : new()
    {
        var name = ActiveCode.Current.Name;
        objSettings = default;
        TraceLogger.TraceWarning(name + ": Loading config for Type <{0}>.", typeof(TConfigType));
        var flag = false;
        using (var streamReader = FileHelper.OpenResourceForRead(resourceName, resourceNamespace, fromAssembly, name))
        {
            if (streamReader == null)
            {
                return flag;
            }

            var xmlSerializer = new XmlSerializer(typeof(TConfigType));
            xmlSerializer.UnknownNode += serializer_UnknownNode;
            xmlSerializer.UnknownAttribute += serializer_UnknownAttribute;
            try
            {
                TraceLogger.TraceWarning(name + ": Reading / deserializing data from file.");
                objSettings = (TConfigType)xmlSerializer.Deserialize(streamReader);
            }
            catch (InvalidOperationException ex)
            {
                TraceLogger.TraceWarning(
                    name +
                    ": Resource data uses stale datatype definition! (Hit {0}.) Will pass an empty object back to the caller.",
                    ex.GetType().Name);
                objSettings = new TConfigType();
                return flag;
            }

            flag = true;
            streamReader.Close();
        }

        TraceLogger.TraceWarning(name + ": Returning {0}.", flag);
        return flag;
    }

    public static bool LoadFromFile<TData>(out TData objToLoad, string nameFile = null, string labelFile = null,
        Action<TData, string> fnLoadAction = null) where TData : class
    {
        TData localObjToLoad = null;
        Action<string> fnFileAction = null;
        if (fnLoadAction != null)
        {
            fnFileAction = strFile => { fnLoadAction(localObjToLoad, strFile); };
        }

        var result = LoadOrSaveXmlFile(ref localObjToLoad, FileOperation.Load, nameFile, labelFile, fnFileAction);
        objToLoad = localObjToLoad;
        return result;
    }

    public static bool LoadFromXml<TConfigType>(out TConfigType objSettings, string nameFile)
    {
        var nameFn = ActiveCode.Current.Name;
        objSettings = default;
        TraceLogger.TraceWarning(nameFn + ": Loading config for Type <{0}>.", typeof(TConfigType));
        var retval = false;
        if (nameFile == null)
        {
            throw new ArgumentNullException(nameof(nameFile));
        }

        TextReader fileReader = null;
        object objLock = null;
        _lock_xmlFiles.ThreadSafeWrite(lockTable =>
        {
            if (!lockTable.TryGetValue(nameFile, out objLock))
            {
                lockTable[nameFile] = objLock = new object();
            }
        });
        var objSettingsLocal = objSettings;
        try
        {
            objLock.ThreadSafeRead(delegate
            {
                try
                {
                    TraceLogger.TraceWarning(nameFn + ": Opening config file \"{0}\".", nameFile);
                    fileReader = File.OpenText(nameFile);
                }
                catch (FileNotFoundException)
                {
                }
                catch (DirectoryNotFoundException)
                {
                }

                if (fileReader == null)
                {
                    TraceLogger.TraceWarning(nameFn + ": Couldn't open config file \"{0}\"!", nameFile);
                }
                else if (fileReader is StreamReader { EndOfStream: true })
                {
                    TraceLogger.TraceWarning(nameFn + ": Config file \"{0}\" is empty!", nameFile);
                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(TConfigType));
                    xmlSerializer.UnknownNode += serializer_UnknownNode;
                    xmlSerializer.UnknownAttribute += serializer_UnknownAttribute;
                    try
                    {
                        TraceLogger.TraceWarning(nameFn + ": Reading / deserializing data from file.");
                        objSettingsLocal = (TConfigType)xmlSerializer.Deserialize(fileReader);
                    }
                    catch (InvalidOperationException ex3)
                    {
                        TraceLogger.TraceWarning(nameFn + ": Handling stale config file. (Hit {0}.)",
                            ex3.GetType().Name);
                        objSettingsLocal = default;
                        var text = nameFile.Replace(".xml", ".old-version.xml");
                        try
                        {
                            File.Delete(text);
                        }
                        catch
                        {
                        }

                        try
                        {
                            File.Move(nameFile, text);
                        }
                        catch
                        {
                        }
                    }

                    if (objSettingsLocal != null)
                    {
                        retval = true;
                    }
                }
            });
        }
        finally
        {
            if (fileReader != null)
            {
                fileReader.Close();
                fileReader.Dispose();
                fileReader = null;
            }
        }

        objSettings = objSettingsLocal;
        TraceLogger.TraceWarning(nameFn + ": Returning {0}.", retval);
        return retval;
    }

    public static bool LoadOrSaveXmlFile<TData>(ref TData objData, FileOperation loadOrSave, string nameFile = null,
        string labelFile = null, Action<string> fnFileAction = null) where TData : class
    {
        var name = ActiveCode.Current.Name;
        var flag = false;
        bool? result = null;
        var flag2 = false;
        var localObjData = objData;
        if (objData == null && loadOrSave == FileOperation.Save)
        {
            throw new ArgumentNullException(nameof(objData));
        }

        if (nameFile == null)
        {
            throw new ArgumentNullException(nameof(nameFile));
        }

        if (labelFile == null)
        {
            labelFile = "data";
        }

        if (fnFileAction == null)
        {
            flag2 = true;
            fnFileAction = loadOrSave != FileOperation.Save
                ? strFile => { result = LoadFromXml(out localObjData, strFile); }
            : strFile => { SaveToXml(strFile, localObjData); };
        }

        var list = new List<string>();
        var directoryName = Path.GetDirectoryName(nameFile);
        if (!string.IsNullOrEmpty(directoryName))
        {
            list.Add(directoryName);
        }

        //TODO:
        string text = null; // ConfigurationManager.AppSettings[Path.GetFileName(nameFile)];
        if (!string.IsNullOrEmpty(text))
        {
            list.Add(Environment.ExpandEnvironmentVariables(text));
        }
        else
        {
            var array = new[]
            {
                ThisAssembly.Application,
                ThisAssembly.Current,
                Assembly.GetAssembly(typeof(TData))
            };
            foreach (var assembly in array)
            {
                if (assembly != null)
                {
                    list.Add(Path.GetDirectoryName(assembly.Location));
                }
            }

            list.Add(Directory.GetCurrentDirectory());
            list.Add(Path.GetTempPath());
        }

        nameFile = Path.GetFileName(nameFile);
        string text2 = null;
        foreach (var item in list.Distinct())
        {
            try
            {
                fnFileAction(Path.Combine(item, nameFile));
                flag = !result.HasValue || result.Value;
            }
            catch (Exception ex)
            {
                flag = false;
            }

            if (flag)
            {
                text2 = item;
                break;
            }
        }

        var text3 = "from";
        if (loadOrSave == FileOperation.Save)
        {
            text3 = "to";
        }

        if (text2 == null)
        {
            TraceLogger.TraceWarning("{0}: Warning: Couldn't {1} {2} {3} disk!", name, loadOrSave, labelFile, text3);
        }
        else
        {
            if (flag2)
            {
                objData = localObjData;
            }

            TraceLogger.TraceInformation("{0} {1} {2} \"{3}\".", labelFile.Capitalize(),
                loadOrSave.ToString().ToSimplePastTense(), text3, Path.Combine(text2, nameFile));
            flag = true;
        }

        return flag;
    }

    public static bool SaveToFile<TData>(TData objToSave, string nameFile = null, string labelFile = null,
        Action<string, TData> fnSaveAction = null) where TData : class
    {
        Action<string> fnFileAction = null;
        if (fnSaveAction != null)
        {
            fnFileAction = strFile => { fnSaveAction(strFile, objToSave); };
        }

        return LoadOrSaveXmlFile(ref objToSave, FileOperation.Save, nameFile, labelFile, fnFileAction);
    }

    public static void SaveToXml<TConfigType>(string nameFile, TConfigType objToSave)
    {
        var nameFn = ActiveCode.Current.Name;
        if (objToSave == null)
        {
            throw new ArgumentNullException(nameof(objToSave));
        }

        object objLock = null;
        _lock_xmlFiles.ThreadSafeWrite(lockTable =>
        {
            if (!lockTable.TryGetValue(nameFile, out objLock))
            {
                lockTable[nameFile] = objLock = new object();
            }
        });
        objLock.ThreadSafeWrite(delegate
        {
            try
            {
                using var textWriter = FileHelper.OpenFileForSave(nameFile, nameFn);
                if (textWriter == null)
                {
                    TraceLogger.TraceWarning(
                        nameFn +
                        ": Couldn't open configuration file \"{0}\" for writing; discarding any changes from runtime.",
                        nameFile);
                }
                else
                {
                    new XmlSerializer(objToSave.GetType()).Serialize(textWriter, objToSave);
                }
            }
            catch (Exception ex)
            {
                TraceLogger.TraceWarning(
                    nameFn +
                    ": Couldn't open configuration file \"{0}\" for writing; discarding any changes from runtime.  Hit {1}:\n{2}",
                    nameFile, ex.GetType().Name, ex);
            }
        });
    }

    private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
    {
        var attr = e.Attr;
        TraceLogger.TraceWarning("{0}: Unknown attribute: {1} = \"{2}\"", ActiveCode.Current.Name, attr.Name,
            attr.Value);
    }

    private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
    {
        TraceLogger.TraceWarning("{0}: Unknown Node: {1}\t{2}", ActiveCode.Current.Name, e.Name, e.Text);
    }
}