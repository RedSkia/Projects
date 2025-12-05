using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using UnityEngine;



/* IDEAS
 * Modes for plugins like admintoggle
 * Blacklist plugins for modes
 * Popup Warning Save (IF NOT ASK DISNY)
 */

namespace Oxide.Plugins
{
    [Info("ConfigEditor", "InfinityNet/Xray", "1.0.0")]
    [Description("WIP")]
    internal sealed class ConfigEditor : RustPlugin
    {
        private static System.Type THIS => MethodBase.GetCurrentMethod().DeclaringType;
        private static ConfigEditor This => new ConfigEditor();



        private void Loaded()
        {
            //var lic = new DllLib.Authorizer.Licence("1111", "22222");


            This.cmd.AddConsoleCommand(UI.Command, This, UI.ccHandle);
        }
        private void Unload()
        {
            This.cmd.RemoveConsoleCommand(UI.Command, This);
            foreach (var player in BasePlayer.activePlayerList)
            {
                UI.Destroy(player);
            }
        }

        [ChatCommand("ui")]
        private void dd(BasePlayer player)
        {
            UI.Draw(player);
        }
        [ChatCommand("no")]
        private void ff(BasePlayer player)
        {
            UI.Destroy(player);
        }



   
        private static class UI
        {
            internal static string Command => $"{THIS.Name}UI";
            private struct Settings
            {
                internal const int height = 300, width = 400;
                internal const int offsetX = 0, offsetY = -22;
                internal const int margin = 5;
                internal const float overlayFade = 0.2f;
                internal const float titleBarHeight = 0.075f;
                internal const float sideNavWidth = 0.2f;
                internal const float fileTypeNavHeight = 0.7f;
                internal const float saveBtnWidth = 0.2f;
                internal const float elementSize = 0.1f;
                internal const string emptyOffset = "0 0 0 0";

                internal const int pluginPaginationLimit = 10;
            }

            internal static class Elements
            {
                private const int w = Settings.width;
                private const int h = Settings.height;
                private const int x = Settings.offsetX;
                private const int y = Settings.offsetY;
                private const int m = Settings.margin;
                private const float th = Settings.titleBarHeight;
                private const float sn = Settings.sideNavWidth;
                private const float es = Settings.elementSize;
                private const string o = Settings.emptyOffset;
                private const TextAnchor ac = TextAnchor.MiddleCenter;
                private const Controls.RoundType ra = Controls.RoundType.All;
                private const Controls.RoundType rt = Controls.RoundType.Top;
                private const int pl = Settings.pluginPaginationLimit;
                private const float fh = Settings.fileTypeNavHeight;
                private const float sb = Settings.saveBtnWidth;
                private static string Margin(float margin) => $"{margin} {-margin} {margin} {-margin}";
                
