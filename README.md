# M3U Playlist Helper

A small Windows app for trimming down large M3U / M3U8 IPTV playlists: pick the categories and channels you want, rename or reorder them, and save a clean playlist.

## Features

- Open playlists from a file, a URL, drag and drop, or the command line (`M3UPlaylistHelper.exe playlist.m3u`)
- Recent files and URLs (File > Recent)
- Include/exclude whole categories or single channels; Select All / Clear All / Invert act on the filtered rows
- Space toggles every selected row; excluded rows are greyed out
- Search categories, search channels in the current category, or tick **All categories** to search the whole playlist
- Rename categories and channels in place (double-click or F2); reorder categories from the right-click menu (Alt+Up / Alt+Down)
- Channel logos are downloaded on demand for the rows on screen and cached
- Right-click a channel to play it in your default media player, copy its URL or name, or jump to its category
- Exclude duplicate channels (same stream URL)
- **Selection profiles**: save your choices once and reapply them when your provider updates the playlist
- Keeps everything it does not edit: `#EXTM3U` header attributes (EPG `url-tvg`), `tvg-chno`, `catchup`, `#EXTVLCOPT`, `#KODIPROP`, etc.
- Warns about unsaved changes; remembers window size and settings

## Keyboard shortcuts

| Shortcut | Action |
| --- | --- |
| Ctrl+O | Open file |
| Ctrl+U | Open URL |
| Ctrl+S / Ctrl+Shift+S | Save / Save As |
| Ctrl+F | Search channels |
| Space | Toggle selected rows |
| F2 | Rename |
| Alt+Up / Alt+Down | Move category |
| Esc (in a search box) | Clear the search |
| F1 | Help |

## Building

Requires the .NET 8 SDK.

```
dotnet build
dotnet test
```

The parsing logic lives in `M3UPlaylistHelper.Core` (plain .NET 8, runs anywhere) and is covered by `M3UPlaylistHelper.Tests`. The WinForms app in `M3UPlaylistHelper` only runs on Windows.
