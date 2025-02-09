using SubhadraSolutions.Utils.CodeContracts.Annotations;
using System;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.Reporting;

public interface IDocumentBuilder : IDisposable
{
    string BasePath { get; set; }
    string Extension { get; }
    string MimeType { get; }

    IDocumentBuilder AddChart<T>(IEnumerable<T> data, string chartTitle = null);

    IDocumentBuilder AddHeading(string heading, int fontSize, bool centerAligned = false, string textColor = "000080");

    IDocumentBuilder AddImage(Stream stream, string imageExtn);

    IDocumentBuilder AddLineBreak();

    IDocumentBuilder AddPageBreak();

    [DynamicallyInvoked]
    IDocumentBuilder AddTable<T>(IEnumerable<T> items, int fontSize = 18);

    void Build(Stream stream);
}