                internal static CuiElementContainer Base
                {
                    get
                    {
                        CuiElementContainer Container = new CuiElementContainer();
                        Controls.Container(ref Container, Names.Base, "0.5 0.5 0.5 0.5", "DimGray", $"{-(w + x)} {(w - x)} {-(h + y)} {(h - y)}", m, ra);
                        Controls.Panel(ref Container, Names.Base, $"0 1 0 {1 - th}", "DarkRed", Names.Main, $"{Margin(m)}", ra);
                        Controls.Panel(ref Container, Names.Base, $"0 1 {1 - th} 1", "DarkSlateGray", Names.TitleBar, o, rt);
                        Controls.Label(ref Container, Names.TitleBar, "0 1 0 1", $"ConfigEditor v{This.Version}", 20);
                        Controls.Button(ref Container, Names.TitleBar, "0.94 1 0 1", "Maroon", $"{ccAction.close}", "X", 24, Margin(m), ac, ra);
                        Controls.Panel(ref Container, Names.Main, $"0 {sn} 0 1", "Green", Names.SideNav);
                        Controls.Panel(ref Container, Names.Main, $"{sn} 1 {1 - es} 1", "DarkSlateBlue", Names.TopNav, $"{m} 0 0 0");
                        Controls.Panel(ref Container, Names.Main, $"{sn} 1 0 {es}", "DarkSlateBlue", Names.BottomNav, $"{m} 0 0 0");
                        Controls.Panel(ref Container, Names.Main, $"{sn} 1 {es} {1 - es}", "SeaGreen", Names.DisplayPanel, $"{m} 0 {m} {-m}");
                        return Container;
                    }
                }
                internal static void Overlay(ref CuiElementContainer Container, float fadeDelay = Settings.overlayFade) => Overlay(ref Container, fadeDelay, fadeDelay);
                internal static void Overlay(ref CuiElementContainer Container, float fadeIn, float fadeOut)
                {
                    Container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = Hex2Cui("Black", 0.9f),
                            Material = "assets/content/ui/uibackgroundblur.mat",
                            FadeIn = fadeIn,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0 0", AnchorMax = "1 1",
                            OffsetMin = "0 0", OffsetMax = "0 0"
                        },
                        FadeOut = fadeOut,
                    }, "Overlay", Names.Overlay);
                }
                internal static void Plugins(ref CuiElementContainer Container, ulong id)
                {
                    /*
                    var data = Data.Get(id);
                    Controls.Label(ref Container, Names.SideNav, $"0 1 {1 - (es / 2)} 1", "Search", 16);
                    Controls.Panel(ref Container, Names.SideNav, $"0 1 {1 - es} {1 - (es / 2)}", "PaleVioletRed", Names.PluginSearchPanel);
                    Controls.Input(ref Container, Names.PluginSearchPanel, $"{ccAction.setPluginSearch}", data.PluginSearch);
                    Controls.Panel(ref Container, Names.SideNav, $"0 1 {es} {1 - (es)}", "SaddleBrown", Names.PluginPanel, $"0 0 {m} {-m}");
                    var array = DataHelper.GetPlugins().Skip(data.PluginPage * pl).Take(pl).OrderBy(p => p.Name).ToArray();
                    float height = 1.0f / pl;
                    for (int i = 0; i < array.Length; i++)
                    {
                        var index = array[i];
                        string color = index.Name == data.PluginSelected ? "Teal" : "Black";
                        Controls.Button(ref Container, Names.PluginPanel, $"0 1 {1 - ((i + 1) * height)} {1 - (i * height)}", color, $"{ccAction.setPlugin} {index.Name}", index.Name);
                    }
                    Controls.Button(ref Container, Names.SideNav, $"0 1 {es / 2} {es}", "Navy", $"{ccAction.setPluginPage} {data.PluginPage - 1}", "▲", 16);
                    Controls.Button(ref Container, Names.SideNav, $"0 1 0 {es / 2}", "Goldenrod", $"{ccAction.setPluginPage} {data.PluginPage + 1}", "▼", 16);
                    */                
                }
                internal static void TopNav(ref CuiElementContainer Container, ulong id)
                {
                    /*
                    var data = Data.Get(id);
                    var types = Enum.GetNames(typeof(DataHelper.DataType));
                    float width1 = 1.0f / types.Length;
                    for (int i = 0; i < types.Length; i++)
                    {
                        var index = types[i];
                        This.PrintWarning($"{data.FileType} -> {index}");
                        string color = (data.FileType.ToString() == index) ? "DarkGreen" : "DarkRed";
                        Controls.Button(ref Container, Names.TopNav, $"{width1 * i} {width1 * (i + 1)} {1 - fh} 1", color, $"{ccAction.setFileType} {index}", $"{index} files", 16);
                    }
                    Controls.Panel(ref Container, Names.TopNav, $"0 {1 - es} 0 {1 - fh}", "Black", Names.TokenPathPanel);
                    string[] path = new string[] { "Plugin", "Obj1", "Obj2", "Obj3", "List" };
                    float width2 = 1.0f / path.Length;
                    for (int i = 0; i < path.Length; i++)
                    {
                        float margin1 = i > 0 ? m : 0;
                        float margin2 = i < path.Length - 1 ? m : 0;
                        var index = path[i];
                        Controls.Button(ref Container, Names.TokenPathPanel, $"{width2 * i} {width2 * (i + 1)} 0 1", "DarkOrange", "????", index, 14, $"{margin1} {-margin2} 0 0");
                        if (i < path.Length - 1)
                        {
                            Controls.Label(ref Container, Names.TokenPathPanel, $"{width2 * (i + 1)} {width2 * (i + 1)} 0 1", "/", 12, $"{-m} {m} 0 0");
                        }
                    }
                    Controls.Button(ref Container, Names.TopNav, $"{1 - es} {1 - es + (es / 2)} 0 {1 - fh}", "Brown", "????", "◀");
                    Controls.Button(ref Container, Names.TopNav, $"{1 - es + (es / 2)} 1 0 {1 - fh}", "Navy", "????", "▶");
                    */
                }
                internal static void BottomNav(ref CuiElementContainer Container)
                {
                    Controls.Button(ref Container, Names.BottomNav, $"0 {es} 0 1", "SeaGreen", "????", " ◀", 30, $"{m} 0 {m} {-m}", TextAnchor.MiddleLeft, ra);
                    Controls.Button(ref Container, Names.BottomNav, $"{1 - sb - es} {1 - sb} 0 1", "SeaGreen", "????", "▶ ", 30, $"{m} 0 {m} {-m}", TextAnchor.MiddleRight, ra);
                    Controls.Panel(ref Container, Names.BottomNav, $"{es / 1.75} {1 - sb - (es / 2)} 0 1", "Teal", Names.PaginationPanel, $"0 0 {m} {-m}");
                    var array = Enumerable.Range(1, 100).ToArray().Skip(0 * pl).Take(pl).OrderByDescending(x => x).ToArray();
                    float width = (1.0f / pl);
                    for (int i = 0; i < array.Length; i++)
                    {
                        var index = array[i];
                        Controls.Button(ref Container, Names.PaginationPanel, $"{1 - ((i + 1) * width)} {1 - (i * width)} 0 1", "SaddleBrown", "????", $"{index}");
                    }
                    Controls.Button(ref Container, Names.BottomNav, $"{1 - sb} 1 0 1", "SeaGreen", "????", "Save", 16, Margin(m), TextAnchor.MiddleCenter, ra);
                }
                internal static void Display(ref CuiElementContainer Container)
                {
                    int[] arr = Enumerable.Range(1, 21).ToArray();
                    float width = 0.5f;
                    float height = 0.1f;
                    const int rowLimit = 2;
                    int row = 0;
                    int i = 0;
                    foreach (var item in arr.Skip(0 * 20).Take(20))
                    {
                        if (i >= rowLimit)
                        {
                            row++;
                            i = 0;
                        }
                        Types.Bool(ref Container, $"{width * i} {width * (i + 1)} {1 - (height * (row + 1))} {1 - (height * row)}");
                        i++;
                    }
                }
                private static class Types
                {
                    private static void Base(ref CuiElementContainer Container, string anchor, string property, JTokenType type)
                    {
                        Controls.Panel(ref Container, Names.DisplayPanel, anchor, "Black", Names.TokenValue, Margin(m / 2));
               

                        float startWidth = 0;
                        if (type != JTokenType.Boolean)
                        {
                            startWidth = 0.125f;
                            Controls.Button(ref Container, Names.TokenValue, $"0 {startWidth} 0 1", "Purple", "????", "↗", 24, Margin(m));
                        }
                        Controls.Label(ref Container, Names.TokenValue, $"{startWidth} 0.6 0 1", $"[{property}] ({type})", 14, $"{m} 0 0 0", TextAnchor.MiddleLeft);
                    }
                    internal static void String(ref CuiElementContainer Container, string anchor)
                    {
                        Base(ref Container, anchor, "EpicValueThingOY", JTokenType.String);
                        Controls.Panel(ref Container, Names.TokenValue, "0.6 1 0 1", "PaleVioletRed", Names.TokenValueInput, Margin(m));
                        Controls.Input(ref Container, Names.TokenValueInput, "????");
                    }
                    internal static void Bool(ref CuiElementContainer Container, string anchor)
                    {
                        Base(ref Container, anchor, "EpicValueThingOY", JTokenType.Boolean);
                        Controls.Button(ref Container, Names.TokenValue, "0.6 1 0 1", "Green", "????", "On", 16, Margin(m));
                    }
                }

