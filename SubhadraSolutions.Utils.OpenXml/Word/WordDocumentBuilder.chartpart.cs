//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using SubhadraSolutions.Utils;
//using SubhadraSolutions.Utils.Data;
//using SubhadraSolutions.Utils.Drawing;
//using SubhadraSolutions.Utils.Reflection;
//using SubhadraSolutions.Utils.Reporting;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using A = DocumentFormat.OpenXml.Drawing;
//using C = DocumentFormat.OpenXml.Drawing.Charts;
//using C14 = DocumentFormat.OpenXml.Office2010.Drawing.Charts;
//using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

//namespace SubhadraSolutions.Utils.OpenXml.Word
//{
//    public partial class WordDocumentBuilder
//    {
//        public IDocumentBuilder AddChart<T>(IEnumerable<T> data, string chartTitle = null)
//        {
//            MainDocumentPart mainDocumentPart1 = wordDocument.MainDocumentPart;
//            string chartReference = "chart" + GeneralHelper.Identity.ToString();
//            ChartPart chartPart1 = mainDocumentPart1.AddNewPart<ChartPart>(chartReference);

//            int itemsCount = 0;
//            EmbeddedPackagePart embeddedPackagePart1 = chartPart1.AddNewPart<EmbeddedPackagePart>("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rId3");
//            // GenerateEmbeddedPackagePart1Content(embeddedPackagePart1);
//            using (var ms = new MemoryStream())
//            {
//                itemsCount = ExcelHelper.ExportToExcel(data, ms);
//                ms.Seek(0, SeekOrigin.Begin);
//                embeddedPackagePart1.FeedData(ms);
//            }

//            Build(chartPart1, chartTitle, data, itemsCount);

//            ChartColorStylePart chartColorStylePart1 = chartPart1.AddNewPart<ChartColorStylePart>("rId2");
//            GenerateChartColorStylePart1Content(chartColorStylePart1);

//            ChartStylePart chartStylePart1 = chartPart1.AddNewPart<ChartStylePart>("rId1");
//            GenerateChartStylePart1Content(chartStylePart1);

//            var paragraph1 = new Paragraph() { RsidParagraphAddition = "00C840FF", RsidRunAdditionDefault = "003D59B1", ParagraphId = "5E5CC248", TextId = "0863F1FE" };

//            var run1 = new Run();

//            var runProperties1 = new RunProperties();
//            var noProof1 = new NoProof();

//            runProperties1.Append(noProof1);

//            var drawing1 = new Drawing();

//            var inline1 = new Wp.Inline() { DistanceFromTop = 0U, DistanceFromBottom = 0U, DistanceFromLeft = 0U, DistanceFromRight = 0U, AnchorId = "7D4740DC", EditId = "319042D7" };
//            var extent1 = new Wp.Extent() { Cx = 5486400L, Cy = 3200400L };
//            var effectExtent1 = new Wp.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L };
//            var docProperties1 = new Wp.DocProperties() { Id = 1U, Name = "Chart 1" };
//            var nonVisualGraphicFrameDrawingProperties1 = new Wp.NonVisualGraphicFrameDrawingProperties();

//            var graphic1 = new A.Graphic();
//            graphic1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

//            var graphicData1 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" };

//            var chartReference1 = new C.ChartReference() { Id = chartReference };
//            chartReference1.AddNamespaceDeclaration("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
//            chartReference1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

//            graphicData1.Append(chartReference1);

//            graphic1.Append(graphicData1);

//            inline1.Append(extent1);
//            inline1.Append(effectExtent1);
//            inline1.Append(docProperties1);
//            inline1.Append(nonVisualGraphicFrameDrawingProperties1);
//            inline1.Append(graphic1);

//            drawing1.Append(inline1);

//            run1.Append(runProperties1);
//            run1.Append(drawing1);

//            paragraph1.Append(run1);

//            Body body = wordDocument.MainDocumentPart.Document.Body;
//            body.Append(paragraph1);

//            // ExtendedPart extendedPart1 = document.AddExtendedPart("http://schemas.microsoft.com/office/2020/02/relationships/classificationlabels", "application/vnd.ms-office.classificationlabels+xml", "xml",  chartReference);
//            // GenerateExtendedPart1Content(extendedPart1);

//            return this;
//        }

//        private static void Build<T>(ChartPart chartPart1, string chartTitle, IEnumerable<T> items, int itemsCount)
//        {
//            const string sheetName = "Sheet1";
//            List<System.Reflection.PropertyInfo> properties = ReflectionHelper.GetPublicProperties(typeof(T), true);
//            var dimensionsAndMeasures = DimensionsAndMeasuresHelper.GetDimensionsAndMeasuresProperties(typeof(T));
//            var dimensions = dimensionsAndMeasures.Key.ToList();
//            var measures = dimensionsAndMeasures.Value.ToList();
//            int measuresCount = measures.Count;
//            var measureIndexes = new int[measuresCount];

