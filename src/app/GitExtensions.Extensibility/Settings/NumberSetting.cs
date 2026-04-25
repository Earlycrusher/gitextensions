namespace GitExtensions.Extensibility.Settings;

public static class NumberSetting
{
    /// <summary>
    ///  The text to be shown as a hint in the input control when the value is null.
    /// </summary>
    public static string PlaceholderText { get; set; } = "";
}

public class NumberSetting<T> : ISetting
{
    public NumberSetting(string name, T defaultValue)
        : this(name, name, defaultValue)
    {
    }

    public NumberSetting(string name, string caption, T defaultValue)
    {
        Name = name;
        Caption = caption;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public string Caption { get; }
    public T DefaultValue { get; }
    public Control? CustomControl { get; set; }

    public ISettingControlBinding CreateControlBinding(Control customControl)
    {
        CustomControl = customControl;
        return CreateControlBinding();
    }

    public ISettingControlBinding CreateControlBinding()
    {
        if (typeof(T) == typeof(int) && CustomControl is not TextBox)
        {
            // It'd be great to write new NumericUpDownBinding((NumberSetting<int>)this, ...) but the compiler doesn't like that,
            // so we are forced to use the !-operator.
            return new NumericUpDownBinding((this as NumberSetting<int>)!, CustomControl as NumericUpDown);
        }
        else
        {
            return new TextBoxBinding(this, CustomControl as TextBox);
        }
    }

    // TODO: honestly, NumericUpDownBinding might be a better choice than TextBox in general since its internal type is `decimal`.
    //       We would just need to appropriately choose an increment based on NumberSetting's type.
    private class NumericUpDownBinding : SettingControlBinding<NumberSetting<int>, NumericUpDown>
    {
        private readonly ToolTip _toolTip = new();

        public NumericUpDownBinding(NumberSetting<int> setting, NumericUpDown? customControl)
            : base(setting, customControl)
        {
        }

        public override NumericUpDown CreateControl()
        {
            NumericUpDown numericUpDown = new()
            {
                // TODO: if we need negative values, int.MinValue should be the Minimum.
                //       Or, we can attempt to introduce a NumberSetting<int> constructor that accepts a min and max value parameter.
                Minimum = 0,
                Maximum = int.MaxValue
            };

            Setting.CustomControl = numericUpDown;
            return (NumericUpDown)Setting.CustomControl;
        }

        public override void LoadSetting(SettingsSource settings, NumericUpDown control)
        {
            object? value = Setting[settings];
            if (value is null)
            {
                if (settings.SettingLevel != SettingLevel.Effective)
                {
                    control.ResetText();
                    _toolTip.SetToolTip(control, NumberSetting.PlaceholderText);
                    return;
                }

                value = Setting.DefaultValue;
            }

            control.Value = (int)value;
            control.Text = control.Value.ToString(); // needed if Text was cleared
            _toolTip.SetToolTip(control, "");
        }

        public override void SaveSetting(SettingsSource settings, NumericUpDown control)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                Setting[settings] = null;
                return;
            }

            int controlValue = (int)control.Value;

            if (settings.SettingLevel == SettingLevel.Effective && Setting.ValueOrDefault(settings) == controlValue)
            {
                return;
            }

            Setting[settings] = controlValue;
        }
    }

    private class TextBoxBinding : SettingControlBinding<NumberSetting<T>, TextBox>
    {
        public TextBoxBinding(NumberSetting<T> setting, TextBox? customControl)
            : base(setting, customControl)
        {
            if (customControl is not null)
            {
                customControl.TextChanged += OnTextChanged;
            }
        }

        private static string ConvertToString(object? value)
        {
            return value?.ToString() ?? "";
        }

        public override TextBox CreateControl()
        {
            TextBox textBox = new();
            textBox.TextChanged += OnTextChanged;
            Setting.CustomControl = textBox;
            return textBox;
        }

        public override void LoadSetting(SettingsSource settings, TextBox control)
        {
            if (control.PlaceholderText.Length == 0 && NumberSetting.PlaceholderText.Length > 0)
            {
                control.PlaceholderText = NumberSetting.PlaceholderText;
            }

            object? settingVal = settings.SettingLevel == SettingLevel.Effective
                ? Setting.ValueOrDefault(settings)
                : Setting[settings];

            control.Text = ConvertToString(settingVal);
        }

        public override void SaveSetting(SettingsSource settings, TextBox control)
        {
            string controlValue = control.Text;

            if (string.IsNullOrEmpty(controlValue) || !TryConvertFromString(controlValue, out object? parsedValue))
            {
                Setting[settings] = null;
                return;
            }

            if (settings.SettingLevel == SettingLevel.Effective)
            {
                if (ConvertToString(Setting.ValueOrDefault(settings)) == controlValue)
                {
                    return;
                }
            }

            Setting[settings] = parsedValue;
        }

        private static void OnTextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                bool isValid = string.IsNullOrEmpty(textBox.Text) || TryConvertFromString(textBox.Text, out _);
                textBox.BackColor = isValid ? TextBoxValidationColors.ValidBackColor : TextBoxValidationColors.InvalidBackColor;
                textBox.ForeColor = isValid ? TextBoxValidationColors.ValidForeColor : TextBoxValidationColors.InvalidForeColor;
            }
        }
    }

    private static bool TryConvertFromString(string value, out object? result)
    {
        Type type = typeof(T);
        if (type == typeof(int) && int.TryParse(value, out int intResult))
        {
            result = intResult;
            return true;
        }

        if (type == typeof(float) && float.TryParse(value, out float floatResult))
        {
            result = floatResult;
            return true;
        }

        if (type == typeof(double) && double.TryParse(value, out double doubleResult))
        {
            result = doubleResult;
            return true;
        }

        if (type == typeof(long) && long.TryParse(value, out long longResult))
        {
            result = longResult;
            return true;
        }

        result = null;
        return false;
    }

    public object? this[SettingsSource settings]
    {
        get
        {
            string? stringValue = settings.GetValue(Name);

            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            _ = TryConvertFromString(stringValue, out object? result);
            return result;
        }

        set => settings.SetValue(Name, value?.ToString());
    }

    public T ValueOrDefault(SettingsSource settings)
    {
        object? settingVal = this[settings];
        if (settingVal is null)
        {
            return DefaultValue;
        }
        else
        {
            return (T)settingVal;
        }
    }
}
