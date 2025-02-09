using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using A = DocumentFormat.OpenXml.Drawing;
using Cs = DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;

namespace SubhadraSolutions.Utils.OpenXml.Word;

public class WordDocumentBuilder
{
    // Generates content of chartColorStylePart1.
    private static void GenerateChartColorStylePart1Content(ChartColorStylePart chartColorStylePart1)
    {
        var colorStyle1 = new Cs.ColorStyle { Method = "cycle", Id = 10U };
        colorStyle1.AddNamespaceDeclaration("cs", "http://schemas.microsoft.com/office/drawing/2012/chakraStyle");
        colorStyle1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        var schemeColor27 = new A.SchemeColor { Val = A.SchemeColorValues.Accent1 };
        var schemeColor28 = new A.SchemeColor { Val = A.SchemeColorValues.Accent2 };
        var schemeColor29 = new A.SchemeColor { Val = A.SchemeColorValues.Accent3 };
        var schemeColor30 = new A.SchemeColor { Val = A.SchemeColorValues.Accent4 };
        var schemeColor31 = new A.SchemeColor { Val = A.SchemeColorValues.Accent5 };
        var schemeColor32 = new A.SchemeColor { Val = A.SchemeColorValues.Accent6 };
        var colorStyleVariation1 = new Cs.ColorStyleVariation();

        var colorStyleVariation2 = new Cs.ColorStyleVariation();
        var luminanceModulation16 = new A.LuminanceModulation { Val = 60000 };

        colorStyleVariation2.Append(luminanceModulation16);

        var colorStyleVariation3 = new Cs.ColorStyleVariation();
        var luminanceModulation17 = new A.LuminanceModulation { Val = 80000 };
        var luminanceOffset8 = new A.LuminanceOffset { Val = 20000 };

        colorStyleVariation3.Append(luminanceModulation17);
        colorStyleVariation3.Append(luminanceOffset8);

        var colorStyleVariation4 = new Cs.ColorStyleVariation();
        var luminanceModulation18 = new A.LuminanceModulation { Val = 80000 };

        colorStyleVariation4.Append(luminanceModulation18);

        var colorStyleVariation5 = new Cs.ColorStyleVariation();
        var luminanceModulation19 = new A.LuminanceModulation { Val = 60000 };
        var luminanceOffset9 = new A.LuminanceOffset { Val = 40000 };

        colorStyleVariation5.Append(luminanceModulation19);
        colorStyleVariation5.Append(luminanceOffset9);

        var colorStyleVariation6 = new Cs.ColorStyleVariation();
        var luminanceModulation20 = new A.LuminanceModulation { Val = 50000 };

        colorStyleVariation6.Append(luminanceModulation20);

        var colorStyleVariation7 = new Cs.ColorStyleVariation();
        var luminanceModulation21 = new A.LuminanceModulation { Val = 70000 };
        var luminanceOffset10 = new A.LuminanceOffset { Val = 30000 };

        colorStyleVariation7.Append(luminanceModulation21);
        colorStyleVariation7.Append(luminanceOffset10);

        var colorStyleVariation8 = new Cs.ColorStyleVariation();
        var luminanceModulation22 = new A.LuminanceModulation { Val = 70000 };

        colorStyleVariation8.Append(luminanceModulation22);

        var colorStyleVariation9 = new Cs.ColorStyleVariation();
        var luminanceModulation23 = new A.LuminanceModulation { Val = 50000 };
        var luminanceOffset11 = new A.LuminanceOffset { Val = 50000 };

        colorStyleVariation9.Append(luminanceModulation23);
        colorStyleVariation9.Append(luminanceOffset11);

        colorStyle1.Append(schemeColor27);
        colorStyle1.Append(schemeColor28);
        colorStyle1.Append(schemeColor29);
        colorStyle1.Append(schemeColor30);
        colorStyle1.Append(schemeColor31);
        colorStyle1.Append(schemeColor32);
        colorStyle1.Append(colorStyleVariation1);
        colorStyle1.Append(colorStyleVariation2);
        colorStyle1.Append(colorStyleVariation3);
        colorStyle1.Append(colorStyleVariation4);
        colorStyle1.Append(colorStyleVariation5);
        colorStyle1.Append(colorStyleVariation6);
        colorStyle1.Append(colorStyleVariation7);
        colorStyle1.Append(colorStyleVariation8);
        colorStyle1.Append(colorStyleVariation9);

        chartColorStylePart1.ColorStyle = colorStyle1;
    }