//            for (int i = 0; i < measuresCount; i++)
//            {
//                measureIndexes[i] = properties.IndexOf(measures[i]);
//            }

//            int dimensionsCount = dimensions.Count;
//            var dimensionIndexes = new int[dimensionsCount];

//            for (int i = 0; i < dimensionsCount; i++)
//            {
//                dimensionIndexes[i] = properties.IndexOf(dimensions[i]);
//            }

//            var series = new SeriesInfo[measuresCount];
//            var dimensionFormulaBuilder = new StringBuilder();

//            for (int i = 0; i < dimensionsCount; i++)
//            {
//                // TODO:
//                if (i > 0)
//                {
//                    dimensionFormulaBuilder.Append("&\"-\"&");
//                }

//                // dimensionFormula += ExcelHelper.GetReference(dimensionIndexes[i], 0, sheetName);
//                dimensionFormulaBuilder.Append(string.Format("{0}!{1}:{2}", sheetName, ExcelHelper.GetReference(dimensionIndexes[i], 1), ExcelHelper.GetReference(dimensionIndexes[i], itemsCount)));// "Sheet1!$B$2:$B$5";
//            }

//            List<string> pallette = ColorPalettes.GetPalette((uint)measuresCount);

//            var dimensionFormula = dimensionFormulaBuilder.ToString();
//            if (string.IsNullOrEmpty(dimensionFormula))
//            {
//                dimensionFormula = null;
//            }
//            for (int i = 0; i < measuresCount; i++)
//            {
//                string color = pallette[i][1..];
//                string seriesName = measures[i].GetMemberDisplayName(true);
//                series[i] = BuildChartSeries(sheetName, i, measureIndexes[i], seriesName, color, itemsCount, dimensionFormula);
//            }

//            System.Action<T, string[]> action = PropertiesToStringValuesHelper.BuildActionForToStringArray<T>(properties);
//            var stringValues = new string[properties.Count];
//            uint index = 0;

//            foreach (T item in items)
//            {
//                action(item, stringValues);

//                for (int i = 0; i < measuresCount; i++)
//                {
//                    var stringPoint = new C.StringPoint() { Index = index };
//                    var stringPointNumericValue = new C.NumericValue
//                    {
//                        Text = stringValues[dimensionIndexes[0]]//TODO: for multiple dimensions
//                    };
//                    stringPoint.Append(stringPointNumericValue);
//                    series[i].StringCache.Append(stringPoint);

//                    var numericPoint = new C.NumericPoint() { Index = index };
//                    var numericValue = new C.NumericValue
//                    {
//                        Text = stringValues[measureIndexes[i]]
//                    };
//                    numericPoint.Append(numericValue);
//                    series[i].NumberingCache.Append(numericPoint);
//                }

//                index++;
//            }

//            GenerateChartPart1Content(chartPart1, chartTitle, series.Select(s => s.Series).ToArray());
//        }

//        private static SeriesInfo BuildChartSeries(string sheetName, int seriesIndex, int seriesColumnIndex, string seriesName, string color, int itemsCount, string dimensionFormula)
//        {
//            var chartSeries = new C.BarChartSeries();
//            // var chartSeries = new C.PieChartSeries();
//            var index1 = new C.Index() { Val = (uint)seriesIndex };
//            var order1 = new C.Order() { Val = (uint)seriesIndex };

//            var seriesText1 = new C.SeriesText();

//            var stringReference1 = new C.StringReference();
//            var formula1 = new C.Formula
//            {
//                Text = ExcelHelper.GetReference(seriesColumnIndex, 0, sheetName)// "Sheet1!$B$1";
//            };

//            var stringCache1 = new C.StringCache();
//            var pointCount1 = new C.PointCount() { Val = 1U };

//            var stringPoint1 = new C.StringPoint() { Index = 0U };
//            var numericValue1 = new C.NumericValue
//            {
//                Text = seriesName
//            };

//            stringPoint1.Append(numericValue1);

//            stringCache1.Append(pointCount1);
//            stringCache1.Append(stringPoint1);

//            stringReference1.Append(formula1);
//            stringReference1.Append(stringCache1);

//            seriesText1.Append(stringReference1);

//            var chartShapeProperties = new C.ChartShapeProperties();

//            var solidFill8 = new A.SolidFill();
//            // var schemeColor17 = new A.SchemeColor() { Val = A.SchemeColorValues.Accent1 };
//            // solidFill8.Append(schemeColor17);

//            var rgbColorModelHex = new A.RgbColorModelHex() { Val = color };
//            solidFill8.Append(rgbColorModelHex);

//            var outline5 = new A.Outline();
//            var noFill3 = new A.NoFill();

//            outline5.Append(noFill3);
//            var effectList5 = new A.EffectList();

//            chartShapeProperties.Append(solidFill8);
//            chartShapeProperties.Append(outline5);
//            chartShapeProperties.Append(effectList5);
//            var invertIfNegative = new C.InvertIfNegative() { Val = false };

//            var categoryAxisData = new C.CategoryAxisData();