                private static class Controls
                {
                    internal enum RoundType
                    {
                        None,
                        All,
                        Top
                    }
                    internal static void Container(ref CuiElementContainer Container, string name, string anchor, string color, string offset = Settings.emptyOffset, int margin = Settings.margin, RoundType rounded = RoundType.None)
                    {
                        var a = anchor.Split(' ');
                        var o = offset.Split(' ');
                        Container.Add(new CuiPanel
                        {
                            CursorEnabled = true,
                            KeyboardEnabled = true,
                            Image =
                            {
                                Color = Hex2Cui(color),
                                Sprite = (rounded == RoundType.All) ? "assets/content/ui/ui.background.rounded.png" : (rounded == RoundType.Top) ? "assets/content/ui/ui.background.rounded.top.png" : null,
                                ImageType = UnityEngine.UI.Image.Type.Tiled,
                            },
                            RectTransform =
                            {
                                //AnchorMin = $"{a.Index(0)} {a.Index(2)}",
                                //AnchorMax = $"{a.Index(1)} {a.Index(3)}",
                                //OffsetMin = $"{o.Index(0)} {o.Index(2)}",
                                //OffsetMax = $"{o.Index(1)} {o.Index(3)}"
                            },
                        }, "Overall", name);
                    }
                    internal static void Panel(ref CuiElementContainer Container, string parent, string anchor, string color, string name = null, string offset = Settings.emptyOffset, RoundType rounded = RoundType.None)
                    {
                        var a = anchor.Split(' ');
                        var o = offset.Split(' ');
                        Container.Add(new CuiPanel
                        {
                            Image =
                            {
                                Color = Hex2Cui(color),
                                Sprite = (rounded == RoundType.All) ? "assets/content/ui/ui.background.rounded.png" : (rounded == RoundType.Top) ? "assets/content/ui/ui.background.rounded.top.png" : null,
                                ImageType = UnityEngine.UI.Image.Type.Tiled,
                            },
                            RectTransform =
                            {
                                //AnchorMin = $"{a.Index(0)} {a.Index(2)}",
                                //AnchorMax = $"{a.Index(1)} {a.Index(3)}",
                                //OffsetMin = $"{o.Index(0)} {o.Index(2)}",
                                //OffsetMax = $"{o.Index(1)} {o.Index(3)}"
                            },
                        }, parent, name);
                    }
                    internal static void Label(ref CuiElementContainer Container, string parent, string anchor, string text, int fontSize = 14, string offset = Settings.emptyOffset, TextAnchor align = TextAnchor.MiddleCenter)
                    {
                        var a = anchor.Split(' ');
                        var o = offset.Split(' ');
                        Container.Add(new CuiLabel
                        {
                            Text = { Text = text, Align = align, FontSize = fontSize },
                            RectTransform =
                            {
                                //AnchorMin = $"{a.Index(0)} {a.Index(2)}",
                                //AnchorMax = $"{a.Index(1)} {a.Index(3)}",
                                //OffsetMin = $"{o.Index(0)} {o.Index(2)}",
                                //OffsetMax = $"{o.Index(1)} {o.Index(3)}"
                            },
                        }, parent);
                    }
                    internal static void Button(ref CuiElementContainer Container, string parent, string anchor, string color, string command, string text, int fontSize = 14, string offset = Settings.emptyOffset, TextAnchor align = TextAnchor.MiddleCenter, RoundType rounded = RoundType.None)
                    {
                        var a = anchor.Split(' ');
                        var o = offset.Split(' ');
                        Container.Add(new CuiButton
                        {
                            Text = { Align = align, Text = text, FontSize = fontSize },
                            Button =
                            {
                                Color = Hex2Cui(color),
                                Command = $"{UI.Command} {command}",
                                Sprite = (rounded == RoundType.All) ? "assets/content/ui/ui.background.rounded.png" : (rounded == RoundType.Top) ? "assets/content/ui/ui.background.rounded.top.png" : null,
                                ImageType = UnityEngine.UI.Image.Type.Tiled,
                            },
                            RectTransform =
                            {
                                //AnchorMin = $"{a.Index(0)} {a.Index(2)}",
                                //AnchorMax = $"{a.Index(1)} {a.Index(3)}",
                                //OffsetMin = $"{o.Index(0)} {o.Index(2)}",
                                //OffsetMax = $"{o.Index(1)} {o.Index(3)}"
                            },
                        }, parent);
                    }
                    internal static void Input(ref CuiElementContainer Container, string parent, string command, string text = "", int fontSize = 14)
                    {
                        Container.Add(new CuiElement
                        {
                            Parent = parent,
                            Components =
                            {
                                new CuiInputFieldComponent
                                {
                                    Command = $"{UI.Command} {command}",
                                    Align = TextAnchor.MiddleLeft,
                                    FontSize = fontSize,
                                    Text = text,
                                    NeedsKeyboard = true,
                                },
                                new CuiRectTransformComponent
                                {
                                    AnchorMin = "0 0", AnchorMax = "1 1",
                                    OffsetMin = $"{m} 0", OffsetMax = "0 0"
                                },
                            }
                        });
                    }
                }
                internal static class Names
                {
                    internal static string Base => $"{THIS.Name}.Base";
                    internal static string Main => $"{THIS.Name}.Main";
                    internal static string TitleBar => $"{THIS.Name}.TitleBar";
                    internal static string SideNav => $"{THIS.Name}.SideNav";
                    internal static string TopNav => $"{THIS.Name}.TopNav";
                    internal static string BottomNav => $"{THIS.Name}.BottomNav";
                    internal static string DisplayPanel => $"{THIS.Name}.DisplayPanel";
                    internal static string Overlay => $"{THIS.Name}.Overlay";
                    internal static string PluginSearchPanel => $"{THIS.Name}.PluginSearchPanel";
                    internal static string PluginPanel => $"{THIS.Name}.PluginPanel";
                    internal static string TokenPathPanel => $"{THIS.Name}.TokenPathPanel";
                    internal static string PaginationPanel => $"{THIS.Name}.PaginationPanel";
                    internal static string TokenValue => $"{THIS.Name}.TokenValue";
                    internal static string TokenValueInput => $"{THIS.Name}.TokenValueInput";
                }
            }

            private enum ccAction
            {
                close,
                setPlugin,
                setPluginPage,
                setPluginSearch,
                setFileType,
            }
            private static ccAction ToAction(string value)
            {
                ccAction action = default(ccAction);
                Enum.TryParse(value, out action);
                return action;
            }
            internal static bool ccHandle(ConsoleSystem.Arg console)
            {
                /*
                var selector = ToAction(console.Args.Index(0));
                var value = console.Args.Index(1);
                var player = console.Player();
                if(player == null) { return false; }
                switch (selector)
                {
                    case ccAction.close:{
                        Data.Set(player.userID);
                        UI.Destroy(player);
                    } return false;
                    case ccAction.setPlugin:{
                        Data.Get(player.userID).PluginSelected = value;
                    } break;
                    case ccAction.setPluginPage:{
                        int page = 0;
                        int.TryParse(value, out page);
                        Data.Get(player.userID).PluginPage = page;
                    } break;
                    case ccAction.setFileType:{
                        DataHelper.DataType type;
                        Enum.TryParse(value, out type);
                        Data.Get(player.userID).FileType = type;
                    } break;
                    case ccAction.setPluginSearch:{
                        Data.Get(player.userID).PluginSearch = value;
                    } break;
                }
                UI.Draw(player, true);
                */
                return false;
            }
            internal static class Data
            {
                private static Dictionary<ulong, Model> UiData = new Dictionary<ulong, Model>();
                internal sealed class Model
                {
                    internal string PluginSelected;
                    internal string PluginSearch;
                    internal int PluginPage;
                    //internal DataHelper.DataType FileType;
                    internal string[] PathTokens;
                    internal int PathPage;
                    internal int NavPage;
                }
                private static void Create(ulong id)
                {
                    if (!UiData.ContainsKey(id))
                    {
                        UiData.Add(id, new Model());
                    }
                }
                internal static void Set(ulong id, Model model = null)
                {
                    Create(id);
                    UiData[id] = model ?? new Model();
                }
                internal static Model Get(ulong id)
                {
                    Create(id);
                    return UiData[id];
                }
            }
            internal static void Draw(BasePlayer player, bool instant = false)
            {
                UI.Destroy(player);
                var Root = Elements.Base;
                Elements.Overlay(ref Root);
                Elements.Plugins(ref Root, player.userID);
                Elements.TopNav(ref Root, player.userID);
                Elements.BottomNav(ref Root);
                Elements.Display(ref Root);
                CuiHelper.AddUi(player, Root);
            }
            internal static void Destroy(BasePlayer player)
            {
                if (player == null || player.net == null) { return; }
                foreach (var element in typeof(Elements.Names).GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    try { CuiHelper.DestroyUi(player, element.GetValue(element).ToString()); }
                    catch { continue; }
                }
            }
            private static string Hex2Cui(string HexOrKnown, float alpha = 1)
            {
                var color = System.Drawing.Color.FromArgb(1, 0, 0, 0);
                var sysColor = System.Drawing.Color.FromName(HexOrKnown);
                if (sysColor.IsKnownColor) { color = sysColor; }
                else
                {
                    try
                    {
                        string Hex = HexOrKnown.StartsWith("#") ? HexOrKnown : $"#{HexOrKnown}";
                        color = ColorTranslator.FromHtml(Hex);
                    }
                    catch { }
                }
                return $"{(color.R / (float)byte.MaxValue)} {(color.G / (float)byte.MaxValue)} {(color.B / (float)byte.MaxValue)} {Mathf.Clamp(alpha, 0, 1)}";
            }
        }
    }
}