    // Generates content of chartStylePart1.
    private static void GenerateChartStylePart1Content(ChartStylePart chartStylePart1)
    {
        var chartStyle1 = new Cs.ChartStyle { Id = 201U };
        chartStyle1.AddNamespaceDeclaration("cs", "http://schemas.microsoft.com/office/drawing/2012/chakraStyle");
        chartStyle1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

        var axisTitle1 = new Cs.AxisTitle();
        var lineReference1 = new Cs.LineReference { Index = 0U };
        var fillReference1 = new Cs.FillReference { Index = 0U };
        var effectReference1 = new Cs.EffectReference { Index = 0U };

        var fontReference1 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor33 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation24 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset12 = new A.LuminanceOffset { Val = 35000 };

        schemeColor33.Append(luminanceModulation24);
        schemeColor33.Append(luminanceOffset12);

        fontReference1.Append(schemeColor33);
        var textCharacterPropertiesType1 = new Cs.TextCharacterPropertiesType { FontSize = 1000, Kerning = 1200 };

        axisTitle1.Append(lineReference1);
        axisTitle1.Append(fillReference1);
        axisTitle1.Append(effectReference1);
        axisTitle1.Append(fontReference1);
        axisTitle1.Append(textCharacterPropertiesType1);

        var categoryAxis2 = new Cs.CategoryAxis();
        var lineReference2 = new Cs.LineReference { Index = 0U };
        var fillReference2 = new Cs.FillReference { Index = 0U };
        var effectReference2 = new Cs.EffectReference { Index = 0U };

        var fontReference2 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor34 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation25 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset13 = new A.LuminanceOffset { Val = 35000 };

        schemeColor34.Append(luminanceModulation25);
        schemeColor34.Append(luminanceOffset13);

        fontReference2.Append(schemeColor34);

        var shapeProperties3 = new Cs.ShapeProperties();

        var outline14 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill18 = new A.SolidFill();

        var schemeColor35 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation26 = new A.LuminanceModulation { Val = 15000 };
        var luminanceOffset14 = new A.LuminanceOffset { Val = 85000 };

        schemeColor35.Append(luminanceModulation26);
        schemeColor35.Append(luminanceOffset14);

        solidFill18.Append(schemeColor35);
        var round4 = new A.Round();

        outline14.Append(solidFill18);
        outline14.Append(round4);

        shapeProperties3.Append(outline14);
        var textCharacterPropertiesType2 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        categoryAxis2.Append(lineReference2);
        categoryAxis2.Append(fillReference2);
        categoryAxis2.Append(effectReference2);
        categoryAxis2.Append(fontReference2);
        categoryAxis2.Append(shapeProperties3);
        categoryAxis2.Append(textCharacterPropertiesType2);

        var chartArea1 = new Cs.ChartArea
        { Modifiers = new ListValue<StringValue> { InnerText = "allowNoFillOverride allowNoLineOverride" } };
        var lineReference3 = new Cs.LineReference { Index = 0U };
        var fillReference3 = new Cs.FillReference { Index = 0U };
        var effectReference3 = new Cs.EffectReference { Index = 0U };

        var fontReference3 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor36 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference3.Append(schemeColor36);

        var shapeProperties4 = new Cs.ShapeProperties();

        var solidFill19 = new A.SolidFill();
        var schemeColor37 = new A.SchemeColor { Val = A.SchemeColorValues.Background1 };

        solidFill19.Append(schemeColor37);

        var outline15 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill20 = new A.SolidFill();

        var schemeColor38 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation27 = new A.LuminanceModulation { Val = 15000 };
        var luminanceOffset15 = new A.LuminanceOffset { Val = 85000 };

        schemeColor38.Append(luminanceModulation27);
        schemeColor38.Append(luminanceOffset15);

        solidFill20.Append(schemeColor38);
        var round5 = new A.Round();

        outline15.Append(solidFill20);
        outline15.Append(round5);

        shapeProperties4.Append(solidFill19);
        shapeProperties4.Append(outline15);
        var textCharacterPropertiesType3 = new Cs.TextCharacterPropertiesType { FontSize = 1000, Kerning = 1200 };

        chartArea1.Append(lineReference3);
        chartArea1.Append(fillReference3);
        chartArea1.Append(effectReference3);
        chartArea1.Append(fontReference3);
        chartArea1.Append(shapeProperties4);
        chartArea1.Append(textCharacterPropertiesType3);

        var dataLabel1 = new Cs.DataLabel();
        var lineReference4 = new Cs.LineReference { Index = 0U };
        var fillReference4 = new Cs.FillReference { Index = 0U };
        var effectReference4 = new Cs.EffectReference { Index = 0U };

        var fontReference4 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor39 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation28 = new A.LuminanceModulation { Val = 75000 };
        var luminanceOffset16 = new A.LuminanceOffset { Val = 25000 };

        schemeColor39.Append(luminanceModulation28);
        schemeColor39.Append(luminanceOffset16);

        fontReference4.Append(schemeColor39);
        var textCharacterPropertiesType4 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        dataLabel1.Append(lineReference4);
        dataLabel1.Append(fillReference4);
        dataLabel1.Append(effectReference4);
        dataLabel1.Append(fontReference4);
        dataLabel1.Append(textCharacterPropertiesType4);

        var dataLabelCallout1 = new Cs.DataLabelCallout();
        var lineReference5 = new Cs.LineReference { Index = 0U };
        var fillReference5 = new Cs.FillReference { Index = 0U };
        var effectReference5 = new Cs.EffectReference { Index = 0U };

        var fontReference5 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor40 = new A.SchemeColor { Val = A.SchemeColorValues.Dark1 };
        var luminanceModulation29 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset17 = new A.LuminanceOffset { Val = 35000 };

        schemeColor40.Append(luminanceModulation29);
        schemeColor40.Append(luminanceOffset17);

        fontReference5.Append(schemeColor40);

        var shapeProperties5 = new Cs.ShapeProperties();

        var solidFill21 = new A.SolidFill();
        var schemeColor41 = new A.SchemeColor { Val = A.SchemeColorValues.Light1 };

        solidFill21.Append(schemeColor41);

        var outline16 = new A.Outline();

        var solidFill22 = new A.SolidFill();

        var schemeColor42 = new A.SchemeColor { Val = A.SchemeColorValues.Dark1 };
        var luminanceModulation30 = new A.LuminanceModulation { Val = 25000 };
        var luminanceOffset18 = new A.LuminanceOffset { Val = 75000 };

        schemeColor42.Append(luminanceModulation30);
        schemeColor42.Append(luminanceOffset18);

        solidFill22.Append(schemeColor42);

        outline16.Append(solidFill22);

        shapeProperties5.Append(solidFill21);
        shapeProperties5.Append(outline16);
        var textCharacterPropertiesType5 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        var textBodyProperties1 = new Cs.TextBodyProperties
        {
            Rotation = 0,
            UseParagraphSpacing = true,
            VerticalOverflow = A.TextVerticalOverflowValues.Clip,
            HorizontalOverflow = A.TextHorizontalOverflowValues.Clip,
            Vertical = A.TextVerticalValues.Horizontal,
            Wrap = A.TextWrappingValues.Square,
            LeftInset = 36576,
            TopInset = 18288,
            RightInset = 36576,
            BottomInset = 18288,
            Anchor = A.TextAnchoringTypeValues.Center,
            AnchorCenter = true
        };
        var shapeAutoFit1 = new A.ShapeAutoFit();

        textBodyProperties1.Append(shapeAutoFit1);

        dataLabelCallout1.Append(lineReference5);
        dataLabelCallout1.Append(fillReference5);
        dataLabelCallout1.Append(effectReference5);
        dataLabelCallout1.Append(fontReference5);
        dataLabelCallout1.Append(shapeProperties5);
        dataLabelCallout1.Append(textCharacterPropertiesType5);
        dataLabelCallout1.Append(textBodyProperties1);

        var dataPoint1 = new Cs.DataPoint();
        var lineReference6 = new Cs.LineReference { Index = 0U };

        var fillReference6 = new Cs.FillReference { Index = 1U };
        var styleColor1 = new Cs.StyleColor { Val = "auto" };

        fillReference6.Append(styleColor1);
        var effectReference6 = new Cs.EffectReference { Index = 0U };

        var fontReference6 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor43 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference6.Append(schemeColor43);

        dataPoint1.Append(lineReference6);
        dataPoint1.Append(fillReference6);
        dataPoint1.Append(effectReference6);
        dataPoint1.Append(fontReference6);

        var dataPoint3D1 = new Cs.DataPoint3D();
        var lineReference7 = new Cs.LineReference { Index = 0U };

        var fillReference7 = new Cs.FillReference { Index = 1U };
        var styleColor2 = new Cs.StyleColor { Val = "auto" };

        fillReference7.Append(styleColor2);
        var effectReference7 = new Cs.EffectReference { Index = 0U };

        var fontReference7 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor44 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference7.Append(schemeColor44);

        dataPoint3D1.Append(lineReference7);
        dataPoint3D1.Append(fillReference7);
        dataPoint3D1.Append(effectReference7);
        dataPoint3D1.Append(fontReference7);

        var dataPointLine1 = new Cs.DataPointLine();

        var lineReference8 = new Cs.LineReference { Index = 0U };
        var styleColor3 = new Cs.StyleColor { Val = "auto" };

        lineReference8.Append(styleColor3);
        var fillReference8 = new Cs.FillReference { Index = 1U };
        var effectReference8 = new Cs.EffectReference { Index = 0U };

        var fontReference8 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor45 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference8.Append(schemeColor45);

        var shapeProperties6 = new Cs.ShapeProperties();

        var outline17 = new A.Outline { Width = 28575, CapType = A.LineCapValues.Round };

        var solidFill23 = new A.SolidFill();
        var schemeColor46 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill23.Append(schemeColor46);
        var round6 = new A.Round();

        outline17.Append(solidFill23);
        outline17.Append(round6);

        shapeProperties6.Append(outline17);

        dataPointLine1.Append(lineReference8);
        dataPointLine1.Append(fillReference8);
        dataPointLine1.Append(effectReference8);
        dataPointLine1.Append(fontReference8);
        dataPointLine1.Append(shapeProperties6);

        var dataPointMarker1 = new Cs.DataPointMarker();

        var lineReference9 = new Cs.LineReference { Index = 0U };
        var styleColor4 = new Cs.StyleColor { Val = "auto" };

        lineReference9.Append(styleColor4);

        var fillReference9 = new Cs.FillReference { Index = 1U };
        var styleColor5 = new Cs.StyleColor { Val = "auto" };

        fillReference9.Append(styleColor5);
        var effectReference9 = new Cs.EffectReference { Index = 0U };

        var fontReference9 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor47 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference9.Append(schemeColor47);

        var shapeProperties7 = new Cs.ShapeProperties();

        var outline18 = new A.Outline { Width = 9525 };

        var solidFill24 = new A.SolidFill();
        var schemeColor48 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill24.Append(schemeColor48);

        outline18.Append(solidFill24);

        shapeProperties7.Append(outline18);

        dataPointMarker1.Append(lineReference9);
        dataPointMarker1.Append(fillReference9);
        dataPointMarker1.Append(effectReference9);
        dataPointMarker1.Append(fontReference9);
        dataPointMarker1.Append(shapeProperties7);
        var markerLayoutProperties1 = new Cs.MarkerLayoutProperties { Symbol = Cs.MarkerStyle.Circle, Size = 5 };

        var dataPointWireframe1 = new Cs.DataPointWireframe();

        var lineReference10 = new Cs.LineReference { Index = 0U };
        var styleColor6 = new Cs.StyleColor { Val = "auto" };

        lineReference10.Append(styleColor6);
        var fillReference10 = new Cs.FillReference { Index = 1U };
        var effectReference10 = new Cs.EffectReference { Index = 0U };

        var fontReference10 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor49 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference10.Append(schemeColor49);

        var shapeProperties8 = new Cs.ShapeProperties();

        var outline19 = new A.Outline { Width = 9525, CapType = A.LineCapValues.Round };

        var solidFill25 = new A.SolidFill();
        var schemeColor50 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill25.Append(schemeColor50);
        var round7 = new A.Round();

        outline19.Append(solidFill25);
        outline19.Append(round7);

        shapeProperties8.Append(outline19);

        dataPointWireframe1.Append(lineReference10);
        dataPointWireframe1.Append(fillReference10);
        dataPointWireframe1.Append(effectReference10);
        dataPointWireframe1.Append(fontReference10);
        dataPointWireframe1.Append(shapeProperties8);

        var dataTableStyle1 = new Cs.DataTableStyle();
        var lineReference11 = new Cs.LineReference { Index = 0U };
        var fillReference11 = new Cs.FillReference { Index = 0U };
        var effectReference11 = new Cs.EffectReference { Index = 0U };

        var fontReference11 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor51 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation31 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset19 = new A.LuminanceOffset { Val = 35000 };

        schemeColor51.Append(luminanceModulation31);
        schemeColor51.Append(luminanceOffset19);

        fontReference11.Append(schemeColor51);

        var shapeProperties9 = new Cs.ShapeProperties();
        var noFill13 = new A.NoFill();

        var outline20 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill26 = new A.SolidFill();

        var schemeColor52 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation32 = new A.LuminanceModulation { Val = 15000 };
        var luminanceOffset20 = new A.LuminanceOffset { Val = 85000 };

        schemeColor52.Append(luminanceModulation32);
        schemeColor52.Append(luminanceOffset20);

        solidFill26.Append(schemeColor52);
        var round8 = new A.Round();

        outline20.Append(solidFill26);
        outline20.Append(round8);

        shapeProperties9.Append(noFill13);
        shapeProperties9.Append(outline20);
        var textCharacterPropertiesType6 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        dataTableStyle1.Append(lineReference11);
        dataTableStyle1.Append(fillReference11);
        dataTableStyle1.Append(effectReference11);
        dataTableStyle1.Append(fontReference11);
        dataTableStyle1.Append(shapeProperties9);
        dataTableStyle1.Append(textCharacterPropertiesType6);

        var downBar1 = new Cs.DownBar();
        var lineReference12 = new Cs.LineReference { Index = 0U };
        var fillReference12 = new Cs.FillReference { Index = 0U };
        var effectReference12 = new Cs.EffectReference { Index = 0U };

        var fontReference12 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor53 = new A.SchemeColor { Val = A.SchemeColorValues.Dark1 };

        fontReference12.Append(schemeColor53);

        var shapeProperties10 = new Cs.ShapeProperties();

        var solidFill27 = new A.SolidFill();

        var schemeColor54 = new A.SchemeColor { Val = A.SchemeColorValues.Dark1 };
        var luminanceModulation33 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset21 = new A.LuminanceOffset { Val = 35000 };

        schemeColor54.Append(luminanceModulation33);
        schemeColor54.Append(luminanceOffset21);

        solidFill27.Append(schemeColor54);

        var outline21 = new A.Outline { Width = 9525 };

        var solidFill28 = new A.SolidFill();

        var schemeColor55 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation34 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset22 = new A.LuminanceOffset { Val = 35000 };

        schemeColor55.Append(luminanceModulation34);
        schemeColor55.Append(luminanceOffset22);

        solidFill28.Append(schemeColor55);

        outline21.Append(solidFill28);

        shapeProperties10.Append(solidFill27);
        shapeProperties10.Append(outline21);

        downBar1.Append(lineReference12);
        downBar1.Append(fillReference12);
        downBar1.Append(effectReference12);
        downBar1.Append(fontReference12);
        downBar1.Append(shapeProperties10);

        var dropLine1 = new Cs.DropLine();
        var lineReference13 = new Cs.LineReference { Index = 0U };
        var fillReference13 = new Cs.FillReference { Index = 0U };
        var effectReference13 = new Cs.EffectReference { Index = 0U };

        var fontReference13 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor56 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference13.Append(schemeColor56);

        var shapeProperties11 = new Cs.ShapeProperties();

        var outline22 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill29 = new A.SolidFill();

        var schemeColor57 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation35 = new A.LuminanceModulation { Val = 35000 };
        var luminanceOffset23 = new A.LuminanceOffset { Val = 65000 };

        schemeColor57.Append(luminanceModulation35);
        schemeColor57.Append(luminanceOffset23);

        solidFill29.Append(schemeColor57);
        var round9 = new A.Round();

        outline22.Append(solidFill29);
        outline22.Append(round9);

        shapeProperties11.Append(outline22);

        dropLine1.Append(lineReference13);
        dropLine1.Append(fillReference13);
        dropLine1.Append(effectReference13);
        dropLine1.Append(fontReference13);
        dropLine1.Append(shapeProperties11);

        var errorBar1 = new Cs.ErrorBar();
        var lineReference14 = new Cs.LineReference { Index = 0U };
        var fillReference14 = new Cs.FillReference { Index = 0U };
        var effectReference14 = new Cs.EffectReference { Index = 0U };

        var fontReference14 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor58 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference14.Append(schemeColor58);

        var shapeProperties12 = new Cs.ShapeProperties();

        var outline23 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill30 = new A.SolidFill();

        var schemeColor59 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation36 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset24 = new A.LuminanceOffset { Val = 35000 };

        schemeColor59.Append(luminanceModulation36);
        schemeColor59.Append(luminanceOffset24);

        solidFill30.Append(schemeColor59);
        var round10 = new A.Round();

        outline23.Append(solidFill30);
        outline23.Append(round10);

        shapeProperties12.Append(outline23);

        errorBar1.Append(lineReference14);
        errorBar1.Append(fillReference14);
        errorBar1.Append(effectReference14);
        errorBar1.Append(fontReference14);
        errorBar1.Append(shapeProperties12);

        var floor1 = new Cs.Floor();
        var lineReference15 = new Cs.LineReference { Index = 0U };
        var fillReference15 = new Cs.FillReference { Index = 0U };
        var effectReference15 = new Cs.EffectReference { Index = 0U };

        var fontReference15 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor60 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference15.Append(schemeColor60);

        var shapeProperties13 = new Cs.ShapeProperties();
        var noFill14 = new A.NoFill();

        var outline24 = new A.Outline();
        var noFill15 = new A.NoFill();

        outline24.Append(noFill15);

        shapeProperties13.Append(noFill14);
        shapeProperties13.Append(outline24);

        floor1.Append(lineReference15);
        floor1.Append(fillReference15);
        floor1.Append(effectReference15);
        floor1.Append(fontReference15);
        floor1.Append(shapeProperties13);

        var gridlineMajor1 = new Cs.GridlineMajor();
        var lineReference16 = new Cs.LineReference { Index = 0U };
        var fillReference16 = new Cs.FillReference { Index = 0U };
        var effectReference16 = new Cs.EffectReference { Index = 0U };

        var fontReference16 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor61 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference16.Append(schemeColor61);

        var shapeProperties14 = new Cs.ShapeProperties();

        var outline25 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill31 = new A.SolidFill();

        var schemeColor62 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation37 = new A.LuminanceModulation { Val = 15000 };
        var luminanceOffset25 = new A.LuminanceOffset { Val = 85000 };

        schemeColor62.Append(luminanceModulation37);
        schemeColor62.Append(luminanceOffset25);

        solidFill31.Append(schemeColor62);
        var round11 = new A.Round();

        outline25.Append(solidFill31);
        outline25.Append(round11);

        shapeProperties14.Append(outline25);

        gridlineMajor1.Append(lineReference16);
        gridlineMajor1.Append(fillReference16);
        gridlineMajor1.Append(effectReference16);
        gridlineMajor1.Append(fontReference16);
        gridlineMajor1.Append(shapeProperties14);

        var gridlineMinor1 = new Cs.GridlineMinor();
        var lineReference17 = new Cs.LineReference { Index = 0U };
        var fillReference17 = new Cs.FillReference { Index = 0U };
        var effectReference17 = new Cs.EffectReference { Index = 0U };

        var fontReference17 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor63 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference17.Append(schemeColor63);

        var shapeProperties15 = new Cs.ShapeProperties();

        var outline26 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill32 = new A.SolidFill();

        var schemeColor64 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation38 = new A.LuminanceModulation { Val = 5000 };
        var luminanceOffset26 = new A.LuminanceOffset { Val = 95000 };

        schemeColor64.Append(luminanceModulation38);
        schemeColor64.Append(luminanceOffset26);

        solidFill32.Append(schemeColor64);
        var round12 = new A.Round();

        outline26.Append(solidFill32);
        outline26.Append(round12);

        shapeProperties15.Append(outline26);

        gridlineMinor1.Append(lineReference17);
        gridlineMinor1.Append(fillReference17);
        gridlineMinor1.Append(effectReference17);
        gridlineMinor1.Append(fontReference17);
        gridlineMinor1.Append(shapeProperties15);

        var hiLoLine1 = new Cs.HiLoLine();
        var lineReference18 = new Cs.LineReference { Index = 0U };
        var fillReference18 = new Cs.FillReference { Index = 0U };
        var effectReference18 = new Cs.EffectReference { Index = 0U };

        var fontReference18 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor65 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference18.Append(schemeColor65);

        var shapeProperties16 = new Cs.ShapeProperties();

        var outline27 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill33 = new A.SolidFill();

        var schemeColor66 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation39 = new A.LuminanceModulation { Val = 75000 };
        var luminanceOffset27 = new A.LuminanceOffset { Val = 25000 };

        schemeColor66.Append(luminanceModulation39);
        schemeColor66.Append(luminanceOffset27);

        solidFill33.Append(schemeColor66);
        var round13 = new A.Round();

        outline27.Append(solidFill33);
        outline27.Append(round13);

        shapeProperties16.Append(outline27);

        hiLoLine1.Append(lineReference18);
        hiLoLine1.Append(fillReference18);
        hiLoLine1.Append(effectReference18);
        hiLoLine1.Append(fontReference18);
        hiLoLine1.Append(shapeProperties16);

        var leaderLine1 = new Cs.LeaderLine();
        var lineReference19 = new Cs.LineReference { Index = 0U };
        var fillReference19 = new Cs.FillReference { Index = 0U };
        var effectReference19 = new Cs.EffectReference { Index = 0U };

        var fontReference19 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor67 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference19.Append(schemeColor67);

        var shapeProperties17 = new Cs.ShapeProperties();

        var outline28 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill34 = new A.SolidFill();

        var schemeColor68 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation40 = new A.LuminanceModulation { Val = 35000 };
        var luminanceOffset28 = new A.LuminanceOffset { Val = 65000 };

        schemeColor68.Append(luminanceModulation40);
        schemeColor68.Append(luminanceOffset28);

        solidFill34.Append(schemeColor68);
        var round14 = new A.Round();

        outline28.Append(solidFill34);
        outline28.Append(round14);

        shapeProperties17.Append(outline28);

        leaderLine1.Append(lineReference19);
        leaderLine1.Append(fillReference19);
        leaderLine1.Append(effectReference19);
        leaderLine1.Append(fontReference19);
        leaderLine1.Append(shapeProperties17);

        var legendStyle1 = new Cs.LegendStyle();
        var lineReference20 = new Cs.LineReference { Index = 0U };
        var fillReference20 = new Cs.FillReference { Index = 0U };
        var effectReference20 = new Cs.EffectReference { Index = 0U };

        var fontReference20 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor69 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation41 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset29 = new A.LuminanceOffset { Val = 35000 };

        schemeColor69.Append(luminanceModulation41);
        schemeColor69.Append(luminanceOffset29);

        fontReference20.Append(schemeColor69);
        var textCharacterPropertiesType7 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        legendStyle1.Append(lineReference20);
        legendStyle1.Append(fillReference20);
        legendStyle1.Append(effectReference20);
        legendStyle1.Append(fontReference20);
        legendStyle1.Append(textCharacterPropertiesType7);

        var plotArea2 = new Cs.PlotArea
        { Modifiers = new ListValue<StringValue> { InnerText = "allowNoFillOverride allowNoLineOverride" } };
        var lineReference21 = new Cs.LineReference { Index = 0U };
        var fillReference21 = new Cs.FillReference { Index = 0U };
        var effectReference21 = new Cs.EffectReference { Index = 0U };

        var fontReference21 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor70 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference21.Append(schemeColor70);

        plotArea2.Append(lineReference21);
        plotArea2.Append(fillReference21);
        plotArea2.Append(effectReference21);
        plotArea2.Append(fontReference21);

        var plotArea3D1 = new Cs.PlotArea3D
        { Modifiers = new ListValue<StringValue> { InnerText = "allowNoFillOverride allowNoLineOverride" } };
        var lineReference22 = new Cs.LineReference { Index = 0U };
        var fillReference22 = new Cs.FillReference { Index = 0U };
        var effectReference22 = new Cs.EffectReference { Index = 0U };

        var fontReference22 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor71 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference22.Append(schemeColor71);

        plotArea3D1.Append(lineReference22);
        plotArea3D1.Append(fillReference22);
        plotArea3D1.Append(effectReference22);
        plotArea3D1.Append(fontReference22);

        var seriesAxis1 = new Cs.SeriesAxis();
        var lineReference23 = new Cs.LineReference { Index = 0U };
        var fillReference23 = new Cs.FillReference { Index = 0U };
        var effectReference23 = new Cs.EffectReference { Index = 0U };

        var fontReference23 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor72 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation42 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset30 = new A.LuminanceOffset { Val = 35000 };

        schemeColor72.Append(luminanceModulation42);
        schemeColor72.Append(luminanceOffset30);

        fontReference23.Append(schemeColor72);
        var textCharacterPropertiesType8 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        seriesAxis1.Append(lineReference23);
        seriesAxis1.Append(fillReference23);
        seriesAxis1.Append(effectReference23);
        seriesAxis1.Append(fontReference23);
        seriesAxis1.Append(textCharacterPropertiesType8);

        var seriesLine1 = new Cs.SeriesLine();
        var lineReference24 = new Cs.LineReference { Index = 0U };
        var fillReference24 = new Cs.FillReference { Index = 0U };
        var effectReference24 = new Cs.EffectReference { Index = 0U };

        var fontReference24 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor73 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference24.Append(schemeColor73);

        var shapeProperties18 = new Cs.ShapeProperties();

        var outline29 = new A.Outline
        {
            Width = 9525,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill35 = new A.SolidFill();

        var schemeColor74 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation43 = new A.LuminanceModulation { Val = 35000 };
        var luminanceOffset31 = new A.LuminanceOffset { Val = 65000 };

        schemeColor74.Append(luminanceModulation43);
        schemeColor74.Append(luminanceOffset31);

        solidFill35.Append(schemeColor74);
        var round15 = new A.Round();

        outline29.Append(solidFill35);
        outline29.Append(round15);

        shapeProperties18.Append(outline29);

        seriesLine1.Append(lineReference24);
        seriesLine1.Append(fillReference24);
        seriesLine1.Append(effectReference24);
        seriesLine1.Append(fontReference24);
        seriesLine1.Append(shapeProperties18);

        var titleStyle1 = new Cs.TitleStyle();
        var lineReference25 = new Cs.LineReference { Index = 0U };
        var fillReference25 = new Cs.FillReference { Index = 0U };
        var effectReference25 = new Cs.EffectReference { Index = 0U };

        var fontReference25 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor75 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation44 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset32 = new A.LuminanceOffset { Val = 35000 };

        schemeColor75.Append(luminanceModulation44);
        schemeColor75.Append(luminanceOffset32);

        fontReference25.Append(schemeColor75);
        var textCharacterPropertiesType9 = new Cs.TextCharacterPropertiesType
        { FontSize = 1400, Bold = false, Kerning = 1200, Spacing = 0, Baseline = 0 };

        titleStyle1.Append(lineReference25);
        titleStyle1.Append(fillReference25);
        titleStyle1.Append(effectReference25);
        titleStyle1.Append(fontReference25);
        titleStyle1.Append(textCharacterPropertiesType9);

        var trendlineStyle1 = new Cs.TrendlineStyle();

        var lineReference26 = new Cs.LineReference { Index = 0U };
        var styleColor7 = new Cs.StyleColor { Val = "auto" };

        lineReference26.Append(styleColor7);
        var fillReference26 = new Cs.FillReference { Index = 0U };
        var effectReference26 = new Cs.EffectReference { Index = 0U };

        var fontReference26 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor76 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference26.Append(schemeColor76);

        var shapeProperties19 = new Cs.ShapeProperties();

        var outline30 = new A.Outline { Width = 19050, CapType = A.LineCapValues.Round };

        var solidFill36 = new A.SolidFill();
        var schemeColor77 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill36.Append(schemeColor77);
        var presetDash4 = new A.PresetDash { Val = A.PresetLineDashValues.SystemDot };

        outline30.Append(solidFill36);
        outline30.Append(presetDash4);

        shapeProperties19.Append(outline30);

        trendlineStyle1.Append(lineReference26);
        trendlineStyle1.Append(fillReference26);
        trendlineStyle1.Append(effectReference26);
        trendlineStyle1.Append(fontReference26);
        trendlineStyle1.Append(shapeProperties19);

        var trendlineLabel1 = new Cs.TrendlineLabel();
        var lineReference27 = new Cs.LineReference { Index = 0U };
        var fillReference27 = new Cs.FillReference { Index = 0U };
        var effectReference27 = new Cs.EffectReference { Index = 0U };

        var fontReference27 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor78 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation45 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset33 = new A.LuminanceOffset { Val = 35000 };

        schemeColor78.Append(luminanceModulation45);
        schemeColor78.Append(luminanceOffset33);

        fontReference27.Append(schemeColor78);
        var textCharacterPropertiesType10 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        trendlineLabel1.Append(lineReference27);
        trendlineLabel1.Append(fillReference27);
        trendlineLabel1.Append(effectReference27);
        trendlineLabel1.Append(fontReference27);
        trendlineLabel1.Append(textCharacterPropertiesType10);

        var upBar1 = new Cs.UpBar();
        var lineReference28 = new Cs.LineReference { Index = 0U };
        var fillReference28 = new Cs.FillReference { Index = 0U };
        var effectReference28 = new Cs.EffectReference { Index = 0U };

        var fontReference28 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor79 = new A.SchemeColor { Val = A.SchemeColorValues.Dark1 };

        fontReference28.Append(schemeColor79);

        var shapeProperties20 = new Cs.ShapeProperties();

        var solidFill37 = new A.SolidFill();
        var schemeColor80 = new A.SchemeColor { Val = A.SchemeColorValues.Light1 };

        solidFill37.Append(schemeColor80);

        var outline31 = new A.Outline { Width = 9525 };

        var solidFill38 = new A.SolidFill();

        var schemeColor81 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation46 = new A.LuminanceModulation { Val = 15000 };
        var luminanceOffset34 = new A.LuminanceOffset { Val = 85000 };

        schemeColor81.Append(luminanceModulation46);
        schemeColor81.Append(luminanceOffset34);

        solidFill38.Append(schemeColor81);

        outline31.Append(solidFill38);

        shapeProperties20.Append(solidFill37);
        shapeProperties20.Append(outline31);

        upBar1.Append(lineReference28);
        upBar1.Append(fillReference28);
        upBar1.Append(effectReference28);
        upBar1.Append(fontReference28);
        upBar1.Append(shapeProperties20);

        var valueAxis2 = new Cs.ValueAxis();
        var lineReference29 = new Cs.LineReference { Index = 0U };
        var fillReference29 = new Cs.FillReference { Index = 0U };
        var effectReference29 = new Cs.EffectReference { Index = 0U };

        var fontReference29 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };

        var schemeColor82 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };
        var luminanceModulation47 = new A.LuminanceModulation { Val = 65000 };
        var luminanceOffset35 = new A.LuminanceOffset { Val = 35000 };