//            var stringReference2 = new C.StringReference();
//            var formula2 = new C.Formula
//            {
//                Text = dimensionFormula //"Sheet1!$A$2:$A$5";
//            };

//            var stringCache = new C.StringCache();
//            var pointCount2 = new C.PointCount() { Val = (uint)itemsCount };
//            stringCache.Append(pointCount2);

//            var values = new C.Values();

//            var numberReference1 = new C.NumberReference();
//            var formula3 = new C.Formula
//            {
//                Text = string.Format("{0}!{1}:{2}", sheetName, ExcelHelper.GetReference(seriesColumnIndex, 1), ExcelHelper.GetReference(seriesColumnIndex, itemsCount))// "Sheet1!$B$2:$B$5";
//            };

//            var numberingCache = new C.NumberingCache();
//            var formatCode1 = new C.FormatCode
//            {
//                Text = "General"
//            };
//            var pointCount3 = new C.PointCount() { Val = (uint)itemsCount };

//            numberingCache.Append(formatCode1);
//            numberingCache.Append(pointCount3);

//            stringReference2.Append(formula2);
//            stringReference2.Append(stringCache);

//            categoryAxisData.Append(stringReference2);

//            numberReference1.Append(formula3);
//            numberReference1.Append(numberingCache);

//            values.Append(numberReference1);

//            var barSerExtensionList = new C.BarSerExtensionList();

//            var barSerExtension = new C.BarSerExtension() { Uri = "{C3380CC4-5D6E-409C-BE32-E72D297353CC}" };
//            barSerExtension.AddNamespaceDeclaration("c16", "http://schemas.microsoft.com/office/drawing/2014/chart");

//            OpenXmlUnknownElement openXmlUnknownElement = OpenXmlUnknownElement.CreateOpenXmlUnknownElement("<c16:uniqueId val=\"{00000000-B7C7-48C9-8FBF-E84FFF4495EE}\" xmlns:c16=\"http://schemas.microsoft.com/office/drawing/2014/chart\" />");

//            barSerExtension.Append(openXmlUnknownElement);

//            barSerExtensionList.Append(barSerExtension);

//            chartSeries.Append(index1);
//            chartSeries.Append(order1);
//            chartSeries.Append(seriesText1);
//            chartSeries.Append(chartShapeProperties);
//            chartSeries.Append(invertIfNegative);
//            chartSeries.Append(categoryAxisData);
//            chartSeries.Append(values);
//            chartSeries.Append(barSerExtensionList);

//            return new()
//            {
//                Series = chartSeries,
//                StringCache = stringCache,
//                NumberingCache = numberingCache
//            };
//        }

//        // Generates content of chartPart1.
//        private static void GenerateChartPart1Content(ChartPart chartPart1, string chartTitle, params OpenXmlCompositeElement[] series)
//        {
//            var chartSpace1 = new C.ChartSpace();
//            chartSpace1.AddNamespaceDeclaration("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
//            chartSpace1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
//            chartSpace1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
//            chartSpace1.AddNamespaceDeclaration("c16r2", "http://schemas.microsoft.com/office/drawing/2015/06/chart");
//            var date19041 = new C.Date1904() { Val = false };
//            var editingLanguage1 = new C.EditingLanguage() { Val = "en-US" };
//            var roundedCorners1 = new C.RoundedCorners() { Val = false };

//            var alternateContent1 = new AlternateContent();
//            alternateContent1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");

//            var alternateContentChoice1 = new AlternateContentChoice() { Requires = "c14" };
//            alternateContentChoice1.AddNamespaceDeclaration("c14", "http://schemas.microsoft.com/office/drawing/2007/8/2/chart");
//            var style5 = new C14.Style() { Val = 102 };

//            alternateContentChoice1.Append(style5);

//            var alternateContentFallback1 = new AlternateContentFallback();
//            var style6 = new C.Style() { Val = 2 };

//            alternateContentFallback1.Append(style6);

//            alternateContent1.Append(alternateContentChoice1);
//            alternateContent1.Append(alternateContentFallback1);

//            var chart1 = new C.Chart();

//            var title1 = new C.Title();

//            var chartText1 = new C.ChartText();

//            var richText1 = new C.RichText();
//            var bodyProperties1 = new A.BodyProperties() { Rotation = 0, UseParagraphSpacing = true, VerticalOverflow = A.TextVerticalOverflowValues.Ellipsis, Vertical = A.TextVerticalValues.Horizontal, Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center, AnchorCenter = true };
//            var listStyle1 = new A.ListStyle();

//            var paragraph2 = new A.Paragraph();

//            var paragraphProperties1 = new A.ParagraphProperties();

//            var defaultRunProperties1 = new A.DefaultRunProperties() { FontSize = 1400, Bold = false, Italic = false, Underline = A.TextUnderlineValues.None, Strike = A.TextStrikeValues.NoStrike, Kerning = 1200, Spacing = 0, Baseline = 0 };

