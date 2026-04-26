namespace GitExtensions.Extensibility.Settings;

public class StringSetting : ISetting
{
    public StringSetting(string name, string defaultValue)
        : this(name, name, defaultValue)
    {
    }

    public StringSetting(string name, string caption, string defaultValue)
    {
        Name = name;
        Caption = caption;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public string Caption { get; }
    public string DefaultValue { get; }
    public TextBox? CustomControl { get; set; }

    public ISettingControlBinding CreateControlBinding()
    {
        return new TextBoxBinding(this, CustomControl);
    }

    private class TextBoxBinding : SettingControlBinding<StringSetting, TextBox>
    {
        public TextBoxBinding(StringSetting setting, TextBox? customControl)
            : base(setting, customControl)
        {
        }

        public override TextBox CreateControl()
        {
            Setting.CustomControl = new TextBox();
            return Setting.CustomControl;
        }

        public override void LoadSetting(SettingsSource settings, TextBox control)
        {
            string? settingVal = settings.SettingLevel == SettingLevel.Effective
                ? Setting.ValueOrDefault(settings)
                : Setting[settings];

            // for multiline control, transform "\n" in "\r\n" but prevent "\r\n" to be transformed in "\r\r\n"
            control.Text = control.Multiline
                ? settingVal?.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine)
                : settingVal;
        }

        public override void SaveSetting(SettingsSource settings, TextBox control)
        {
            string controlValue = control.Text;
            if (settings.SettingLevel == SettingLevel.Effective)
            {
                if (Setting.ValueOrDefault(settings) == controlValue)
                {
                    return;
                }
            }

            Setting[settings] = controlValue;
        }
    }

    public string? this[SettingsSource settings]
    {
        get => settings.GetString(Name, null);
        set => settings.SetString(Name, value);
    }

    public string ValueOrDefault(SettingsSource settings)
    {
        return this[settings] ?? DefaultValue;
    }
}
