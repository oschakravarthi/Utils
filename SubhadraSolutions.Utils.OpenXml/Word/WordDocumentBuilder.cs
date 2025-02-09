//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using SubhadraSolutions.Utils;
//using SubhadraSolutions.Utils.Data.Annotations;
//using SubhadraSolutions.Utils.Diagnostics;
//using SubhadraSolutions.Utils.Reflection;
//using SubhadraSolutions.Utils.Reporting;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;

//namespace SubhadraSolutions.Utils.OpenXml.Word
//{
//    public partial class WordDocumentBuilder : AbstractDisposable, IDocumentBuilder
//    {
//        private readonly MemoryStream memoryStream;
//        private readonly WordprocessingDocument wordDocument;
//        private bool hasAnyContentOnPage;

//        public WordDocumentBuilder()
//        {
//            memoryStream = new();
//            wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
//            WordHelper.CreateDefaultParts(wordDocument);
//            ApplyHeader();
//            ApplyFooter();
//        }

//        public string BasePath { get; set; }

//        public string Extension => "docx";
//        public string MimeType => "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

//        public IDocumentBuilder AddHeading(string heading, int fontSize, bool centerAligned = false, string textColor = "000080")
//        {
//            Paragraph p = WordHelper.CreateParagraph(wordDocument.MainDocumentPart, heading, centerAligned ? JustificationValues.Center : JustificationValues.Both, fontSize, textColor);
//            Body body = wordDocument.MainDocumentPart.Document.Body;
//            body.AppendChild(p);
//            hasAnyContentOnPage = true;

//            return this;
//        }

//        public IDocumentBuilder AddImage(Stream stream, string imageExtn)
//        {
//            ImagePartType imagePartType = WordHelper.GetImagePartType(imageExtn);
//            MainDocumentPart mainPart = wordDocument.MainDocumentPart;
//            ImagePart imagePart = mainPart.AddImagePart(imagePartType);
//            imagePart.FeedData(stream);
//            WordHelper.AddImageToBody(wordDocument, mainPart.GetIdOfPart(imagePart));
//            hasAnyContentOnPage = true;

//            return this;
//        }

//        public IDocumentBuilder AddLineBreak()
//        {
//            var p = new Paragraph();
//            Body body = wordDocument.MainDocumentPart.Document.Body;
//            body.AppendChild(p);
//            hasAnyContentOnPage = true;

//            return this;
//        }

//        public IDocumentBuilder AddPageBreak()
//        {
//            if (hasAnyContentOnPage)
//            {
//                var p = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
//                Body body = wordDocument.MainDocumentPart.Document.Body;
//                body.AppendChild(p);
//                hasAnyContentOnPage = false;
//            }

//            return this;
//        }

//        [DynamicallyInvoked]
//        public IDocumentBuilder AddTable<T>(IEnumerable<T> items, int fontSize = 18)
//        {
//            Body body = wordDocument.MainDocumentPart.Document.Body;
//            Table table = BuildListTable(items, fontSize);
//            body.Append(table);
//            hasAnyContentOnPage = true;

//            return this;
//        }

//        public void Build(Stream stream)
//        {
//            wordDocument.Dispose();
//            memoryStream.Seek(0, SeekOrigin.Begin);
//            memoryStream.CopyTo(stream);
//            stream.Flush();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            wordDocument.Dispose();
//            memoryStream.Dispose();
//        }

//        private static void AddTextToParagraph(Paragraph p, string text)
//        {
//            var rp = new RunProperties();
//            var t = new Text { Space = SpaceProcessingModeValues.Preserve, Text = text };
//            var r = new Run(rp, t);
//            p.Append(r);
//        }

//        private static Paragraph BuildBoldParagraph(string content, int fontSize, JustificationValues justification)
//        {
//            var rp = new RunProperties();

//            var fontSizeObj = new FontSize() { Val = fontSize.ToString() };
//            var fontSizeComplexScript = new FontSizeComplexScript() { Val = fontSize.ToString() };
//            rp.Append(fontSizeObj);
//            rp.Append(fontSizeComplexScript);

//            rp.Bold = new();
//            var r = new Run(rp);
//            r.Append(new Text(content));
//            var p = new Paragraph(r);

//            var pp = new ParagraphProperties();
//            var justification1 = new Justification() { Val = justification };
//            pp.Append(justification1);
//            p.Append(pp);

//            return p;
//        }

//        private static TableRow BuildListHeaderRow(IEnumerable<PropertyInfo> properties, int fontSize)
//        {
//            var tr = new TableRow();

