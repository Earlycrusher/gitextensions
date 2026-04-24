using GitExtensions.Extensibility.Settings;
using GitUIPluginInterfaces.BuildServerIntegration;

namespace GitCommands.Settings;

public static class SettingsSourceExtension
{
    public static IBuildServerSettings GetBuildServerSettings(this SettingsSource settingsSource)
        => new BuildServerSettings(settingsSource);

    public static IDetachedSettings Detached(this SettingsSource settingsSource)
        => new DetachedSettings(settingsSource);
}
