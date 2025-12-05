using Oxide.Game.Rust.Cui;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace CoreRP
{
    internal static class UI
    {
        internal static string Hex2Cui(string HexOrKnown, float alpha = 1)
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
                } catch { }
            }
            return $"{(color.R / (float)byte.MaxValue)} {(color.G / (float)byte.MaxValue)} {(color.B / (float)byte.MaxValue)} {Mathf.Clamp(alpha, 0, 1)}";
        }

        internal static string Margin(float margin) => Offset(margin, margin, margin, margin);
        internal static string Offset(float top, float bottom, float left, float right) => $"{left} {-right} {bottom} {-top}";

        internal static void Draw(BasePlayer player, params CuiElement[] elements)
        {
            if (!player.IsPlayer(true) || elements.Length < 1) { return; }
            Destroy(player, elements.Select(element => element.Name).ToArray());
            CuiElementContainer Container = new CuiElementContainer();
            Container.AddRange(elements);
            CuiHelper.AddUi(player, Container);
        }
        internal static void Destroy(BasePlayer player, params string[] elements)
        {
            if(!player.IsPlayer(true) || elements.Length < 1) { return; }
            foreach (var element in elements)
            {
                try { CuiHelper.DestroyUi(player, element); }
                catch { continue; }
            }
        }

        internal static class Controls
        {
            private static readonly CuiNeedsCursorComponent UseCursor = new CuiNeedsCursorComponent();
            private static readonly CuiNeedsKeyboardComponent UseKeyboard = new CuiNeedsKeyboardComponent();
            private static CuiRectTransformComponent GetRect(string anchor, string offset = "0 0 0 0")
            {
                var a = anchor.Split(' '); /*minW maxW minH maxH*/
                var o = offset.Split(' '); /*minW masW minH maxH*/
                return new CuiRectTransformComponent
                {
                    AnchorMin = $"{a.ElementAt(0)} {a.ElementAt(2)}",
                    AnchorMax = $"{a.ElementAt(1)} {a.ElementAt(3)}",
                    OffsetMin = $"{o.ElementAt(0)} {o.ElementAt(2)}",
                    OffsetMax = $"{o.ElementAt(1)} {o.ElementAt(3)}"
                };
            }
            private static string GetSprite(RoundType rounded) =>
                (rounded == RoundType.All) ? "assets/content/ui/ui.background.rounded.png" :
                (rounded == RoundType.Top) ? "assets/content/ui/ui.background.rounded.top.png" : default(string);
            internal enum RoundType
            {
                None,
                All,
                Top
            }
            internal static CuiElement Container(string name, string anchor, string color, string offset = "0 0 0 0", bool controls = false, RoundType rounded = RoundType.None)
            {
                if(controls)
                {
                    return new CuiElement
                    {
                        Name = name,
                        Parent = "Overall",
                        Components =
                        {
                            UseKeyboard,
                            UseCursor,
                            GetRect(anchor, offset),
                            new CuiImageComponent
                            {
                                Color = Hex2Cui(color),
                                Sprite = GetSprite(rounded),
                                ImageType = UnityEngine.UI.Image.Type.Tiled,
                            }
                        }
                    };
                }
                return new CuiElement
                {
                    Name = name,
                    Parent = "Overall",
                    Components =
                    {
                        GetRect(anchor, offset),
                        new CuiImageComponent
                        {
                            Color = Hex2Cui(color),
                            Sprite = GetSprite(rounded),
                            ImageType = UnityEngine.UI.Image.Type.Tiled,
                        }
                    }
                };
            }
            internal static CuiElement Panel(string parent, string anchor, string color, string name = default(string), string offset = "0 0 0 0", RoundType rounded = RoundType.None)
            {
                return new CuiElement
                {
                    Name = name,
                    Parent = parent,
                    Components =
                    {
                        GetRect(anchor, offset),
                        new CuiImageComponent
                        {
                            Color = Hex2Cui(color),
                            Sprite = GetSprite(rounded),
                            ImageType = UnityEngine.UI.Image.Type.Tiled,
                        }
                    }
                };
            }
            internal static CuiElement Label(string parent, string anchor, string text, int fontSize = 14, string offset = "0 0 0 0", TextAnchor align = TextAnchor.MiddleCenter)
            {
                return new CuiElement
                {
                    Parent = parent,
                    Components =
                    {
                        GetRect(anchor, offset),
                        new CuiTextComponent
                        {
                            Text = text,
                            FontSize = fontSize,
                            Align = align,
                        },
                    }
                };
            }
            internal static CuiElement Button(string parent, string anchor, string color, string command, string text, int fontSize = 14, string offset = "0 0 0 0", TextAnchor align = TextAnchor.MiddleCenter, RoundType rounded = RoundType.None)
            {
                return new CuiElement
                {
                    Parent = parent,
                    Components =
                    {
                        GetRect(anchor, offset),
                        new CuiTextComponent
                        {
                            Text = text,
                            FontSize = fontSize,
                            Align = align,
                        },
                        new CuiButtonComponent
                        {
                            Command = command,
                            Color = Hex2Cui(color),
                            Sprite = GetSprite(rounded),
                            ImageType = UnityEngine.UI.Image.Type.Tiled,
                        }
                    }
                };
            }
            internal static CuiElement Input(string parent, string command, string text, int fontSize = 14, bool readOnly = false, TextAnchor align = TextAnchor.MiddleCenter)
            {
                return new CuiElement
                {
                    Parent = parent,
                    Components =
                    {
                        GetRect("0 1 0 1", "5 0 0 0"),
                        new CuiInputFieldComponent
                        {
                            ReadOnly = readOnly,
                            NeedsKeyboard = true,
                            Command = command,
                            Text = text,
                            FontSize = fontSize,
                            Align = align,
                        }
                    }
                };
            }
            internal static CuiElement Image(string parent, string image, string anchor, string color, string offset = "0 0 0 0")
            {
                return new CuiElement
                {
                    Parent = parent,
                    Components =
                    {
                        GetRect(anchor, offset),
                        new CuiRawImageComponent
                        {
                            Png = ImageManager.Instance.GetImage(nameof(image)),
                            Color = Hex2Cui(color),
                        },
                    }
                };
            }
        }
    }
}