        schemeColor82.Append(luminanceModulation47);
        schemeColor82.Append(luminanceOffset35);

        fontReference29.Append(schemeColor82);
        var textCharacterPropertiesType11 = new Cs.TextCharacterPropertiesType { FontSize = 900, Kerning = 1200 };

        valueAxis2.Append(lineReference29);
        valueAxis2.Append(fillReference29);
        valueAxis2.Append(effectReference29);
        valueAxis2.Append(fontReference29);
        valueAxis2.Append(textCharacterPropertiesType11);

        var wall1 = new Cs.Wall();
        var lineReference30 = new Cs.LineReference { Index = 0U };
        var fillReference30 = new Cs.FillReference { Index = 0U };
        var effectReference30 = new Cs.EffectReference { Index = 0U };

        var fontReference30 = new Cs.FontReference { Index = A.FontCollectionIndexValues.Minor };
        var schemeColor83 = new A.SchemeColor { Val = A.SchemeColorValues.Text1 };

        fontReference30.Append(schemeColor83);

        var shapeProperties21 = new Cs.ShapeProperties();
        var noFill16 = new A.NoFill();

        var outline32 = new A.Outline();
        var noFill17 = new A.NoFill();

        outline32.Append(noFill17);

        shapeProperties21.Append(noFill16);
        shapeProperties21.Append(outline32);