//            var solidFill7 = new A.SolidFill();

//            var schemeColor16 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation9 = new A.LuminanceModulation() { Val = 65000 };
//            var luminanceOffset1 = new A.LuminanceOffset() { Val = 35000 };

//            schemeColor16.Append(luminanceModulation9);
//            schemeColor16.Append(luminanceOffset1);

//            solidFill7.Append(schemeColor16);
//            var latinFont3 = new A.LatinFont() { Typeface = "+mn-lt" };
//            var eastAsianFont3 = new A.EastAsianFont() { Typeface = "+mn-ea" };
//            var complexScriptFont3 = new A.ComplexScriptFont() { Typeface = "+mn-cs" };

//            defaultRunProperties1.Append(solidFill7);
//            defaultRunProperties1.Append(latinFont3);
//            defaultRunProperties1.Append(eastAsianFont3);
//            defaultRunProperties1.Append(complexScriptFont3);

//            paragraphProperties1.Append(defaultRunProperties1);

//            var run2 = new A.Run();
//            var runProperties2 = new A.RunProperties() { Language = "en-IN" };
//            var text1 = new A.Text
//            {
//                Text = chartTitle
//            };

//            run2.Append(runProperties2);
//            run2.Append(text1);

//            paragraph2.Append(paragraphProperties1);
//            paragraph2.Append(run2);

//            richText1.Append(bodyProperties1);
//            richText1.Append(listStyle1);
//            richText1.Append(paragraph2);

//            chartText1.Append(richText1);
//            var overlay1 = new C.Overlay() { Val = false };

//            var chartShapeProperties1 = new C.ChartShapeProperties();
//            var noFill1 = new A.NoFill();

//            var outline4 = new A.Outline();
//            var noFill2 = new A.NoFill();

//            outline4.Append(noFill2);
//            var effectList4 = new A.EffectList();

//            chartShapeProperties1.Append(noFill1);
//            chartShapeProperties1.Append(outline4);
//            chartShapeProperties1.Append(effectList4);

//            var textProperties1 = new C.TextProperties();
//            var bodyProperties2 = new A.BodyProperties() { Rotation = 0, UseParagraphSpacing = true, VerticalOverflow = A.TextVerticalOverflowValues.Ellipsis, Vertical = A.TextVerticalValues.Horizontal, Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center, AnchorCenter = true };
//            var listStyle2 = new A.ListStyle();

//            var paragraph3 = new A.Paragraph();

//            var paragraphProperties2 = new A.ParagraphProperties();

//            var defaultRunProperties2 = new A.DefaultRunProperties() { FontSize = 1400, Bold = false, Italic = false, Underline = A.TextUnderlineValues.None, Strike = A.TextStrikeValues.NoStrike, Kerning = 1200, Spacing = 0, Baseline = 0 };

//            var solidFill8 = new A.SolidFill();

//            var schemeColor17 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation10 = new A.LuminanceModulation() { Val = 65000 };
//            var luminanceOffset2 = new A.LuminanceOffset() { Val = 35000 };

//            schemeColor17.Append(luminanceModulation10);
//            schemeColor17.Append(luminanceOffset2);

//            solidFill8.Append(schemeColor17);
//            var latinFont4 = new A.LatinFont() { Typeface = "+mn-lt" };
//            var eastAsianFont4 = new A.EastAsianFont() { Typeface = "+mn-ea" };
//            var complexScriptFont4 = new A.ComplexScriptFont() { Typeface = "+mn-cs" };

//            defaultRunProperties2.Append(solidFill8);
//            defaultRunProperties2.Append(latinFont4);
//            defaultRunProperties2.Append(eastAsianFont4);
//            defaultRunProperties2.Append(complexScriptFont4);

//            paragraphProperties2.Append(defaultRunProperties2);
//            var endParagraphRunProperties1 = new A.EndParagraphRunProperties() { Language = "en-US" };

//            paragraph3.Append(paragraphProperties2);
//            paragraph3.Append(endParagraphRunProperties1);

//            textProperties1.Append(bodyProperties2);
//            textProperties1.Append(listStyle2);
//            textProperties1.Append(paragraph3);

//            title1.Append(chartText1);
//            title1.Append(overlay1);
//            title1.Append(chartShapeProperties1);
//            title1.Append(textProperties1);
//            var autoTitleDeleted1 = new C.AutoTitleDeleted() { Val = false };

//            var plotArea1 = new C.PlotArea();
//            var layout1 = new C.Layout();

//            var chart = new C.BarChart();
//            var direction = new C.BarDirection() { Val = C.BarDirectionValues.Column };
//            var grouping = new C.BarGrouping() { Val = C.BarGroupingValues.Clustered };

//            // var chart = new C.PieChart();

//            var varyColors1 = new C.VaryColors() { Val = false };

