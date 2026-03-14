using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace ShowSkillLimitCompatPatch
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "ShowSkillLimitCompatPatch";
        private const string ExpectedScreenTypeName = "SandBox.GauntletUI.GauntletCharacterDeveloperScreen";
        private const string DebugMessagePrefix = "[ShowSkillLimitCompatPatch] ";

        private static Harmony _harmony;
        private static bool _showSkillLimitPatched;
        private static int _lastUnsupportedScreenIdentity;

        private static bool EnableDebugMessages
        {
            get
            {
                try
                {
                    return ShowSkillLimitCompatPatchSettings.Instance != null &&
                           ShowSkillLimitCompatPatchSettings.Instance.EnableDebugMessages;
                }
                catch
                {
                    return false;
                }
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                _harmony = new Harmony(HarmonyId);
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                DebugMessage("Loading.");
                TryPatchShowSkillLimit();
            }
            catch (Exception ex)
            {
                DebugMessage("OnSubModuleLoad failed: " + ex.Message);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;

                if (_harmony != null)
                {
                    _harmony.UnpatchAll(HarmonyId);
                }

                DebugMessage("Unloaded.");
            }
            catch (Exception ex)
            {
                DebugMessage("OnSubModuleUnloaded failed: " + ex.Message);
            }

            base.OnSubModuleUnloaded();
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                if (_showSkillLimitPatched || args.LoadedAssembly == null)
                {
                    return;
                }

                if (string.Equals(args.LoadedAssembly.GetName().Name, "ShowSkillLimit", StringComparison.Ordinal))
                {
                    DebugMessage("Show Skill Limit loaded after this patch. Retrying compatibility hook.");
                    TryPatchShowSkillLimit();
                }
            }
            catch (Exception ex)
            {
                DebugMessage("AssemblyLoad handler failed: " + ex.Message);
            }
        }

        private static void TryPatchShowSkillLimit()
        {
            if (_showSkillLimitPatched || _harmony == null)
            {
                return;
            }

            try
            {
                Type targetType = AccessTools.TypeByName("ShowSkillLimit.PerkSelectionBarWidgetPatch");
                if (targetType == null)
                {
                    DebugMessage("ShowSkillLimit.PerkSelectionBarWidgetPatch not found yet.");
                    return;
                }

                MethodInfo targetMethod = AccessTools.Method(targetType, "OnLateUpdatePostfix");
                if (targetMethod == null)
                {
                    DebugMessage("ShowSkillLimit.PerkSelectionBarWidgetPatch.OnLateUpdatePostfix not found.");
                    return;
                }

                MethodInfo prefixMethod = AccessTools.Method(typeof(SubModule), nameof(ShowSkillLimitCompatibilityPrefix));
                _harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
                _showSkillLimitPatched = true;

                DebugMessage("Patched ShowSkillLimit.PerkSelectionBarWidgetPatch.OnLateUpdatePostfix.");
            }
            catch (Exception ex)
            {
                DebugMessage("Failed to patch compatibility guard: " + ex.Message);
            }
        }

        private static bool ShowSkillLimitCompatibilityPrefix()
        {
            try
            {
                ScreenBase topScreen = ScreenManager.TopScreen;
                if (IsVanillaCharacterDeveloperScreen(topScreen))
                {
                    _lastUnsupportedScreenIdentity = 0;
                    return true;
                }

                if (topScreen == null)
                {
                    _lastUnsupportedScreenIdentity = 0;
                    return false;
                }

                ReportSuppressedContext(topScreen);
                return false;
            }
            catch (Exception ex)
            {
                DebugMessage("Compatibility prefix failed unexpectedly; allowing original method: " + ex.Message);
                return true;
            }
        }

        private static bool IsVanillaCharacterDeveloperScreen(ScreenBase topScreen)
        {
            if (topScreen == null)
            {
                return false;
            }

            Type expectedType = AccessTools.TypeByName(ExpectedScreenTypeName);
            if (expectedType != null)
            {
                return expectedType.IsInstanceOfType(topScreen);
            }

            return string.Equals(topScreen.GetType().FullName, ExpectedScreenTypeName, StringComparison.Ordinal);
        }

        private static void ReportSuppressedContext(ScreenBase topScreen)
        {
            if (topScreen == null)
            {
                return;
            }

            int currentScreenIdentity = RuntimeHelpers.GetHashCode(topScreen);
            if (_lastUnsupportedScreenIdentity == currentScreenIdentity)
            {
                return;
            }

            _lastUnsupportedScreenIdentity = currentScreenIdentity;
            DebugMessage("Triggered");
        }

        internal static void DebugMessage(string message)
        {
            if (!EnableDebugMessages)
            {
                return;
            }

            try
            {
                InformationManager.DisplayMessage(new InformationMessage(DebugMessagePrefix + message));
            }
            catch
            {
            }
        }
    }
}