        wall1.Append(lineReference30);
        wall1.Append(fillReference30);
        wall1.Append(effectReference30);
        wall1.Append(fontReference30);
        wall1.Append(shapeProperties21);

        chartStyle1.Append(axisTitle1);
        chartStyle1.Append(categoryAxis2);
        chartStyle1.Append(chartArea1);
        chartStyle1.Append(dataLabel1);
        chartStyle1.Append(dataLabelCallout1);
        chartStyle1.Append(dataPoint1);
        chartStyle1.Append(dataPoint3D1);
        chartStyle1.Append(dataPointLine1);
        chartStyle1.Append(dataPointMarker1);
        chartStyle1.Append(markerLayoutProperties1);
        chartStyle1.Append(dataPointWireframe1);
        chartStyle1.Append(dataTableStyle1);
        chartStyle1.Append(downBar1);
        chartStyle1.Append(dropLine1);
        chartStyle1.Append(errorBar1);
        chartStyle1.Append(floor1);
        chartStyle1.Append(gridlineMajor1);
        chartStyle1.Append(gridlineMinor1);
        chartStyle1.Append(hiLoLine1);
        chartStyle1.Append(leaderLine1);
        chartStyle1.Append(legendStyle1);
        chartStyle1.Append(plotArea2);
        chartStyle1.Append(plotArea3D1);
        chartStyle1.Append(seriesAxis1);
        chartStyle1.Append(seriesLine1);
        chartStyle1.Append(titleStyle1);
        chartStyle1.Append(trendlineStyle1);
        chartStyle1.Append(trendlineLabel1);
        chartStyle1.Append(upBar1);
        chartStyle1.Append(valueAxis2);
        chartStyle1.Append(wall1);

        chartStylePart1.ChartStyle = chartStyle1;
    }
}