//            var dataLabels1 = new C.DataLabels();
//            var showLegendKey1 = new C.ShowLegendKey() { Val = false };
//            var showValue1 = new C.ShowValue() { Val = false };
//            var showCategoryName1 = new C.ShowCategoryName() { Val = false };
//            var showSeriesName1 = new C.ShowSeriesName() { Val = false };
//            var showPercent1 = new C.ShowPercent() { Val = false };
//            var showBubbleSize1 = new C.ShowBubbleSize() { Val = false };

//            dataLabels1.Append(showLegendKey1);
//            dataLabels1.Append(showValue1);
//            dataLabels1.Append(showCategoryName1);
//            dataLabels1.Append(showSeriesName1);
//            dataLabels1.Append(showPercent1);
//            dataLabels1.Append(showBubbleSize1);
//            var gapWidth1 = new C.GapWidth() { Val = (UInt16Value)219U };
//            var overlap1 = new C.Overlap() { Val = -27 };
//            var axisId1 = new C.AxisId() { Val = 2016340943U };
//            var axisId2 = new C.AxisId() { Val = 2016343023U };

//            chart.Append(direction);
//            chart.Append(grouping);
//            chart.Append(varyColors1);

//            for (int i = 0; i < series.Length; i++)
//            {
//                chart.Append(series[i]);
//            }

//            chart.Append(dataLabels1);
//            chart.Append(gapWidth1);
//            chart.Append(overlap1);
//            chart.Append(axisId1);
//            chart.Append(axisId2);

//            var categoryAxis1 = new C.CategoryAxis();
//            var axisId3 = new C.AxisId() { Val = 2016340943U };

//            var scaling1 = new C.Scaling();
//            var orientation1 = new C.Orientation() { Val = C.OrientationValues.MinMax };

//            scaling1.Append(orientation1);
//            var delete1 = new C.Delete() { Val = false };
//            var axisPosition1 = new C.AxisPosition() { Val = C.AxisPositionValues.Bottom };
//            var numberingFormat1 = new C.NumberingFormat() { FormatCode = "General", SourceLinked = true };
//            var majorTickMark1 = new C.MajorTickMark() { Val = C.TickMarkValues.None };
//            var minorTickMark1 = new C.MinorTickMark() { Val = C.TickMarkValues.None };
//            var tickLabelPosition1 = new C.TickLabelPosition() { Val = C.TickLabelPositionValues.NextTo };

//            var chartShapeProperties5 = new C.ChartShapeProperties();
//            var noFill6 = new A.NoFill();

//            var outline8 = new A.Outline() { Width = 9525, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

//            var solidFill12 = new A.SolidFill();

//            var schemeColor21 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation11 = new A.LuminanceModulation() { Val = 15000 };
//            var luminanceOffset3 = new A.LuminanceOffset() { Val = 85000 };

//            schemeColor21.Append(luminanceModulation11);
//            schemeColor21.Append(luminanceOffset3);

//            solidFill12.Append(schemeColor21);
//            var round1 = new A.Round();

//            outline8.Append(solidFill12);
//            outline8.Append(round1);
//            var effectList8 = new A.EffectList();

//            chartShapeProperties5.Append(noFill6);
//            chartShapeProperties5.Append(outline8);
//            chartShapeProperties5.Append(effectList8);

//            var textProperties2 = new C.TextProperties();
//            var bodyProperties3 = new A.BodyProperties() { Rotation = -60000000, UseParagraphSpacing = true, VerticalOverflow = A.TextVerticalOverflowValues.Ellipsis, Vertical = A.TextVerticalValues.Horizontal, Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center, AnchorCenter = true };
//            var listStyle3 = new A.ListStyle();

//            var paragraph4 = new A.Paragraph();

//            var paragraphProperties3 = new A.ParagraphProperties();

//            var defaultRunProperties3 = new A.DefaultRunProperties() { FontSize = 900, Bold = false, Italic = false, Underline = A.TextUnderlineValues.None, Strike = A.TextStrikeValues.NoStrike, Kerning = 1200, Baseline = 0 };

//            var solidFill13 = new A.SolidFill();

//            var schemeColor22 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation12 = new A.LuminanceModulation() { Val = 65000 };
//            var luminanceOffset4 = new A.LuminanceOffset() { Val = 35000 };

//            schemeColor22.Append(luminanceModulation12);
//            schemeColor22.Append(luminanceOffset4);

//            solidFill13.Append(schemeColor22);
//            var latinFont5 = new A.LatinFont() { Typeface = "+mn-lt" };
//            var eastAsianFont5 = new A.EastAsianFont() { Typeface = "+mn-ea" };
//            var complexScriptFont5 = new A.ComplexScriptFont() { Typeface = "+mn-cs" };

//            defaultRunProperties3.Append(solidFill13);
//            defaultRunProperties3.Append(latinFont5);
//            defaultRunProperties3.Append(eastAsianFont5);
//            defaultRunProperties3.Append(complexScriptFont5);

//            paragraphProperties3.Append(defaultRunProperties3);
//            var endParagraphRunProperties2 = new A.EndParagraphRunProperties() { Language = "en-US" };

