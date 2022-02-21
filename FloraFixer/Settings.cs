using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace FloraFixer
{
    public class Settings
    {
        [SettingName("Name of Flora Respawn fix script ('florafix' by default)")]
        public string ScriptName = "florafix";
    }
}
