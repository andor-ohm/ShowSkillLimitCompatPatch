using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace ShowSkillLimitCompatPatch
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "ShowSkillLimitCompatPatch";
        private const string ExpectedScreenTypeName = "SandBox.GauntletUI.GauntletCharacterDeveloperScreen";

        private static readonly HashSet<string> LoggedContexts = new HashSet<string>(StringComparer.Ordinal);
        private static Harmony _harmony;
        private static bool _showSkillLimitPatched;
        private static string _logPath;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                EnsureLogReady();
                Log("ShowSkillLimitCompatPatch loading.");

                _harmony = new Harmony(HarmonyId);
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

                TryPatchShowSkillLimit();
            }
            catch (Exception ex)
            {
                Log("OnSubModuleLoad failed: " + ex);
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

                Log("ShowSkillLimitCompatPatch unloaded.");
            }
            catch (Exception ex)
            {
                Log("OnSubModuleUnloaded failed: " + ex);
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
                    Log("ShowSkillLimit assembly loaded after CharacterReloadPatch. Retrying patch.");
                    TryPatchShowSkillLimit();
                }
            }
            catch (Exception ex)
            {
                Log("AssemblyLoad handler failed: " + ex);
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
                    Log("ShowSkillLimit.PerkSelectionBarWidgetPatch not found yet.");
                    return;
                }

                MethodInfo targetMethod = AccessTools.Method(targetType, "OnLateUpdatePostfix");
                if (targetMethod == null)
                {
                    Log("ShowSkillLimit.PerkSelectionBarWidgetPatch.OnLateUpdatePostfix not found.");
                    return;
                }

                MethodInfo prefixMethod = AccessTools.Method(typeof(SubModule), nameof(ShowSkillLimitCompatibilityPrefix));
                _harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
                _showSkillLimitPatched = true;

                Log("Patched ShowSkillLimit.PerkSelectionBarWidgetPatch.OnLateUpdatePostfix.");
            }
            catch (Exception ex)
            {
                Log("Failed to patch ShowSkillLimit compatibility guard: " + ex);
            }
        }

        private static bool ShowSkillLimitCompatibilityPrefix()
        {
            try
            {
                ScreenBase topScreen = ScreenManager.TopScreen;
                if (IsVanillaCharacterDeveloperScreen(topScreen))
                {
                    return true;
                }

                LogSuppressedContext(topScreen);
                return false;
            }
            catch (Exception ex)
            {
                Log("Compatibility prefix failed unexpectedly, allowing original method: " + ex);
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

        private static void LogSuppressedContext(ScreenBase topScreen)
        {
            string screenTypeName = topScreen != null ? topScreen.GetType().FullName : "<null>";
            string contextKey = "suppressed:" + screenTypeName;

            if (!LoggedContexts.Add(contextKey))
            {
                return;
            }

            Log(
                "Suppressed ShowSkillLimit.OnLateUpdatePostfix because TopScreen was '" +
                screenTypeName +
                "' instead of '" +
                ExpectedScreenTypeName +
                "'. This is the compatibility fix for Character Reload style custom screens."
            );
        }

        private static void EnsureLogReady()
        {
            if (!string.IsNullOrEmpty(_logPath))
            {
                return;
            }

            string dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string moduleDirectory = Directory.GetParent(Directory.GetParent(dllDirectory).FullName).FullName;
            string logDirectory = Path.Combine(moduleDirectory, "Logs");

            Directory.CreateDirectory(logDirectory);

            _logPath = Path.Combine(logDirectory, "ShowSkillLimitCompatPatch.log");
            File.AppendAllText(
                _logPath,
                Environment.NewLine +
                "========== ShowSkillLimitCompatPatch startup " +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " ==========" +
                Environment.NewLine
            );
        }

        internal static void Log(string message)
        {
            try
            {
                EnsureLogReady();
                File.AppendAllText(_logPath, "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