//            paragraph4.Append(paragraphProperties3);
//            paragraph4.Append(endParagraphRunProperties2);

//            textProperties2.Append(bodyProperties3);
//            textProperties2.Append(listStyle3);
//            textProperties2.Append(paragraph4);
//            var crossingAxis1 = new C.CrossingAxis() { Val = 2016343023U };
//            var crosses1 = new C.Crosses() { Val = C.CrossesValues.AutoZero };
//            var autoLabeled1 = new C.AutoLabeled() { Val = true };
//            var labelAlignment1 = new C.LabelAlignment() { Val = C.LabelAlignmentValues.Center };
//            var labelOffset1 = new C.LabelOffset() { Val = (UInt16Value)100U };
//            var noMultiLevelLabels1 = new C.NoMultiLevelLabels() { Val = false };

//            categoryAxis1.Append(axisId3);
//            categoryAxis1.Append(scaling1);
//            categoryAxis1.Append(delete1);
//            categoryAxis1.Append(axisPosition1);
//            categoryAxis1.Append(numberingFormat1);
//            categoryAxis1.Append(majorTickMark1);
//            categoryAxis1.Append(minorTickMark1);
//            categoryAxis1.Append(tickLabelPosition1);
//            categoryAxis1.Append(chartShapeProperties5);
//            categoryAxis1.Append(textProperties2);
//            categoryAxis1.Append(crossingAxis1);
//            categoryAxis1.Append(crosses1);
//            categoryAxis1.Append(autoLabeled1);
//            categoryAxis1.Append(labelAlignment1);
//            categoryAxis1.Append(labelOffset1);
//            categoryAxis1.Append(noMultiLevelLabels1);

//            var valueAxis1 = new C.ValueAxis();
//            var axisId4 = new C.AxisId() { Val = 2016343023U };

//            var scaling2 = new C.Scaling();
//            var orientation2 = new C.Orientation() { Val = C.OrientationValues.MinMax };

//            scaling2.Append(orientation2);
//            var delete2 = new C.Delete() { Val = false };
//            var axisPosition2 = new C.AxisPosition() { Val = C.AxisPositionValues.Left };

//            var majorGridlines1 = new C.MajorGridlines();

//            var chartShapeProperties6 = new C.ChartShapeProperties();

//            var outline9 = new A.Outline() { Width = 9525, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

//            var solidFill14 = new A.SolidFill();

//            var schemeColor23 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation13 = new A.LuminanceModulation() { Val = 15000 };
//            var luminanceOffset5 = new A.LuminanceOffset() { Val = 85000 };

//            schemeColor23.Append(luminanceModulation13);
//            schemeColor23.Append(luminanceOffset5);

//            solidFill14.Append(schemeColor23);
//            var round2 = new A.Round();

//            outline9.Append(solidFill14);
//            outline9.Append(round2);
//            var effectList9 = new A.EffectList();

//            chartShapeProperties6.Append(outline9);
//            chartShapeProperties6.Append(effectList9);

//            majorGridlines1.Append(chartShapeProperties6);
//            var numberingFormat2 = new C.NumberingFormat() { FormatCode = "General", SourceLinked = true };
//            var majorTickMark2 = new C.MajorTickMark() { Val = C.TickMarkValues.None };
//            var minorTickMark2 = new C.MinorTickMark() { Val = C.TickMarkValues.None };
//            var tickLabelPosition2 = new C.TickLabelPosition() { Val = C.TickLabelPositionValues.NextTo };

//            var chartShapeProperties7 = new C.ChartShapeProperties();
//            var noFill7 = new A.NoFill();

//            var outline10 = new A.Outline();
//            var noFill8 = new A.NoFill();

//            outline10.Append(noFill8);
//            var effectList10 = new A.EffectList();

//            chartShapeProperties7.Append(noFill7);
//            chartShapeProperties7.Append(outline10);
//            chartShapeProperties7.Append(effectList10);

//            var textProperties3 = new C.TextProperties();
//            var bodyProperties4 = new A.BodyProperties() { Rotation = -60000000, UseParagraphSpacing = true, VerticalOverflow = A.TextVerticalOverflowValues.Ellipsis, Vertical = A.TextVerticalValues.Horizontal, Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center, AnchorCenter = true };
//            var listStyle4 = new A.ListStyle();

//            var paragraph5 = new A.Paragraph();

//            var paragraphProperties4 = new A.ParagraphProperties();

//            var defaultRunProperties4 = new A.DefaultRunProperties() { FontSize = 900, Bold = false, Italic = false, Underline = A.TextUnderlineValues.None, Strike = A.TextStrikeValues.NoStrike, Kerning = 1200, Baseline = 0 };

//            var solidFill15 = new A.SolidFill();

//            var schemeColor24 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation14 = new A.LuminanceModulation() { Val = 65000 };
//            var luminanceOffset6 = new A.LuminanceOffset() { Val = 35000 };

//            schemeColor24.Append(luminanceModulation14);
//            schemeColor24.Append(luminanceOffset6);

