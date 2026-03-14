using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace ShowSkillLimitCompatPatch
{
    public class ShowSkillLimitCompatPatchSettings : AttributeGlobalSettings<ShowSkillLimitCompatPatchSettings>
    {
        public override string Id => "ShowSkillLimitCompatPatch_v1";
        public override string DisplayName => "Show Skill Limit Compat Patch";
        public override string FolderName => "ShowSkillLimitCompatPatch";
        public override string FormatType => "json";

        [SettingPropertyBool("Enable Debug Messages", HintText = "Shows lightweight compatibility messages in the in-game message feed. Default is off.", RequireRestart = false, Order = 0)]
        [SettingPropertyGroup("General")]
        public bool EnableDebugMessages { get; set; } = false;
    }
}
