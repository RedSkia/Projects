using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Game.Rust.Cui;
using Oxide.Core.Plugins;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Oxide.Core.Libraries;
using Oxide.Plugins;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace Oxide.Plugins
{
    [Info("UIExample", "k1lly0u", "0.1.0", ResourceId = 0)]
    class UIExample : RustPlugin
    {
        private void CreateUI(BasePlayer player)
        {
            // First start by creating the container, call UI.Container to generate a new container using your variables
            // Arg1 - This is the name of the UI element, you use this name to destroy the element later
            // Arg2 - The color of the container, use the UI.Color function to convert a hex color and alpha to RGBA. For a invisible panel set alpha to 0
            // Arg3 - a UI4 struct containing the dimensions of the container, new UI4(left, bottom, right, top)
            // Arg4 - Use a cursor (true/false)
            // Arg5 - The layer to parent the UI to (Hud, Overlay, etc)
            CuiElementContainer container = UI.Container("panelname", UI.Color("000000", 0.1f), new UI4(0.4f, 0.8f, 0.6f, 0.9f));

            // Use panels to create other boxes inside the main container
            // Arg1 - Reference the container
            // Arg2 - The name of your UI element
            // Arg3 - The color of the panel
            // Arg4 - UI4 struct containing the dimensions of this individual element. This is relative to the containers size (0.8 is 80% of the size of the container)
            UI.Panel(ref container, "panelname", UI.Color("ffffff", 0.1f), new UI4(0.8f, 0.8f, 1f, 1f));

            // Insert text into the container
            // Arg1 - Reference the container
            // Arg2 - The name of your UI element
            // Arg3 - The text you want to display
            // Arg4 - The size of the font
            // Arg5 - UI4 struct containing the dimensions of this individual element. This is relative to the containers size (0.8 is 80% of the size of the container)
            // Arg6 - Where to anchor the text
            UI.Label(ref container, "panelname", "My text goes here", 15, new UI4(0, 0.8f, 0.8f, 1f), TextAnchor.MiddleLeft);

            // Insert a button into the container
            // Arg1 - Reference the container
            // Arg2 - The name of your UI element
            // Arg3 - The color of the button
            // Arg4 - The text on the button
            // Arg5 - The size of the button text
            // Arg6 - UI4 struct containing the dimensions of this individual element. This is relative to the containers size (0.8 is 80% of the size of the container)
            // Arg7 - The console command to run when the button is pressed
            UI.Button(ref container, "panelname", UI.Color("ffffff", 0.1f), "Button text", 15, new UI4(0, 0.8f, 0.8f, 1f), "consolecommand");

            // Insert a image into the container
            // Arg1 - Reference the container
            // Arg2 - The name of your UI element
            // Arg3 - The ID of the target image from FileStorage
            // Arg4 - UI4 struct containing the dimensions of this individual element. This is relative to the containers size (0.8 is 80% of the size of the container)
            UI.Image(ref container, "panelname", "imageId from file storage", new UI4(0, 0.8f, 0.8f, 1f));


            // Add the UI to the player
            CuiHelper.AddUi(player, container);


            // Destroy the UI for the player
            CuiHelper.DestroyUi(player, "panelname");
        }

        public static class UI
        {
            // The base container to hold all the individual elements
            static public CuiElementContainer Container(string panel, string color, UI4 dimensions, bool useCursor = false, string parent = "Overlay")
            {
                CuiElementContainer container = new CuiElementContainer()
                {
                    {
                        new CuiPanel
                        {
                            Image = {Color = color},
                            RectTransform = {AnchorMin = dimensions.GetMin(), AnchorMax = dimensions.GetMax()},
                            CursorEnabled = useCursor
                        },
                        new CuiElement().Parent = parent,
                        panel
                    }
                };
                return container;
            }

            // Creates a panel
            static public void Panel(ref CuiElementContainer container, string panel, string color, UI4 dimensions, bool cursor = false)
            {
                container.Add(new CuiPanel
                {
                    Image = { Color = color },
                    RectTransform = { AnchorMin = dimensions.GetMin(), AnchorMax = dimensions.GetMax() },
                    CursorEnabled = cursor
                },
                panel);
            }

            // Creates a text label
            static public void Label(ref CuiElementContainer container, string panel, string text, int size, UI4 dimensions, TextAnchor align = TextAnchor.MiddleCenter)
            {
                container.Add(new CuiLabel
                {
                    Text = { FontSize = size, Align = align, Text = text },
                    RectTransform = { AnchorMin = dimensions.GetMin(), AnchorMax = dimensions.GetMax() }
                },
                panel);

            }

            // Creates a button
            static public void Button(ref CuiElementContainer container, string panel, string color, string text, int size, UI4 dimensions, string command, TextAnchor align = TextAnchor.MiddleCenter)
            {
                container.Add(new CuiButton
                {
                    Button = { Color = color, Command = command, FadeIn = 0f },
                    RectTransform = { AnchorMin = dimensions.GetMin(), AnchorMax = dimensions.GetMax() },
                    Text = { Text = text, FontSize = size, Align = align }
                },
                panel);
            }

            // Loads a image from FileStorage
            static public void Image(ref CuiElementContainer container, string panel, string png, UI4 dimensions)
            {
                container.Add(new CuiElement
                {
                    Name = CuiHelper.GetGuid(),
                    Parent = panel,
                    Components =
                    {
                        new CuiRawImageComponent {Png = png },
                        new CuiRectTransformComponent {AnchorMin = dimensions.GetMin(), AnchorMax = dimensions.GetMax() }
                    }
                });
            }

            // Converts a hex color to RGBA to be used with CUI
            public static string Color(string hexColor, float alpha)
            {
                if (hexColor.StartsWith("#"))
                    hexColor = hexColor.Substring(1);
                int red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                int green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                int blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                return $"{(double)red / 255} {(double)green / 255} {(double)blue / 255} {alpha}";
            }
        }
        public class UI4
        {
            public float xMin, yMin, xMax, yMax;
            public UI4(float xMin, float yMin, float xMax, float yMax)
            {
                this.xMin = xMin;
                this.yMin = yMin;
                this.xMax = xMax;
                this.yMax = yMax;
            }
            public string GetMin() => $"{xMin} {yMin}";
            public string GetMax() => $"{xMax} {yMax}";
        }
    }
}
