using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;
using System.Windows.Media;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Text.RegularExpressions;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using System.Reflection;

namespace BardMusicPlayer.Ui.Skinned
{
    public partial class Skinned_MainView : UserControl
    {
        #region Tile definitions and offset
        Dictionary<SkinContainer.CBUTTON_TYPES, List<int>> cbuttonsdata = new Dictionary<SkinContainer.CBUTTON_TYPES, List<int>>
        {
            {SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON, new List<int> {0,0,23,18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON_ACTIVE, new List<int> {0,18,23,18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON, new List<int> {23,0,23,18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON_ACTIVE, new List<int> {23,18,23,18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON, new List<int> {46, 0, 23, 18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON_ACTIVE, new List<int> {46, 18, 23, 18}},
            {SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON, new List<int> {69, 0, 23, 18 }},
            {SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON_ACTIVE,new List<int> {69, 18, 23, 18 }},
            {SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON,new List<int> {92, 0, 22, 18 }},
            {SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON_ACTIVE,new List<int> {92, 18, 22, 18 } },
            {SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON,new List<int> {114, 0, 22, 16 } },
            {SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON_ACTIVE,new List<int> {114, 16, 22, 16 } }
        };

        Dictionary<SkinContainer.NUMBER_TYPES, List<int>> numbersdata = new Dictionary<SkinContainer.NUMBER_TYPES, List<int>>
        {
            {SkinContainer.NUMBER_TYPES.DIGIT_0, new List<int> {0, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_1, new List<int> {9, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_2, new List<int> {18, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_3, new List<int> {27, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_4, new List<int> {36, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_5, new List<int> {45, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_6, new List<int> {54, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_7, new List<int> {63, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_8, new List<int> {72, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.DIGIT_9, new List<int> {81, 0, 9, 13 } },
            {SkinContainer.NUMBER_TYPES.NO_MINUS_SIGN, new List<int> { 9, 6, 5, 1 } },
            {SkinContainer.NUMBER_TYPES.MINUS_SIGN, new List<int> {20, 6, 5, 1 } }
        };

        Dictionary<SkinContainer.TITLEBAR_TYPES, List<int>> titlebardata = new Dictionary<SkinContainer.TITLEBAR_TYPES, List<int>>
        {
            {SkinContainer.TITLEBAR_TYPES.MAIN_TITLE_BAR, new List<int> {27, 15, 275, 14 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_TITLE_BAR_SELECTED, new List<int> {27, 0, 275, 14 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_EASTER_EGG_TITLE_BAR, new List<int> { 27, 72, 275, 14} },
            {SkinContainer.TITLEBAR_TYPES.MAIN_EASTER_EGG_TITLE_BAR_SELECTED, new List<int> {27, 57, 275, 14 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON, new List<int> {0, 0, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON_DEPRESSED, new List<int> {0, 9, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_MINIMIZE_BUTTON, new List<int> {9, 0, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_MINIMIZE_BUTTON_DEPRESSED, new List<int> {9, 9, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BUTTON, new List<int> {0, 18, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BUTTON_DEPRESSED, new List<int> {9, 18, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON, new List<int> {18, 0, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON_DEPRESSED, new List<int> {18, 9, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BACKGROUND, new List<int> {304, 0, 8, 43,} },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BACKGROUND_DISABLED, new List<int> {312, 0, 8, 43} },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BUTTON_O_SELECTED, new List<int> {304, 47, 8, 8 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BUTTON_A_SELECTED, new List<int> {312, 55, 8, 7 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BUTTON_I_SELECTED, new List<int> {320, 62, 8, 7 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BUTTON_D_SELECTED, new List<int> {328, 69, 8, 8 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_CLUTTER_BAR_BUTTON_V_SELECTED, new List<int> {336, 77, 8, 7 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BACKGROUND, new List<int> {27, 42, 275, 14 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BACKGROUND_SELECTED, new List<int> {27, 29, 275, 14 }},
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BUTTON_SELECTED, new List<int> {0, 27, 9, 9 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_BUTTON_SELECTED_DEPRESSED, new List<int> {9, 27, 9, 9 }},
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_POSITION_BACKGROUND, new List<int> {0, 36, 17, 7 }},
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_POSITION_THUMB, new List<int> {20, 36, 3, 7 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_POSITION_THUMB_LEFT, new List<int> {17, 36, 3, 7 } },
            {SkinContainer.TITLEBAR_TYPES.MAIN_SHADE_POSITION_THUMB_RIGHT, new List<int> {23, 36, 3, 7 } },
        };

        Dictionary<SkinContainer.VOLUME_TYPES, List<int>> volumedata = new Dictionary<SkinContainer.VOLUME_TYPES, List<int>>
        { 
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_0, new List<int> {0,0,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_1, new List<int> {0,45,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_2, new List<int> {0,90,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_3, new List<int> {0,135,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_4, new List<int> {0,165,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_5, new List<int> {0,255,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_6, new List<int> {0,300,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_7, new List<int> {0,360,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_8, new List<int> {0,405,68,14} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_THUMB, new List<int> {15,422,14,11} },
            { SkinContainer.VOLUME_TYPES.MAIN_VOLUME_THUMB_SELECTED, new List<int> {0,422,14,11} }
        };

        Dictionary<SkinContainer.BALANCE_TYPES, List<int>> balancedata = new Dictionary<SkinContainer.BALANCE_TYPES, List<int>>
        {
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_0, new List<int> {9,0,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_1, new List<int> {9,45,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_2, new List<int> {9,90,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_3, new List<int> {9,135,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_4, new List<int> {9,165,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_5, new List<int> {9,255,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_6, new List<int> {9,300,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_7, new List<int> {9,360,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_BACKGROUND_8, new List<int> {9,405,38,14} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_THUMB,        new List<int> {15,422,14,11} },
            { SkinContainer.BALANCE_TYPES.MAIN_BALANCE_THUMB_SELECTED, new List<int> {0,422,14,11} }
        };

        Dictionary<SkinContainer.EQ_TYPES, List<int>> eqdata = new Dictionary<SkinContainer.EQ_TYPES, List<int>>
        {
            { SkinContainer.EQ_TYPES.EQ_WINDOW_BACKGROUND, new List<int> {0,0,275, 116}},
            { SkinContainer.EQ_TYPES.EQ_TITLE_BAR, new List<int> {0,149,275, 14}},
            { SkinContainer.EQ_TYPES.EQ_TITLE_BAR_SELECTED, new List<int> {0,134,275, 14}},
            { SkinContainer.EQ_TYPES.EQ_SLIDER_BACKGROUND, new List<int> {13,164,209, 129}},
            { SkinContainer.EQ_TYPES.EQ_SLIDER_THUMB, new List<int> {0,164,11, 11}},
            { SkinContainer.EQ_TYPES.EQ_SLIDER_THUMB_SELECTED, new List<int> {0,176,11, 11}},
            { SkinContainer.EQ_TYPES.EQ_CLOSE_BUTTON, new List<int> {0,116,9, 9}},
            { SkinContainer.EQ_TYPES.EQ_CLOSE_BUTTON_ACTIVE, new List<int> {0,125,9, 9}},
            { SkinContainer.EQ_TYPES.EQ_MAXIMIZE_BUTTON_ACTIVE_FALLBACK, new List<int> {254,152,9,9,}},
            { SkinContainer.EQ_TYPES.EQ_ON_BUTTON, new List<int> {10,119,26, 12}},
            { SkinContainer.EQ_TYPES.EQ_ON_BUTTON_DEPRESSED, new List<int> {128,119,26, 12}},
            { SkinContainer.EQ_TYPES.EQ_ON_BUTTON_SELECTED, new List<int> {69,119,26, 12}},
            { SkinContainer.EQ_TYPES.EQ_ON_BUTTON_SELECTED_DEPRESSED, new List<int> {187,119,26,12,}},
            { SkinContainer.EQ_TYPES.EQ_AUTO_BUTTON, new List<int> {36,119,32, 12}},
            { SkinContainer.EQ_TYPES.EQ_AUTO_BUTTON_DEPRESSED, new List<int> {154,119,32,12,}},
            { SkinContainer.EQ_TYPES.EQ_AUTO_BUTTON_SELECTED, new List<int> {95,119,32, 12}},
            { SkinContainer.EQ_TYPES.EQ_AUTO_BUTTON_SELECTED_DEPRESSED, new List<int> {213,119,32,12,}},
            { SkinContainer.EQ_TYPES.EQ_GRAPH_BACKGROUND, new List<int> {0,294,113, 19}},
            { SkinContainer.EQ_TYPES.EQ_GRAPH_LINE_COLORS, new List<int> {115,294,1, 19}},
            { SkinContainer.EQ_TYPES.EQ_PRESETS_BUTTON, new List<int> {224,164,44, 12}},
            { SkinContainer.EQ_TYPES.EQ_PRESETS_BUTTON_SELECTED, new List<int> {224,176,44,12}},
            { SkinContainer.EQ_TYPES.EQ_PREAMP_LINE, new List<int> {0,314,113, 1}}
        };

        Dictionary<SkinContainer.SHUFREP_TYPES, List<int>> shufrepdata = new Dictionary<SkinContainer.SHUFREP_TYPES, List<int>>
        {
            { SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON, new List<int> {28,0,  47,  15}},
            { SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_DEPRESSED, new List<int> {28,15, 47, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED, new List<int> {28, 30, 47, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED_DEPRESSED, new List<int> {28, 45, 47, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON, new List<int> {0,0,  28,  15}},
            { SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_DEPRESSED, new List<int> {0,15, 28, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED, new List<int> {0, 30, 28, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED_DEPRESSED, new List<int> {0, 45, 28, 15,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_EQ_BUTTON, new List<int> {0,61,  23, 12}},
            { SkinContainer.SHUFREP_TYPES.MAIN_EQ_BUTTON_SELECTED, new List<int> {0,73,  23,  12}},
            { SkinContainer.SHUFREP_TYPES.MAIN_EQ_BUTTON_DEPRESSED, new List<int> {46,61,  23,  12}},
            { SkinContainer.SHUFREP_TYPES.MAIN_EQ_BUTTON_DEPRESSED_SELECTED, new List<int> {46, 73, 23, 12,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON, new List<int> {23,61,  23,  12}},
            { SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED, new List<int> {23, 73, 23, 12,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED, new List<int> {69, 61, 23, 12,}},
            { SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED_SELECTED, new List<int> {69, 73, 23, 12, } }
        };

        Dictionary<SkinContainer.MEDIABROWSER_TYPES, List<int>> mediabrowserdata = new Dictionary<SkinContainer.MEDIABROWSER_TYPES, List<int>>
        {
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_LEFT, new List<int> {0,0,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TITLE, new List<int> {26,0,100, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TILE, new List<int> {127,0,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_RIGHT, new List<int> {153,0,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_LEFT_UNSELECTED, new List<int> {0,21,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TITLE_UNSELECTED, new List<int> {26,21,100, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TILE_UNSELECTED, new List<int> {127,21,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_RIGHT_UNSELECTED, new List<int> {153,21,25, 20}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_MID_LEFT, new List<int> {127,42, 11, 29}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_MID_RIGHT, new List<int> {139,42, 8, 29}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_LEFT, new List<int> {0,42, 125, 38}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_TILE, new List<int> {127,81, 25, 38}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_RIGHT, new List<int> {0,81, 125, 38}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_CLOSE, new List<int> {148, 42, 9, 9}},

            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_PREV, new List<int> {158, 42, 14, 14}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_NEXT, new List<int> {173, 42, 14, 14}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_NEW, new List<int> {188, 42, 14, 14}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_RELOAD, new List<int> {203, 42, 14, 14}},
            { SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_REMOVE, new List<int> {218, 42, 14, 14}}

        };

        Dictionary<SkinContainer.GENEX_TYPES, List<int>> genexdata = new Dictionary<SkinContainer.GENEX_TYPES, List<int>>
        {
            {SkinContainer.GENEX_TYPES.GENEX_BUTTON_BACKGROUND_LEFT_UNPRESSED, new List<int> {0,0,15,4 } },
            {SkinContainer.GENEX_TYPES.GENEX_BUTTON_BACKGROUND_CENTER_UNPRESSED, new List<int> {4,0,15,39 } },
            {SkinContainer.GENEX_TYPES.GENEX_BUTTON_BACKGROUND_RIGHT_UNPRESSED, new List<int> {43,0,15,4 } },
            {SkinContainer.GENEX_TYPES.GENEX_BUTTON_BACKGROUND_PRESSED, new List<int> {0,1,15,47 }},
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_UP_UNPRESSED, new List<int> {0, 31, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_DOWN_UNPRESSED, new List<int> {14,31,14,14 }},
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_UP_PRESSED, new List<int> {28, 31, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_DOWN_PRESSED, new List<int> {42, 31, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_UNPRESSED, new List<int> {0, 45, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_RIGHT_UNPRESSED, new List<int> {14,45,14,14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_PRESSED, new List<int> {28, 45, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_SCROLL_RIGHT_PRESSED, new List<int> {42, 45, 14, 14 } },
            {SkinContainer.GENEX_TYPES.GENEX_VERTICAL_SCROLL_HANDLE_UNPRESSED, new List<int> {56,31,28,14 } },
            {SkinContainer.GENEX_TYPES.GENEX_VERTICAL_SCROLL_HANDLE_PRESSED, new List<int> {70,31,28,14 } },
            {SkinContainer.GENEX_TYPES.GENEX_HORIZONTAL_SCROLL_HANDLE_UNPRESSED, new List<int> {84,31,14,28 } },
            {SkinContainer.GENEX_TYPES.GENEX_HORIZONTAL_SCROLL_HANDLE_PRESSED, new List<int> {84,45,14,28 } },
        };

        Dictionary<SkinContainer.PLAYLIST_TYPES, List<int>> playlistdata = new Dictionary<SkinContainer.PLAYLIST_TYPES, List<int>>
        {
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_TILE, new List<int> {127,21,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_LEFT_CORNER, new List<int> {0,21,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TITLE_BAR, new List<int> {26,21,100,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_RIGHT_CORNER, new List<int> {153,21,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_TILE_SELECTED, new List<int> {127,0,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_LEFT_SELECTED, new List<int> {0,0,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TITLE_BAR_SELECTED, new List<int> {26,0,100,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_RIGHT_CORNER_SELECTED, new List<int> {153,0,25,20 } },
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_LEFT_TILE, new List<int> {0,42,12,29}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_RIGHT_TILE, new List<int> {31,42,20,29}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_TILE, new List<int> {179,0,25,38}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_LEFT_CORNER, new List<int> {0,72,125,38}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_RIGHT_CORNER, new List<int> {126,72,150,38}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_VISUALIZER_BACKGROUND, new List<int> {205,0,75,38}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SHADE_BACKGROUND, new List<int> {72,57,25,14}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SHADE_BACKGROUND_LEFT, new List<int> {72,42,25,14}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SHADE_BACKGROUND_RIGHT, new List<int> {99,57,50,14}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SHADE_BACKGROUND_RIGHT_SELECTED, new List<int> {99,42,50,14}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SCROLL_HANDLE_SELECTED, new List<int> {61,53,8,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SCROLL_HANDLE, new List<int> {52,53,8,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_URL, new List<int> {0,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_URL_SELECTED, new List<int> {23,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_DIR, new List<int> {0,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_DIR_SELECTED, new List<int> {23,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_FILE, new List<int> {0,149,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_FILE_SELECTED, new List<int> {23,149,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_ALL, new List<int> {54,111,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_ALL_SELECTED, new List<int> {77,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_CROP, new List<int> {54,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_CROP_SELECTED, new List<int> {77,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_SELECTED, new List<int> {54,149,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_SELECTED_SELECTED, new List<int> {77,149,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_MISC, new List<int> {54,168,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_MISC_SELECTED, new List<int> {77,168,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_INVERT_SELECTION, new List<int> {104,111,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_INVERT_SELECTION_SELECTED, new List<int> {127,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SELECT_ZERO, new List<int> {104,130,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_SELECT_ZERO_SELECTED, new List<int> {127,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SELECT_ALL, new List<int> {104,149,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_SELECT_ALL_SELECTED, new List<int> {127,149,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SORT_LIST, new List<int> {154,111,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_SORT_LIST_SELECTED, new List<int> {177,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_FILE_INFO, new List<int> {154,130,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_FILE_INFO_SELECTED, new List<int> {177,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_MISC_OPTIONS, new List<int> {154,149,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_MISC_OPTIONS_SELECTED, new List<int> {177,149,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_NEW_LIST, new List<int> {204,111,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_NEW_LIST_SELECTED, new List<int> {227,111,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SAVE_LIST, new List<int> {204,130,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_SAVE_LIST_SELECTED, new List<int> {227,130,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_LOAD_LIST, new List<int> {204,149,22,18}},
            {SkinContainer.PLAYLIST_TYPES.PLAYLIST_LOAD_LIST_SELECTED, new List<int> {227,149,22,18}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_ADD_MENU_BAR, new List<int> {48,111,3,54}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_REMOVE_MENU_BAR, new List<int> {100,111,3,72}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_SELECT_MENU_BAR, new List<int> {150,111,3,54}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_MISC_MENU_BAR, new List<int> {200,111,3,54}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_LIST_BAR, new List<int> {250,111,3,54}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_CLOSE_SELECTED, new List<int> {52,42,9,9}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_COLLAPSE_SELECTED, new List<int> {62,42,9,9}},
            { SkinContainer.PLAYLIST_TYPES.PLAYLIST_EXPAND_SELECTED, new List<int> {150,42,9,9}}
        };

        Dictionary<SkinContainer.SWINDOW_TYPES, List<int>> swindowdata = new Dictionary<SkinContainer.SWINDOW_TYPES, List<int>>
        {
            { SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER, new List<int> {15,0,50,15 } },
            { SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE, new List<int> {66,0,14,15 } },
            { SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER, new List<int> {81,0, 16,15 } },
            { SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE, new List<int> {0,16,7,172}},
            { SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE, new List<int> {8,16,6,172}},
            { SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER, new List<int> {15,16,50,5}},
            { SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE, new List<int> {66,16,14,5}},
            { SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER, new List<int> {81,16,16,5}},
            { SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED, new List<int> {0,0,9,9}}
        };
        #endregion

        /// <summary>
        /// Load the selected skin
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        public void LoadSkin(string filename)
        {
            //Load the background image
            loadBackgroundMain(filename);

            //Clear the container
            SkinContainer.TITLEBAR.Clear();
            SkinContainer.VOLUME.Clear();
            SkinContainer.BALANCE.Clear();
            SkinContainer.CBUTTONS.Clear();
            SkinContainer.FONT.Clear();
            SkinContainer.GENEX.Clear();
            SkinContainer.NUMBERS.Clear();
            SkinContainer.PLAYLIST.Clear();
            SkinContainer.PLAYLISTCOLOR.Clear();
            SkinContainer.SHUFREP.Clear();
            SkinContainer.MEDIABROWSER.Clear();
            SkinContainer.SWINDOW.Clear();
            SkinContainer.EQUALIZER.Clear();
            SkinContainer.VISCOLOR.Clear();
            

            List<string> visdata = ExtractViscolorFromZip(filename, "viscolor.txt");
            SkinContainer.VISCOLOR.Add(SkinContainer.VISCOLOR_TYPES.VISCOLOR_BACKGROUND, GetColor(visdata[(int)SkinContainer.VISCOLOR_TYPES.VISCOLOR_BACKGROUND]));
            //SkinContainer.VISCOLOR.Add(SkinContainer.VISCOLOR_TYPES.VISCOLOR_PEAKS, GetColor(visdata[(int)SkinContainer.VISCOLOR_TYPES.VISCOLOR_PEAKS]));

            //load the bitmaps
            loadTitlebarAndButtons(filename);
            loadVolumebar(filename);
            loadBalancebar(filename);
            loadControlButtons(filename);
            loadNumbersAndFont(filename);
            loadTransportbarAndClutter(filename);
            loadPlaylistDesign(filename);
            loadPlaylistColor(filename);
            loadEqBackGround(filename);
            loadShufRepDesign(filename);
            loadMediaBrowserWindow(filename);
            loadAVSWindow(filename);
            loadGenEx(filename);

            //apply skin
            ApplySkin();
        }

        /// <summary>
        /// Apply the skin
        /// </summary>
        private void ApplySkin()
        {
            //Set the main window skin
            this.TitleBar.Fill = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_TITLE_BAR_SELECTED];
            this.Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON];
            this.Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON];

            this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED];
            this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON];
            this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON];

            this.Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON];
            this.Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON];
            this.Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON];
            this.Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON];
            this.Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON];
            this.Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON];

            this.Second_First.Fill = SkinContainer.NUMBERS[0];
            this.Second_Last.Fill = SkinContainer.NUMBERS[0];
            this.Minutes_First.Fill = SkinContainer.NUMBERS[0];
            this.Minutes_Last.Fill = SkinContainer.NUMBERS[0];

            this.Trackbar_Background.Fill = SkinContainer.VOLUME[SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_0];
            this.Octavebar_Background.Fill = SkinContainer.VOLUME[SkinContainer.VOLUME_TYPES.MAIN_VOLUME_BACKGROUND_5];

            //set the global resources
            Application.Current.Resources["MAIN_VOLUME_THUMB"] = SkinContainer.VOLUME[SkinContainer.VOLUME_TYPES.MAIN_VOLUME_THUMB];
            Application.Current.Resources["MAIN_VOLUME_THUMB_SELECTED"] = SkinContainer.VOLUME[SkinContainer.VOLUME_TYPES.MAIN_VOLUME_THUMB_SELECTED];
            Application.Current.Resources["MAIN_BALANCE_THUMB"] = SkinContainer.BALANCE[SkinContainer.BALANCE_TYPES.MAIN_BALANCE_THUMB];
            Application.Current.Resources["MAIN_BALANCE_THUMB_SELECTED"] = SkinContainer.BALANCE[SkinContainer.BALANCE_TYPES.MAIN_BALANCE_THUMB_SELECTED];
            
            //inform all members a new skin was loaded
            SkinContainer.NewSkinLoaded();
        }

        #region loaders
        /// <summary>
        /// load the main background
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadBackgroundMain(string filename)
        {
            BitmapImage image = ExtractBitmapFromZip(filename, "main.bmp");
            if (image == null)
                image = loadDefaultSkinBitmap("main.bmp");

            this.Background = new ImageBrush(image);

            /*Image img = ExtractImageFromZip(filename, "main.bmp");
            if (img == null)
                img = loadDefaultSkinData("main.bmp");

            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(img, new Rectangle(0,0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, GetTransparentAttribFromColor()); ;
            this.Background = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions()));*/

        }


        /// <summary>
        /// load titlebar and buttons
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadTitlebarAndButtons(string filename)
        {
            Image img = ExtractImageFromZip(filename, "titlebar.bmp");
            if (img == null)
                img = loadDefaultSkinData("titlebar.bmp");
            foreach (KeyValuePair<SkinContainer.TITLEBAR_TYPES, List<int>> data in titlebardata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.TITLEBAR.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// load volumebar
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadVolumebar(string filename)
        {
            Image img = ExtractImageFromZip(filename, "volume.bmp");
            if (img == null)
                img = loadDefaultSkinData("volume.bmp");
            foreach (KeyValuePair<SkinContainer.VOLUME_TYPES, List<int>> data in volumedata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.VOLUME.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// load volumebar
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadBalancebar(string filename)
        {
            Image img = ExtractImageFromZip(filename, "balance.bmp");
            if (img == null)
                img = loadDefaultSkinData("balance.bmp");
            foreach (KeyValuePair<SkinContainer.BALANCE_TYPES, List<int>> data in balancedata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.BALANCE.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// load the playcontrol button for the main window
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadControlButtons(string filename)
        {
            Image img = ExtractImageFromZip(filename, "CBUTTONS.BMP");
            if (img == null)
                img = loadDefaultSkinData("cbuttons.bmp");
            foreach (KeyValuePair<SkinContainer.CBUTTON_TYPES, List<int>> data in cbuttonsdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.CBUTTONS.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// loads the transportbar
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadTransportbarAndClutter(string filename)
        {
            //Transportbar
            Image img = ExtractImageFromZip(filename, "posbar.bmp");
            if (img == null)
                img = loadDefaultSkinData("posbar.bmp");
            this.PlayBar_Background.Fill = ExtractImage(img, 248, 9, 0, 0);
            Application.Current.Resources["MAIN_POSITION_SLIDER_THUMB"] = ExtractImage(img, 27, 9, 249, 0);
            Application.Current.Resources["MAIN_POSITION_SLIDER_THUMB_SELECTED"] = ExtractImage(img, 28, 9, 278, 0);
        }

        /// <summary>
        /// load the playlist design
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadPlaylistDesign(string filename)
        {
            Image img = ExtractImageFromZip(filename, "pledit.bmp");
            if (img == null)
                img = loadDefaultSkinData("pledit.bmp");
            foreach (KeyValuePair<SkinContainer.PLAYLIST_TYPES, List<int>> data in playlistdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);

                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.PLAYLIST.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
            Application.Current.Resources["PLAYLIST_SLIDER_THUMB"] = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_SCROLL_HANDLE];
            Application.Current.Resources["PLAYLIST_SLIDER_THUMB_SELECTED"] = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_SCROLL_HANDLE_SELECTED];

        }


        private void loadEqBackGround(string filename)
        {
            Image img = ExtractImageFromZip(filename, "eqmain.bmp");
            if (img == null)
                img = loadDefaultSkinData("eqmain.bmp");
            //foreach (KeyValuePair<SkinContainer.EQ_TYPES, List<int>> data in eqdata)
            {
                KeyValuePair<SkinContainer.EQ_TYPES, List<int>> data = eqdata.First();
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);

                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.EQUALIZER.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// load the shufle repeat button design
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadShufRepDesign(string filename)
        {
            Image img = ExtractImageFromZip(filename, "shufrep.bmp");
            if (img == null)
                img = loadDefaultSkinData("shufrep.bmp");
            foreach (KeyValuePair<SkinContainer.SHUFREP_TYPES, List<int>> data in shufrepdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);

                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.SHUFREP.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// Load the mediabrowser window/small window decoration
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadMediaBrowserWindow(string filename)
        {
            //avs window (small window decoration)
            Image img = ExtractImageFromZip(filename, "mb.bmp");
            if (img == null)
                img = loadDefaultSkinData("mb.bmp");

            foreach (KeyValuePair<SkinContainer.MEDIABROWSER_TYPES, List<int>> data in mediabrowserdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.MEDIABROWSER.Add((SkinContainer.MEDIABROWSER_TYPES)data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                           Int32Rect.Empty,
                                           BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// Load the avs window/small window decoration
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadAVSWindow(string filename)
        {
            //avs window (small window decoration)
            Image img = ExtractImageFromZip(filename, "avs.bmp");
            if (img == null)
                img = loadDefaultSkinData("avs.bmp");

            foreach (KeyValuePair<SkinContainer.SWINDOW_TYPES, List<int>> data in swindowdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.SWINDOW.Add((SkinContainer.SWINDOW_TYPES)data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                           Int32Rect.Empty,
                                           BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// load the font and the numbers
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadNumbersAndFont(string filename)
        {
            Image img = ExtractImageFromZip(filename, "numbers.bmp");
            if (img == null)
                img = ExtractImageFromZip(filename, "nums_ex.bmp");
            if (img == null)
                img = loadDefaultSkinData("nums_ex.bmp");
            foreach (KeyValuePair<SkinContainer.NUMBER_TYPES, List<int>> data in numbersdata)
            {
                Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                SkinContainer.NUMBERS.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions())));
                bitmap.Dispose();
            }

            //The "Font"
            img = ExtractImageFromZip(filename, "text.bmp");
            if (img == null)
                img = loadDefaultSkinData("text.bmp");
            {
                Bitmap bitmap = new Bitmap(5, 6);
                var graphics = Graphics.FromImage(bitmap);


                for (int row = 0; row != 3; row++)
                {
                    for (int col = 0; col < 30; col++)
                    {
                        graphics.DrawImage(img, new Rectangle(0, 0, 5, 6), new Rectangle(col * 5, row*6, 5, 6), GraphicsUnit.Pixel);
                        if (row == 0)
                        {
                            if (65 + col <= 90) //A-Z
                            {
                                SkinContainer.FONT.Add(65 + col, new Bitmap(bitmap));
                                SkinContainer.FONT.Add(97 + col, new Bitmap(bitmap));
                            }
                            else if (col == 26 || col == 27)//@ or "
                                SkinContainer.FONT.Add(((col == 26) ? 34 : 64), new Bitmap(bitmap));
                            else if (col == 29) //Space
                                SkinContainer.FONT.Add(32, new Bitmap(bitmap));
                        }
                        else if(row==1)
                        {
                            if(col < 10) //0-9
                                SkinContainer.FONT.Add(48 + col, new Bitmap(bitmap));
                            if (col == 12) //-
                                SkinContainer.FONT.Add(58, new Bitmap(bitmap));
                            if (col == 13 || col == 14) // ()
                                SkinContainer.FONT.Add(27+col, new Bitmap(bitmap)); //40-13+col
                            if (col == 15) //-
                                SkinContainer.FONT.Add(45, new Bitmap(bitmap));
                            if (col == 16) //'
                                SkinContainer.FONT.Add(39, new Bitmap(bitmap));
                            if (col == 17) //!
                                SkinContainer.FONT.Add(33, new Bitmap(bitmap));
                            if (col == 18) //_
                                SkinContainer.FONT.Add(95, new Bitmap(bitmap));
                            if (col == 19) //+
                                SkinContainer.FONT.Add(43, new Bitmap(bitmap));
                            if (col == 20) // \
                                SkinContainer.FONT.Add(92, new Bitmap(bitmap));
                            if (col == 21) // /
                                SkinContainer.FONT.Add(47, new Bitmap(bitmap));
                            if (col == 22) // [
                                SkinContainer.FONT.Add(91, new Bitmap(bitmap));
                            if (col == 23) // ]
                                SkinContainer.FONT.Add(93, new Bitmap(bitmap));
                        }
                    }
                }
                bitmap.Dispose();
            }

        }

        /// <summary>
        /// load the generix extends
        /// </summary>
        /// <param name="filename">wsz fullpath and filename</param>
        private void loadGenEx(string filename)
        {
            //Buttons / or similar
            Image img = ExtractImageFromZip(filename, "genex.bmp");
            if (img != null)
            {
                foreach (KeyValuePair<SkinContainer.GENEX_TYPES, List<int>> data in genexdata)
                {
                    Bitmap bitmap = new Bitmap(data.Value.ElementAt(2), data.Value.ElementAt(3));
                    var graphics = Graphics.FromImage(bitmap);
                    graphics.DrawImage(img, new Rectangle(0, 0, data.Value.ElementAt(2), data.Value.ElementAt(3)), new Rectangle(data.Value.ElementAt(0), data.Value.ElementAt(1), data.Value.ElementAt(2), data.Value.ElementAt(3)), GraphicsUnit.Pixel);
                    SkinContainer.GENEX.Add(data.Key, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                                    Int32Rect.Empty,
                                                                                    BitmapSizeOptions.FromEmptyOptions())));
                    bitmap.Dispose();
                }
            }
            else
            {
                List<int> data = genexdata[SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_UNPRESSED];
                Bitmap bitmap = new Bitmap(data.ElementAt(2) - 2, data.ElementAt(3) - 2);
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(SkinContainer.FONT[68], 0, 0);
                SkinContainer.GENEX.Add(SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_UNPRESSED, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                                Int32Rect.Empty,
                                                                                BitmapSizeOptions.FromEmptyOptions())));
                SkinContainer.GENEX.Add(SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_PRESSED, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                                Int32Rect.Empty,
                                                                                BitmapSizeOptions.FromEmptyOptions())));
                graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(SkinContainer.FONT[85], 0, 0);
                SkinContainer.GENEX.Add(SkinContainer.GENEX_TYPES.GENEX_SCROLL_RIGHT_UNPRESSED, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                                Int32Rect.Empty,
                                                                                BitmapSizeOptions.FromEmptyOptions())));
                SkinContainer.GENEX.Add(SkinContainer.GENEX_TYPES.GENEX_SCROLL_RIGHT_PRESSED, new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                                                                                Int32Rect.Empty,
                                                                                BitmapSizeOptions.FromEmptyOptions())));

                SkinContainer.GENEX[SkinContainer.GENEX_TYPES.GENEX_SCROLL_RIGHT_UNPRESSED].Stretch = Stretch.UniformToFill;
                SkinContainer.GENEX[SkinContainer.GENEX_TYPES.GENEX_SCROLL_LEFT_UNPRESSED].Stretch = Stretch.UniformToFill;
                bitmap.Dispose();
            }
        }
        #endregion

        #region extractors and helpers
        /// <summary>
        /// load the default skin data
        /// </summary>
        /// <param name="name">name of the bitmap</param>
        /// <returns>Bitmap</returns>
        private Image loadDefaultSkinData(string name)
        {
            var bitmapImage = new BitmapImage(new Uri(@"pack://application:,,,/"
                                + Assembly.GetExecutingAssembly().GetName().Name
                                + ";component/"
                                + "Resources/Skin/"
                                + name, UriKind.Absolute));

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)bitmapImage));
            var stream = new MemoryStream();
            encoder.Save(stream);
            stream.Flush();
            return new Bitmap(stream);
        }

        /// <summary>
        /// load the default bimapimage
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BitmapImage</returns>
        private BitmapImage loadDefaultSkinBitmap(string name)
        {
            var bitmapImage = new BitmapImage(new Uri(@"pack://application:,,,/"
                                + Assembly.GetExecutingAssembly().GetName().Name
                                + ";component/"
                                + "Resources/Skin/"
                                + name, UriKind.Absolute));
            return bitmapImage;
        }

        /// <summary>
        /// extracts an image from the wsz archive
        /// </summary>
        /// <param name="archivename">wsz fullpath and filename</param>
        /// <param name="imagename">image name</param>
        /// <returns>Image</returns>
        Image ExtractImageFromZip(string archivename, string imagename)
        {
            ZipArchive zip;
            try { zip = ZipFile.OpenRead(@archivename); }
            catch { return null; }

            var ent = zip.Entries;
            string regex = @"\b(" + imagename + @")\b";
            foreach (var entry in ent)
            {
                if (Regex.IsMatch(entry.Name, regex, RegexOptions.IgnoreCase))
                {
                    if (entry != null)
                    {
                        using (var zipStream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            zipStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            var image = Image.FromStream(memoryStream);
                            memoryStream.Close();
                            zipStream.Close();
                            return image;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// extracts a bitmap from the wsz archive
        /// </summary>
        /// <param name="archivename">wsz fullpath and filename</param>
        /// <param name="imagename">name of the image</param>
        /// <returns>BitmapImage</returns>
        public BitmapImage ExtractBitmapFromZip(string archivename, string imagename)
        {
            ZipArchive zip;
            try { zip = ZipFile.OpenRead(@archivename); }
            catch { return null; }
            
            var ent = zip.Entries;
            string regex = @"\b(" + imagename + @")\b";
            foreach (var entry in ent)
            {
                if (Regex.IsMatch(entry.Name, regex, RegexOptions.IgnoreCase))
                {
                    if (entry != null)
                    {
                        using (var zipStream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            zipStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = memoryStream;
                            bitmap.EndInit();
                            memoryStream.Close();
                            zipStream.Close();
                            return bitmap;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// extracts the color palette for the playlist
        /// </summary>
        /// <param name="archivename">wsz fullpath and filename</param>
        private void loadPlaylistColor(string archivename)
        {
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL, GetColorFromHex("Normal=#C4FFC4"));
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_CURRENT, GetColorFromHex("Normal=#FFFFFF"));
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG, GetColorFromHex("Normal=#000000"));
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_SELECTBG, GetColorFromHex("Normal=#6B6B6F"));
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_MBBG, GetColorFromHex("Normal=#000000"));
            SkinContainer.PLAYLISTCOLOR.Add(SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_MBFG, GetColorFromHex("Normal=#FFFFFF"));
            ZipArchive zip;
            try { zip = ZipFile.OpenRead(@archivename); }
            catch
            {
                return;
            }
            var ent = zip.Entries;
            string regex = @"\b(" + "pledit.txt" + @")\b";
            foreach (var entry in ent)
            {
                if (Regex.IsMatch(entry.Name, regex, RegexOptions.IgnoreCase))
                {
                    if (entry != null)
                    {
                        using (var zipStream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            zipStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            var data = new List<string>();
                            using (var reader = new StreamReader(memoryStream, Encoding.ASCII))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.IndexOf("NormalBG", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG] = GetColorFromHex(line);
                                    else if (line.IndexOf("Normal", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL] = GetColorFromHex(line);
                                    else if (line.IndexOf("Current", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_CURRENT] = GetColorFromHex(line);
                                    else if (line.IndexOf("SelectedBG", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_SELECTBG] = GetColorFromHex(line);
                                    else if (line.IndexOf("mbBG", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_MBBG] = GetColorFromHex(line);
                                    else if (line.IndexOf("mbFG", StringComparison.OrdinalIgnoreCase) >= 0)
                                        SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_MBFG] = GetColorFromHex(line);
                                }
                            }
                            memoryStream.Close();
                            zipStream.Close();
                            return;
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// extracts the viscolor palette
        /// </summary>
        /// <param name="archivename">wsz fullpath and filename</param>
        /// <param name="imagename"></param>
        /// <returns>List</returns>
        List<string> ExtractViscolorFromZip(string archivename, string imagename)
        {
            ZipArchive zip;
            try { zip = ZipFile.OpenRead(@archivename); }
            catch
            {
                var data = new List<string>();
                data.Add("0,0,0");
                data.Add("0,0,0");
                data.Add("239,49,16");
                data.Add("206,41,16");
                data.Add("214,90,0");
                data.Add("214,102,0");
                data.Add("214,115,0");      // 6
                data.Add("198,123,8");      // 7
                data.Add("222,165,24");     // 8
                data.Add("214,181,33");     // 9
                data.Add("189,222,41");     // 10 = mid of spec
                data.Add("148,222,33");     // 11
                data.Add("41,206,16");      // 12
                data.Add("50,190,16");      // 13
                data.Add("57,181,16");      // 14
                data.Add("49,156,8");       // 15
                data.Add("41,148,0");       // 16
                data.Add("24,132,8");       // 17 = bottom of spec
                data.Add("255,255,255");    // 18 = osc 1 (brightest)
                data.Add("214,214,222");    // 19 = osc 2 (slightly dimmer)
                data.Add("181,189,189");    // 20 = osc 3
                data.Add("160,170,175");    // 21 = osc 4
                data.Add("148,156,165");    // 22 = osc 5 (dimmest)
                data.Add("150,150,150");
                return data;
            }

            var ent = zip.Entries;
            string regex = @"\b(" + imagename + @")\b";
            foreach (var entry in ent)
            {
                if (Regex.IsMatch(entry.Name, regex, RegexOptions.IgnoreCase))
                {
                    if (entry != null)
                    {
                        using (var zipStream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            zipStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            var data = new List<string>();
                            using (var reader = new StreamReader(memoryStream, Encoding.ASCII))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    data.Add(line);
                                }
                            }
                            memoryStream.Close();
                            zipStream.Close();
                            return data;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// extracts an image tile from a image
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        /// <returns>ImageBrush</returns>
        ImageBrush ExtractImage(Image img, int x, int y, int offset_x, int offset_y)
        {
            Bitmap p = new Bitmap(x, y);
            var graphics = Graphics.FromImage(p);
            graphics.DrawImage(img, new Rectangle(0, 0, x, y), new Rectangle(offset_x, offset_y, x, y), GraphicsUnit.Pixel);
            graphics.Dispose();
            return new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(p.GetHbitmap(), IntPtr.Zero,
                                                                                        Int32Rect.Empty,
                                                                                        BitmapSizeOptions.FromEmptyOptions()));
        }

        /// <summary>
        /// Gets the rgb color from string
        /// </summary>
        /// <param name="data"></param>
        /// <returns>System.Drawing.Color</returns>
        System.Drawing.Color GetColor(string data)
        {
            byte[] colval = new byte[3];
            string[] numbers = Regex.Split(data, @"\D+");
            int idx = 0;
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (idx >= 3)
                        break;
                    int i = int.Parse(value);
                    colval[idx] = ((byte)int.Parse(value));
                    idx++;
                }
            }
            return System.Drawing.Color.FromArgb(colval[0], colval[1], colval[2]);
        }

        System.Drawing.Color GetColorFromHex(string data)
        {
            string x = data.Split('#')[1];
            return ColorTranslator.FromHtml("#"+x);
        }

        System.Drawing.Imaging.ImageAttributes GetTransparentAttribFromColor()
        {
            System.Drawing.Color lowerColor = System.Drawing.Color.FromArgb(100, 0, 100);
            System.Drawing.Color upperColor = System.Drawing.Color.FromArgb(100, 0, 100);
            System.Drawing.Imaging.ImageAttributes imageAttr = new System.Drawing.Imaging.ImageAttributes();
            imageAttr.SetColorKey(lowerColor, upperColor, System.Drawing.Imaging.ColorAdjustType.Default);
            return imageAttr;
        }
        #endregion
    }
}
