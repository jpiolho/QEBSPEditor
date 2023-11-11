# QEBSPEditor

A browser tool to edit BSPs for Quake Enhanced and other versions.

## Getting Started
Visit [https://jpiolho.github.io/QEBSPEditor/](https://jpiolho.github.io/QEBSPEditor/) to start using the QEBSPEditor. No install required.

## Features
- **No install required**: Runs on the browser using WebAssembly.
- **Edit entities**: Create, change or delete entities via a user interface, or using the source directly.
- **View textures**: View which textures are used in the map, along with their mipmaps and get a highlight of the fullbright colors.
- **Edit textures**: Rename or upload a texture replacement.
- **Export textures**: Export all the textures from a map as either a WAD or a zip with individual files.
- **Custom palettes**: It's possible to upload your own palettes so you can view and edit textures appropriately regardless of the engine that you're using.
- **Adjust global light**: Experimental global lighting adjustment feature.
- **Supported BSP versions**: Supports BSP versions 29, BSP2 and 2PSB.
- **Generic BSP support**: You can also load unsupported BSP files from other games using a best-effort BSP loader which will allow you to edit the entities.
- **Edit BSPX features**: Such as embedding .lit files directly into the BSP.
- **Any extra bytes are safe**: All data appended after the BSP, such as BSPX lumps, is preserved.

## Roadmap
- Improve theme compatibility
- Help menu
- Better compatibility with Quake special characters
- Small screen / mobile friendly-UI (gotta edit those BSPs on the go 🤪)
