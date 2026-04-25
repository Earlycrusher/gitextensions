namespace GitExtensions.Extensibility.Settings;

/// <summary>
///  Holds the colors used for TextBox validation feedback.
///  Set by the UI layer after theme initialization so that theme-aware colors can be used
///  without introducing a circular project dependency.
/// </summary>
public static class TextBoxValidationColors
{
    /// <summary>
    ///  The background color for a valid input.
    /// </summary>
    public static Color ValidBackColor { get; set; } = SystemColors.Window;

    /// <summary>
    ///  The foreground color for a valid input.
    /// </summary>
    public static Color ValidForeColor { get; set; } = SystemColors.WindowText;

    /// <summary>
    ///  The background color for an invalid input.
    /// </summary>
    public static Color InvalidBackColor { get; set; } = Color.FromArgb(255, 128, 128);

    /// <summary>
    ///  Gets or sets the foreground color for an invalid input.
    /// </summary>
    public static Color InvalidForeColor { get; set; } = SystemColors.WindowText;
}
