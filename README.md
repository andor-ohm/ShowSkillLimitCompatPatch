# Show Skill Limit Compat Patch

Compatibility patch for `Show Skill Limit` when used with `Character Reload` in Mount & Blade II: Bannerlord.

## What This Fixes

When `Show Skill Limit` is enabled alongside `Character Reload`, opening Character Reload's custom character editor can spam this message continuously:

`ShowSkillLimit: error`

The issue is caused by `Show Skill Limit` patching the vanilla character developer UI and then running that logic against Character Reload's custom screen.

This patch prevents `Show Skill Limit` from running its `PerkSelectionBarWidget.OnLateUpdate` compatibility-breaking logic unless the active screen is the vanilla Bannerlord character developer screen.

## How It Works

This mod applies a Harmony prefix to:

`ShowSkillLimit.PerkSelectionBarWidgetPatch.OnLateUpdatePostfix`

If the active `TopScreen` is not:

`SandBox.GauntletUI.GauntletCharacterDeveloperScreen`

the Show Skill Limit postfix is skipped for that frame. This keeps `Show Skill Limit` working on the normal vanilla screen while stopping the endless error spam inside Character Reload's custom UI.

## Requirements

- Mount & Blade II: Bannerlord
- `Bannerlord.Harmony`
- `Bannerlord.MBOptionScreen`
- `CharacterReload`
- `ShowSkillLimit`

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

The module also declares both as dependencies in `SubModule.xml`

## Debug Messages

The mod includes an MCM setting named `Enable Debug Messages`.

When enabled, the patch prints lightweight compatibility messages to Bannerlord's in-game message feed. It is off by default.

## Building

This project targets `.NET Framework 4.7.2` and references local Bannerlord assemblies from a Steam install.

Open:

`ShowSkillLimitCompatPatch.csproj`

By default the project tries to deploy directly into your Bannerlord `Modules` folder after a successful build.

If Bannerlord is not installed in one of the common Steam locations, set the `BANNERLORD_MODULES_DIR` environment variable to your Bannerlord `Modules` folder or build with:

`/p:BannerlordModulesDir=YourPathHere\Modules`

If you want to build without deploying, use:

`/p:SkipBannerlordDeploy=true`

## Credits

- TaleWorlds for Bannerlord
- The authors of `Character Reload`
- The authors of `Show Skill Limit`

## License

See [LICENSE](LICENSE).
