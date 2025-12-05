<!--
Thank you for supporting my work by purchasing one of my products! 
-->

# Installation
- Place the **AdminToggle.cs** file in **/oxide/plugins/**
- Place the **XLIB.dll** Dependency in **/RustDedicated_Data/Managed/**
- Grant yourself the default permission **o.grant user YOURNAME admintoggle.master** the .master permission selector needs to be adapted to what you called the permission setting for the mode

# Links
- Plugin Developer:    **https://steamcommunity.com/id/InfinityNet/**
- Download Link:       **https://codefling.com/plugins/admin-toggle**

# Information
- **AdminToggle** Allows admins with permission to toggle between player & admin mode
- **Reset Command** Open f1 console & write at.fix - Reverts you to player mode (the hard way)

# Featues / Functionality
## Core Mode Featues
- Unlimited custom modes
- Customizable permission name
- Priority system for modes
- Master mode (Can override many restrictions)
- Custom commands to toggle
- Restriction system to specfic steam ids a certan mode
### Mode Settings
#### On Admin
- Require a reason to toggle
- Autorun commands on toggle
- Toggle multiple oxide groups
- Auth level can be specified 1 or 2
- Separated inventories
- Teleport back to toggle location upon exiting
- Revert auth to 0 on disconnect
- Blocking of server violations
- Blocked commands
- Custom outfit while in mode
- Notifications (global-chat notification, local-chat notification, popup notification, sound perfab notification & Discord embed Notification)
- Interface toggle button, pulsing panel, action menu
- Blocked actions
- Autorun third-party plugins
- Blocked third-party plugins
#### On Player
- Autorun commands on revert
- Notifications (global-chat notification, local-chat notification, popup notification, sound perfab notification & Discord embed Notification)
- Blocked commands
- Blocked third-party plugins

## Functionality
### API
#### Hooks
```csharp
void admintoggle_onAdmin (BasePlayer player) { /*Do something epic*/ }
void admintoggle_onPlayer (BasePlayer player) { /*Do something epic*/ }
```
#### Methods
```csharp
bool IsAdmin(BasePlayer player);
object[] GetMode(BasePlayer player, bool TrueMode = false);

/*
IF player.userID IS ASSIGNED ANY MODE RETURNS TRUE
IF player.userID IS NOT ASSIGNED ANY MODE RETURNS FALSE
*/
bool isAdmin = AdminToggle.Call<bool>("IsAdmin", player.userID);


/*
IF player IS NOT ASSIGNED MODE RETURNS NULL
IF bool IS SET FALSE RETURNS CURRENT MODE -- object[0] permission (string), object[1] priority (int), object[2] isMaster (bool)
IF bool IS SET TRUE RETURNS HIGHEST MODE -- object[0] permission (string), object[1] priority (int), object[2] isMaster (bool)
*/
object[] getMode = AdminToggle.Call<object[]>("GetMode", player, false);
```