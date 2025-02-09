//---------------------------------------------------------------------
// <copyright file="CompressionLevel.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// Part of the Deployment Tools Foundation project.
// </summary>
//---------------------------------------------------------------------

namespace SubhadraSolutions.Utils.Compression;

/// <summary>
/// Specifies the compression level ranging from minimum compresion to
/// maximum compression, or no compression at all.
/// </summary>
/// <remarks>
/// Although only four values are enumerated, any integral value between
/// <see cref="Min"/> and <see cref="Max"/> can also be used.
/// </remarks>
public enum CompressionLevel
{
    /// <summary>Do not compress files, only store.</summary>
    None = 0,

    /// <summary>Minimum compression; fastest.</summary>
    Min = 1,

    /// <summary>A compromize between speed and compression efficiency.</summary>
    Normal = 6,

    /// <summary>Maximum compression; slowest.</summary>
    Max = 10
}