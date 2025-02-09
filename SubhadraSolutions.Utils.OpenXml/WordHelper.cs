using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using A = DocumentFormat.OpenXml.Drawing;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using M = DocumentFormat.OpenXml.Math;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Drawing.ChartDrawing.NonVisualDrawingProperties;
using Ovml = DocumentFormat.OpenXml.Vml.Office;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.ChartDrawing.ShapeProperties;
using Thm15 = DocumentFormat.OpenXml.Office2013.Theme;
using V = DocumentFormat.OpenXml.Vml;
using W14 = DocumentFormat.OpenXml.Office2010.Word;
using W15 = DocumentFormat.OpenXml.Office2013.Word;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace SubhadraSolutions.Utils.OpenXml;

public static class WordHelper
{
    public static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
    {
        // Define the reference of the image.
        var element =
            new DocumentFormat.OpenXml.Office.Drawing.Drawing(
                new Wp.Inline(
                    new Extent { Cx = 990000L, Cy = 792000L },
                    new Wp.EffectExtent
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new Wp.DocProperties
                    {
                        Id = 1U
                        //Name = "Picture " + GeneralHelper.Identity.ToString()
                    },
                    new A.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                                new A.Picture(
                                    new A.NonVisualPictureProperties(
                                        new NonVisualDrawingProperties
                                        {
                                            Id = 0U
                                            //Name = "New Bitmap Image.jpg"
                                        },
                                        new A.NonVisualPictureDrawingProperties()),
                                    new A.BlipFill(
                                        new A.Blip(
                                            new A.BlipExtensionList(
                                                new A.BlipExtension
                                                {
                                                    Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                })
                                        )
                                        {
                                            Embed = relationshipId,
                                            CompressionState =
                                                A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(
                                            new A.FillRectangle())),
                                    new ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset { X = 0L, Y = 0L },
                                            new A.Extents { Cx = 990000L, Cy = 792000L }),
                                        new A.PresetGeometry(
                                                new A.AdjustValueList()
                                            )
                                        { Preset = A.ShapeTypeValues.Rectangle }))
                            )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U,
                    EditId = "50D07946"
                });

        // Append the reference to body, the element should be in a Run.
        wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
    }

    // Adds child parts and generates content of the specified part.
    public static void CreateDefaultParts(WordprocessingDocument document)
    {
        var extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
        GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

        var mainDocumentPart1 = document.AddMainDocumentPart();
        GenerateMainDocumentPart1Content(mainDocumentPart1);

        var webSettingsPart1 = mainDocumentPart1.AddNewPart<WebSettingsPart>("rId3");
        GenerateWebSettingsPart1Content(webSettingsPart1);

        var documentSettingsPart1 = mainDocumentPart1.AddNewPart<DocumentSettingsPart>("rId2");
        GenerateDocumentSettingsPart1Content(documentSettingsPart1);

        var styleDefinitionsPart1 = mainDocumentPart1.AddNewPart<StyleDefinitionsPart>("rId1");
        GenerateStyleDefinitionsPart1Content(styleDefinitionsPart1);

        var themePart1 = mainDocumentPart1.AddNewPart<ThemePart>("rId6");
        GenerateThemePart1Content(themePart1);

        var fontTablePart1 = mainDocumentPart1.AddNewPart<FontTablePart>("rId5");
        GenerateFontTablePart1Content(fontTablePart1);

        var extendedPart1 =
            document.AddExtendedPart("http://schemas.microsoft.com/office/2020/02/relationships/classificationlabels",
                "application/vnd.ms-office.classificationlabels+xml", "xml", "rId4");
        GenerateExtendedPart1Content(extendedPart1);

        SetPackageProperties(document);
    }

    public static Paragraph CreateParagraph(MainDocumentPart part, string text, JustificationValues alignment,
        int fontSize, string textColor)
    {
        var styleId = "style" + StringHelper.BuildUniqueString() + fontSize;
        CreateStyleHeading(part, styleId, fontSize, textColor);

        var p = new Paragraph();
        var pp = new ParagraphProperties();

        var justification = new Justification { Val = alignment };
        pp.Append(justification);

        pp.ParagraphStyleId = new ParagraphStyleId { Val = styleId };
        pp.SpacingBetweenLines = new SpacingBetweenLines { After = "0" };
        p.Append(pp);

        var r = new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text)
        { Space = SpaceProcessingModeValues.Preserve });
        p.Append(r);

        return p;
    }

    public static void CreateStyleHeading(MainDocumentPart mdoc, string styleid, int fontSize, string textColor)
    {
        if (mdoc.StyleDefinitionsPart == null)
        {
            AddStylesPartToPackage(mdoc);
        }

        var styles = mdoc.StyleDefinitionsPart.Styles;
        if (styles == null)
        {
            mdoc.StyleDefinitionsPart.Styles = new Styles();
            mdoc.StyleDefinitionsPart.Styles.Save();
        }

        var style = new Style
        {
            Type = StyleValues.Paragraph,
            StyleId = styleid,
            CustomStyle = true,
            Default = false
        };

        // Create and add the child elements (properties of the style).
        var uiPriority = new UIPriority { Val = 9 };
        _ = new Aliases { Val = "" };
        var autoredefine = new AutoRedefine { Val = OnOffOnlyValues.Off };
        var basedon = new BasedOn { Val = "Normal" };
        var linkedStyle = new LinkedStyle { Val = "OverdueAmountChar" };
        var locked = new Locked { Val = OnOffOnlyValues.Off };
        var primarystyle = new PrimaryStyle { Val = OnOffOnlyValues.On };
        var stylehidden = new StyleHidden { Val = OnOffOnlyValues.Off };
        var semihidden = new SemiHidden { Val = OnOffOnlyValues.Off };
        //var styleName = new StyleName() { Val = stylename };
        var nextParagraphStyle = new NextParagraphStyle { Val = "Normal" };
        var uipriority = new UIPriority { Val = 1 };
        var unhidewhenused = new UnhideWhenUsed { Val = OnOffOnlyValues.On };

        style.Append(uiPriority);
        style.Append(autoredefine);
        style.Append(basedon);
        style.Append(linkedStyle);
        style.Append(locked);
        style.Append(primarystyle);
        style.Append(stylehidden);
        style.Append(semihidden);
        //style.Append(styleName);
        style.Append(nextParagraphStyle);
        style.Append(uipriority);
        style.Append(unhidewhenused);

        // Create the StyleRunProperties object and specify some of the run properties.
        var styleRunProperties = new StyleRunProperties();
        //var color = new Color() { Val = textColor, ThemeColor = ThemeColorValues.Accent2 };
        var color = new Color { Val = textColor };
        var font = new RunFonts { Ascii = "Lucida Console" };
        _ = new Italic();
        // Specify a 12 point size.
        var fontSize1 = new FontSize { Val = fontSize.ToString() };
        //styleRunProperties.Append(bold);
        styleRunProperties.Append(color);
        styleRunProperties.Append(font);
        styleRunProperties.Append(fontSize1);
        //styleRunProperties.Append(italic);

        // Add the run properties to the style.
        style.Append(styleRunProperties);

        // Add the style to the styles part.
        styles.Append(style);
    }

    public static PartTypeInfo GetImagePartType(string extn)
    {
        if (string.Equals(extn, "Emf", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Emf;
        }

        if (string.Equals(extn, "Wmf", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Wmf;
        }

        if (string.Equals(extn, "ico", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Icon;
        }

        if (string.Equals(extn, "Gif", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Gif;
        }

        if (string.Equals(extn, "Jpeg", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Jpeg;
        }

        if (string.Equals(extn, "jpg", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Jpeg;
        }

        if (string.Equals(extn, "Tiff", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Tiff;
        }

        if (string.Equals(extn, "Pcx", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Pcx;
        }

        if (string.Equals(extn, "Bmp", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Bmp;
        }

        if (string.Equals(extn, "Png", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Png;
        }

        if (string.Equals(extn, "Svg", StringComparison.OrdinalIgnoreCase))
        {
            return ImagePartType.Svg;
        }

        return ImagePartType.Png;
    }

    private static StyleDefinitionsPart AddStylesPartToPackage(MainDocumentPart docPart)
    {
        var part = docPart.AddNewPart<StyleDefinitionsPart>();
        var root = new Styles();
        root.Save(part);
        return part;
    }

    // Generates content of documentSettingsPart1.
    private static void GenerateDocumentSettingsPart1Content(DocumentSettingsPart documentSettingsPart1)
    {
        var settings1 = new Settings
        {
            MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "w14 w15 w16se w16cid w16 w16cex w16sdtdh" }
        };
        settings1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
        settings1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
        settings1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        settings1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
        settings1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
        settings1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
        settings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        settings1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        settings1.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
        settings1.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
        settings1.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
        settings1.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
        settings1.AddNamespaceDeclaration("w16sdtdh",
            "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash");
        settings1.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");
        settings1.AddNamespaceDeclaration("sl", "http://schemas.openxmlformats.org/schemaLibrary/2006/main");
        var zoom1 = new Zoom { Percent = "100" };
        var proofState1 = new ProofState { Spelling = ProofingStateValues.Clean, Grammar = ProofingStateValues.Clean };
        var defaultTabStop1 = new DefaultTabStop { Val = 720 };
        var characterSpacingControl1 = new CharacterSpacingControl { Val = CharacterSpacingValues.DoNotCompress };

        var compatibility1 = new Compatibility();
        var compatibilitySetting1 = new CompatibilitySetting
        {
            Name = CompatSettingNameValues.CompatibilityMode,
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "15"
        };
        var compatibilitySetting2 = new CompatibilitySetting
        {
            Name = CompatSettingNameValues.OverrideTableStyleFontSizeAndJustification,
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "1"
        };
        var compatibilitySetting3 = new CompatibilitySetting
        {
            Name = CompatSettingNameValues.EnableOpenTypeFeatures,
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "1"
        };
        var compatibilitySetting4 = new CompatibilitySetting
        {
            Name = CompatSettingNameValues.DoNotFlipMirrorIndents,
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "1"
        };
        var compatibilitySetting5 = new CompatibilitySetting
        {
            Name = CompatSettingNameValues.DifferentiateMultirowTableHeaders,
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "1"
        };
        var compatibilitySetting6 = new CompatibilitySetting
        {
            Name = new EnumValue<CompatSettingNameValues> { InnerText = "useWord2013TrackBottomHyphenation" },
            Uri = "http://schemas.microsoft.com/office/word",
            Val = "0"
        };

        compatibility1.Append(compatibilitySetting1);
        compatibility1.Append(compatibilitySetting2);
        compatibility1.Append(compatibilitySetting3);
        compatibility1.Append(compatibilitySetting4);
        compatibility1.Append(compatibilitySetting5);
        compatibility1.Append(compatibilitySetting6);

        var rsids1 = new Rsids();
        var rsidRoot1 = new RsidRoot { Val = "003D59B1" };
        var rsid1 = new Rsid { Val = "002416B4" };
        var rsid2 = new Rsid { Val = "003D59B1" };
        var rsid3 = new Rsid { Val = "00420D2E" };
        var rsid4 = new Rsid { Val = "00C840FF" };

        rsids1.Append(rsidRoot1);
        rsids1.Append(rsid1);
        rsids1.Append(rsid2);
        rsids1.Append(rsid3);
        rsids1.Append(rsid4);

        var mathProperties1 = new M.MathProperties();
        var mathFont1 = new M.MathFont { Val = "Cambria Math" };
        var breakBinary1 = new M.BreakBinary { Val = M.BreakBinaryOperatorValues.Before };
        var breakBinarySubtraction1 = new M.BreakBinarySubtraction { Val = M.BreakBinarySubtractionValues.MinusMinus };
        var smallFraction1 = new M.SmallFraction { Val = M.BooleanValues.Zero };
        var displayDefaults1 = new M.DisplayDefaults();
        var leftMargin1 = new M.LeftMargin { Val = 0U };
        var rightMargin1 = new M.RightMargin { Val = 0U };
        var defaultJustification1 = new M.DefaultJustification { Val = M.JustificationValues.CenterGroup };
        var wrapIndent1 = new M.WrapIndent { Val = 1440U };
        var integralLimitLocation1 = new M.IntegralLimitLocation { Val = M.LimitLocationValues.SubscriptSuperscript };
        var naryLimitLocation1 = new M.NaryLimitLocation { Val = M.LimitLocationValues.UnderOver };

        mathProperties1.Append(mathFont1);
        mathProperties1.Append(breakBinary1);
        mathProperties1.Append(breakBinarySubtraction1);
        mathProperties1.Append(smallFraction1);
        mathProperties1.Append(displayDefaults1);
        mathProperties1.Append(leftMargin1);
        mathProperties1.Append(rightMargin1);
        mathProperties1.Append(defaultJustification1);
        mathProperties1.Append(wrapIndent1);
        mathProperties1.Append(integralLimitLocation1);
        mathProperties1.Append(naryLimitLocation1);
        var themeFontLanguages1 = new ThemeFontLanguages { Val = "en-IN" };
        var colorSchemeMapping1 = new ColorSchemeMapping
        {
            Background1 = ColorSchemeIndexValues.Light1,
            Text1 = ColorSchemeIndexValues.Dark1,
            Background2 = ColorSchemeIndexValues.Light2,
            Text2 = ColorSchemeIndexValues.Dark2,
            Accent1 = ColorSchemeIndexValues.Accent1,
            Accent2 = ColorSchemeIndexValues.Accent2,
            Accent3 = ColorSchemeIndexValues.Accent3,
            Accent4 = ColorSchemeIndexValues.Accent4,
            Accent5 = ColorSchemeIndexValues.Accent5,
            Accent6 = ColorSchemeIndexValues.Accent6,
            Hyperlink = ColorSchemeIndexValues.Hyperlink,
            FollowedHyperlink = ColorSchemeIndexValues.FollowedHyperlink
        };

        var shapeDefaults1 = new ShapeDefaults();
        var shapeDefaults2 = new Ovml.ShapeDefaults
        { Extension = V.ExtensionHandlingBehaviorValues.Edit, MaxShapeId = 1026 };

        var shapeLayout1 = new Ovml.ShapeLayout { Extension = V.ExtensionHandlingBehaviorValues.Edit };
        var shapeIdMap1 = new Ovml.ShapeIdMap { Extension = V.ExtensionHandlingBehaviorValues.Edit, Data = "1" };

        shapeLayout1.Append(shapeIdMap1);

        shapeDefaults1.Append(shapeDefaults2);
        shapeDefaults1.Append(shapeLayout1);
        var decimalSymbol1 = new DecimalSymbol { Val = "." };
        var listSeparator1 = new ListSeparator { Val = "," };
        var documentId1 = new W14.DocumentId { Val = "46F7FB10" };
        var chartTrackingRefBased1 = new W15.ChartTrackingRefBased();
        var persistentDocumentId1 = new W15.PersistentDocumentId { Val = "{70337E1B-364D-44E5-9080-CB694DDE350B}" };

        settings1.Append(zoom1);
        settings1.Append(proofState1);
        settings1.Append(defaultTabStop1);
        settings1.Append(characterSpacingControl1);
        settings1.Append(compatibility1);
        settings1.Append(rsids1);
        settings1.Append(mathProperties1);
        settings1.Append(themeFontLanguages1);
        settings1.Append(colorSchemeMapping1);
        settings1.Append(shapeDefaults1);
        settings1.Append(decimalSymbol1);
        settings1.Append(listSeparator1);
        settings1.Append(documentId1);
        settings1.Append(chartTrackingRefBased1);
        settings1.Append(persistentDocumentId1);

        documentSettingsPart1.Settings = settings1;
    }

    // Generates content of extendedFilePropertiesPart1.
    private static void GenerateExtendedFilePropertiesPart1Content(
        ExtendedFilePropertiesPart extendedFilePropertiesPart1)
    {
        var properties1 = new Ap.Properties();
        properties1.AddNamespaceDeclaration("vt",
            "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
        var template1 = new Ap.Template
        {
            Text = "Normal.dotm"
        };
        var totalTime1 = new Ap.TotalTime
        {
            Text = "1"
        };
        var pages1 = new Ap.Pages
        {
            Text = "1"
        };
        var words1 = new Ap.Words
        {
            Text = "0"
        };
        var characters1 = new Ap.Characters
        {
            Text = "1"
        };
        var application1 = new Ap.Application
        {
            Text = "Microsoft Office Word"
        };
        var documentSecurity1 = new Ap.DocumentSecurity
        {
            Text = "0"
        };
        var lines1 = new Ap.Lines
        {
            Text = "1"
        };
        var paragraphs1 = new Ap.Paragraphs
        {
            Text = "1"
        };
        var scaleCrop1 = new Ap.ScaleCrop
        {
            Text = "false"
        };
        var company1 = new Ap.Company
        {
            Text = ""
        };
        var linksUpToDate1 = new Ap.LinksUpToDate
        {
            Text = "false"
        };
        var charactersWithSpaces1 = new Ap.CharactersWithSpaces
        {
            Text = "1"
        };
        var sharedDocument1 = new Ap.SharedDocument
        {
            Text = "false"
        };
        var hyperlinksChanged1 = new Ap.HyperlinksChanged
        {
            Text = "false"
        };
        var applicationVersion1 = new Ap.ApplicationVersion
        {
            Text = "16.0000"
        };

        properties1.Append(template1);
        properties1.Append(totalTime1);
        properties1.Append(pages1);
        properties1.Append(words1);
        properties1.Append(characters1);
        properties1.Append(application1);
        properties1.Append(documentSecurity1);
        properties1.Append(lines1);
        properties1.Append(paragraphs1);
        properties1.Append(scaleCrop1);
        properties1.Append(company1);
        properties1.Append(linksUpToDate1);
        properties1.Append(charactersWithSpaces1);
        properties1.Append(sharedDocument1);
        properties1.Append(hyperlinksChanged1);
        properties1.Append(applicationVersion1);

        extendedFilePropertiesPart1.Properties = properties1;
    }

    // Generates content of extendedPart1.
    private static void GenerateExtendedPart1Content(ExtendedPart extendedPart1)
    {
        const string extendedPart1Data =
            "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiIHN0YW5kYWxvbmU9InllcyI/PjxjbGJsOmxhYmVsTGlzdCB4bWxuczpjbGJsPSJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL29mZmljZS8yMDIwL21pcExhYmVsTWV0YWRhdGEiPjxjbGJsOmxhYmVsIGlkPSJ7ZjQyYWEzNDItODcwNi00Mjg4LWJkMTEtZWJiODU5OTUwMjhjfSIgZW5hYmxlZD0iMSIgbWV0aG9kPSJTdGFuZGFyZCIgc2l0ZUlkPSJ7NzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3fSIgY29udGVudEJpdHM9IjAiIHJlbW92ZWQ9IjAiIC8+PC9jbGJsOmxhYmVsTGlzdD4=";
        var data = GetBinaryDataStream(extendedPart1Data);
        extendedPart1.FeedData(data);
        data.Close();
    }

    // Generates content of fontTablePart1.
    private static void GenerateFontTablePart1Content(FontTablePart fontTablePart1)
    {
        var fonts1 = new Fonts
        {
            MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "w14 w15 w16se w16cid w16 w16cex w16sdtdh" }
        };
        fonts1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
        fonts1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        fonts1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        fonts1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        fonts1.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
        fonts1.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
        fonts1.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
        fonts1.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
        fonts1.AddNamespaceDeclaration("w16sdtdh", "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash");
        fonts1.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");

        var font1 = new Font { Name = "Calibri" };
        var panose1Number1 = new Panose1Number { Val = "020F0502020204030204" };
        var fontCharSet1 = new FontCharSet { Val = "00" };
        var fontFamily1 = new FontFamily { Val = FontFamilyValues.Swiss };
        var pitch1 = new Pitch { Val = FontPitchValues.Variable };
        var fontSignature1 = new FontSignature
        {
            UnicodeSignature0 = "E4002EFF",
            UnicodeSignature1 = "C000247B",
            UnicodeSignature2 = "00000009",
            UnicodeSignature3 = "00000000",
            CodePageSignature0 = "000001FF",
            CodePageSignature1 = "00000000"
        };

        font1.Append(panose1Number1);
        font1.Append(fontCharSet1);
        font1.Append(fontFamily1);
        font1.Append(pitch1);
        font1.Append(fontSignature1);

        var font2 = new Font { Name = "Times New Roman" };
        var panose1Number2 = new Panose1Number { Val = "02020603050405020304" };
        var fontCharSet2 = new FontCharSet { Val = "00" };
        var fontFamily2 = new FontFamily { Val = FontFamilyValues.Roman };
        var pitch2 = new Pitch { Val = FontPitchValues.Variable };
        var fontSignature2 = new FontSignature
        {
            UnicodeSignature0 = "E0002EFF",
            UnicodeSignature1 = "C000785B",
            UnicodeSignature2 = "00000009",
            UnicodeSignature3 = "00000000",
            CodePageSignature0 = "000001FF",
            CodePageSignature1 = "00000000"
        };

        font2.Append(panose1Number2);
        font2.Append(fontCharSet2);
        font2.Append(fontFamily2);
        font2.Append(pitch2);
        font2.Append(fontSignature2);

        var font3 = new Font { Name = "Calibri Light" };
        var panose1Number3 = new Panose1Number { Val = "020F0302020204030204" };
        var fontCharSet3 = new FontCharSet { Val = "00" };
        var fontFamily3 = new FontFamily { Val = FontFamilyValues.Swiss };
        var pitch3 = new Pitch { Val = FontPitchValues.Variable };
        var fontSignature3 = new FontSignature
        {
            UnicodeSignature0 = "E4002EFF",
            UnicodeSignature1 = "C000247B",
            UnicodeSignature2 = "00000009",
            UnicodeSignature3 = "00000000",
            CodePageSignature0 = "000001FF",
            CodePageSignature1 = "00000000"
        };

        font3.Append(panose1Number3);
        font3.Append(fontCharSet3);
        font3.Append(fontFamily3);
        font3.Append(pitch3);
        font3.Append(fontSignature3);

        fonts1.Append(font1);
        fonts1.Append(font2);
        fonts1.Append(font3);

        fontTablePart1.Fonts = fonts1;
    }

    // Generates content of mainDocumentPart1.
    private static void GenerateMainDocumentPart1Content(MainDocumentPart mainDocumentPart1)
    {
        var document1 = new Document
        {
            MCAttributes = new MarkupCompatibilityAttributes
            { Ignorable = "w14 w15 w16se w16cid w16 w16cex w16sdtdh wp14" }
        };
        document1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
        document1.AddNamespaceDeclaration("cx", "http://schemas.microsoft.com/office/drawing/2014/chartex");
        document1.AddNamespaceDeclaration("cx1", "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex");
        document1.AddNamespaceDeclaration("cx2", "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex");
        document1.AddNamespaceDeclaration("cx3", "http://schemas.microsoft.com/office/drawing/2016/5/9/chartex");
        document1.AddNamespaceDeclaration("cx4", "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex");
        document1.AddNamespaceDeclaration("cx5", "http://schemas.microsoft.com/office/drawing/2016/5/11/chartex");
        document1.AddNamespaceDeclaration("cx6", "http://schemas.microsoft.com/office/drawing/2016/5/12/chartex");
        document1.AddNamespaceDeclaration("cx7", "http://schemas.microsoft.com/office/drawing/2016/5/13/chartex");
        document1.AddNamespaceDeclaration("cx8", "http://schemas.microsoft.com/office/drawing/2016/5/14/chartex");
        document1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
        document1.AddNamespaceDeclaration("aink", "http://schemas.microsoft.com/office/drawing/2016/ink");
        document1.AddNamespaceDeclaration("am3d", "http://schemas.microsoft.com/office/drawing/2017/model3d");
        document1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
        document1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        document1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
        document1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
        document1.AddNamespaceDeclaration("wp14",
            "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
        document1.AddNamespaceDeclaration("wp",
            "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
        document1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
        document1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        document1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        document1.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
        document1.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
        document1.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
        document1.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
        document1.AddNamespaceDeclaration("w16sdtdh",
            "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash");
        document1.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");
        document1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
        document1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
        document1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
        document1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

        var body1 = new Body();

        var sectionProperties1 = new SectionProperties { RsidR = "00C840FF" };
        var pageSize1 = new PageSize { Width = 11906U, Height = 16838U };
        //var pageSize1 = new PageSize() { Width = (UInt32Value)16838U, Height = (UInt32Value)11906U, Orient = PageOrientationValues.Landscape };

        var pageMargin1 = new PageMargin
        { Top = 1440, Right = 1440U, Bottom = 1440, Left = 1440U, Header = 708U, Footer = 708U, Gutter = 0U };
        var columns1 = new Columns { Space = "708" };
        var docGrid1 = new DocGrid { LinePitch = 360 };

        sectionProperties1.Append(pageSize1);
        sectionProperties1.Append(pageMargin1);
        sectionProperties1.Append(columns1);
        sectionProperties1.Append(docGrid1);

        //body1.Append(paragraph1);
        body1.Append(sectionProperties1);

        document1.Body = body1;

        mainDocumentPart1.Document = document1;
    }

    // Generates content of styleDefinitionsPart1.
    private static void GenerateStyleDefinitionsPart1Content(StyleDefinitionsPart styleDefinitionsPart1)
    {
        var styles1 = new Styles
        {
            MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "w14 w15 w16se w16cid w16 w16cex w16sdtdh" }
        };
        styles1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
        styles1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        styles1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        styles1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        styles1.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
        styles1.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
        styles1.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
        styles1.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
        styles1.AddNamespaceDeclaration("w16sdtdh", "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash");
        styles1.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");

        var docDefaults1 = new DocDefaults();

        var runPropertiesDefault1 = new RunPropertiesDefault();

        var runPropertiesBaseStyle1 = new RunPropertiesBaseStyle();
        var runFonts1 = new RunFonts
        {
            AsciiTheme = ThemeFontValues.MinorHighAnsi,
            HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
            EastAsiaTheme = ThemeFontValues.MinorHighAnsi,
            ComplexScriptTheme = ThemeFontValues.MinorBidi
        };
        var fontSize1 = new FontSize { Val = "22" };
        var fontSizeComplexScript1 = new FontSizeComplexScript { Val = "22" };
        var languages1 = new Languages { Val = "en-IN", EastAsia = "en-US", Bidi = "ar-SA" };

        runPropertiesBaseStyle1.Append(runFonts1);
        runPropertiesBaseStyle1.Append(fontSize1);
        runPropertiesBaseStyle1.Append(fontSizeComplexScript1);
        runPropertiesBaseStyle1.Append(languages1);

        runPropertiesDefault1.Append(runPropertiesBaseStyle1);

        var paragraphPropertiesDefault1 = new ParagraphPropertiesDefault();

        var paragraphPropertiesBaseStyle1 = new ParagraphPropertiesBaseStyle();
        var spacingBetweenLines1 = new SpacingBetweenLines
        { After = "160", Line = "259", LineRule = LineSpacingRuleValues.Auto };

        paragraphPropertiesBaseStyle1.Append(spacingBetweenLines1);

        paragraphPropertiesDefault1.Append(paragraphPropertiesBaseStyle1);

        docDefaults1.Append(runPropertiesDefault1);
        docDefaults1.Append(paragraphPropertiesDefault1);

        var latentStyles1 = new LatentStyles
        {
            DefaultLockedState = false,
            DefaultUiPriority = 99,
            DefaultSemiHidden = false,
            DefaultUnhideWhenUsed = false,
            DefaultPrimaryStyle = false,
            Count = 376
        };
        var latentStyleExceptionInfo1 = new LatentStyleExceptionInfo
        { Name = "Normal", UiPriority = 0, PrimaryStyle = true };
        var latentStyleExceptionInfo2 = new LatentStyleExceptionInfo
        { Name = "heading 1", UiPriority = 9, PrimaryStyle = true };
        var latentStyleExceptionInfo3 = new LatentStyleExceptionInfo
        { Name = "heading 2", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo4 = new LatentStyleExceptionInfo
        { Name = "heading 3", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo5 = new LatentStyleExceptionInfo
        { Name = "heading 4", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo6 = new LatentStyleExceptionInfo
        { Name = "heading 5", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo7 = new LatentStyleExceptionInfo
        { Name = "heading 6", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo8 = new LatentStyleExceptionInfo
        { Name = "heading 7", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo9 = new LatentStyleExceptionInfo
        { Name = "heading 8", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo10 = new LatentStyleExceptionInfo
        { Name = "heading 9", UiPriority = 9, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo11 = new LatentStyleExceptionInfo
        { Name = "index 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo12 = new LatentStyleExceptionInfo
        { Name = "index 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo13 = new LatentStyleExceptionInfo
        { Name = "index 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo14 = new LatentStyleExceptionInfo
        { Name = "index 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo15 = new LatentStyleExceptionInfo
        { Name = "index 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo16 = new LatentStyleExceptionInfo
        { Name = "index 6", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo17 = new LatentStyleExceptionInfo
        { Name = "index 7", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo18 = new LatentStyleExceptionInfo
        { Name = "index 8", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo19 = new LatentStyleExceptionInfo
        { Name = "index 9", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo20 = new LatentStyleExceptionInfo
        { Name = "toc 1", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo21 = new LatentStyleExceptionInfo
        { Name = "toc 2", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo22 = new LatentStyleExceptionInfo
        { Name = "toc 3", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo23 = new LatentStyleExceptionInfo
        { Name = "toc 4", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo24 = new LatentStyleExceptionInfo
        { Name = "toc 5", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo25 = new LatentStyleExceptionInfo
        { Name = "toc 6", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo26 = new LatentStyleExceptionInfo
        { Name = "toc 7", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo27 = new LatentStyleExceptionInfo
        { Name = "toc 8", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo28 = new LatentStyleExceptionInfo
        { Name = "toc 9", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo29 = new LatentStyleExceptionInfo
        { Name = "Normal Indent", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo30 = new LatentStyleExceptionInfo
        { Name = "footnote text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo31 = new LatentStyleExceptionInfo
        { Name = "annotation text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo32 = new LatentStyleExceptionInfo
        { Name = "header", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo33 = new LatentStyleExceptionInfo
        { Name = "footer", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo34 = new LatentStyleExceptionInfo
        { Name = "index heading", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo35 = new LatentStyleExceptionInfo
        { Name = "caption", UiPriority = 35, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo36 = new LatentStyleExceptionInfo
        { Name = "table of figures", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo37 = new LatentStyleExceptionInfo
        { Name = "envelope address", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo38 = new LatentStyleExceptionInfo
        { Name = "envelope return", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo39 = new LatentStyleExceptionInfo
        { Name = "footnote reference", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo40 = new LatentStyleExceptionInfo
        { Name = "annotation reference", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo41 = new LatentStyleExceptionInfo
        { Name = "line number", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo42 = new LatentStyleExceptionInfo
        { Name = "page number", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo43 = new LatentStyleExceptionInfo
        { Name = "endnote reference", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo44 = new LatentStyleExceptionInfo
        { Name = "endnote text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo45 = new LatentStyleExceptionInfo
        { Name = "table of authorities", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo46 = new LatentStyleExceptionInfo
        { Name = "macro", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo47 = new LatentStyleExceptionInfo
        { Name = "toa heading", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo48 = new LatentStyleExceptionInfo
        { Name = "List", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo49 = new LatentStyleExceptionInfo
        { Name = "List Bullet", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo50 = new LatentStyleExceptionInfo
        { Name = "List Number", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo51 = new LatentStyleExceptionInfo
        { Name = "List 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo52 = new LatentStyleExceptionInfo
        { Name = "List 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo53 = new LatentStyleExceptionInfo
        { Name = "List 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo54 = new LatentStyleExceptionInfo
        { Name = "List 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo55 = new LatentStyleExceptionInfo
        { Name = "List Bullet 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo56 = new LatentStyleExceptionInfo
        { Name = "List Bullet 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo57 = new LatentStyleExceptionInfo
        { Name = "List Bullet 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo58 = new LatentStyleExceptionInfo
        { Name = "List Bullet 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo59 = new LatentStyleExceptionInfo
        { Name = "List Number 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo60 = new LatentStyleExceptionInfo
        { Name = "List Number 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo61 = new LatentStyleExceptionInfo
        { Name = "List Number 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo62 = new LatentStyleExceptionInfo
        { Name = "List Number 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo63 = new LatentStyleExceptionInfo
        { Name = "Title", UiPriority = 10, PrimaryStyle = true };
        var latentStyleExceptionInfo64 = new LatentStyleExceptionInfo
        { Name = "Closing", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo65 = new LatentStyleExceptionInfo
        { Name = "Signature", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo66 = new LatentStyleExceptionInfo
        { Name = "Default Paragraph Font", UiPriority = 1, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo67 = new LatentStyleExceptionInfo
        { Name = "Body Text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo68 = new LatentStyleExceptionInfo
        { Name = "Body Text Indent", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo69 = new LatentStyleExceptionInfo
        { Name = "List Continue", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo70 = new LatentStyleExceptionInfo
        { Name = "List Continue 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo71 = new LatentStyleExceptionInfo
        { Name = "List Continue 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo72 = new LatentStyleExceptionInfo
        { Name = "List Continue 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo73 = new LatentStyleExceptionInfo
        { Name = "List Continue 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo74 = new LatentStyleExceptionInfo
        { Name = "Message Header", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo75 = new LatentStyleExceptionInfo
        { Name = "Subtitle", UiPriority = 11, PrimaryStyle = true };
        var latentStyleExceptionInfo76 = new LatentStyleExceptionInfo
        { Name = "Salutation", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo77 = new LatentStyleExceptionInfo
        { Name = "Date", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo78 = new LatentStyleExceptionInfo
        { Name = "Body Text First Indent", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo79 = new LatentStyleExceptionInfo
        { Name = "Body Text First Indent 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo80 = new LatentStyleExceptionInfo
        { Name = "Note Heading", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo81 = new LatentStyleExceptionInfo
        { Name = "Body Text 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo82 = new LatentStyleExceptionInfo
        { Name = "Body Text 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo83 = new LatentStyleExceptionInfo
        { Name = "Body Text Indent 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo84 = new LatentStyleExceptionInfo
        { Name = "Body Text Indent 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo85 = new LatentStyleExceptionInfo
        { Name = "Block Text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo86 = new LatentStyleExceptionInfo
        { Name = "Hyperlink", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo87 = new LatentStyleExceptionInfo
        { Name = "FollowedHyperlink", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo88 = new LatentStyleExceptionInfo
        { Name = "Strong", UiPriority = 22, PrimaryStyle = true };
        var latentStyleExceptionInfo89 = new LatentStyleExceptionInfo
        { Name = "Emphasis", UiPriority = 20, PrimaryStyle = true };
        var latentStyleExceptionInfo90 = new LatentStyleExceptionInfo
        { Name = "Document Map", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo91 = new LatentStyleExceptionInfo
        { Name = "Plain Text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo92 = new LatentStyleExceptionInfo
        { Name = "E-mail Signature", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo93 = new LatentStyleExceptionInfo
        { Name = "HTML Top of Form", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo94 = new LatentStyleExceptionInfo
        { Name = "HTML Bottom of Form", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo95 = new LatentStyleExceptionInfo
        { Name = "Normal (Web)", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo96 = new LatentStyleExceptionInfo
        { Name = "HTML Acronym", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo97 = new LatentStyleExceptionInfo
        { Name = "HTML Address", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo98 = new LatentStyleExceptionInfo
        { Name = "HTML Cite", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo99 = new LatentStyleExceptionInfo
        { Name = "HTML Code", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo100 = new LatentStyleExceptionInfo
        { Name = "HTML Definition", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo101 = new LatentStyleExceptionInfo
        { Name = "HTML Keyboard", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo102 = new LatentStyleExceptionInfo
        { Name = "HTML Preformatted", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo103 = new LatentStyleExceptionInfo
        { Name = "HTML Sample", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo104 = new LatentStyleExceptionInfo
        { Name = "HTML Typewriter", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo105 = new LatentStyleExceptionInfo
        { Name = "HTML Variable", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo106 = new LatentStyleExceptionInfo
        { Name = "Normal Table", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo107 = new LatentStyleExceptionInfo
        { Name = "annotation subject", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo108 = new LatentStyleExceptionInfo
        { Name = "No List", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo109 = new LatentStyleExceptionInfo
        { Name = "Outline List 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo110 = new LatentStyleExceptionInfo
        { Name = "Outline List 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo111 = new LatentStyleExceptionInfo
        { Name = "Outline List 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo112 = new LatentStyleExceptionInfo
        { Name = "Table Simple 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo113 = new LatentStyleExceptionInfo
        { Name = "Table Simple 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo114 = new LatentStyleExceptionInfo
        { Name = "Table Simple 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo115 = new LatentStyleExceptionInfo
        { Name = "Table Classic 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo116 = new LatentStyleExceptionInfo
        { Name = "Table Classic 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo117 = new LatentStyleExceptionInfo
        { Name = "Table Classic 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo118 = new LatentStyleExceptionInfo
        { Name = "Table Classic 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo119 = new LatentStyleExceptionInfo
        { Name = "Table Colorful 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo120 = new LatentStyleExceptionInfo
        { Name = "Table Colorful 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo121 = new LatentStyleExceptionInfo
        { Name = "Table Colorful 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo122 = new LatentStyleExceptionInfo
        { Name = "Table Columns 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo123 = new LatentStyleExceptionInfo
        { Name = "Table Columns 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo124 = new LatentStyleExceptionInfo
        { Name = "Table Columns 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo125 = new LatentStyleExceptionInfo
        { Name = "Table Columns 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo126 = new LatentStyleExceptionInfo
        { Name = "Table Columns 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo127 = new LatentStyleExceptionInfo
        { Name = "Table Grid 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo128 = new LatentStyleExceptionInfo
        { Name = "Table Grid 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo129 = new LatentStyleExceptionInfo
        { Name = "Table Grid 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo130 = new LatentStyleExceptionInfo
        { Name = "Table Grid 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo131 = new LatentStyleExceptionInfo
        { Name = "Table Grid 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo132 = new LatentStyleExceptionInfo
        { Name = "Table Grid 6", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo133 = new LatentStyleExceptionInfo
        { Name = "Table Grid 7", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo134 = new LatentStyleExceptionInfo
        { Name = "Table Grid 8", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo135 = new LatentStyleExceptionInfo
        { Name = "Table List 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo136 = new LatentStyleExceptionInfo
        { Name = "Table List 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo137 = new LatentStyleExceptionInfo
        { Name = "Table List 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo138 = new LatentStyleExceptionInfo
        { Name = "Table List 4", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo139 = new LatentStyleExceptionInfo
        { Name = "Table List 5", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo140 = new LatentStyleExceptionInfo
        { Name = "Table List 6", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo141 = new LatentStyleExceptionInfo
        { Name = "Table List 7", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo142 = new LatentStyleExceptionInfo
        { Name = "Table List 8", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo143 = new LatentStyleExceptionInfo
        { Name = "Table 3D effects 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo144 = new LatentStyleExceptionInfo
        { Name = "Table 3D effects 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo145 = new LatentStyleExceptionInfo
        { Name = "Table 3D effects 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo146 = new LatentStyleExceptionInfo
        { Name = "Table Contemporary", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo147 = new LatentStyleExceptionInfo
        { Name = "Table Elegant", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo148 = new LatentStyleExceptionInfo
        { Name = "Table Professional", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo149 = new LatentStyleExceptionInfo
        { Name = "Table Subtle 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo150 = new LatentStyleExceptionInfo
        { Name = "Table Subtle 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo151 = new LatentStyleExceptionInfo
        { Name = "Table Web 1", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo152 = new LatentStyleExceptionInfo
        { Name = "Table Web 2", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo153 = new LatentStyleExceptionInfo
        { Name = "Table Web 3", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo154 = new LatentStyleExceptionInfo
        { Name = "Balloon Text", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo155 = new LatentStyleExceptionInfo { Name = "Table Grid", UiPriority = 39 };
        var latentStyleExceptionInfo156 = new LatentStyleExceptionInfo
        { Name = "Table Theme", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo157 = new LatentStyleExceptionInfo { Name = "Placeholder Text", SemiHidden = true };
        var latentStyleExceptionInfo158 = new LatentStyleExceptionInfo
        { Name = "No Spacing", UiPriority = 1, PrimaryStyle = true };
        var latentStyleExceptionInfo159 = new LatentStyleExceptionInfo { Name = "Light Shading", UiPriority = 60 };
        var latentStyleExceptionInfo160 = new LatentStyleExceptionInfo { Name = "Light List", UiPriority = 61 };
        var latentStyleExceptionInfo161 = new LatentStyleExceptionInfo { Name = "Light Grid", UiPriority = 62 };
        var latentStyleExceptionInfo162 = new LatentStyleExceptionInfo { Name = "Medium Shading 1", UiPriority = 63 };
        var latentStyleExceptionInfo163 = new LatentStyleExceptionInfo { Name = "Medium Shading 2", UiPriority = 64 };
        var latentStyleExceptionInfo164 = new LatentStyleExceptionInfo { Name = "Medium List 1", UiPriority = 65 };
        var latentStyleExceptionInfo165 = new LatentStyleExceptionInfo { Name = "Medium List 2", UiPriority = 66 };
        var latentStyleExceptionInfo166 = new LatentStyleExceptionInfo { Name = "Medium Grid 1", UiPriority = 67 };
        var latentStyleExceptionInfo167 = new LatentStyleExceptionInfo { Name = "Medium Grid 2", UiPriority = 68 };
        var latentStyleExceptionInfo168 = new LatentStyleExceptionInfo { Name = "Medium Grid 3", UiPriority = 69 };
        var latentStyleExceptionInfo169 = new LatentStyleExceptionInfo { Name = "Dark List", UiPriority = 70 };
        var latentStyleExceptionInfo170 = new LatentStyleExceptionInfo { Name = "Colorful Shading", UiPriority = 71 };
        var latentStyleExceptionInfo171 = new LatentStyleExceptionInfo { Name = "Colorful List", UiPriority = 72 };
        var latentStyleExceptionInfo172 = new LatentStyleExceptionInfo { Name = "Colorful Grid", UiPriority = 73 };
        var latentStyleExceptionInfo173 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 1", UiPriority = 60 };
        var latentStyleExceptionInfo174 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 1", UiPriority = 61 };
        var latentStyleExceptionInfo175 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 1", UiPriority = 62 };
        var latentStyleExceptionInfo176 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 1", UiPriority = 63 };
        var latentStyleExceptionInfo177 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 1", UiPriority = 64 };
        var latentStyleExceptionInfo178 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 1", UiPriority = 65 };
        var latentStyleExceptionInfo179 = new LatentStyleExceptionInfo { Name = "Revision", SemiHidden = true };
        var latentStyleExceptionInfo180 = new LatentStyleExceptionInfo
        { Name = "List Paragraph", UiPriority = 34, PrimaryStyle = true };
        var latentStyleExceptionInfo181 = new LatentStyleExceptionInfo
        { Name = "Quote", UiPriority = 29, PrimaryStyle = true };
        var latentStyleExceptionInfo182 = new LatentStyleExceptionInfo
        { Name = "Intense Quote", UiPriority = 30, PrimaryStyle = true };
        var latentStyleExceptionInfo183 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 1", UiPriority = 66 };
        var latentStyleExceptionInfo184 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 1", UiPriority = 67 };
        var latentStyleExceptionInfo185 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 1", UiPriority = 68 };
        var latentStyleExceptionInfo186 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 1", UiPriority = 69 };
        var latentStyleExceptionInfo187 = new LatentStyleExceptionInfo { Name = "Dark List Accent 1", UiPriority = 70 };
        var latentStyleExceptionInfo188 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 1", UiPriority = 71 };
        var latentStyleExceptionInfo189 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 1", UiPriority = 72 };
        var latentStyleExceptionInfo190 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 1", UiPriority = 73 };
        var latentStyleExceptionInfo191 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 2", UiPriority = 60 };
        var latentStyleExceptionInfo192 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 2", UiPriority = 61 };
        var latentStyleExceptionInfo193 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 2", UiPriority = 62 };
        var latentStyleExceptionInfo194 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 2", UiPriority = 63 };
        var latentStyleExceptionInfo195 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 2", UiPriority = 64 };
        var latentStyleExceptionInfo196 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 2", UiPriority = 65 };
        var latentStyleExceptionInfo197 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 2", UiPriority = 66 };
        var latentStyleExceptionInfo198 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 2", UiPriority = 67 };
        var latentStyleExceptionInfo199 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 2", UiPriority = 68 };
        var latentStyleExceptionInfo200 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 2", UiPriority = 69 };
        var latentStyleExceptionInfo201 = new LatentStyleExceptionInfo { Name = "Dark List Accent 2", UiPriority = 70 };
        var latentStyleExceptionInfo202 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 2", UiPriority = 71 };
        var latentStyleExceptionInfo203 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 2", UiPriority = 72 };
        var latentStyleExceptionInfo204 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 2", UiPriority = 73 };
        var latentStyleExceptionInfo205 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 3", UiPriority = 60 };
        var latentStyleExceptionInfo206 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 3", UiPriority = 61 };
        var latentStyleExceptionInfo207 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 3", UiPriority = 62 };
        var latentStyleExceptionInfo208 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 3", UiPriority = 63 };
        var latentStyleExceptionInfo209 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 3", UiPriority = 64 };
        var latentStyleExceptionInfo210 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 3", UiPriority = 65 };
        var latentStyleExceptionInfo211 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 3", UiPriority = 66 };
        var latentStyleExceptionInfo212 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 3", UiPriority = 67 };
        var latentStyleExceptionInfo213 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 3", UiPriority = 68 };
        var latentStyleExceptionInfo214 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 3", UiPriority = 69 };
        var latentStyleExceptionInfo215 = new LatentStyleExceptionInfo { Name = "Dark List Accent 3", UiPriority = 70 };
        var latentStyleExceptionInfo216 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 3", UiPriority = 71 };
        var latentStyleExceptionInfo217 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 3", UiPriority = 72 };
        var latentStyleExceptionInfo218 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 3", UiPriority = 73 };
        var latentStyleExceptionInfo219 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 4", UiPriority = 60 };
        var latentStyleExceptionInfo220 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 4", UiPriority = 61 };
        var latentStyleExceptionInfo221 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 4", UiPriority = 62 };
        var latentStyleExceptionInfo222 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 4", UiPriority = 63 };
        var latentStyleExceptionInfo223 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 4", UiPriority = 64 };
        var latentStyleExceptionInfo224 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 4", UiPriority = 65 };
        var latentStyleExceptionInfo225 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 4", UiPriority = 66 };
        var latentStyleExceptionInfo226 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 4", UiPriority = 67 };
        var latentStyleExceptionInfo227 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 4", UiPriority = 68 };
        var latentStyleExceptionInfo228 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 4", UiPriority = 69 };
        var latentStyleExceptionInfo229 = new LatentStyleExceptionInfo { Name = "Dark List Accent 4", UiPriority = 70 };
        var latentStyleExceptionInfo230 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 4", UiPriority = 71 };
        var latentStyleExceptionInfo231 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 4", UiPriority = 72 };
        var latentStyleExceptionInfo232 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 4", UiPriority = 73 };
        var latentStyleExceptionInfo233 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 5", UiPriority = 60 };
        var latentStyleExceptionInfo234 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 5", UiPriority = 61 };
        var latentStyleExceptionInfo235 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 5", UiPriority = 62 };
        var latentStyleExceptionInfo236 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 5", UiPriority = 63 };
        var latentStyleExceptionInfo237 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 5", UiPriority = 64 };
        var latentStyleExceptionInfo238 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 5", UiPriority = 65 };
        var latentStyleExceptionInfo239 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 5", UiPriority = 66 };
        var latentStyleExceptionInfo240 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 5", UiPriority = 67 };
        var latentStyleExceptionInfo241 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 5", UiPriority = 68 };
        var latentStyleExceptionInfo242 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 5", UiPriority = 69 };
        var latentStyleExceptionInfo243 = new LatentStyleExceptionInfo { Name = "Dark List Accent 5", UiPriority = 70 };
        var latentStyleExceptionInfo244 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 5", UiPriority = 71 };
        var latentStyleExceptionInfo245 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 5", UiPriority = 72 };
        var latentStyleExceptionInfo246 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 5", UiPriority = 73 };
        var latentStyleExceptionInfo247 = new LatentStyleExceptionInfo
        { Name = "Light Shading Accent 6", UiPriority = 60 };
        var latentStyleExceptionInfo248 = new LatentStyleExceptionInfo
        { Name = "Light List Accent 6", UiPriority = 61 };
        var latentStyleExceptionInfo249 = new LatentStyleExceptionInfo
        { Name = "Light Grid Accent 6", UiPriority = 62 };
        var latentStyleExceptionInfo250 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 1 Accent 6", UiPriority = 63 };
        var latentStyleExceptionInfo251 = new LatentStyleExceptionInfo
        { Name = "Medium Shading 2 Accent 6", UiPriority = 64 };
        var latentStyleExceptionInfo252 = new LatentStyleExceptionInfo
        { Name = "Medium List 1 Accent 6", UiPriority = 65 };
        var latentStyleExceptionInfo253 = new LatentStyleExceptionInfo
        { Name = "Medium List 2 Accent 6", UiPriority = 66 };
        var latentStyleExceptionInfo254 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 1 Accent 6", UiPriority = 67 };
        var latentStyleExceptionInfo255 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 2 Accent 6", UiPriority = 68 };
        var latentStyleExceptionInfo256 = new LatentStyleExceptionInfo
        { Name = "Medium Grid 3 Accent 6", UiPriority = 69 };
        var latentStyleExceptionInfo257 = new LatentStyleExceptionInfo { Name = "Dark List Accent 6", UiPriority = 70 };
        var latentStyleExceptionInfo258 = new LatentStyleExceptionInfo
        { Name = "Colorful Shading Accent 6", UiPriority = 71 };
        var latentStyleExceptionInfo259 = new LatentStyleExceptionInfo
        { Name = "Colorful List Accent 6", UiPriority = 72 };
        var latentStyleExceptionInfo260 = new LatentStyleExceptionInfo
        { Name = "Colorful Grid Accent 6", UiPriority = 73 };
        var latentStyleExceptionInfo261 = new LatentStyleExceptionInfo
        { Name = "Subtle Emphasis", UiPriority = 19, PrimaryStyle = true };
        var latentStyleExceptionInfo262 = new LatentStyleExceptionInfo
        { Name = "Intense Emphasis", UiPriority = 21, PrimaryStyle = true };
        var latentStyleExceptionInfo263 = new LatentStyleExceptionInfo
        { Name = "Subtle Reference", UiPriority = 31, PrimaryStyle = true };
        var latentStyleExceptionInfo264 = new LatentStyleExceptionInfo
        { Name = "Intense Reference", UiPriority = 32, PrimaryStyle = true };
        var latentStyleExceptionInfo265 = new LatentStyleExceptionInfo
        { Name = "Book Title", UiPriority = 33, PrimaryStyle = true };
        var latentStyleExceptionInfo266 = new LatentStyleExceptionInfo
        { Name = "Bibliography", UiPriority = 37, SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo267 = new LatentStyleExceptionInfo
        { Name = "TOC Heading", UiPriority = 39, SemiHidden = true, UnhideWhenUsed = true, PrimaryStyle = true };
        var latentStyleExceptionInfo268 = new LatentStyleExceptionInfo { Name = "Plain Table 1", UiPriority = 41 };
        var latentStyleExceptionInfo269 = new LatentStyleExceptionInfo { Name = "Plain Table 2", UiPriority = 42 };
        var latentStyleExceptionInfo270 = new LatentStyleExceptionInfo { Name = "Plain Table 3", UiPriority = 43 };
        var latentStyleExceptionInfo271 = new LatentStyleExceptionInfo { Name = "Plain Table 4", UiPriority = 44 };
        var latentStyleExceptionInfo272 = new LatentStyleExceptionInfo { Name = "Plain Table 5", UiPriority = 45 };
        var latentStyleExceptionInfo273 = new LatentStyleExceptionInfo { Name = "Grid Table Light", UiPriority = 40 };
        var latentStyleExceptionInfo274 = new LatentStyleExceptionInfo { Name = "Grid Table 1 Light", UiPriority = 46 };
        var latentStyleExceptionInfo275 = new LatentStyleExceptionInfo { Name = "Grid Table 2", UiPriority = 47 };
        var latentStyleExceptionInfo276 = new LatentStyleExceptionInfo { Name = "Grid Table 3", UiPriority = 48 };
        var latentStyleExceptionInfo277 = new LatentStyleExceptionInfo { Name = "Grid Table 4", UiPriority = 49 };
        var latentStyleExceptionInfo278 = new LatentStyleExceptionInfo { Name = "Grid Table 5 Dark", UiPriority = 50 };
        var latentStyleExceptionInfo279 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful", UiPriority = 51 };
        var latentStyleExceptionInfo280 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful", UiPriority = 52 };
        var latentStyleExceptionInfo281 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 1", UiPriority = 46 };
        var latentStyleExceptionInfo282 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 1", UiPriority = 47 };
        var latentStyleExceptionInfo283 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 1", UiPriority = 48 };
        var latentStyleExceptionInfo284 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 1", UiPriority = 49 };
        var latentStyleExceptionInfo285 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 1", UiPriority = 50 };
        var latentStyleExceptionInfo286 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 1", UiPriority = 51 };
        var latentStyleExceptionInfo287 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 1", UiPriority = 52 };
        var latentStyleExceptionInfo288 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 2", UiPriority = 46 };
        var latentStyleExceptionInfo289 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 2", UiPriority = 47 };
        var latentStyleExceptionInfo290 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 2", UiPriority = 48 };
        var latentStyleExceptionInfo291 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 2", UiPriority = 49 };
        var latentStyleExceptionInfo292 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 2", UiPriority = 50 };
        var latentStyleExceptionInfo293 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 2", UiPriority = 51 };
        var latentStyleExceptionInfo294 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 2", UiPriority = 52 };
        var latentStyleExceptionInfo295 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 3", UiPriority = 46 };
        var latentStyleExceptionInfo296 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 3", UiPriority = 47 };
        var latentStyleExceptionInfo297 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 3", UiPriority = 48 };
        var latentStyleExceptionInfo298 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 3", UiPriority = 49 };
        var latentStyleExceptionInfo299 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 3", UiPriority = 50 };
        var latentStyleExceptionInfo300 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 3", UiPriority = 51 };
        var latentStyleExceptionInfo301 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 3", UiPriority = 52 };
        var latentStyleExceptionInfo302 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 4", UiPriority = 46 };
        var latentStyleExceptionInfo303 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 4", UiPriority = 47 };
        var latentStyleExceptionInfo304 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 4", UiPriority = 48 };
        var latentStyleExceptionInfo305 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 4", UiPriority = 49 };
        var latentStyleExceptionInfo306 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 4", UiPriority = 50 };
        var latentStyleExceptionInfo307 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 4", UiPriority = 51 };
        var latentStyleExceptionInfo308 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 4", UiPriority = 52 };
        var latentStyleExceptionInfo309 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 5", UiPriority = 46 };
        var latentStyleExceptionInfo310 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 5", UiPriority = 47 };
        var latentStyleExceptionInfo311 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 5", UiPriority = 48 };
        var latentStyleExceptionInfo312 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 5", UiPriority = 49 };
        var latentStyleExceptionInfo313 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 5", UiPriority = 50 };
        var latentStyleExceptionInfo314 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 5", UiPriority = 51 };
        var latentStyleExceptionInfo315 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 5", UiPriority = 52 };
        var latentStyleExceptionInfo316 = new LatentStyleExceptionInfo
        { Name = "Grid Table 1 Light Accent 6", UiPriority = 46 };
        var latentStyleExceptionInfo317 = new LatentStyleExceptionInfo
        { Name = "Grid Table 2 Accent 6", UiPriority = 47 };
        var latentStyleExceptionInfo318 = new LatentStyleExceptionInfo
        { Name = "Grid Table 3 Accent 6", UiPriority = 48 };
        var latentStyleExceptionInfo319 = new LatentStyleExceptionInfo
        { Name = "Grid Table 4 Accent 6", UiPriority = 49 };
        var latentStyleExceptionInfo320 = new LatentStyleExceptionInfo
        { Name = "Grid Table 5 Dark Accent 6", UiPriority = 50 };
        var latentStyleExceptionInfo321 = new LatentStyleExceptionInfo
        { Name = "Grid Table 6 Colorful Accent 6", UiPriority = 51 };
        var latentStyleExceptionInfo322 = new LatentStyleExceptionInfo
        { Name = "Grid Table 7 Colorful Accent 6", UiPriority = 52 };
        var latentStyleExceptionInfo323 = new LatentStyleExceptionInfo { Name = "List Table 1 Light", UiPriority = 46 };
        var latentStyleExceptionInfo324 = new LatentStyleExceptionInfo { Name = "List Table 2", UiPriority = 47 };
        var latentStyleExceptionInfo325 = new LatentStyleExceptionInfo { Name = "List Table 3", UiPriority = 48 };
        var latentStyleExceptionInfo326 = new LatentStyleExceptionInfo { Name = "List Table 4", UiPriority = 49 };
        var latentStyleExceptionInfo327 = new LatentStyleExceptionInfo { Name = "List Table 5 Dark", UiPriority = 50 };
        var latentStyleExceptionInfo328 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful", UiPriority = 51 };
        var latentStyleExceptionInfo329 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful", UiPriority = 52 };
        var latentStyleExceptionInfo330 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 1", UiPriority = 46 };
        var latentStyleExceptionInfo331 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 1", UiPriority = 47 };
        var latentStyleExceptionInfo332 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 1", UiPriority = 48 };
        var latentStyleExceptionInfo333 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 1", UiPriority = 49 };
        var latentStyleExceptionInfo334 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 1", UiPriority = 50 };
        var latentStyleExceptionInfo335 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 1", UiPriority = 51 };
        var latentStyleExceptionInfo336 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 1", UiPriority = 52 };
        var latentStyleExceptionInfo337 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 2", UiPriority = 46 };
        var latentStyleExceptionInfo338 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 2", UiPriority = 47 };
        var latentStyleExceptionInfo339 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 2", UiPriority = 48 };
        var latentStyleExceptionInfo340 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 2", UiPriority = 49 };
        var latentStyleExceptionInfo341 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 2", UiPriority = 50 };
        var latentStyleExceptionInfo342 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 2", UiPriority = 51 };
        var latentStyleExceptionInfo343 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 2", UiPriority = 52 };
        var latentStyleExceptionInfo344 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 3", UiPriority = 46 };
        var latentStyleExceptionInfo345 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 3", UiPriority = 47 };
        var latentStyleExceptionInfo346 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 3", UiPriority = 48 };
        var latentStyleExceptionInfo347 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 3", UiPriority = 49 };
        var latentStyleExceptionInfo348 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 3", UiPriority = 50 };
        var latentStyleExceptionInfo349 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 3", UiPriority = 51 };
        var latentStyleExceptionInfo350 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 3", UiPriority = 52 };
        var latentStyleExceptionInfo351 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 4", UiPriority = 46 };
        var latentStyleExceptionInfo352 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 4", UiPriority = 47 };
        var latentStyleExceptionInfo353 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 4", UiPriority = 48 };
        var latentStyleExceptionInfo354 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 4", UiPriority = 49 };
        var latentStyleExceptionInfo355 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 4", UiPriority = 50 };
        var latentStyleExceptionInfo356 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 4", UiPriority = 51 };
        var latentStyleExceptionInfo357 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 4", UiPriority = 52 };
        var latentStyleExceptionInfo358 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 5", UiPriority = 46 };
        var latentStyleExceptionInfo359 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 5", UiPriority = 47 };
        var latentStyleExceptionInfo360 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 5", UiPriority = 48 };
        var latentStyleExceptionInfo361 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 5", UiPriority = 49 };
        var latentStyleExceptionInfo362 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 5", UiPriority = 50 };
        var latentStyleExceptionInfo363 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 5", UiPriority = 51 };
        var latentStyleExceptionInfo364 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 5", UiPriority = 52 };
        var latentStyleExceptionInfo365 = new LatentStyleExceptionInfo
        { Name = "List Table 1 Light Accent 6", UiPriority = 46 };
        var latentStyleExceptionInfo366 = new LatentStyleExceptionInfo
        { Name = "List Table 2 Accent 6", UiPriority = 47 };
        var latentStyleExceptionInfo367 = new LatentStyleExceptionInfo
        { Name = "List Table 3 Accent 6", UiPriority = 48 };
        var latentStyleExceptionInfo368 = new LatentStyleExceptionInfo
        { Name = "List Table 4 Accent 6", UiPriority = 49 };
        var latentStyleExceptionInfo369 = new LatentStyleExceptionInfo
        { Name = "List Table 5 Dark Accent 6", UiPriority = 50 };
        var latentStyleExceptionInfo370 = new LatentStyleExceptionInfo
        { Name = "List Table 6 Colorful Accent 6", UiPriority = 51 };
        var latentStyleExceptionInfo371 = new LatentStyleExceptionInfo
        { Name = "List Table 7 Colorful Accent 6", UiPriority = 52 };
        var latentStyleExceptionInfo372 = new LatentStyleExceptionInfo
        { Name = "Mention", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo373 = new LatentStyleExceptionInfo
        { Name = "Smart Hyperlink", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo374 = new LatentStyleExceptionInfo
        { Name = "Hashtag", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo375 = new LatentStyleExceptionInfo
        { Name = "Unresolved Mention", SemiHidden = true, UnhideWhenUsed = true };
        var latentStyleExceptionInfo376 = new LatentStyleExceptionInfo
        { Name = "Smart Link", SemiHidden = true, UnhideWhenUsed = true };

        latentStyles1.Append(latentStyleExceptionInfo1);
        latentStyles1.Append(latentStyleExceptionInfo2);
        latentStyles1.Append(latentStyleExceptionInfo3);
        latentStyles1.Append(latentStyleExceptionInfo4);
        latentStyles1.Append(latentStyleExceptionInfo5);
        latentStyles1.Append(latentStyleExceptionInfo6);
        latentStyles1.Append(latentStyleExceptionInfo7);
        latentStyles1.Append(latentStyleExceptionInfo8);
        latentStyles1.Append(latentStyleExceptionInfo9);
        latentStyles1.Append(latentStyleExceptionInfo10);
        latentStyles1.Append(latentStyleExceptionInfo11);
        latentStyles1.Append(latentStyleExceptionInfo12);
        latentStyles1.Append(latentStyleExceptionInfo13);
        latentStyles1.Append(latentStyleExceptionInfo14);
        latentStyles1.Append(latentStyleExceptionInfo15);
        latentStyles1.Append(latentStyleExceptionInfo16);
        latentStyles1.Append(latentStyleExceptionInfo17);
        latentStyles1.Append(latentStyleExceptionInfo18);
        latentStyles1.Append(latentStyleExceptionInfo19);
        latentStyles1.Append(latentStyleExceptionInfo20);
        latentStyles1.Append(latentStyleExceptionInfo21);
        latentStyles1.Append(latentStyleExceptionInfo22);
        latentStyles1.Append(latentStyleExceptionInfo23);
        latentStyles1.Append(latentStyleExceptionInfo24);
        latentStyles1.Append(latentStyleExceptionInfo25);
        latentStyles1.Append(latentStyleExceptionInfo26);
        latentStyles1.Append(latentStyleExceptionInfo27);
        latentStyles1.Append(latentStyleExceptionInfo28);
        latentStyles1.Append(latentStyleExceptionInfo29);
        latentStyles1.Append(latentStyleExceptionInfo30);
        latentStyles1.Append(latentStyleExceptionInfo31);
        latentStyles1.Append(latentStyleExceptionInfo32);
        latentStyles1.Append(latentStyleExceptionInfo33);
        latentStyles1.Append(latentStyleExceptionInfo34);
        latentStyles1.Append(latentStyleExceptionInfo35);
        latentStyles1.Append(latentStyleExceptionInfo36);
        latentStyles1.Append(latentStyleExceptionInfo37);
        latentStyles1.Append(latentStyleExceptionInfo38);
        latentStyles1.Append(latentStyleExceptionInfo39);
        latentStyles1.Append(latentStyleExceptionInfo40);
        latentStyles1.Append(latentStyleExceptionInfo41);
        latentStyles1.Append(latentStyleExceptionInfo42);
        latentStyles1.Append(latentStyleExceptionInfo43);
        latentStyles1.Append(latentStyleExceptionInfo44);
        latentStyles1.Append(latentStyleExceptionInfo45);
        latentStyles1.Append(latentStyleExceptionInfo46);
        latentStyles1.Append(latentStyleExceptionInfo47);
        latentStyles1.Append(latentStyleExceptionInfo48);
        latentStyles1.Append(latentStyleExceptionInfo49);
        latentStyles1.Append(latentStyleExceptionInfo50);
        latentStyles1.Append(latentStyleExceptionInfo51);
        latentStyles1.Append(latentStyleExceptionInfo52);
        latentStyles1.Append(latentStyleExceptionInfo53);
        latentStyles1.Append(latentStyleExceptionInfo54);
        latentStyles1.Append(latentStyleExceptionInfo55);
        latentStyles1.Append(latentStyleExceptionInfo56);
        latentStyles1.Append(latentStyleExceptionInfo57);
        latentStyles1.Append(latentStyleExceptionInfo58);
        latentStyles1.Append(latentStyleExceptionInfo59);
        latentStyles1.Append(latentStyleExceptionInfo60);
        latentStyles1.Append(latentStyleExceptionInfo61);
        latentStyles1.Append(latentStyleExceptionInfo62);
        latentStyles1.Append(latentStyleExceptionInfo63);
        latentStyles1.Append(latentStyleExceptionInfo64);
        latentStyles1.Append(latentStyleExceptionInfo65);
        latentStyles1.Append(latentStyleExceptionInfo66);
        latentStyles1.Append(latentStyleExceptionInfo67);
        latentStyles1.Append(latentStyleExceptionInfo68);
        latentStyles1.Append(latentStyleExceptionInfo69);
        latentStyles1.Append(latentStyleExceptionInfo70);
        latentStyles1.Append(latentStyleExceptionInfo71);
        latentStyles1.Append(latentStyleExceptionInfo72);
        latentStyles1.Append(latentStyleExceptionInfo73);
        latentStyles1.Append(latentStyleExceptionInfo74);
        latentStyles1.Append(latentStyleExceptionInfo75);
        latentStyles1.Append(latentStyleExceptionInfo76);
        latentStyles1.Append(latentStyleExceptionInfo77);
        latentStyles1.Append(latentStyleExceptionInfo78);
        latentStyles1.Append(latentStyleExceptionInfo79);
        latentStyles1.Append(latentStyleExceptionInfo80);
        latentStyles1.Append(latentStyleExceptionInfo81);
        latentStyles1.Append(latentStyleExceptionInfo82);
        latentStyles1.Append(latentStyleExceptionInfo83);
        latentStyles1.Append(latentStyleExceptionInfo84);
        latentStyles1.Append(latentStyleExceptionInfo85);
        latentStyles1.Append(latentStyleExceptionInfo86);
        latentStyles1.Append(latentStyleExceptionInfo87);
        latentStyles1.Append(latentStyleExceptionInfo88);
        latentStyles1.Append(latentStyleExceptionInfo89);
        latentStyles1.Append(latentStyleExceptionInfo90);
        latentStyles1.Append(latentStyleExceptionInfo91);
        latentStyles1.Append(latentStyleExceptionInfo92);
        latentStyles1.Append(latentStyleExceptionInfo93);
        latentStyles1.Append(latentStyleExceptionInfo94);
        latentStyles1.Append(latentStyleExceptionInfo95);
        latentStyles1.Append(latentStyleExceptionInfo96);
        latentStyles1.Append(latentStyleExceptionInfo97);
        latentStyles1.Append(latentStyleExceptionInfo98);
        latentStyles1.Append(latentStyleExceptionInfo99);
        latentStyles1.Append(latentStyleExceptionInfo100);
        latentStyles1.Append(latentStyleExceptionInfo101);
        latentStyles1.Append(latentStyleExceptionInfo102);
        latentStyles1.Append(latentStyleExceptionInfo103);
        latentStyles1.Append(latentStyleExceptionInfo104);
        latentStyles1.Append(latentStyleExceptionInfo105);
        latentStyles1.Append(latentStyleExceptionInfo106);
        latentStyles1.Append(latentStyleExceptionInfo107);
        latentStyles1.Append(latentStyleExceptionInfo108);
        latentStyles1.Append(latentStyleExceptionInfo109);
        latentStyles1.Append(latentStyleExceptionInfo110);
        latentStyles1.Append(latentStyleExceptionInfo111);
        latentStyles1.Append(latentStyleExceptionInfo112);
        latentStyles1.Append(latentStyleExceptionInfo113);
        latentStyles1.Append(latentStyleExceptionInfo114);
        latentStyles1.Append(latentStyleExceptionInfo115);
        latentStyles1.Append(latentStyleExceptionInfo116);
        latentStyles1.Append(latentStyleExceptionInfo117);
        latentStyles1.Append(latentStyleExceptionInfo118);
        latentStyles1.Append(latentStyleExceptionInfo119);
        latentStyles1.Append(latentStyleExceptionInfo120);
        latentStyles1.Append(latentStyleExceptionInfo121);
        latentStyles1.Append(latentStyleExceptionInfo122);
        latentStyles1.Append(latentStyleExceptionInfo123);
        latentStyles1.Append(latentStyleExceptionInfo124);
        latentStyles1.Append(latentStyleExceptionInfo125);
        latentStyles1.Append(latentStyleExceptionInfo126);
        latentStyles1.Append(latentStyleExceptionInfo127);
        latentStyles1.Append(latentStyleExceptionInfo128);
        latentStyles1.Append(latentStyleExceptionInfo129);
        latentStyles1.Append(latentStyleExceptionInfo130);
        latentStyles1.Append(latentStyleExceptionInfo131);
        latentStyles1.Append(latentStyleExceptionInfo132);
        latentStyles1.Append(latentStyleExceptionInfo133);
        latentStyles1.Append(latentStyleExceptionInfo134);
        latentStyles1.Append(latentStyleExceptionInfo135);
        latentStyles1.Append(latentStyleExceptionInfo136);
        latentStyles1.Append(latentStyleExceptionInfo137);
        latentStyles1.Append(latentStyleExceptionInfo138);
        latentStyles1.Append(latentStyleExceptionInfo139);
        latentStyles1.Append(latentStyleExceptionInfo140);
        latentStyles1.Append(latentStyleExceptionInfo141);
        latentStyles1.Append(latentStyleExceptionInfo142);
        latentStyles1.Append(latentStyleExceptionInfo143);
        latentStyles1.Append(latentStyleExceptionInfo144);
        latentStyles1.Append(latentStyleExceptionInfo145);
        latentStyles1.Append(latentStyleExceptionInfo146);
        latentStyles1.Append(latentStyleExceptionInfo147);
        latentStyles1.Append(latentStyleExceptionInfo148);
        latentStyles1.Append(latentStyleExceptionInfo149);
        latentStyles1.Append(latentStyleExceptionInfo150);
        latentStyles1.Append(latentStyleExceptionInfo151);
        latentStyles1.Append(latentStyleExceptionInfo152);
        latentStyles1.Append(latentStyleExceptionInfo153);
        latentStyles1.Append(latentStyleExceptionInfo154);
        latentStyles1.Append(latentStyleExceptionInfo155);
        latentStyles1.Append(latentStyleExceptionInfo156);
        latentStyles1.Append(latentStyleExceptionInfo157);
        latentStyles1.Append(latentStyleExceptionInfo158);
        latentStyles1.Append(latentStyleExceptionInfo159);
        latentStyles1.Append(latentStyleExceptionInfo160);
        latentStyles1.Append(latentStyleExceptionInfo161);
        latentStyles1.Append(latentStyleExceptionInfo162);
        latentStyles1.Append(latentStyleExceptionInfo163);
        latentStyles1.Append(latentStyleExceptionInfo164);
        latentStyles1.Append(latentStyleExceptionInfo165);
        latentStyles1.Append(latentStyleExceptionInfo166);
        latentStyles1.Append(latentStyleExceptionInfo167);
        latentStyles1.Append(latentStyleExceptionInfo168);
        latentStyles1.Append(latentStyleExceptionInfo169);
        latentStyles1.Append(latentStyleExceptionInfo170);
        latentStyles1.Append(latentStyleExceptionInfo171);
        latentStyles1.Append(latentStyleExceptionInfo172);
        latentStyles1.Append(latentStyleExceptionInfo173);
        latentStyles1.Append(latentStyleExceptionInfo174);
        latentStyles1.Append(latentStyleExceptionInfo175);
        latentStyles1.Append(latentStyleExceptionInfo176);
        latentStyles1.Append(latentStyleExceptionInfo177);
        latentStyles1.Append(latentStyleExceptionInfo178);
        latentStyles1.Append(latentStyleExceptionInfo179);
        latentStyles1.Append(latentStyleExceptionInfo180);
        latentStyles1.Append(latentStyleExceptionInfo181);
        latentStyles1.Append(latentStyleExceptionInfo182);
        latentStyles1.Append(latentStyleExceptionInfo183);
        latentStyles1.Append(latentStyleExceptionInfo184);
        latentStyles1.Append(latentStyleExceptionInfo185);
        latentStyles1.Append(latentStyleExceptionInfo186);
        latentStyles1.Append(latentStyleExceptionInfo187);
        latentStyles1.Append(latentStyleExceptionInfo188);
        latentStyles1.Append(latentStyleExceptionInfo189);
        latentStyles1.Append(latentStyleExceptionInfo190);
        latentStyles1.Append(latentStyleExceptionInfo191);
        latentStyles1.Append(latentStyleExceptionInfo192);
        latentStyles1.Append(latentStyleExceptionInfo193);
        latentStyles1.Append(latentStyleExceptionInfo194);
        latentStyles1.Append(latentStyleExceptionInfo195);
        latentStyles1.Append(latentStyleExceptionInfo196);
        latentStyles1.Append(latentStyleExceptionInfo197);
        latentStyles1.Append(latentStyleExceptionInfo198);
        latentStyles1.Append(latentStyleExceptionInfo199);
        latentStyles1.Append(latentStyleExceptionInfo200);
        latentStyles1.Append(latentStyleExceptionInfo201);
        latentStyles1.Append(latentStyleExceptionInfo202);
        latentStyles1.Append(latentStyleExceptionInfo203);
        latentStyles1.Append(latentStyleExceptionInfo204);
        latentStyles1.Append(latentStyleExceptionInfo205);
        latentStyles1.Append(latentStyleExceptionInfo206);
        latentStyles1.Append(latentStyleExceptionInfo207);
        latentStyles1.Append(latentStyleExceptionInfo208);
        latentStyles1.Append(latentStyleExceptionInfo209);
        latentStyles1.Append(latentStyleExceptionInfo210);
        latentStyles1.Append(latentStyleExceptionInfo211);
        latentStyles1.Append(latentStyleExceptionInfo212);
        latentStyles1.Append(latentStyleExceptionInfo213);
        latentStyles1.Append(latentStyleExceptionInfo214);
        latentStyles1.Append(latentStyleExceptionInfo215);
        latentStyles1.Append(latentStyleExceptionInfo216);
        latentStyles1.Append(latentStyleExceptionInfo217);
        latentStyles1.Append(latentStyleExceptionInfo218);
        latentStyles1.Append(latentStyleExceptionInfo219);
        latentStyles1.Append(latentStyleExceptionInfo220);
        latentStyles1.Append(latentStyleExceptionInfo221);
        latentStyles1.Append(latentStyleExceptionInfo222);
        latentStyles1.Append(latentStyleExceptionInfo223);
        latentStyles1.Append(latentStyleExceptionInfo224);
        latentStyles1.Append(latentStyleExceptionInfo225);
        latentStyles1.Append(latentStyleExceptionInfo226);
        latentStyles1.Append(latentStyleExceptionInfo227);
        latentStyles1.Append(latentStyleExceptionInfo228);
        latentStyles1.Append(latentStyleExceptionInfo229);
        latentStyles1.Append(latentStyleExceptionInfo230);
        latentStyles1.Append(latentStyleExceptionInfo231);
        latentStyles1.Append(latentStyleExceptionInfo232);
        latentStyles1.Append(latentStyleExceptionInfo233);
        latentStyles1.Append(latentStyleExceptionInfo234);
        latentStyles1.Append(latentStyleExceptionInfo235);
        latentStyles1.Append(latentStyleExceptionInfo236);
        latentStyles1.Append(latentStyleExceptionInfo237);
        latentStyles1.Append(latentStyleExceptionInfo238);
        latentStyles1.Append(latentStyleExceptionInfo239);
        latentStyles1.Append(latentStyleExceptionInfo240);
        latentStyles1.Append(latentStyleExceptionInfo241);
        latentStyles1.Append(latentStyleExceptionInfo242);
        latentStyles1.Append(latentStyleExceptionInfo243);
        latentStyles1.Append(latentStyleExceptionInfo244);
        latentStyles1.Append(latentStyleExceptionInfo245);
        latentStyles1.Append(latentStyleExceptionInfo246);
        latentStyles1.Append(latentStyleExceptionInfo247);
        latentStyles1.Append(latentStyleExceptionInfo248);
        latentStyles1.Append(latentStyleExceptionInfo249);
        latentStyles1.Append(latentStyleExceptionInfo250);
        latentStyles1.Append(latentStyleExceptionInfo251);
        latentStyles1.Append(latentStyleExceptionInfo252);
        latentStyles1.Append(latentStyleExceptionInfo253);
        latentStyles1.Append(latentStyleExceptionInfo254);
        latentStyles1.Append(latentStyleExceptionInfo255);
        latentStyles1.Append(latentStyleExceptionInfo256);
        latentStyles1.Append(latentStyleExceptionInfo257);
        latentStyles1.Append(latentStyleExceptionInfo258);
        latentStyles1.Append(latentStyleExceptionInfo259);
        latentStyles1.Append(latentStyleExceptionInfo260);
        latentStyles1.Append(latentStyleExceptionInfo261);
        latentStyles1.Append(latentStyleExceptionInfo262);
        latentStyles1.Append(latentStyleExceptionInfo263);
        latentStyles1.Append(latentStyleExceptionInfo264);
        latentStyles1.Append(latentStyleExceptionInfo265);
        latentStyles1.Append(latentStyleExceptionInfo266);
        latentStyles1.Append(latentStyleExceptionInfo267);
        latentStyles1.Append(latentStyleExceptionInfo268);
        latentStyles1.Append(latentStyleExceptionInfo269);
        latentStyles1.Append(latentStyleExceptionInfo270);
        latentStyles1.Append(latentStyleExceptionInfo271);
        latentStyles1.Append(latentStyleExceptionInfo272);
        latentStyles1.Append(latentStyleExceptionInfo273);
        latentStyles1.Append(latentStyleExceptionInfo274);
        latentStyles1.Append(latentStyleExceptionInfo275);
        latentStyles1.Append(latentStyleExceptionInfo276);
        latentStyles1.Append(latentStyleExceptionInfo277);
        latentStyles1.Append(latentStyleExceptionInfo278);
        latentStyles1.Append(latentStyleExceptionInfo279);
        latentStyles1.Append(latentStyleExceptionInfo280);
        latentStyles1.Append(latentStyleExceptionInfo281);
        latentStyles1.Append(latentStyleExceptionInfo282);
        latentStyles1.Append(latentStyleExceptionInfo283);
        latentStyles1.Append(latentStyleExceptionInfo284);
        latentStyles1.Append(latentStyleExceptionInfo285);
        latentStyles1.Append(latentStyleExceptionInfo286);
        latentStyles1.Append(latentStyleExceptionInfo287);
        latentStyles1.Append(latentStyleExceptionInfo288);
        latentStyles1.Append(latentStyleExceptionInfo289);
        latentStyles1.Append(latentStyleExceptionInfo290);
        latentStyles1.Append(latentStyleExceptionInfo291);
        latentStyles1.Append(latentStyleExceptionInfo292);
        latentStyles1.Append(latentStyleExceptionInfo293);
        latentStyles1.Append(latentStyleExceptionInfo294);
        latentStyles1.Append(latentStyleExceptionInfo295);
        latentStyles1.Append(latentStyleExceptionInfo296);
        latentStyles1.Append(latentStyleExceptionInfo297);
        latentStyles1.Append(latentStyleExceptionInfo298);
        latentStyles1.Append(latentStyleExceptionInfo299);
        latentStyles1.Append(latentStyleExceptionInfo300);
        latentStyles1.Append(latentStyleExceptionInfo301);
        latentStyles1.Append(latentStyleExceptionInfo302);
        latentStyles1.Append(latentStyleExceptionInfo303);
        latentStyles1.Append(latentStyleExceptionInfo304);
        latentStyles1.Append(latentStyleExceptionInfo305);
        latentStyles1.Append(latentStyleExceptionInfo306);
        latentStyles1.Append(latentStyleExceptionInfo307);
        latentStyles1.Append(latentStyleExceptionInfo308);
        latentStyles1.Append(latentStyleExceptionInfo309);
        latentStyles1.Append(latentStyleExceptionInfo310);
        latentStyles1.Append(latentStyleExceptionInfo311);
        latentStyles1.Append(latentStyleExceptionInfo312);
        latentStyles1.Append(latentStyleExceptionInfo313);
        latentStyles1.Append(latentStyleExceptionInfo314);
        latentStyles1.Append(latentStyleExceptionInfo315);
        latentStyles1.Append(latentStyleExceptionInfo316);
        latentStyles1.Append(latentStyleExceptionInfo317);
        latentStyles1.Append(latentStyleExceptionInfo318);
        latentStyles1.Append(latentStyleExceptionInfo319);
        latentStyles1.Append(latentStyleExceptionInfo320);
        latentStyles1.Append(latentStyleExceptionInfo321);
        latentStyles1.Append(latentStyleExceptionInfo322);
        latentStyles1.Append(latentStyleExceptionInfo323);
        latentStyles1.Append(latentStyleExceptionInfo324);
        latentStyles1.Append(latentStyleExceptionInfo325);
        latentStyles1.Append(latentStyleExceptionInfo326);
        latentStyles1.Append(latentStyleExceptionInfo327);
        latentStyles1.Append(latentStyleExceptionInfo328);
        latentStyles1.Append(latentStyleExceptionInfo329);
        latentStyles1.Append(latentStyleExceptionInfo330);
        latentStyles1.Append(latentStyleExceptionInfo331);
        latentStyles1.Append(latentStyleExceptionInfo332);
        latentStyles1.Append(latentStyleExceptionInfo333);
        latentStyles1.Append(latentStyleExceptionInfo334);
        latentStyles1.Append(latentStyleExceptionInfo335);
        latentStyles1.Append(latentStyleExceptionInfo336);
        latentStyles1.Append(latentStyleExceptionInfo337);
        latentStyles1.Append(latentStyleExceptionInfo338);
        latentStyles1.Append(latentStyleExceptionInfo339);
        latentStyles1.Append(latentStyleExceptionInfo340);
        latentStyles1.Append(latentStyleExceptionInfo341);
        latentStyles1.Append(latentStyleExceptionInfo342);
        latentStyles1.Append(latentStyleExceptionInfo343);
        latentStyles1.Append(latentStyleExceptionInfo344);
        latentStyles1.Append(latentStyleExceptionInfo345);
        latentStyles1.Append(latentStyleExceptionInfo346);
        latentStyles1.Append(latentStyleExceptionInfo347);
        latentStyles1.Append(latentStyleExceptionInfo348);
        latentStyles1.Append(latentStyleExceptionInfo349);
        latentStyles1.Append(latentStyleExceptionInfo350);
        latentStyles1.Append(latentStyleExceptionInfo351);
        latentStyles1.Append(latentStyleExceptionInfo352);
        latentStyles1.Append(latentStyleExceptionInfo353);
        latentStyles1.Append(latentStyleExceptionInfo354);
        latentStyles1.Append(latentStyleExceptionInfo355);
        latentStyles1.Append(latentStyleExceptionInfo356);
        latentStyles1.Append(latentStyleExceptionInfo357);
        latentStyles1.Append(latentStyleExceptionInfo358);
        latentStyles1.Append(latentStyleExceptionInfo359);
        latentStyles1.Append(latentStyleExceptionInfo360);
        latentStyles1.Append(latentStyleExceptionInfo361);
        latentStyles1.Append(latentStyleExceptionInfo362);
        latentStyles1.Append(latentStyleExceptionInfo363);
        latentStyles1.Append(latentStyleExceptionInfo364);
        latentStyles1.Append(latentStyleExceptionInfo365);
        latentStyles1.Append(latentStyleExceptionInfo366);
        latentStyles1.Append(latentStyleExceptionInfo367);
        latentStyles1.Append(latentStyleExceptionInfo368);
        latentStyles1.Append(latentStyleExceptionInfo369);
        latentStyles1.Append(latentStyleExceptionInfo370);
        latentStyles1.Append(latentStyleExceptionInfo371);
        latentStyles1.Append(latentStyleExceptionInfo372);
        latentStyles1.Append(latentStyleExceptionInfo373);
        latentStyles1.Append(latentStyleExceptionInfo374);
        latentStyles1.Append(latentStyleExceptionInfo375);
        latentStyles1.Append(latentStyleExceptionInfo376);

        var style1 = new Style { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true };
        var styleName1 = new StyleName { Val = "Normal" };
        var primaryStyle1 = new PrimaryStyle();

        style1.Append(styleName1);
        style1.Append(primaryStyle1);

        var style2 = new Style { Type = StyleValues.Character, StyleId = "DefaultParagraphFont", Default = true };
        var styleName2 = new StyleName { Val = "Default Paragraph Font" };
        var uIPriority1 = new UIPriority { Val = 1 };
        var semiHidden1 = new SemiHidden();
        var unhideWhenUsed1 = new UnhideWhenUsed();

        style2.Append(styleName2);
        style2.Append(uIPriority1);
        style2.Append(semiHidden1);
        style2.Append(unhideWhenUsed1);

        var style3 = new Style { Type = StyleValues.Table, StyleId = "TableNormal", Default = true };
        var styleName3 = new StyleName { Val = "Normal Table" };
        var uIPriority2 = new UIPriority { Val = 99 };
        var semiHidden2 = new SemiHidden();
        var unhideWhenUsed2 = new UnhideWhenUsed();

        var styleTableProperties1 = new StyleTableProperties();
        var tableIndentation1 = new TableIndentation { Width = 0, Type = TableWidthUnitValues.Dxa };

        var tableCellMarginDefault1 = new TableCellMarginDefault();
        var topMargin1 = new TopMargin { Width = "0", Type = TableWidthUnitValues.Dxa };
        var tableCellLeftMargin1 = new TableCellLeftMargin { Width = 108, Type = TableWidthValues.Dxa };
        var bottomMargin1 = new BottomMargin { Width = "0", Type = TableWidthUnitValues.Dxa };
        var tableCellRightMargin1 = new TableCellRightMargin { Width = 108, Type = TableWidthValues.Dxa };

        tableCellMarginDefault1.Append(topMargin1);
        tableCellMarginDefault1.Append(tableCellLeftMargin1);
        tableCellMarginDefault1.Append(bottomMargin1);
        tableCellMarginDefault1.Append(tableCellRightMargin1);

        styleTableProperties1.Append(tableIndentation1);
        styleTableProperties1.Append(tableCellMarginDefault1);

        style3.Append(styleName3);
        style3.Append(uIPriority2);
        style3.Append(semiHidden2);
        style3.Append(unhideWhenUsed2);
        style3.Append(styleTableProperties1);

        var style4 = new Style { Type = StyleValues.Numbering, StyleId = "NoList", Default = true };
        var styleName4 = new StyleName { Val = "No List" };
        var uIPriority3 = new UIPriority { Val = 99 };
        var semiHidden3 = new SemiHidden();
        var unhideWhenUsed3 = new UnhideWhenUsed();

        style4.Append(styleName4);
        style4.Append(uIPriority3);
        style4.Append(semiHidden3);
        style4.Append(unhideWhenUsed3);

        var style5 = new Style { Type = StyleValues.Character, StyleId = "Hyperlink" };
        var styleName5 = new StyleName { Val = "Hyperlink" };
        var basedOn1 = new BasedOn { Val = "DefaultParagraphFont" };
        var uIPriority4 = new UIPriority { Val = 99 };
        var unhideWhenUsed4 = new UnhideWhenUsed();
        var rsid6 = new Rsid { Val = "00032EBE" };

        var styleRunProperties1 = new StyleRunProperties();
        var color1 = new Color { Val = "0563C1", ThemeColor = ThemeColorValues.Hyperlink };
        var underline1 = new Underline { Val = UnderlineValues.Single };

        styleRunProperties1.Append(color1);
        styleRunProperties1.Append(underline1);

        style5.Append(styleName5);
        style5.Append(basedOn1);
        style5.Append(uIPriority4);
        style5.Append(unhideWhenUsed4);
        style5.Append(rsid6);
        style5.Append(styleRunProperties1);

        var style6 = new Style { Type = StyleValues.Character, StyleId = "UnresolvedMention" };
        var styleName6 = new StyleName { Val = "Unresolved Mention" };
        var basedOn2 = new BasedOn { Val = "DefaultParagraphFont" };
        var uIPriority5 = new UIPriority { Val = 99 };
        var semiHidden4 = new SemiHidden();
        var unhideWhenUsed5 = new UnhideWhenUsed();
        var rsid7 = new Rsid { Val = "00032EBE" };

        var styleRunProperties2 = new StyleRunProperties();
        var color2 = new Color { Val = "605E5C" };
        var shading1 = new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "E1DFDD" };

        styleRunProperties2.Append(color2);
        styleRunProperties2.Append(shading1);

        style6.Append(styleName6);
        style6.Append(basedOn2);
        style6.Append(uIPriority5);
        style6.Append(semiHidden4);
        style6.Append(unhideWhenUsed5);
        style6.Append(rsid7);
        style6.Append(styleRunProperties2);

        var style7 = new Style { Type = StyleValues.Table, StyleId = "TableGrid" };
        var styleName7 = new StyleName { Val = "Table Grid" };
        var basedOn3 = new BasedOn { Val = "TableNormal" };
        var uIPriority6 = new UIPriority { Val = 39 };
        var rsid8 = new Rsid { Val = "00A11BEE" };

        var styleParagraphProperties1 = new StyleParagraphProperties();
        var spacingBetweenLines2 = new SpacingBetweenLines
        { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto };

        styleParagraphProperties1.Append(spacingBetweenLines2);

        var styleTableProperties2 = new StyleTableProperties();

        var tableBorders1 = new TableBorders();
        var topBorder1 = new TopBorder { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };
        var leftBorder1 = new LeftBorder { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };
        var bottomBorder1 = new BottomBorder { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };
        var rightBorder1 = new RightBorder { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };
        var insideHorizontalBorder1 = new InsideHorizontalBorder
        { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };
        var insideVerticalBorder1 = new InsideVerticalBorder
        { Val = BorderValues.Single, Color = "auto", Size = 4U, Space = 0U };

        tableBorders1.Append(topBorder1);
        tableBorders1.Append(leftBorder1);
        tableBorders1.Append(bottomBorder1);
        tableBorders1.Append(rightBorder1);
        tableBorders1.Append(insideHorizontalBorder1);
        tableBorders1.Append(insideVerticalBorder1);

        styleTableProperties2.Append(tableBorders1);

        style7.Append(styleName7);
        style7.Append(basedOn3);
        style7.Append(uIPriority6);
        style7.Append(rsid8);
        style7.Append(styleParagraphProperties1);
        style7.Append(styleTableProperties2);

        styles1.Append(docDefaults1);
        styles1.Append(latentStyles1);
        styles1.Append(style1);
        styles1.Append(style2);
        styles1.Append(style3);
        styles1.Append(style4);
        styles1.Append(style5);
        styles1.Append(style6);
        styles1.Append(style7);

        styleDefinitionsPart1.Styles = styles1;
    }

    // Generates content of themePart1.
    private static void GenerateThemePart1Content(ThemePart themePart1)
    {
        var theme1 = new A.Theme { Name = "Office Theme" };
        theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

        var themeElements1 = new A.ThemeElements();

        var colorScheme1 = new A.ColorScheme { Name = "Office" };

        var dark1Color1 = new A.Dark1Color();
        var systemColor1 = new A.SystemColor { Val = A.SystemColorValues.WindowText, LastColor = "000000" };

        dark1Color1.Append(systemColor1);

        var light1Color1 = new A.Light1Color();
        var systemColor2 = new A.SystemColor { Val = A.SystemColorValues.Window, LastColor = "FFFFFF" };

        light1Color1.Append(systemColor2);

        var dark2Color1 = new A.Dark2Color();
        var rgbColorModelHex1 = new A.RgbColorModelHex { Val = "44546A" };

        dark2Color1.Append(rgbColorModelHex1);

        var light2Color1 = new A.Light2Color();
        var rgbColorModelHex2 = new A.RgbColorModelHex { Val = "E7E6E6" };

        light2Color1.Append(rgbColorModelHex2);

        var accent1Color1 = new A.Accent1Color();
        var rgbColorModelHex3 = new A.RgbColorModelHex { Val = "4472C4" };

        accent1Color1.Append(rgbColorModelHex3);

        var accent2Color1 = new A.Accent2Color();
        var rgbColorModelHex4 = new A.RgbColorModelHex { Val = "ED7D31" };

        accent2Color1.Append(rgbColorModelHex4);

        var accent3Color1 = new A.Accent3Color();
        var rgbColorModelHex5 = new A.RgbColorModelHex { Val = "A5A5A5" };

        accent3Color1.Append(rgbColorModelHex5);

        var accent4Color1 = new A.Accent4Color();
        var rgbColorModelHex6 = new A.RgbColorModelHex { Val = "FFC000" };

        accent4Color1.Append(rgbColorModelHex6);

        var accent5Color1 = new A.Accent5Color();
        var rgbColorModelHex7 = new A.RgbColorModelHex { Val = "5B9BD5" };

        accent5Color1.Append(rgbColorModelHex7);

        var accent6Color1 = new A.Accent6Color();
        var rgbColorModelHex8 = new A.RgbColorModelHex { Val = "70AD47" };

        accent6Color1.Append(rgbColorModelHex8);

        var hyperlink1 = new A.Hyperlink();
        var rgbColorModelHex9 = new A.RgbColorModelHex { Val = "0563C1" };

        hyperlink1.Append(rgbColorModelHex9);

        var followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
        var rgbColorModelHex10 = new A.RgbColorModelHex { Val = "954F72" };

        followedHyperlinkColor1.Append(rgbColorModelHex10);

        colorScheme1.Append(dark1Color1);
        colorScheme1.Append(light1Color1);
        colorScheme1.Append(dark2Color1);
        colorScheme1.Append(light2Color1);
        colorScheme1.Append(accent1Color1);
        colorScheme1.Append(accent2Color1);
        colorScheme1.Append(accent3Color1);
        colorScheme1.Append(accent4Color1);
        colorScheme1.Append(accent5Color1);
        colorScheme1.Append(accent6Color1);
        colorScheme1.Append(hyperlink1);
        colorScheme1.Append(followedHyperlinkColor1);

        var fontScheme1 = new A.FontScheme { Name = "Office" };

        var majorFont1 = new A.MajorFont();
        var latinFont1 = new A.LatinFont { Typeface = "Calibri Light", Panose = "020F0302020204030204" };
        var eastAsianFont1 = new A.EastAsianFont { Typeface = "" };
        var complexScriptFont1 = new A.ComplexScriptFont { Typeface = "" };
        var supplementalFont1 = new A.SupplementalFont { Script = "Jpan", Typeface = "??????????????? Light" };
        var supplementalFont2 = new A.SupplementalFont { Script = "Hang", Typeface = "?????? ??????" };
        var supplementalFont3 = new A.SupplementalFont { Script = "Hans", Typeface = "?????? Light" };
        var supplementalFont4 = new A.SupplementalFont { Script = "Hant", Typeface = "????????????" };
        var supplementalFont5 = new A.SupplementalFont { Script = "Arab", Typeface = "Times New Roman" };
        var supplementalFont6 = new A.SupplementalFont { Script = "Hebr", Typeface = "Times New Roman" };
        var supplementalFont7 = new A.SupplementalFont { Script = "Thai", Typeface = "Angsana New" };
        var supplementalFont8 = new A.SupplementalFont { Script = "Ethi", Typeface = "Nyala" };
        var supplementalFont9 = new A.SupplementalFont { Script = "Beng", Typeface = "Vrinda" };
        var supplementalFont10 = new A.SupplementalFont { Script = "Gujr", Typeface = "Shruti" };
        var supplementalFont11 = new A.SupplementalFont { Script = "Khmr", Typeface = "MoolBoran" };
        var supplementalFont12 = new A.SupplementalFont { Script = "Knda", Typeface = "Tunga" };
        var supplementalFont13 = new A.SupplementalFont { Script = "Guru", Typeface = "Raavi" };
        var supplementalFont14 = new A.SupplementalFont { Script = "Cans", Typeface = "Euphemia" };
        var supplementalFont15 = new A.SupplementalFont { Script = "Cher", Typeface = "Plantagenet Cherokee" };
        var supplementalFont16 = new A.SupplementalFont { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
        var supplementalFont17 = new A.SupplementalFont { Script = "Tibt", Typeface = "Microsoft Himalaya" };
        var supplementalFont18 = new A.SupplementalFont { Script = "Thaa", Typeface = "MV Boli" };
        var supplementalFont19 = new A.SupplementalFont { Script = "Deva", Typeface = "Mangal" };
        var supplementalFont20 = new A.SupplementalFont { Script = "Telu", Typeface = "Gautami" };
        var supplementalFont21 = new A.SupplementalFont { Script = "Taml", Typeface = "Latha" };
        var supplementalFont22 = new A.SupplementalFont { Script = "Syrc", Typeface = "Estrangelo Edessa" };
        var supplementalFont23 = new A.SupplementalFont { Script = "Orya", Typeface = "Kalinga" };
        var supplementalFont24 = new A.SupplementalFont { Script = "Mlym", Typeface = "Kartika" };
        var supplementalFont25 = new A.SupplementalFont { Script = "Laoo", Typeface = "DokChampa" };
        var supplementalFont26 = new A.SupplementalFont { Script = "Sinh", Typeface = "Iskoola Pota" };
        var supplementalFont27 = new A.SupplementalFont { Script = "Mong", Typeface = "Mongolian Baiti" };
        var supplementalFont28 = new A.SupplementalFont { Script = "Viet", Typeface = "Times New Roman" };
        var supplementalFont29 = new A.SupplementalFont { Script = "Uigh", Typeface = "Microsoft Uighur" };
        var supplementalFont30 = new A.SupplementalFont { Script = "Geor", Typeface = "Sylfaen" };
        var supplementalFont31 = new A.SupplementalFont { Script = "Armn", Typeface = "Arial" };
        var supplementalFont32 = new A.SupplementalFont { Script = "Bugi", Typeface = "Leelawadee UI" };
        var supplementalFont33 = new A.SupplementalFont { Script = "Bopo", Typeface = "Microsoft JhengHei" };
        var supplementalFont34 = new A.SupplementalFont { Script = "Java", Typeface = "Javanese Text" };
        var supplementalFont35 = new A.SupplementalFont { Script = "Lisu", Typeface = "Segoe UI" };
        var supplementalFont36 = new A.SupplementalFont { Script = "Mymr", Typeface = "Myanmar Text" };
        var supplementalFont37 = new A.SupplementalFont { Script = "Nkoo", Typeface = "Ebrima" };
        var supplementalFont38 = new A.SupplementalFont { Script = "Olck", Typeface = "Nirmala UI" };
        var supplementalFont39 = new A.SupplementalFont { Script = "Osma", Typeface = "Ebrima" };
        var supplementalFont40 = new A.SupplementalFont { Script = "Phag", Typeface = "Phagspa" };
        var supplementalFont41 = new A.SupplementalFont { Script = "Syrn", Typeface = "Estrangelo Edessa" };
        var supplementalFont42 = new A.SupplementalFont { Script = "Syrj", Typeface = "Estrangelo Edessa" };
        var supplementalFont43 = new A.SupplementalFont { Script = "Syre", Typeface = "Estrangelo Edessa" };
        var supplementalFont44 = new A.SupplementalFont { Script = "Sora", Typeface = "Nirmala UI" };
        var supplementalFont45 = new A.SupplementalFont { Script = "Tale", Typeface = "Microsoft Tai Le" };
        var supplementalFont46 = new A.SupplementalFont { Script = "Talu", Typeface = "Microsoft New Tai Lue" };
        var supplementalFont47 = new A.SupplementalFont { Script = "Tfng", Typeface = "Ebrima" };

        majorFont1.Append(latinFont1);
        majorFont1.Append(eastAsianFont1);
        majorFont1.Append(complexScriptFont1);
        majorFont1.Append(supplementalFont1);
        majorFont1.Append(supplementalFont2);
        majorFont1.Append(supplementalFont3);
        majorFont1.Append(supplementalFont4);
        majorFont1.Append(supplementalFont5);
        majorFont1.Append(supplementalFont6);
        majorFont1.Append(supplementalFont7);
        majorFont1.Append(supplementalFont8);
        majorFont1.Append(supplementalFont9);
        majorFont1.Append(supplementalFont10);
        majorFont1.Append(supplementalFont11);
        majorFont1.Append(supplementalFont12);
        majorFont1.Append(supplementalFont13);
        majorFont1.Append(supplementalFont14);
        majorFont1.Append(supplementalFont15);
        majorFont1.Append(supplementalFont16);
        majorFont1.Append(supplementalFont17);
        majorFont1.Append(supplementalFont18);
        majorFont1.Append(supplementalFont19);
        majorFont1.Append(supplementalFont20);
        majorFont1.Append(supplementalFont21);
        majorFont1.Append(supplementalFont22);
        majorFont1.Append(supplementalFont23);
        majorFont1.Append(supplementalFont24);
        majorFont1.Append(supplementalFont25);
        majorFont1.Append(supplementalFont26);
        majorFont1.Append(supplementalFont27);
        majorFont1.Append(supplementalFont28);
        majorFont1.Append(supplementalFont29);
        majorFont1.Append(supplementalFont30);
        majorFont1.Append(supplementalFont31);
        majorFont1.Append(supplementalFont32);
        majorFont1.Append(supplementalFont33);
        majorFont1.Append(supplementalFont34);
        majorFont1.Append(supplementalFont35);
        majorFont1.Append(supplementalFont36);
        majorFont1.Append(supplementalFont37);
        majorFont1.Append(supplementalFont38);
        majorFont1.Append(supplementalFont39);
        majorFont1.Append(supplementalFont40);
        majorFont1.Append(supplementalFont41);
        majorFont1.Append(supplementalFont42);
        majorFont1.Append(supplementalFont43);
        majorFont1.Append(supplementalFont44);
        majorFont1.Append(supplementalFont45);
        majorFont1.Append(supplementalFont46);
        majorFont1.Append(supplementalFont47);

        var minorFont1 = new A.MinorFont();
        var latinFont2 = new A.LatinFont { Typeface = "Calibri", Panose = "020F0502020204030204" };
        var eastAsianFont2 = new A.EastAsianFont { Typeface = "" };
        var complexScriptFont2 = new A.ComplexScriptFont { Typeface = "" };
        var supplementalFont48 = new A.SupplementalFont { Script = "Jpan", Typeface = "?????????" };
        var supplementalFont49 = new A.SupplementalFont { Script = "Hang", Typeface = "?????? ??????" };
        var supplementalFont50 = new A.SupplementalFont { Script = "Hans", Typeface = "??????" };
        var supplementalFont51 = new A.SupplementalFont { Script = "Hant", Typeface = "????????????" };
        var supplementalFont52 = new A.SupplementalFont { Script = "Arab", Typeface = "Arial" };
        var supplementalFont53 = new A.SupplementalFont { Script = "Hebr", Typeface = "Arial" };
        var supplementalFont54 = new A.SupplementalFont { Script = "Thai", Typeface = "Cordia New" };
        var supplementalFont55 = new A.SupplementalFont { Script = "Ethi", Typeface = "Nyala" };
        var supplementalFont56 = new A.SupplementalFont { Script = "Beng", Typeface = "Vrinda" };
        var supplementalFont57 = new A.SupplementalFont { Script = "Gujr", Typeface = "Shruti" };
        var supplementalFont58 = new A.SupplementalFont { Script = "Khmr", Typeface = "DaunPenh" };
        var supplementalFont59 = new A.SupplementalFont { Script = "Knda", Typeface = "Tunga" };
        var supplementalFont60 = new A.SupplementalFont { Script = "Guru", Typeface = "Raavi" };
        var supplementalFont61 = new A.SupplementalFont { Script = "Cans", Typeface = "Euphemia" };
        var supplementalFont62 = new A.SupplementalFont { Script = "Cher", Typeface = "Plantagenet Cherokee" };
        var supplementalFont63 = new A.SupplementalFont { Script = "Yiii", Typeface = "Microsoft Yi Baiti" };
        var supplementalFont64 = new A.SupplementalFont { Script = "Tibt", Typeface = "Microsoft Himalaya" };
        var supplementalFont65 = new A.SupplementalFont { Script = "Thaa", Typeface = "MV Boli" };
        var supplementalFont66 = new A.SupplementalFont { Script = "Deva", Typeface = "Mangal" };
        var supplementalFont67 = new A.SupplementalFont { Script = "Telu", Typeface = "Gautami" };
        var supplementalFont68 = new A.SupplementalFont { Script = "Taml", Typeface = "Latha" };
        var supplementalFont69 = new A.SupplementalFont { Script = "Syrc", Typeface = "Estrangelo Edessa" };
        var supplementalFont70 = new A.SupplementalFont { Script = "Orya", Typeface = "Kalinga" };
        var supplementalFont71 = new A.SupplementalFont { Script = "Mlym", Typeface = "Kartika" };
        var supplementalFont72 = new A.SupplementalFont { Script = "Laoo", Typeface = "DokChampa" };
        var supplementalFont73 = new A.SupplementalFont { Script = "Sinh", Typeface = "Iskoola Pota" };
        var supplementalFont74 = new A.SupplementalFont { Script = "Mong", Typeface = "Mongolian Baiti" };
        var supplementalFont75 = new A.SupplementalFont { Script = "Viet", Typeface = "Arial" };
        var supplementalFont76 = new A.SupplementalFont { Script = "Uigh", Typeface = "Microsoft Uighur" };
        var supplementalFont77 = new A.SupplementalFont { Script = "Geor", Typeface = "Sylfaen" };
        var supplementalFont78 = new A.SupplementalFont { Script = "Armn", Typeface = "Arial" };
        var supplementalFont79 = new A.SupplementalFont { Script = "Bugi", Typeface = "Leelawadee UI" };
        var supplementalFont80 = new A.SupplementalFont { Script = "Bopo", Typeface = "Microsoft JhengHei" };
        var supplementalFont81 = new A.SupplementalFont { Script = "Java", Typeface = "Javanese Text" };
        var supplementalFont82 = new A.SupplementalFont { Script = "Lisu", Typeface = "Segoe UI" };
        var supplementalFont83 = new A.SupplementalFont { Script = "Mymr", Typeface = "Myanmar Text" };
        var supplementalFont84 = new A.SupplementalFont { Script = "Nkoo", Typeface = "Ebrima" };
        var supplementalFont85 = new A.SupplementalFont { Script = "Olck", Typeface = "Nirmala UI" };
        var supplementalFont86 = new A.SupplementalFont { Script = "Osma", Typeface = "Ebrima" };
        var supplementalFont87 = new A.SupplementalFont { Script = "Phag", Typeface = "Phagspa" };
        var supplementalFont88 = new A.SupplementalFont { Script = "Syrn", Typeface = "Estrangelo Edessa" };
        var supplementalFont89 = new A.SupplementalFont { Script = "Syrj", Typeface = "Estrangelo Edessa" };
        var supplementalFont90 = new A.SupplementalFont { Script = "Syre", Typeface = "Estrangelo Edessa" };
        var supplementalFont91 = new A.SupplementalFont { Script = "Sora", Typeface = "Nirmala UI" };
        var supplementalFont92 = new A.SupplementalFont { Script = "Tale", Typeface = "Microsoft Tai Le" };
        var supplementalFont93 = new A.SupplementalFont { Script = "Talu", Typeface = "Microsoft New Tai Lue" };
        var supplementalFont94 = new A.SupplementalFont { Script = "Tfng", Typeface = "Ebrima" };

        minorFont1.Append(latinFont2);
        minorFont1.Append(eastAsianFont2);
        minorFont1.Append(complexScriptFont2);
        minorFont1.Append(supplementalFont48);
        minorFont1.Append(supplementalFont49);
        minorFont1.Append(supplementalFont50);
        minorFont1.Append(supplementalFont51);
        minorFont1.Append(supplementalFont52);
        minorFont1.Append(supplementalFont53);
        minorFont1.Append(supplementalFont54);
        minorFont1.Append(supplementalFont55);
        minorFont1.Append(supplementalFont56);
        minorFont1.Append(supplementalFont57);
        minorFont1.Append(supplementalFont58);
        minorFont1.Append(supplementalFont59);
        minorFont1.Append(supplementalFont60);
        minorFont1.Append(supplementalFont61);
        minorFont1.Append(supplementalFont62);
        minorFont1.Append(supplementalFont63);
        minorFont1.Append(supplementalFont64);
        minorFont1.Append(supplementalFont65);
        minorFont1.Append(supplementalFont66);
        minorFont1.Append(supplementalFont67);
        minorFont1.Append(supplementalFont68);
        minorFont1.Append(supplementalFont69);
        minorFont1.Append(supplementalFont70);
        minorFont1.Append(supplementalFont71);
        minorFont1.Append(supplementalFont72);
        minorFont1.Append(supplementalFont73);
        minorFont1.Append(supplementalFont74);
        minorFont1.Append(supplementalFont75);
        minorFont1.Append(supplementalFont76);
        minorFont1.Append(supplementalFont77);
        minorFont1.Append(supplementalFont78);
        minorFont1.Append(supplementalFont79);
        minorFont1.Append(supplementalFont80);
        minorFont1.Append(supplementalFont81);
        minorFont1.Append(supplementalFont82);
        minorFont1.Append(supplementalFont83);
        minorFont1.Append(supplementalFont84);
        minorFont1.Append(supplementalFont85);
        minorFont1.Append(supplementalFont86);
        minorFont1.Append(supplementalFont87);
        minorFont1.Append(supplementalFont88);
        minorFont1.Append(supplementalFont89);
        minorFont1.Append(supplementalFont90);
        minorFont1.Append(supplementalFont91);
        minorFont1.Append(supplementalFont92);
        minorFont1.Append(supplementalFont93);
        minorFont1.Append(supplementalFont94);

        fontScheme1.Append(majorFont1);
        fontScheme1.Append(minorFont1);

        var formatScheme1 = new A.FormatScheme { Name = "Office" };

        var fillStyleList1 = new A.FillStyleList();

        var solidFill1 = new A.SolidFill();
        var schemeColor1 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill1.Append(schemeColor1);

        var gradientFill1 = new A.GradientFill { RotateWithShape = true };

        var gradientStopList1 = new A.GradientStopList();

        var gradientStop1 = new A.GradientStop { Position = 0 };

        var schemeColor2 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var luminanceModulation1 = new A.LuminanceModulation { Val = 110000 };
        var saturationModulation1 = new A.SaturationModulation { Val = 105000 };
        var tint1 = new A.Tint { Val = 67000 };

        schemeColor2.Append(luminanceModulation1);
        schemeColor2.Append(saturationModulation1);
        schemeColor2.Append(tint1);

        gradientStop1.Append(schemeColor2);

        var gradientStop2 = new A.GradientStop { Position = 50000 };

        var schemeColor3 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var luminanceModulation2 = new A.LuminanceModulation { Val = 105000 };
        var saturationModulation2 = new A.SaturationModulation { Val = 103000 };
        var tint2 = new A.Tint { Val = 73000 };

        schemeColor3.Append(luminanceModulation2);
        schemeColor3.Append(saturationModulation2);
        schemeColor3.Append(tint2);

        gradientStop2.Append(schemeColor3);

        var gradientStop3 = new A.GradientStop { Position = 100000 };

        var schemeColor4 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var luminanceModulation3 = new A.LuminanceModulation { Val = 105000 };
        var saturationModulation3 = new A.SaturationModulation { Val = 109000 };
        var tint3 = new A.Tint { Val = 81000 };

        schemeColor4.Append(luminanceModulation3);
        schemeColor4.Append(saturationModulation3);
        schemeColor4.Append(tint3);

        gradientStop3.Append(schemeColor4);

        gradientStopList1.Append(gradientStop1);
        gradientStopList1.Append(gradientStop2);
        gradientStopList1.Append(gradientStop3);
        var linearGradientFill1 = new A.LinearGradientFill { Angle = 5400000, Scaled = false };

        gradientFill1.Append(gradientStopList1);
        gradientFill1.Append(linearGradientFill1);

        var gradientFill2 = new A.GradientFill { RotateWithShape = true };

        var gradientStopList2 = new A.GradientStopList();

        var gradientStop4 = new A.GradientStop { Position = 0 };

        var schemeColor5 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var saturationModulation4 = new A.SaturationModulation { Val = 103000 };
        var luminanceModulation4 = new A.LuminanceModulation { Val = 102000 };
        var tint4 = new A.Tint { Val = 94000 };

        schemeColor5.Append(saturationModulation4);
        schemeColor5.Append(luminanceModulation4);
        schemeColor5.Append(tint4);

        gradientStop4.Append(schemeColor5);

        var gradientStop5 = new A.GradientStop { Position = 50000 };

        var schemeColor6 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var saturationModulation5 = new A.SaturationModulation { Val = 110000 };
        var luminanceModulation5 = new A.LuminanceModulation { Val = 100000 };
        var shade1 = new A.Shade { Val = 100000 };

        schemeColor6.Append(saturationModulation5);
        schemeColor6.Append(luminanceModulation5);
        schemeColor6.Append(shade1);

        gradientStop5.Append(schemeColor6);

        var gradientStop6 = new A.GradientStop { Position = 100000 };

        var schemeColor7 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var luminanceModulation6 = new A.LuminanceModulation { Val = 99000 };
        var saturationModulation6 = new A.SaturationModulation { Val = 120000 };
        var shade2 = new A.Shade { Val = 78000 };

        schemeColor7.Append(luminanceModulation6);
        schemeColor7.Append(saturationModulation6);
        schemeColor7.Append(shade2);

        gradientStop6.Append(schemeColor7);

        gradientStopList2.Append(gradientStop4);
        gradientStopList2.Append(gradientStop5);
        gradientStopList2.Append(gradientStop6);
        var linearGradientFill2 = new A.LinearGradientFill { Angle = 5400000, Scaled = false };

        gradientFill2.Append(gradientStopList2);
        gradientFill2.Append(linearGradientFill2);

        fillStyleList1.Append(solidFill1);
        fillStyleList1.Append(gradientFill1);
        fillStyleList1.Append(gradientFill2);

        var lineStyleList1 = new A.LineStyleList();

        var outline1 = new A.Outline
        {
            Width = 6350,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill2 = new A.SolidFill();
        var schemeColor8 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill2.Append(schemeColor8);
        var presetDash1 = new A.PresetDash { Val = A.PresetLineDashValues.Solid };
        var miter1 = new A.Miter { Limit = 800000 };

        outline1.Append(solidFill2);
        outline1.Append(presetDash1);
        outline1.Append(miter1);

        var outline2 = new A.Outline
        {
            Width = 12700,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill3 = new A.SolidFill();
        var schemeColor9 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill3.Append(schemeColor9);
        var presetDash2 = new A.PresetDash { Val = A.PresetLineDashValues.Solid };
        var miter2 = new A.Miter { Limit = 800000 };

        outline2.Append(solidFill3);
        outline2.Append(presetDash2);
        outline2.Append(miter2);

        var outline3 = new A.Outline
        {
            Width = 19050,
            CapType = A.LineCapValues.Flat,
            CompoundLineType = A.CompoundLineValues.Single,
            Alignment = A.PenAlignmentValues.Center
        };

        var solidFill4 = new A.SolidFill();
        var schemeColor10 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill4.Append(schemeColor10);
        var presetDash3 = new A.PresetDash { Val = A.PresetLineDashValues.Solid };
        var miter3 = new A.Miter { Limit = 800000 };

        outline3.Append(solidFill4);
        outline3.Append(presetDash3);
        outline3.Append(miter3);

        lineStyleList1.Append(outline1);
        lineStyleList1.Append(outline2);
        lineStyleList1.Append(outline3);

        var effectStyleList1 = new A.EffectStyleList();

        var effectStyle1 = new A.EffectStyle();
        var effectList1 = new A.EffectList();

        effectStyle1.Append(effectList1);

        var effectStyle2 = new A.EffectStyle();
        var effectList2 = new A.EffectList();

        effectStyle2.Append(effectList2);

        var effectStyle3 = new A.EffectStyle();

        var effectList3 = new A.EffectList();

        var outerShadow1 = new A.OuterShadow
        {
            BlurRadius = 57150L,
            Distance = 19050L,
            Direction = 5400000,
            Alignment = A.RectangleAlignmentValues.Center,
            RotateWithShape = false
        };

        var rgbColorModelHex11 = new A.RgbColorModelHex { Val = "000000" };
        var alpha1 = new A.Alpha { Val = 63000 };

        rgbColorModelHex11.Append(alpha1);

        outerShadow1.Append(rgbColorModelHex11);

        effectList3.Append(outerShadow1);

        effectStyle3.Append(effectList3);

        effectStyleList1.Append(effectStyle1);
        effectStyleList1.Append(effectStyle2);
        effectStyleList1.Append(effectStyle3);

        var backgroundFillStyleList1 = new A.BackgroundFillStyleList();

        var solidFill5 = new A.SolidFill();
        var schemeColor11 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };

        solidFill5.Append(schemeColor11);

        var solidFill6 = new A.SolidFill();

        var schemeColor12 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var tint5 = new A.Tint { Val = 95000 };
        var saturationModulation7 = new A.SaturationModulation { Val = 170000 };

        schemeColor12.Append(tint5);
        schemeColor12.Append(saturationModulation7);

        solidFill6.Append(schemeColor12);

        var gradientFill3 = new A.GradientFill { RotateWithShape = true };

        var gradientStopList3 = new A.GradientStopList();

        var gradientStop7 = new A.GradientStop { Position = 0 };

        var schemeColor13 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var tint6 = new A.Tint { Val = 93000 };
        var saturationModulation8 = new A.SaturationModulation { Val = 150000 };
        var shade3 = new A.Shade { Val = 98000 };
        var luminanceModulation7 = new A.LuminanceModulation { Val = 102000 };

        schemeColor13.Append(tint6);
        schemeColor13.Append(saturationModulation8);
        schemeColor13.Append(shade3);
        schemeColor13.Append(luminanceModulation7);

        gradientStop7.Append(schemeColor13);

        var gradientStop8 = new A.GradientStop { Position = 50000 };

        var schemeColor14 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var tint7 = new A.Tint { Val = 98000 };
        var saturationModulation9 = new A.SaturationModulation { Val = 130000 };
        var shade4 = new A.Shade { Val = 90000 };
        var luminanceModulation8 = new A.LuminanceModulation { Val = 103000 };

        schemeColor14.Append(tint7);
        schemeColor14.Append(saturationModulation9);
        schemeColor14.Append(shade4);
        schemeColor14.Append(luminanceModulation8);

        gradientStop8.Append(schemeColor14);

        var gradientStop9 = new A.GradientStop { Position = 100000 };

        var schemeColor15 = new A.SchemeColor { Val = A.SchemeColorValues.PhColor };
        var shade5 = new A.Shade { Val = 63000 };
        var saturationModulation10 = new A.SaturationModulation { Val = 120000 };

        schemeColor15.Append(shade5);
        schemeColor15.Append(saturationModulation10);

        gradientStop9.Append(schemeColor15);

        gradientStopList3.Append(gradientStop7);
        gradientStopList3.Append(gradientStop8);
        gradientStopList3.Append(gradientStop9);
        var linearGradientFill3 = new A.LinearGradientFill { Angle = 5400000, Scaled = false };

        gradientFill3.Append(gradientStopList3);
        gradientFill3.Append(linearGradientFill3);

        backgroundFillStyleList1.Append(solidFill5);
        backgroundFillStyleList1.Append(solidFill6);
        backgroundFillStyleList1.Append(gradientFill3);

        formatScheme1.Append(fillStyleList1);
        formatScheme1.Append(lineStyleList1);
        formatScheme1.Append(effectStyleList1);
        formatScheme1.Append(backgroundFillStyleList1);

        themeElements1.Append(colorScheme1);
        themeElements1.Append(fontScheme1);
        themeElements1.Append(formatScheme1);
        var objectDefaults1 = new A.ObjectDefaults();
        var extraColorSchemeList1 = new A.ExtraColorSchemeList();

        var officeStyleSheetExtensionList1 = new A.OfficeStyleSheetExtensionList();

        var officeStyleSheetExtension1 = new A.OfficeStyleSheetExtension
        { Uri = "{05A4C25C-085E-4340-85A3-A5531E510DB2}" };

        var themeFamily1 = new Thm15.ThemeFamily
        {
            Name = "Office Theme",
            Id = "{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}",
            Vid = "{4A3C46E8-61CC-4603-A589-7422A47A8E4A}"
        };
        themeFamily1.AddNamespaceDeclaration("thm15", "http://schemas.microsoft.com/office/thememl/2012/main");

        officeStyleSheetExtension1.Append(themeFamily1);

        officeStyleSheetExtensionList1.Append(officeStyleSheetExtension1);

        theme1.Append(themeElements1);
        theme1.Append(objectDefaults1);
        theme1.Append(extraColorSchemeList1);
        theme1.Append(officeStyleSheetExtensionList1);

        themePart1.Theme = theme1;
    }

    // Generates content of webSettingsPart1.
    private static void GenerateWebSettingsPart1Content(WebSettingsPart webSettingsPart1)
    {
        var webSettings1 = new WebSettings
        {
            MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "w14 w15 w16se w16cid w16 w16cex w16sdtdh" }
        };
        webSettings1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
        webSettings1.AddNamespaceDeclaration("r",
            "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        webSettings1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
        webSettings1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
        webSettings1.AddNamespaceDeclaration("w15", "http://schemas.microsoft.com/office/word/2012/wordml");
        webSettings1.AddNamespaceDeclaration("w16cex", "http://schemas.microsoft.com/office/word/2018/wordml/cex");
        webSettings1.AddNamespaceDeclaration("w16cid", "http://schemas.microsoft.com/office/word/2016/wordml/cid");
        webSettings1.AddNamespaceDeclaration("w16", "http://schemas.microsoft.com/office/word/2018/wordml");
        webSettings1.AddNamespaceDeclaration("w16sdtdh",
            "http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash");
        webSettings1.AddNamespaceDeclaration("w16se", "http://schemas.microsoft.com/office/word/2015/wordml/symex");
        var optimizeForBrowser1 = new OptimizeForBrowser();
        var allowPNG1 = new AllowPNG();

        webSettings1.Append(optimizeForBrowser1);
        webSettings1.Append(allowPNG1);

        webSettingsPart1.WebSettings = webSettings1;
    }

    private static Stream GetBinaryDataStream(string base64String)
    {
        return new MemoryStream(Convert.FromBase64String(base64String));
    }

    private static void SetPackageProperties(OpenXmlPackage document)
    {
        var now = GlobalSettings.Instance.DateTimeNow;
        document.PackageProperties.Creator = "OJ";
        document.PackageProperties.Title = "";
        document.PackageProperties.Subject = "";
        document.PackageProperties.Keywords = "";
        document.PackageProperties.Description = "";
        document.PackageProperties.Revision = "1";
        document.PackageProperties.Created = now;
        //document.PackageProperties.Modified = now;
        //document.PackageProperties.LastModifiedBy = "GRAM";
    }
}