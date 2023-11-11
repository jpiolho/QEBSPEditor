﻿using Blazor.DownloadFileFast.Interfaces;
using QEBSPEditor.Core.Exceptions;
using QEBSPEditor.Models.BSPFiles;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace QEBSPEditor.Services;

/// <summary>
/// Provides services for managing BSP files
/// </summary>
public class BSPService
{
    private readonly IBlazorDownloadFileService downloadFileService;

    public BSPService(IBlazorDownloadFileService downloadFileService)
    {
        this.downloadFileService = downloadFileService;
    }

    /// <summary>
    /// Asynchronously saves the specified BSP file.
    /// </summary>
    /// <param name="bspFile">The BSP file to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided BSP file is null.</exception>
    /// <exception cref="BSPNotCompatibleException">Thrown if the BSP file does not support the save operation.</exception>
    public async Task SaveAsync(IBSPFile bspFile, CancellationToken cancellationToken = default)
    {
        if (bspFile == null)
            throw new ArgumentNullException(nameof(bspFile));

        var bspSave = GetBSPCapability<IBSPSave>(bspFile);

        using (var ms = new MemoryStream())
        {
            bspSave.Save(ms);
            await downloadFileService.DownloadFileAsync($"{bspFile.Name}.bsp", ms.ToArray());
        }
    }

    /// <summary>
    /// Asynchronously exports the entity data of the specified BSP file to a .ent file.
    /// </summary>
    /// <param name="bspFile">The BSP file to export entities from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided BSP file is null.</exception>
    /// <exception cref="BSPNotCompatibleException">Thrown if the BSP file does not support the entity export operation.</exception>
    public async Task ExportEntFileAsync(IBSPFile bspFile, CancellationToken cancellationToken = default)
    {
        if (bspFile == null)
            throw new ArgumentNullException(nameof(bspFile));

        var bspEntities = GetBSPCapability<IBSPFileEntities>(bspFile);

        using (var ms = new MemoryStream())
        using (var writer = new StreamWriter(ms))
        {
            await writer.WriteAsync(bspEntities.Entities.AsMemory(), cancellationToken);
            await writer.FlushAsync();

            await downloadFileService.DownloadFileAsync($"{bspFile.Name}.ent", ms.ToArray());
        }
    }

    /// <summary>
    /// Imports entity data into the specified BSP file.
    /// </summary>
    /// <param name="bspFile">The BSP file to import entities into.</param>
    /// <param name="contents">The entity data to be imported as a string.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided BSP file is null.</exception>
    /// <exception cref="BSPNotCompatibleException">Thrown if the BSP file does not support the entity import operation.</exception>
    public void ImportEntFile(IBSPFile bspFile, string contents)
    {
        if (bspFile == null)
            throw new ArgumentNullException(nameof(bspFile));

        var bspEntities = GetBSPCapability<IBSPFileEntities>(bspFile);

        bspEntities.Entities = contents;
    }


    private TCapability GetBSPCapability<TCapability>(IBSPFile bsp)
    {
        if (bsp is not TCapability bspAsCapability)
            throw new BSPNotCompatibleException();
        
        return bspAsCapability;
    }
}