//            solidFill15.Append(schemeColor24);
//            var latinFont6 = new A.LatinFont() { Typeface = "+mn-lt" };
//            var eastAsianFont6 = new A.EastAsianFont() { Typeface = "+mn-ea" };
//            var complexScriptFont6 = new A.ComplexScriptFont() { Typeface = "+mn-cs" };

//            defaultRunProperties4.Append(solidFill15);
//            defaultRunProperties4.Append(latinFont6);
//            defaultRunProperties4.Append(eastAsianFont6);
//            defaultRunProperties4.Append(complexScriptFont6);

//            paragraphProperties4.Append(defaultRunProperties4);
//            var endParagraphRunProperties3 = new A.EndParagraphRunProperties() { Language = "en-US" };

//            paragraph5.Append(paragraphProperties4);
//            paragraph5.Append(endParagraphRunProperties3);

//            textProperties3.Append(bodyProperties4);
//            textProperties3.Append(listStyle4);
//            textProperties3.Append(paragraph5);
//            var crossingAxis2 = new C.CrossingAxis() { Val = 2016340943U };
//            var crosses2 = new C.Crosses() { Val = C.CrossesValues.AutoZero };
//            var crossBetween1 = new C.CrossBetween() { Val = C.CrossBetweenValues.Between };

//            valueAxis1.Append(axisId4);
//            valueAxis1.Append(scaling2);
//            valueAxis1.Append(delete2);
//            valueAxis1.Append(axisPosition2);
//            valueAxis1.Append(majorGridlines1);
//            valueAxis1.Append(numberingFormat2);
//            valueAxis1.Append(majorTickMark2);
//            valueAxis1.Append(minorTickMark2);
//            valueAxis1.Append(tickLabelPosition2);
//            valueAxis1.Append(chartShapeProperties7);
//            valueAxis1.Append(textProperties3);
//            valueAxis1.Append(crossingAxis2);
//            valueAxis1.Append(crosses2);
//            valueAxis1.Append(crossBetween1);

//            var shapeProperties1 = new C.ShapeProperties();
//            var noFill9 = new A.NoFill();

//            var outline11 = new A.Outline();
//            var noFill10 = new A.NoFill();

//            outline11.Append(noFill10);
//            var effectList11 = new A.EffectList();

//            shapeProperties1.Append(noFill9);
//            shapeProperties1.Append(outline11);
//            shapeProperties1.Append(effectList11);

//            plotArea1.Append(layout1);
//            plotArea1.Append(chart);
//            plotArea1.Append(categoryAxis1);
//            plotArea1.Append(valueAxis1);
//            plotArea1.Append(shapeProperties1);

//            var legend1 = new C.Legend();
//            var legendPosition1 = new C.LegendPosition() { Val = C.LegendPositionValues.Bottom };
//            var overlay2 = new C.Overlay() { Val = false };

//            var chartShapeProperties8 = new C.ChartShapeProperties();
//            var noFill11 = new A.NoFill();

//            var outline12 = new A.Outline();
//            var noFill12 = new A.NoFill();

//            outline12.Append(noFill12);
//            var effectList12 = new A.EffectList();

//            chartShapeProperties8.Append(noFill11);
//            chartShapeProperties8.Append(outline12);
//            chartShapeProperties8.Append(effectList12);

//            var textProperties4 = new C.TextProperties();
//            var bodyProperties5 = new A.BodyProperties() { Rotation = 0, UseParagraphSpacing = true, VerticalOverflow = A.TextVerticalOverflowValues.Ellipsis, Vertical = A.TextVerticalValues.Horizontal, Wrap = A.TextWrappingValues.Square, Anchor = A.TextAnchoringTypeValues.Center, AnchorCenter = true };
//            var listStyle5 = new A.ListStyle();

//            var paragraph6 = new A.Paragraph();

//            var paragraphProperties5 = new A.ParagraphProperties();

//            var defaultRunProperties5 = new A.DefaultRunProperties() { FontSize = 900, Bold = false, Italic = false, Underline = A.TextUnderlineValues.None, Strike = A.TextStrikeValues.NoStrike, Kerning = 1200, Baseline = 0 };

//            var solidFill16 = new A.SolidFill();

//            var schemeColor25 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation15 = new A.LuminanceModulation() { Val = 65000 };
//            var luminanceOffset7 = new A.LuminanceOffset() { Val = 35000 };

//            schemeColor25.Append(luminanceModulation15);
//            schemeColor25.Append(luminanceOffset7);

//            solidFill16.Append(schemeColor25);
//            var latinFont7 = new A.LatinFont() { Typeface = "+mn-lt" };
//            var eastAsianFont7 = new A.EastAsianFont() { Typeface = "+mn-ea" };
//            var complexScriptFont7 = new A.ComplexScriptFont() { Typeface = "+mn-cs" };

//            defaultRunProperties5.Append(solidFill16);
//            defaultRunProperties5.Append(latinFont7);
//            defaultRunProperties5.Append(eastAsianFont7);
//            defaultRunProperties5.Append(complexScriptFont7);

