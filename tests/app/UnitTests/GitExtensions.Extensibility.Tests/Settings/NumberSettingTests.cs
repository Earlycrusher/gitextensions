using GitExtensions.Extensibility.Settings;
using NSubstitute;

namespace GitUIPluginInterfacesTests.Settings;

[TestFixture]
public sealed class NumberSettingTests
{
    private const string SettingName = "TestSetting";
    private const int DefaultValue = 42;

    [Test]
    public void CreateControlBinding_TextBox_SaveSetting_should_set_null_when_TextBox_is_empty()
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();

        textBox.Text = string.Empty;
        binding.SaveSetting(settingsSource);

        settingsSource.Received(1).SetValue(SettingName, null);
    }

    [Test]
    public void CreateControlBinding_TextBox_LoadSetting_should_show_default_value_when_nothing_stored_at_effective_level()
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(SettingLevel.Effective);
        settingsSource.GetValue(SettingName).Returns((string?)null);

        binding.LoadSetting(settingsSource);

        textBox.Text.Should().Be(DefaultValue.ToString());
    }

    [Test]
    public void CreateControlBinding_TextBox_LoadSetting_should_show_empty_when_nothing_stored_at_non_effective_level(
        [Values(SettingLevel.Global, SettingLevel.Local, SettingLevel.Distributed)] SettingLevel settingLevel)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(settingLevel);
        settingsSource.GetValue(SettingName).Returns((string?)null);

        binding.LoadSetting(settingsSource);

        textBox.Text.Should().BeEmpty();
    }

    [Test]
    public void CreateControlBinding_TextBox_SaveSetting_should_not_write_when_TextBox_shows_default_value_at_effective_level()
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(SettingLevel.Effective);
        settingsSource.GetValue(SettingName).Returns((string?)null);

        textBox.Text = DefaultValue.ToString();
        binding.SaveSetting(settingsSource);

        settingsSource.DidNotReceive().SetValue(Arg.Any<string>(), Arg.Any<string?>());
    }

    [Test]
    public void CreateControlBinding_TextBox_LoadSetting_should_show_stored_value_at_effective_level(
        [Values(0, 1, 99, int.MaxValue)] int storedValue)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(SettingLevel.Effective);
        settingsSource.GetValue(SettingName).Returns(storedValue.ToString());

        binding.LoadSetting(settingsSource);

        textBox.Text.Should().Be(storedValue.ToString());
    }

    [Test]
    public void CreateControlBinding_TextBox_SaveSetting_should_write_valid_number(
        [Values(0, 1, 99, int.MaxValue)] int value)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();

        textBox.Text = value.ToString();
        binding.SaveSetting(settingsSource);

        settingsSource.Received(1).SetValue(SettingName, value.ToString());
    }

    [Test]
    public void CreateControlBinding_TextBox_should_set_red_background_when_text_changes_to_invalid(
        [Values("abc", "1.5", "1e3")] string invalidText)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        setting.CreateControlBinding(textBox);

        textBox.Text = invalidText;

        textBox.BackColor.Should().Be(TextBoxValidationColors.InvalidBackColor);
        textBox.ForeColor.Should().Be(TextBoxValidationColors.InvalidForeColor);
    }

    [Test]
    public void CreateControlBinding_TextBox_SaveSetting_should_store_null_on_invalid_input(
        [Values("abc", "1.5", "1e3")] string invalidText)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();

        textBox.Text = invalidText;
        binding.SaveSetting(settingsSource);

        settingsSource.Received(1).SetValue(SettingName, null);
    }

    [Test]
    public void CreateControlBinding_TextBox_should_restore_background_when_text_changes_to_valid(
        [Values(0, 1, 99, int.MaxValue)] int value)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new() { BackColor = Color.Red, ForeColor = Color.WhiteSmoke };
        setting.CreateControlBinding(textBox);

        textBox.Text = value.ToString();

        textBox.BackColor.Should().Be(TextBoxValidationColors.ValidBackColor);
        textBox.ForeColor.Should().Be(TextBoxValidationColors.ValidForeColor);
    }

    [Test]
    public void CreateControlBinding_TextBox_LoadSetting_should_restore_background(
        [Values(SettingLevel.Global, SettingLevel.Local, SettingLevel.Distributed, SettingLevel.Effective)] SettingLevel settingLevel)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new() { BackColor = Color.Red, ForeColor = Color.WhiteSmoke };
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(settingLevel);
        settingsSource.GetValue(SettingName).Returns(DefaultValue.ToString());

        binding.LoadSetting(settingsSource);

        textBox.BackColor.Should().Be(TextBoxValidationColors.ValidBackColor);
        textBox.ForeColor.Should().Be(TextBoxValidationColors.ValidForeColor);
    }

    [Test]
    public void CreateControlBinding_TextBox_SaveSetting_should_write_default_value_at_non_effective_level(
        [Values(SettingLevel.Global, SettingLevel.Local, SettingLevel.Distributed)] SettingLevel settingLevel)
    {
        NumberSetting<int> setting = new(SettingName, defaultValue: DefaultValue);
        using TextBox textBox = new();
        ISettingControlBinding binding = setting.CreateControlBinding(textBox);
        SettingsSource settingsSource = Substitute.For<SettingsSource>();
        settingsSource.SettingLevel.Returns(settingLevel);

        textBox.Text = DefaultValue.ToString();
        binding.SaveSetting(settingsSource);

        settingsSource.Received(1).SetValue(SettingName, DefaultValue.ToString());
    }
}
