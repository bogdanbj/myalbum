namespace FontManager;

/// <summary>
/// Custom dialog for confirming font family removal with option to delete files.
/// </summary>
public class RemoveConfirmDialog : Form
{
    public bool DeleteFiles { get; private set; } = false;

    public RemoveConfirmDialog(string familyName)
    {
        this.Text = "Confirm Remove";
        this.Size = new Size(400, 180);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Icon
        var iconLabel = new Label
        {
            Text = "?",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 122, 204),
            Location = new Point(20, 20),
            Size = new Size(40, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Message
        var messageLabel = new Label
        {
            Text = $"Remove font family '{familyName}' from the list?",
            Location = new Point(70, 20),
            Size = new Size(300, 40),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Delete files checkbox (unchecked by default)
        var deleteFilesCheckbox = new CheckBox
        {
            Text = "Delete physical files from disk",
            Location = new Point(70, 65),
            Size = new Size(250, 25),
            Checked = false
        };
        deleteFilesCheckbox.CheckedChanged += (s, e) => DeleteFiles = deleteFilesCheckbox.Checked;

        // Buttons
        var yesButton = new Button
        {
            Text = "Yes",
            DialogResult = DialogResult.Yes,
            Location = new Point(200, 105),
            Size = new Size(80, 28)
        };

        var noButton = new Button
        {
            Text = "No",
            DialogResult = DialogResult.No,
            Location = new Point(290, 105),
            Size = new Size(80, 28)
        };

        this.AcceptButton = yesButton;
        this.CancelButton = noButton;

        this.Controls.Add(iconLabel);
        this.Controls.Add(messageLabel);
        this.Controls.Add(deleteFilesCheckbox);
        this.Controls.Add(yesButton);
        this.Controls.Add(noButton);

        // Set focus to Yes button when form loads
        this.Load += (s, e) => yesButton.Focus();
    }
}
