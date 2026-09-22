# Show Skill Limit Compat Patch

Compatibility patch for `Show Skill Limit` when used with `Character Reload` in Mount & Blade II: Bannerlord.

## What This Fixes

When `Show Skill Limit` is enabled alongside `Character Reload`, opening Character Reload's custom character editor can spam this message continuously:

`ShowSkillLimit: error`

The issue is caused by `Show Skill Limit` patching the vanilla character developer UI and then running that logic against Character Reload's custom screen.

This patch prevents `Show Skill Limit` from running its `PerkSelectionBarWidget.OnLateUpdate` compatibility-breaking logic unless the active screen is the vanilla Bannerlord character developer screen.

## How It Works

Version 1.0.2 applies a Harmony prefix to:

`ShowSkillLimit.SkillLimitPatch.OnLateUpdatePostfix`

If the active `TopScreen` is not:

`SandBox.GauntletUI.GauntletCharacterDeveloperScreen`

the Show Skill Limit postfix is skipped for that frame. This keeps `Show Skill Limit` working on the normal vanilla screen while stopping the endless error spam inside Character Reload's custom UI.

## Requirements

- Mount & Blade II: Bannerlord
- `Bannerlord.Harmony`
- `Bannerlord.MBOptionScreen`
- `CharacterReload`
- `ShowSkillLimit`

Version 1.0.2 was tested with Bannerlord 1.4.8, Show Skill Limit 1.0.4, and Character Reload e1.4.5.0. It supports the current Show Skill Limit generation only; use compatibility-patch version 1.0.1 with older Show Skill Limit releases.

## Installation

1. Build the project or use the compiled DLL.
2. Copy the module into your Bannerlord `Modules` folder so the structure looks like this:

```text
Mount & Blade II Bannerlord
\- Modules
   \- ShowSkillLimitCompatPatch
      |- SubModule.xml
      \- bin
         \- Win64_Shipping_Client
            \- ShowSkillLimitCompatPatch.dll
```

3. Enable the module in the Bannerlord launcher.

## Recommended Load Order

Load this patch after both `ShowSkillLimit` and `CharacterReload`.

The module also declares both as dependencies in `SubModule.xml`.

## Debug Messages

The mod includes an MCM setting named `Enable Debug Messages`.

When enabled, the patch prints lightweight compatibility messages to Bannerlord's in-game message feed. It is off by default.

## Development

This repository contains the Visual Studio project used to build the mod:

- `ShowSkillLimitCompatPatch.sln`
- `ShowSkillLimitCompatPatch.csproj`

The project targets `.NET Framework 4.7.2` and references local Bannerlord, Harmony, and MCM assemblies. Set `BannerlordGameRoot` or the `BANNERLORD_GAME_ROOT` environment variable to your Bannerlord installation before building.

Building does not deploy files into the game installation.

## Changelog

See [CHANGELOG.md](./CHANGELOG.md) for release history.

## Credits

- The authors of `Character Reload` https://www.nexusmods.com/mountandblade2bannerlord/mods/3700
- The authors of `Show Skill Limit` https://www.nexusmods.com/mountandblade2bannerlord/mods/9209

## License

See [LICENSE](LICENSE).
