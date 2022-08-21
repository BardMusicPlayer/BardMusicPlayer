/*
 * Copyright(c) 2017-2018 Syroot
 * Licensed under the MIT license. See https://gitlab.com/Syroot/KnownFolders/-/blob/master/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Seer.Utilities.KnownFolder
{
    /// <summary>
    /// A collection of properties to retrieve specific file system paths for the current user.
    /// </summary>
    internal static class KnownFolders
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------
        private static Dictionary<KnownFolderType, KnownFolder> _knownFolderInstances;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The per-user Account Pictures folder. Introduced in Windows 8.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\AccountPictures&quot;.
        /// </summary>
        internal static KnownFolder AccountPictures => GetInstance(KnownFolderType.AccountPictures);

        /// <summary>
        /// The per-user Administrative Tools folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Start Menu\Programs\Administrative Tools&quot;.
        /// </summary>
        internal static KnownFolder AdminTools => GetInstance(KnownFolderType.AdminTools);

        /// <summary>
        /// The per-user app desktop folder, used internally by .NET applications to perform cross-platform app
        /// functionality. Introduced in Windows 10.
        /// Defaults to &quot;%LOCALAPPDATA%\Desktop&quot;.
        /// </summary>
        internal static KnownFolder AppDataDesktop => GetInstance(KnownFolderType.AppDataDesktop);

        /// <summary>
        /// The per-user app documents folder, used internally by .NET applications to perform cross-platform app
        /// functionality. Introduced in Windows 10.
        /// Defaults to &quot;%LOCALAPPDATA%\Documents&quot;.
        /// </summary>
        internal static KnownFolder AppDataDocuments => GetInstance(KnownFolderType.AppDataDocuments);

        /// <summary>
        /// The per-user app favorites folder, used internally by .NET applications to perform cross-platform app
        /// functionality. Introduced in Windows 10.
        /// Defaults to &quot;%LOCALAPPDATA%\Favorites&quot;.
        /// </summary>
        internal static KnownFolder AppDataFavorites => GetInstance(KnownFolderType.AppDataFavorites);

        /// <summary>
        /// The per-user app program data folder, used internally by .NET applications to perform cross-platform app
        /// functionality. Introduced in Windows 10.
        /// Defaults to &quot;%LOCALAPPDATA%\ProgramData&quot;.
        /// </summary>
        internal static KnownFolder AppDataProgramData => GetInstance(KnownFolderType.AppDataProgramData);

        /// <summary>
        /// The per-user Application Shortcuts folder. Introduced in Windows 8.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\Application Shortcuts&quot;.
        /// </summary>
        internal static KnownFolder ApplicationShortcuts => GetInstance(KnownFolderType.ApplicationShortcuts);

        /// <summary>
        /// The per-user Camera Roll folder. Introduced in Windows 8.1.
        /// Defaults to &quot;.%USERPROFILE%\Pictures\Camera Roll&quot;.
        /// </summary>
        internal static KnownFolder CameraRoll => GetInstance(KnownFolderType.CameraRoll);

        /// <summary>
        /// The per-user Temporary Burn Folder.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\Burn\Burn&quot;.
        /// </summary>
        internal static KnownFolder CDBurning => GetInstance(KnownFolderType.CDBurning);

        /// <summary>
        /// The common Administrative Tools folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Administrative Tools&quot;.
        /// </summary>
        internal static KnownFolder CommonAdminTools => GetInstance(KnownFolderType.CommonAdminTools);

        /// <summary>
        /// The common OEM Links folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\OEM Links&quot;.
        /// </summary>
        internal static KnownFolder CommonOemLinks => GetInstance(KnownFolderType.CommonOemLinks);

        /// <summary>
        /// The common Programs folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs&quot;.
        /// </summary>
        internal static KnownFolder CommonPrograms => GetInstance(KnownFolderType.CommonPrograms);

        /// <summary>
        /// The common Start Menu folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu&quot;.
        /// </summary>
        internal static KnownFolder CommonStartMenu => GetInstance(KnownFolderType.CommonStartMenu);

        /// <summary>
        /// The common Startup folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\StartUp&quot;.
        /// </summary>
        internal static KnownFolder CommonStartup => GetInstance(KnownFolderType.CommonStartup);

        /// <summary>
        /// The common Templates folder.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Templates&quot;.
        /// </summary>
        internal static KnownFolder CommonTemplates => GetInstance(KnownFolderType.CommonTemplates);

        /// <summary>
        /// The per-user Contacts folder. Introduced in Windows Vista.
        /// Defaults to &quot;%USERPROFILE%\Contacts&quot;.
        /// </summary>
        internal static KnownFolder Contacts => GetInstance(KnownFolderType.Contacts);

        /// <summary>
        /// The per-user Cookies folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Cookies&quot;.
        /// </summary>
        internal static KnownFolder Cookies => GetInstance(KnownFolderType.Cookies);

        /// <summary>
        /// The per-user Desktop folder.
        /// Defaults to &quot;%USERPROFILE%\Desktop&quot;.
        /// </summary>
        internal static KnownFolder Desktop => GetInstance(KnownFolderType.Desktop);

        /// <summary>
        /// The common DeviceMetadataStore folder. Introduced in Windows 7.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\DeviceMetadataStore&quot;.
        /// </summary>
        internal static KnownFolder DeviceMetadataStore => GetInstance(KnownFolderType.DeviceMetadataStore);

        /// <summary>
        /// The per-user Documents folder.
        /// Defaults to &quot;%USERPROFILE%\Documents&quot;.
        /// </summary>
        internal static KnownFolder Documents => GetInstance(KnownFolderType.Documents);

        /// <summary>
        /// The per-user Documents library. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Libraries\Documents.library-ms&quot;.
        /// </summary>
        internal static KnownFolder DocumentsLibrary => GetInstance(KnownFolderType.DocumentsLibrary);

        /// <summary>
        /// The per-user localized Documents folder.
        /// Defaults to &quot;%USERPROFILE%\Documents&quot;.
        /// </summary>
        internal static KnownFolder DocumentsLocalized => GetInstance(KnownFolderType.DocumentsLocalized);

        /// <summary>
        /// The per-user Downloads folder.
        /// Defaults to &quot;%USERPROFILE%\Downloads&quot;.
        /// </summary>
        internal static KnownFolder Downloads => GetInstance(KnownFolderType.Downloads);

        /// <summary>
        /// The per-user localized Downloads folder.
        /// Defaults to &quot;%USERPROFILE%\Downloads&quot;.
        /// </summary>
        internal static KnownFolder DownloadsLocalized => GetInstance(KnownFolderType.DownloadsLocalized);

        /// <summary>
        /// The per-user Favorites folder.
        /// Defaults to &quot;%USERPROFILE%\Favorites&quot;.
        /// </summary>
        internal static KnownFolder Favorites => GetInstance(KnownFolderType.Favorites);

        /// <summary>
        /// The fixed Fonts folder.
        /// Points to &quot;%WINDIR%\Fonts&quot;.
        /// </summary>
        internal static KnownFolder Fonts => GetInstance(KnownFolderType.Fonts);

        /// <summary>
        /// The per-user GameExplorer folder. Introduced in Windows Vista.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\GameExplorer&quot;.
        /// </summary>
        internal static KnownFolder GameTasks => GetInstance(KnownFolderType.GameTasks);

        /// <summary>
        /// The per-user History folder.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\History&quot;.
        /// </summary>
        internal static KnownFolder History => GetInstance(KnownFolderType.History);

        /// <summary>
        /// The per-user ImplicitAppShortcuts folder. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Internet Explorer\Quick Launch\User Pinned\ImplicitAppShortcuts&quot;.
        /// </summary>
        internal static KnownFolder ImplicitAppShortcuts => GetInstance(KnownFolderType.ImplicitAppShortcuts);

        /// <summary>
        /// The per-user Temporary Internet Files folder.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files&quot;.
        /// </summary>
        internal static KnownFolder InternetCache => GetInstance(KnownFolderType.InternetCache);

        /// <summary>
        /// The per-user Libraries folder. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Libraries&quot;.
        /// </summary>
        internal static KnownFolder Libraries => GetInstance(KnownFolderType.Libraries);

        /// <summary>
        /// The per-user Links folder.
        /// Defaults to &quot;%USERPROFILE%\Links&quot;.
        /// </summary>
        internal static KnownFolder Links => GetInstance(KnownFolderType.Links);

        /// <summary>
        /// The per-user Local folder.
        /// Defaults to &quot;%LOCALAPPDATA%&quot; (&quot;%USERPROFILE%\AppData\Local&quot;)&quot;.
        /// </summary>
        internal static KnownFolder LocalAppData => GetInstance(KnownFolderType.LocalAppData);

        /// <summary>
        /// The per-user LocalLow folder.
        /// Defaults to &quot;%USERPROFILE%\AppData\LocalLow&quot;.
        /// </summary>
        internal static KnownFolder LocalAppDataLow => GetInstance(KnownFolderType.LocalAppDataLow);

        /// <summary>
        /// The fixed LocalizedResourcesDir folder.
        /// Points to &quot;%WINDIR%\resources\0409&quot; (code page).
        /// </summary>
        internal static KnownFolder LocalizedResourcesDir => GetInstance(KnownFolderType.LocalizedResourcesDir);

        /// <summary>
        /// The per-user Music folder.
        /// Defaults to &quot;%USERPROFILE%\Music&quot;.
        /// </summary>
        internal static KnownFolder Music => GetInstance(KnownFolderType.Music);

        /// <summary>
        /// The per-user Music library. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Libraries\Music.library-ms&quot;.
        /// </summary>
        internal static KnownFolder MusicLibrary => GetInstance(KnownFolderType.MusicLibrary);

        /// <summary>
        /// The per-user localized Music folder.
        /// Defaults to &quot;%USERPROFILE%\Music&quot;.
        /// </summary>
        internal static KnownFolder MusicLocalized => GetInstance(KnownFolderType.MusicLocalized);

        /// <summary>
        /// The per-user Network Shortcuts folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Network Shortcuts&quot;.
        /// </summary>
        internal static KnownFolder NetHood => GetInstance(KnownFolderType.NetHood);

        /// <summary>
        /// The per-user 3D Objects folder. Introduced in Windows 10.
        /// Defaults to &quot;%USERPROFILE%\3D Objects&quot;.
        /// </summary>
        internal static KnownFolder Objects3D => GetInstance(KnownFolderType.Objects3D);

        /// <summary>
        /// The per-user Original Images folder. Introduced in Windows Vista.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows Photo Gallery\Original Images&quot;.
        /// </summary>
        internal static KnownFolder OriginalImages => GetInstance(KnownFolderType.OriginalImages);

        /// <summary>
        /// The per-user Slide Shows folder. Introduced in Windows Vista.
        /// Defaults to &quot;%USERPROFILE%\Pictures\Slide Shows&quot;.
        /// </summary>
        internal static KnownFolder PhotoAlbums => GetInstance(KnownFolderType.PhotoAlbums);

        /// <summary>
        /// The per-user Pictures folder.
        /// Defaults to &quot;%USERPROFILE%\Pictures&quot;.
        /// </summary>
        internal static KnownFolder Pictures => GetInstance(KnownFolderType.Pictures);

        /// <summary>
        /// The per-user Pictures library. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Libraries\Pictures.library-ms&quot;.
        /// </summary>
        internal static KnownFolder PicturesLibrary => GetInstance(KnownFolderType.PicturesLibrary);

        /// <summary>
        /// The per-user localized Pictures folder.
        /// Defaults to &quot;%USERPROFILE%\Pictures&quot;.
        /// </summary>
        internal static KnownFolder PicturesLocalized => GetInstance(KnownFolderType.PicturesLocalized);

        /// <summary>
        /// The per-user Playlists folder.
        /// Defaults to &quot;%USERPROFILE%\Music\Playlists&quot;.
        /// </summary>
        internal static KnownFolder Playlists => GetInstance(KnownFolderType.Playlists);

        /// <summary>
        /// The per-user Printer Shortcuts folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Printer Shortcuts&quot;.
        /// </summary>
        internal static KnownFolder PrintHood => GetInstance(KnownFolderType.PrintHood);

        /// <summary>
        /// The fixed user profile folder.
        /// Defaults to &quot;%USERPROFILE%&quot; (&quot;%SYSTEMDRIVE%\USERS\%USERNAME%&quot;)&quot;.
        /// </summary>
        internal static KnownFolder Profile => GetInstance(KnownFolderType.Profile);

        /// <summary>
        /// The fixed ProgramData folder.
        /// Points to &quot;%ALLUSERSPROFILE%&quot; (&quot;%PROGRAMDATA%&quot;, &quot;%SYSTEMDRIVE%\ProgramData&quot;).
        /// </summary>
        internal static KnownFolder ProgramData => GetInstance(KnownFolderType.ProgramData);

        /// <summary>
        /// The fixed Program Files folder.
        /// This is the same as the <see cref="ProgramFilesX86"/> known folder in 32-bit applications or the
        /// <see cref="ProgramFilesX64"/> known folder in 64-bit applications.
        /// Points to %SYSTEMDRIVE%\Program Files on a 32-bit operating system or in 64-bit applications on a 64-bit
        /// operating system and to %SYSTEMDRIVE%\Program Files (x86) in 32-bit applications on a 64-bit operating
        /// system.
        /// </summary>
        internal static KnownFolder ProgramFiles => GetInstance(KnownFolderType.ProgramFiles);

        /// <summary>
        /// The fixed Program Files folder (64-bit forced).
        /// This known folder is unsupported in 32-bit applications.
        /// Points to %SYSTEMDRIVE%\Program Files.
        /// </summary>
        internal static KnownFolder ProgramFilesX64 => GetInstance(KnownFolderType.ProgramFilesX64);

        /// <summary>
        /// The fixed Program Files folder (32-bit forced).
        /// This is the same as the <see cref="ProgramFiles"/> known folder in 32-bit applications.
        /// Points to &quot;%SYSTEMDRIVE%\Program Files&quot; on a 32-bit operating system and to
        /// &quot;%SYSTEMDRIVE%\Program Files (x86)&quot; on a 64-bit operating system.
        /// </summary>
        internal static KnownFolder ProgramFilesX86 => GetInstance(KnownFolderType.ProgramFilesX86);

        /// <summary>
        /// The fixed Common Files folder.
        /// This is the same as the <see cref="ProgramFilesCommonX86"/> known folder in 32-bit applications or the
        /// <see cref="ProgramFilesCommonX64"/> known folder in 64-bit applications.
        /// Points to&quot; %PROGRAMFILES%\Common Files&quot; on a 32-bit operating system or in 64-bit applications on
        /// a 64-bit operating system and to &quot;%PROGRAMFILES(X86)%\Common Files&quot; in 32-bit applications on a
        /// 64-bit operating system.
        /// </summary>
        internal static KnownFolder ProgramFilesCommon => GetInstance(KnownFolderType.ProgramFilesCommon);

        /// <summary>
        /// The fixed Common Files folder (64-bit forced).
        /// This known folder is unsupported in 32-bit applications.
        /// Points to &quot;%PROGRAMFILES%\Common Files&quot;.
        /// </summary>
        internal static KnownFolder ProgramFilesCommonX64 => GetInstance(KnownFolderType.ProgramFilesCommonX64);

        /// <summary>
        /// The fixed Common Files folder (32-bit forced).
        /// This is the same as the <see cref="ProgramFilesCommon"/> known folder in 32-bit applications.
        /// Points to &quot;%PROGRAMFILES%\Common Files&quot; on a 32-bit operating system and to
        /// &quot;%PROGRAMFILES(X86)%\Common Files&quot; on a 64-bit operating system.
        /// </summary>
        internal static KnownFolder ProgramFilesCommonX86 => GetInstance(KnownFolderType.ProgramFilesCommonX86);

        /// <summary>
        /// The per-user Programs folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Start Menu\Programs&quot;.
        /// </summary>
        internal static KnownFolder Programs => GetInstance(KnownFolderType.Programs);

        /// <summary>
        /// The fixed internal folder. Introduced in Windows Vista.
        /// Defaults to &quot;%PUBLIC%&quot; (&quot;%SYSTEMDRIVE%\Users\Public)&quot;.
        /// </summary>
        internal static KnownFolder Public => GetInstance(KnownFolderType.Public);

        /// <summary>
        /// The common internal Desktop folder.
        /// Defaults to &quot;%PUBLIC%\Desktop&quot;.
        /// </summary>
        internal static KnownFolder PublicDesktop => GetInstance(KnownFolderType.PublicDesktop);

        /// <summary>
        /// The common internal Documents folder.
        /// Defaults to &quot;%PUBLIC%\Documents&quot;.
        /// </summary>
        internal static KnownFolder PublicDocuments => GetInstance(KnownFolderType.PublicDocuments);

        /// <summary>
        /// The common internal Downloads folder. Introduced in Windows Vista.
        /// Defaults to &quot;%PUBLIC%\Downloads&quot;.
        /// </summary>
        internal static KnownFolder PublicDownloads => GetInstance(KnownFolderType.PublicDownloads);

        /// <summary>
        /// The common GameExplorer folder. Introduced in Windows Vista.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\GameExplorer&quot;.
        /// </summary>
        internal static KnownFolder PublicGameTasks => GetInstance(KnownFolderType.PublicGameTasks);

        /// <summary>
        /// The common Libraries folder. Introduced in Windows 7.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Libraries&quot;.
        /// </summary>
        internal static KnownFolder PublicLibraries => GetInstance(KnownFolderType.PublicLibraries);

        /// <summary>
        /// The common internal Music folder.
        /// Defaults to &quot;%PUBLIC%\Music&quot;.
        /// </summary>
        internal static KnownFolder PublicMusic => GetInstance(KnownFolderType.PublicMusic);

        /// <summary>
        /// The common internal Pictures folder.
        /// Defaults to &quot;%PUBLIC%\Pictures&quot;.
        /// </summary>
        internal static KnownFolder PublicPictures => GetInstance(KnownFolderType.PublicPictures);

        /// <summary>
        /// The common Ringtones folder. Introduced in Windows 7.
        /// Defaults to &quot;%ALLUSERSPROFILE%\Microsoft\Windows\Ringtones&quot;.
        /// </summary>
        internal static KnownFolder PublicRingtones => GetInstance(KnownFolderType.PublicRingtones);

        /// <summary>
        /// The common internal Account Pictures folder. Introduced in Windows 8.
        /// Defaults to &quot;%PUBLIC%\AccountPictures&quot;.
        /// </summary>
        internal static KnownFolder PublicUserTiles => GetInstance(KnownFolderType.PublicUserTiles);

        /// <summary>
        /// The common internal Videos folder.
        /// Defaults to &quot;%PUBLIC%\Videos&quot;.
        /// </summary>
        internal static KnownFolder PublicVideos => GetInstance(KnownFolderType.PublicVideos);

        /// <summary>
        /// The per-user Quick Launch folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Internet Explorer\Quick Launch&quot;.
        /// </summary>
        internal static KnownFolder QuickLaunch => GetInstance(KnownFolderType.QuickLaunch);

        /// <summary>
        /// The per-user Recent Items folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Recent&quot;.
        /// </summary>
        internal static KnownFolder Recent => GetInstance(KnownFolderType.Recent);

        /// <summary>
        /// The common Recorded TV library. Introduced in Windows 7.
        /// Defaults to &quot;%PUBLIC%\RecordedTV.library-ms&quot;.
        /// </summary>
        internal static KnownFolder RecordedTVLibrary => GetInstance(KnownFolderType.RecordedTVLibrary);

        /// <summary>
        /// The fixed Resources folder.
        /// Points to &quot;%WINDIR%\Resources&quot;.
        /// </summary>
        internal static KnownFolder ResourceDir => GetInstance(KnownFolderType.ResourceDir);

        /// <summary>
        /// The per-user Ringtones folder. Introduced in Windows 7.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\Ringtones&quot;.
        /// </summary>
        internal static KnownFolder Ringtones => GetInstance(KnownFolderType.Ringtones);

        /// <summary>
        /// The per-user Roaming folder.
        /// Defaults to &quot;%APPDATA%&quot; (&quot;%USERPROFILE%\AppData\Roaming&quot;).
        /// </summary>
        internal static KnownFolder RoamingAppData => GetInstance(KnownFolderType.RoamingAppData);

        /// <summary>
        /// The per-user RoamedTileImages folder. Introduced in Windows 8.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\RoamedTileImages&quot;.
        /// </summary>
        internal static KnownFolder RoamedTileImages => GetInstance(KnownFolderType.RoamedTileImages);

        /// <summary>
        /// The per-user RoamingTiles folder. Introduced in Windows 8.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\RoamingTiles&quot;.
        /// </summary>
        internal static KnownFolder RoamingTiles => GetInstance(KnownFolderType.RoamingTiles);

        /// <summary>
        /// The common Sample Music folder.
        /// Defaults to &quot;%PUBLIC%\Music\Sample Music&quot;.
        /// </summary>
        internal static KnownFolder SampleMusic => GetInstance(KnownFolderType.SampleMusic);

        /// <summary>
        /// The common Sample Pictures folder.
        /// Defaults to &quot;%PUBLIC%\Pictures\Sample Pictures&quot;.
        /// </summary>
        internal static KnownFolder SamplePictures => GetInstance(KnownFolderType.SamplePictures);

        /// <summary>
        /// The common Sample Playlists folder. Introduced in Windows Vista.
        /// Defaults to &quot;%PUBLIC%\Music\Sample Playlists&quot;.
        /// </summary>
        internal static KnownFolder SamplePlaylists => GetInstance(KnownFolderType.SamplePlaylists);

        /// <summary>
        /// The common Sample Videos folder.
        /// Defaults to &quot;%PUBLIC%\Videos\Sample Videos&quot;.
        /// </summary>
        internal static KnownFolder SampleVideos => GetInstance(KnownFolderType.SampleVideos);

        /// <summary>
        /// The per-user Saved Games folder. Introduced in Windows Vista.
        /// Defaults to &quot;%USERPROFILE%\Saved Games&quot;.
        /// </summary>
        internal static KnownFolder SavedGames => GetInstance(KnownFolderType.SavedGames);

        /// <summary>
        /// The per-user Searches folder.
        /// Defaults to &quot;%USERPROFILE%\Searches&quot;.
        /// </summary>
        internal static KnownFolder SavedSearches => GetInstance(KnownFolderType.SavedSearches);

        /// <summary>
        /// The per-user Screenshots folder. Introduced in Windows 8.
        /// Defaults to &quot;%USERPROFILE%\Pictures\Screenshots&quot;.
        /// </summary>
        internal static KnownFolder Screenshots => GetInstance(KnownFolderType.Screenshots);

        /// <summary>
        /// The per-user History folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\ConnectedSearch\History&quot;.
        /// </summary>
        internal static KnownFolder SearchHistory => GetInstance(KnownFolderType.SearchHistory);

        /// <summary>
        /// The per-user Templates folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows\ConnectedSearch\Templates&quot;.
        /// </summary>
        internal static KnownFolder SearchTemplates => GetInstance(KnownFolderType.SearchTemplates);

        /// <summary>
        /// The per-user SendTo folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\SendTo&quot;.
        /// </summary>
        internal static KnownFolder SendTo => GetInstance(KnownFolderType.SendTo);

        /// <summary>
        /// The common Gadgets folder. Introduced in Windows 7.
        /// Defaults to &quot;%ProgramFiles%\Windows Sidebar\Gadgets&quot;.
        /// </summary>
        internal static KnownFolder SidebarDefaultParts => GetInstance(KnownFolderType.SidebarDefaultParts);

        /// <summary>
        /// The per-user Gadgets folder. Introduced in Windows 7.
        /// Defaults to &quot;%LOCALAPPDATA%\Microsoft\Windows Sidebar\Gadgets&quot;.
        /// </summary>
        internal static KnownFolder SidebarParts => GetInstance(KnownFolderType.SidebarParts);

        /// <summary>
        /// The per-user OneDrive folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%USERPROFILE%\OneDrive&quot;.
        /// </summary>
        internal static KnownFolder SkyDrive => GetInstance(KnownFolderType.SkyDrive);

        /// <summary>
        /// The per-user OneDrive Camera Roll folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%USERPROFILE%\OneDrive\Pictures\Camera Roll&quot;.
        /// </summary>
        internal static KnownFolder SkyDriveCameraRoll => GetInstance(KnownFolderType.SkyDriveCameraRoll);

        /// <summary>
        /// The per-user OneDrive Documents folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%USERPROFILE%\OneDrive\Documents&quot;.
        /// </summary>
        internal static KnownFolder SkyDriveDocuments => GetInstance(KnownFolderType.SkyDriveDocuments);

        /// <summary>
        /// The per-user OneDrive Pictures folder. Introduced in Windows 8.1.
        /// Defaults to &quot;%USERPROFILE%\OneDrive\Pictures&quot;.
        /// </summary>
        internal static KnownFolder SkyDrivePictures => GetInstance(KnownFolderType.SkyDrivePictures);

        /// <summary>
        /// The per-user Start Menu folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Start Menu&quot;.
        /// </summary>
        internal static KnownFolder StartMenu => GetInstance(KnownFolderType.StartMenu);

        /// <summary>
        /// The per-user Startup folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Start Menu\Programs\StartUp&quot;.
        /// </summary>
        internal static KnownFolder Startup => GetInstance(KnownFolderType.Startup);

        /// <summary>
        /// The fixed System32 folder.
        /// This is the same as the <see cref="SystemX86"/> known folder in 32-bit applications.
        /// Points to &quot;%WINDIR%\system32&quot; on 32-bit operating systems or in 64-bit applications on a 64-bit
        /// operating system and to &quot;%WINDIR%\syswow64&quot; in 32-bit applications on a 64-bit operating system.
        /// </summary>
        internal static KnownFolder System => GetInstance(KnownFolderType.System);

        /// <summary>
        /// The fixed System32 folder (32-bit forced).
        /// This is the same as the <see cref="System"/> known folder in 32-bit applications.
        /// Points to &quot;%WINDIR%\syswow64&quot; in 64-bit applications or in 32-bit applications on a 64-bit
        /// operating system and to &quot;%WINDIR%\system32&quot; on 32-bit operating systems.
        /// </summary>
        internal static KnownFolder SystemX86 => GetInstance(KnownFolderType.SystemX86);

        /// <summary>
        /// The per-user Templates folder.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Templates&quot;.
        /// </summary>
        internal static KnownFolder Templates => GetInstance(KnownFolderType.Templates);

        /// <summary>
        /// The per-user User Pinned folder. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Internet Explorer\Quick Launch\User Pinned&quot;.
        /// </summary>
        internal static KnownFolder UserPinned => GetInstance(KnownFolderType.UserPinned);

        /// <summary>
        /// The fixed Users folder. Introduced in Windows Vista.
        /// Points to &quot;%SYSTEMDRIVE%\Users&quot;.
        /// </summary>
        internal static KnownFolder UserProfiles => GetInstance(KnownFolderType.UserProfiles);

        /// <summary>
        /// The per-user Programs folder. Introduced in Windows 7.
        /// Defaults to &quot;%LOCALAPPDATA%\Programs.&quot;.
        /// </summary>
        internal static KnownFolder UserProgramFiles => GetInstance(KnownFolderType.UserProgramFiles);

        /// <summary>
        /// The per-user common Programs folder. INtroduced in Windows 7.
        /// Defaults to &quot;%LOCALAPPDATA%\Programs\Common&quot;.
        /// </summary>
        internal static KnownFolder UserProgramFilesCommon => GetInstance(KnownFolderType.UserProgramFilesCommon);

        /// <summary>
        /// The per-user Videos folder.
        /// Defaults to &quot;%USERPROFILE%\Videos&quot;.
        /// </summary>
        internal static KnownFolder Videos => GetInstance(KnownFolderType.Videos);

        /// <summary>
        /// The per-user Videos library. Introduced in Windows 7.
        /// Defaults to &quot;%APPDATA%\Microsoft\Windows\Libraries\Videos.library-ms&quot;.
        /// </summary>
        internal static KnownFolder VideosLibrary => GetInstance(KnownFolderType.VideosLibrary);

        /// <summary>
        /// The per-user localized Videos folder.
        /// Defaults to &quot;%USERPROFILE%\Videos&quot;.
        /// </summary>
        internal static KnownFolder VideosLocalized => GetInstance(KnownFolderType.VideosLocalized);

        /// <summary>
        /// The fixed Windows folder.
        /// Points to &quot;%WINDIR%&quot;.
        /// </summary>
        internal static KnownFolder Windows => GetInstance(KnownFolderType.Windows);

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static KnownFolder GetInstance(KnownFolderType type)
        {
            // Check if the caching directory exists yet.
            if (_knownFolderInstances == null)
                _knownFolderInstances = new Dictionary<KnownFolderType, KnownFolder>();

            // Get a KnownFolder instance out of the cache dictionary or create it when not cached yet.
            if (!_knownFolderInstances.TryGetValue(type, out var knownFolder))
            {
                knownFolder = new KnownFolder(type);
                _knownFolderInstances.Add(type, knownFolder);
            }

            return knownFolder;
        }
    }
}