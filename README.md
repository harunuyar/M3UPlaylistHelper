# M3U Playlist Helper

A Windows app for trimming down large M3U / M3U8 IPTV playlists: pick the categories and channels you want, rename, reorder or merge them, check EPG coverage, and save a clean playlist.

## Download

Grab the latest build from the [Releases](../../releases) page:

| File | Use it when |
| --- | --- |
| `M3UPlaylistHelper-<version>-win-x64.exe` | You want a single exe that just runs, nothing to install |
| `M3UPlaylistHelper-<version>-win-x64-portable.zip` | Same exe, but settings stay next to it (USB stick) |
| `M3UPlaylistHelper-<version>-win-x64-requires-dotnet8.exe` | Small download, needs the [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) |

## Features

**Opening**
- Playlist files, URLs (the last URL you entered is remembered), or an **Xtream Codes** login (server, username, password)
- The Xtream account status (expiry date, connections) is shown in the status bar; the password is stored encrypted for your Windows user only if you tick "Remember password"
- **Merge playlists**: select several files at once, drop several files on the window, or use File > Add to Current Playlist. Categories with the same name are combined
- Recent files and URLs, drag and drop, command line (`M3UPlaylistHelper.exe playlist.m3u`)

**Editing**
- Include/exclude whole categories or single channels; Select All / Clear All / Invert act on the filtered rows; Space toggles every selected row; excluded rows are greyed out
- Search categories, search channels in the current category, or tick **All categories** to search the whole playlist
- Rename categories and channels in place (double-click or F2)
- **Drag and drop** channels to reorder them, drop them on a category to move them there, and drag categories to reorder them. Right-click > Move to Category opens a searchable list, where you can also create a new category
- Exclude duplicate channels (same stream URL)
- **Selection profiles**: save your choices once and reapply them when your provider updates the playlist
- Right-click a channel to play it in your default media player or copy its URL or name

**EPG**
- Load the guide from the playlist header (`url-tvg`), your Xtream provider, a URL or a file (`.xml` / `.xml.gz`). Only the channel list is read, so even huge guides load quickly
- An EPG column shows which channels have program information
- Exclude channels without EPG, or fill in missing `tvg-id`s by matching channel names against the guide

**Everything else**
- Dark mode (follows Windows, or pick Light/Dark under View > Theme)
- Keeps what it does not edit: EPG header attributes, `tvg-chno`, `catchup`, `#EXTVLCOPT`, `#KODIPROP`, etc.
- Handles non-UTF-8 (Windows-1252) playlists without mangling accented characters
- Warns about unsaved changes; remembers window size and settings

## Keyboard shortcuts

| Shortcut | Action |
| --- | --- |
| Ctrl+O / Ctrl+U | Open file / URL |
| Ctrl+Shift+O / Ctrl+Shift+U | Add file / URL to the current playlist |
| Ctrl+S / Ctrl+Shift+S | Save / Save As |
| Ctrl+F | Search channels |
| Space | Toggle selected rows |
| Ctrl+C | Copy the selected channels (as M3U lines) or category names |
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

The parsing, merging and EPG logic lives in `M3UPlaylistHelper.Core` (plain .NET 8, runs anywhere) and is covered by `M3UPlaylistHelper.Tests`. The WinForms app in `M3UPlaylistHelper` only runs on Windows.

To publish a release, push a version tag: `git tag v1.2.0 && git push origin v1.2.0`. The Release workflow builds the downloads above and attaches them to a GitHub release.