//            foreach (PropertyInfo property in properties)
//            {
//                string title = property.GetMemberDisplayName(true);
//                TableCell tc = BuilTableHeaderCell(title, fontSize);
//                tr.Append(tc);
//            }

//            return tr;
//        }

//        private static TableCell BuilTableHeaderCell(string content, int fontSize)
//        {
//            var tc = new TableCell();
//            Paragraph p = BuildBoldParagraph(content, fontSize, JustificationValues.Center);
//            tc.Append(p);
//            return tc;
//        }

//        private void AddNavigations(string stringValue, List<NavigationAttribute> links, int fontSize, Paragraph p)
//        {
//            for (int j = 0; j < links.Count; j++)
//            {
//                string linkPath = string.Format(links[j].LinkTemplate, stringValue);

//                if (!linkPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
//                {
//                    linkPath = BasePath.TrimEnd('/') + "/" + linkPath.TrimStart('/');
//                }

//                var run1 = new Run();
//                var fieldChar1 = new FieldChar() { FieldCharType = FieldCharValues.Begin };

//                run1.Append(fieldChar1);

//                var run2 = new Run();
//                var fieldCode1 = new FieldCode() { Space = SpaceProcessingModeValues.Preserve };
//                fieldCode1.Text = string.Format(" HYPERLINK \"{0}\" ", linkPath);

//                run2.Append(fieldCode1);

//                var run3 = new Run();
//                var fieldChar2 = new FieldChar() { FieldCharType = FieldCharValues.Separate };

//                run3.Append(fieldChar2);
//                var proofError1 = new ProofError() { Type = ProofingErrorValues.SpellStart };

//                var run4 = new Run() { RsidRunProperties = "00A11BEE" };
//                var rp = new RunProperties();
//                var fontSizeObj = new FontSize() { Val = fontSize.ToString() };
//                var fontSizeComplexScript = new FontSizeComplexScript() { Val = fontSize.ToString() };
//                rp.Append(fontSizeObj);
//                rp.Append(fontSizeComplexScript);
//                if (j > 0)
//                {
//                    rp.Bold = new();
//                }

//                var runStyle1 = new RunStyle() { Val = "Hyperlink" };

//                rp.Append(runStyle1);
//                var text1 = new Text() { Space = SpaceProcessingModeValues.Preserve };

//                if (j == 0)
//                {
//                    text1.Text = stringValue;
//                }
//                else
//                {
//                    text1.Text = links[j].Name;

//                    if (string.IsNullOrEmpty(text1.Text))
//                    {
//                        text1.Text = "Link" + j.ToString();
//                    }

//                    text1.Text = "[" + text1.Text + "]";
//                }

//                run4.Append(rp);
//                run4.Append(text1);
//                var proofError2 = new ProofError() { Type = ProofingErrorValues.SpellEnd };

//                var run5 = new Run();
//                var fieldChar3 = new FieldChar() { FieldCharType = FieldCharValues.End };

//                run5.Append(fieldChar3);

//                p.Append(run1);
//                p.Append(run2);
//                p.Append(run3);
//                p.Append(proofError1);
//                p.Append(run4);
//                p.Append(proofError2);
//                p.Append(run5);

//                if (j < links.Count - 1)
//                {
//                    AddTextToParagraph(p, " ");
//                }
//            }
//        }

//        private void ApplyFooter()
//        {
//            MainDocumentPart mainDocPart = wordDocument.MainDocumentPart;

//            Paragraph p = WordHelper.CreateParagraph(mainDocPart, "Generated by GOSH-Reliability application.", JustificationValues.Center, 20, "0000FF");
//            const string id = "footer";

//            FooterPart footerPart = mainDocPart.AddNewPart<FooterPart>(id);
//            var footer = new Footer();
//            footer.Append(p);
//            footerPart.Footer = footer;
//            SectionProperties sectionProperties = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();

//            if (sectionProperties == null)
//            {
//                sectionProperties = new() { };
//                mainDocPart.Document.Body.Append(sectionProperties);
//            }

//            var footerReference = new FooterReference() { Type = HeaderFooterValues.Default, Id = id };
//            sectionProperties.InsertAt(footerReference, 0);
//        }

//        private void ApplyHeader()
//        {
//            MainDocumentPart mainDocPart = wordDocument.MainDocumentPart;

//            Paragraph p = WordHelper.CreateParagraph(mainDocPart, "***  Classified information. For Microsoft's internal use only.  ***", JustificationValues.Center, 20, "FF0000");
//            const string id = "header";

//            HeaderPart headerPart = mainDocPart.AddNewPart<HeaderPart>(id);
//            var header = new Header();
//            header.Append(p);
//            headerPart.Header = header;
//            SectionProperties sectionProperties = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();