//            paragraphProperties5.Append(defaultRunProperties5);
//            var endParagraphRunProperties4 = new A.EndParagraphRunProperties() { Language = "en-US" };

//            paragraph6.Append(paragraphProperties5);
//            paragraph6.Append(endParagraphRunProperties4);

//            textProperties4.Append(bodyProperties5);
//            textProperties4.Append(listStyle5);
//            textProperties4.Append(paragraph6);

//            legend1.Append(legendPosition1);
//            legend1.Append(overlay2);
//            legend1.Append(chartShapeProperties8);
//            legend1.Append(textProperties4);
//            var plotVisibleOnly1 = new C.PlotVisibleOnly() { Val = true };
//            var displayBlanksAs1 = new C.DisplayBlanksAs() { Val = C.DisplayBlanksAsValues.Gap };

//            var extensionList1 = new C.ExtensionList();

//            var extension1 = new C.Extension() { Uri = "{56B9EC1D-385E-4148-901F-78D8002777C0}" };
//            extension1.AddNamespaceDeclaration("c16r3", "http://schemas.microsoft.com/office/drawing/2017/03/chart");

//            OpenXmlUnknownElement openXmlUnknownElement4 = OpenXmlUnknownElement.CreateOpenXmlUnknownElement("<c16r3:dataDisplayOptions16 xmlns:c16r3=\"http://schemas.microsoft.com/office/drawing/2017/03/chart\"><c16r3:dispNaAsBlank val=\"1\" /></c16r3:dataDisplayOptions16>");

//            extension1.Append(openXmlUnknownElement4);

//            extensionList1.Append(extension1);
//            var showDataLabelsOverMaximum1 = new C.ShowDataLabelsOverMaximum() { Val = false };

//            chart1.Append(title1);
//            chart1.Append(autoTitleDeleted1);
//            chart1.Append(plotArea1);
//            chart1.Append(legend1);
//            chart1.Append(plotVisibleOnly1);
//            chart1.Append(displayBlanksAs1);
//            chart1.Append(extensionList1);
//            chart1.Append(showDataLabelsOverMaximum1);

//            var shapeProperties2 = new C.ShapeProperties();

//            var solidFill17 = new A.SolidFill();
//            var schemeColor26 = new A.SchemeColor() { Val = A.SchemeColorValues.Background1 };

//            solidFill17.Append(schemeColor26);

//            var outline13 = new A.Outline() { Width = 9525, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center };

//            var solidFill18 = new A.SolidFill();

//            var schemeColor27 = new A.SchemeColor() { Val = A.SchemeColorValues.Text1 };
//            var luminanceModulation16 = new A.LuminanceModulation() { Val = 15000 };
//            var luminanceOffset8 = new A.LuminanceOffset() { Val = 85000 };

//            schemeColor27.Append(luminanceModulation16);
//            schemeColor27.Append(luminanceOffset8);

//            solidFill18.Append(schemeColor27);
//            var round3 = new A.Round();

//            outline13.Append(solidFill18);
//            outline13.Append(round3);
//            var effectList13 = new A.EffectList();

//            shapeProperties2.Append(solidFill17);
//            shapeProperties2.Append(outline13);
//            shapeProperties2.Append(effectList13);

//            var textProperties5 = new C.TextProperties();
//            var bodyProperties6 = new A.BodyProperties();
//            var listStyle6 = new A.ListStyle();

//            var paragraph7 = new A.Paragraph();

//            var paragraphProperties6 = new A.ParagraphProperties();
//            var defaultRunProperties6 = new A.DefaultRunProperties();

//            paragraphProperties6.Append(defaultRunProperties6);
//            var endParagraphRunProperties5 = new A.EndParagraphRunProperties() { Language = "en-US" };

//            paragraph7.Append(paragraphProperties6);
//            paragraph7.Append(endParagraphRunProperties5);

//            textProperties5.Append(bodyProperties6);
//            textProperties5.Append(listStyle6);
//            textProperties5.Append(paragraph7);

//            var externalData1 = new C.ExternalData() { Id = "rId3" };
//            var autoUpdate1 = new C.AutoUpdate() { Val = false };

//            externalData1.Append(autoUpdate1);

//            chartSpace1.Append(date19041);
//            chartSpace1.Append(editingLanguage1);
//            chartSpace1.Append(roundedCorners1);
//            chartSpace1.Append(alternateContent1);
//            chartSpace1.Append(chart1);
//            chartSpace1.Append(shapeProperties2);
//            chartSpace1.Append(textProperties5);
//            chartSpace1.Append(externalData1);

//            chartPart1.ChartSpace = chartSpace1;
//        }

//        private sealed class SeriesInfo
//        {
//            public C.NumberingCache NumberingCache { get; set; }
//            public OpenXmlCompositeElement Series { get; set; }
//            public C.StringCache StringCache { get; set; }
//        }
//    }
//}