//            if (sectionProperties == null)
//            {
//                sectionProperties = new() { };
//                mainDocPart.Document.Body.Append(sectionProperties);
//            }

//            var headerReference = new HeaderReference() { Type = HeaderFooterValues.Default, Id = id };
//            sectionProperties.InsertAt(headerReference, 0);
//        }

//        private Table BuildListTable<T>(IEnumerable<T> items, int fontSize)
//        {
//            var table = new Table();
//            var tableProperties = new TableProperties();
//            var tableStyle = new TableStyle() { Val = "TableGrid" };
//            var tableWidth = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
//            var tableLook = new TableLook() { Val = "04A0" };
//            tableProperties.Append(tableStyle);
//            tableProperties.Append(tableWidth);
//            tableProperties.Append(tableLook);
//            table.Append(tableProperties);

//            List<PropertyInfo> properties = ReflectionHelper.GetPublicProperties(typeof(T), true);
//            int propertiesCount = properties.Count;
//            var propertyValuesLookupIndex = new int[propertiesCount];

//            var nonListProperties = new List<PropertyInfo>();

//            var linksLookup = new List<NavigationAttribute>[propertiesCount];
//            var isListPropertyLookup = new bool[propertiesCount];
//            var propertyToArrayFuncs = new Func<T, IEnumerable<string>>[propertiesCount];

//            for (int i = 0, j = 0; i < propertiesCount; i++)
//            {
//                PropertyInfo property = properties[i];
//                Type propertyType = property.PropertyType;

//                bool isPropertyTypeEnumerable = false;

//                if (!propertyType.IsPrimitiveOrExtendedPrimitive() && ReflectionHelper.IsEnumerableType(propertyType))
//                {
//                    isPropertyTypeEnumerable = true;
//                }

//                if (!isPropertyTypeEnumerable)
//                {
//                    propertyValuesLookupIndex[i] = j;
//                    nonListProperties.Add(property);
//                    j++;
//                }
//                else
//                {
//                    propertyToArrayFuncs[i] = PropertiesToStringValuesHelper.BuildGetValueAsStringEnumerableFunc<T>(property);
//                }

//                isListPropertyLookup[i] = isPropertyTypeEnumerable;

//                List<NavigationAttribute> attribs = property.GetCustomAttributes<NavigationAttribute>().Where(x => x.LinkTemplate != null).ToList();

//                if (attribs.Count > 0)
//                {
//                    attribs.Sort(NavigationAttributeComparer.Instance);
//                    linksLookup[i] = attribs;
//                }
//            }

//            table.Append(BuildListHeaderRow(properties, fontSize));
//            Action<T, string[]> action = PropertiesToStringValuesHelper.BuildActionForToStringArray<T>(nonListProperties);
//            var stringValues = new string[propertiesCount];

//            foreach (T item in items)
//            {
//                action(item, stringValues);

//                var tr = new TableRow();

//                for (int i = 0; i < propertiesCount; i++)
//                {
//                    List<NavigationAttribute> links = linksLookup[i];
//                    var tc = new TableCell();

//                    var tcp = new TableCellProperties();
//                    var tcw = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Dxa };
//                    tcp.Append(tcw);
//                    tc.Append(tcp);
//                    var p = new Paragraph() { RsidParagraphAddition = "00A11BEE", RsidParagraphProperties = "00A11BEE", RsidRunAdditionDefault = "00A11BEE", ParagraphId = "67DE7A60", TextId = "38BB114B" };

//                    if (isListPropertyLookup[i])
//                    {
//                        IEnumerable<string> propertyValueAsStrings = propertyToArrayFuncs[i](item);
//                        int j = 0;

//                        foreach (string stringValue in propertyValueAsStrings)
//                        {
//                            if (links == null || string.IsNullOrEmpty(stringValue))
//                            {
//                                AddTextToParagraph(p, stringValue);
//                            }
//                            else
//                            {
//                                AddNavigations(stringValue, links, fontSize, p);
//                            }

//                            if (j > 0)
//                            {
//                                AddTextToParagraph(p, " ");
//                            }

//                            j++;
//                        }
//                    }
//                    else
//                    {
//                        string stringValue = stringValues[propertyValuesLookupIndex[i]];

//                        if (links == null || string.IsNullOrEmpty(stringValue))
//                        {
//                            AddTextToParagraph(p, stringValue);
//                        }
//                        else
//                        {
//                            AddNavigations(stringValue, links, fontSize, p);
//                        }
//                    }

//                    tc.Append(p);
//                    tr.Append(tc);
//                }

//                table.Append(tr);
//            }

//            return table;
//        }
//    }
//}