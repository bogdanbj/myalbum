using MyAlbum.Common;

namespace FontManager;

public partial class Form1 : Form
{
    private readonly string _fontsFolder;
    private readonly string _fontsJsonPath;
    private FontMap _fontMap;

    private ListView _fontListView = null!;
    private TreeView _fontTreeView = null!;
    private TextBox _jsonTextBox = null!;
    private Panel _viewPanel = null!;
    private Button _listViewButton = null!;
    private Button _treeViewButton = null!;
    private Button _jsonViewButton = null!;
    private Label _statusLabel = null!;

    public Form1()
    {
        InitializeComponent();

        // Get fonts folder from config or use default
        _fontsFolder = Path.GetFullPath(
            System.Configuration.ConfigurationManager.AppSettings["FontsFolder"] 
            ?? "Resources/Fonts");
        _fontsJsonPath = Path.Combine(_fontsFolder, "fonts.json");

        // Ensure fonts folder exists
        Directory.CreateDirectory(_fontsFolder);

        // Load existing font map
        _fontMap = FontMap.Load(_fontsJsonPath);

        // Scan for existing font files not in the map
        ScanExistingFonts();

        SetupUI();
        RefreshFontList();
        _fontListView.BringToFront();
    }

    private void SetupUI()
    {
        this.Text = "Font Manager";
        this.Size = new Size(700, 500);
        this.MinimumSize = new Size(500, 350);

        // Font list view - use monospace font for alignment
        _fontListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Consolas", 9),
            ShowItemToolTips = true
        };
        _fontListView.Columns.Add("Family Name", 200);
        _fontListView.Columns.Add("Regular", 70, HorizontalAlignment.Center);
        _fontListView.Columns.Add("Bold", 70, HorizontalAlignment.Center);
        _fontListView.Columns.Add("Italic", 70, HorizontalAlignment.Center);
        _fontListView.Columns.Add("Bold Italic", 100, HorizontalAlignment.Center);
        _fontListView.DoubleClick += FontListView_DoubleClick;

        // Context menu for list view
        var listContextMenu = new ContextMenuStrip();
        var listAddItem = new ToolStripMenuItem("Add Font...");
        listAddItem.Click += AddButton_Click;
        var listRemoveItem = new ToolStripMenuItem("Remove");
        listRemoveItem.Click += RemoveButton_Click;
        listContextMenu.Items.Add(listAddItem);
        listContextMenu.Items.Add(listRemoveItem);
        _fontListView.ContextMenuStrip = listContextMenu;

        // Font tree view - use monospace font for alignment
        _fontTreeView = new TreeView
        {
            Dock = DockStyle.Fill,
            Visible = false,
            ShowLines = true,
            ShowPlusMinus = true,
            ShowRootLines = true,
            FullRowSelect = true,
            ItemHeight = 22,
            Font = new Font("Consolas", 9)
        };
        _fontTreeView.DoubleClick += FontTreeView_DoubleClick;

        // Context menu for tree view
        var treeContextMenu = new ContextMenuStrip();
        var treeAddItem = new ToolStripMenuItem("Add Font...");
        treeAddItem.Click += AddButton_Click;
        var treeRemoveItem = new ToolStripMenuItem("Remove");
        treeRemoveItem.Click += RemoveButton_Click;
        treeContextMenu.Items.Add(treeAddItem);
        treeContextMenu.Items.Add(treeRemoveItem);
        _fontTreeView.ContextMenuStrip = treeContextMenu;

        // JSON text view
        _jsonTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Consolas", 9),
            WordWrap = false
        };

        // Panel to hold all views
        _viewPanel = new Panel
        {
            Dock = DockStyle.Fill
        };
        // Add in reverse z-order - last added is on top
        _viewPanel.Controls.Add(_jsonTextBox);
        _viewPanel.Controls.Add(_fontTreeView);
        _viewPanel.Controls.Add(_fontListView);

        // Button panel using TableLayoutPanel for left/right alignment
        var buttonPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 40,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(5)
        };
        buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Left side (stretches)
        buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // Right side (auto)

        // Common button size and margin for vertical alignment
        var buttonSize = new Size(60, 28);
        var buttonMargin = new Padding(3, 3, 3, 3);

        // Left panel for Scan button
        var leftPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true
        };

        // Scan button
        var scanButton = new Button { Text = "Scan Folder", AutoSize = true, Margin = buttonMargin };
        scanButton.Click += ScanButton_Click;
        leftPanel.Controls.Add(scanButton);

        // Right panel for view toggle buttons
        var rightPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false
        };

        _listViewButton = new Button
        {
            Text = "List",
            Size = buttonSize,
            Margin = buttonMargin
        };
        _listViewButton.Click += ListViewButton_Click;

        _treeViewButton = new Button
        {
            Text = "Tree",
            Size = buttonSize,
            Margin = buttonMargin
        };
        _treeViewButton.Click += TreeViewButton_Click;

        _jsonViewButton = new Button
        {
            Text = "JSON",
            Size = buttonSize,
            Margin = buttonMargin
        };
        _jsonViewButton.Click += JsonViewButton_Click;

        // Set initial button states
        UpdateViewButtonStates();

        rightPanel.Controls.Add(_listViewButton);
        rightPanel.Controls.Add(_treeViewButton);
        rightPanel.Controls.Add(_jsonViewButton);

        buttonPanel.Controls.Add(leftPanel, 0, 0);
        buttonPanel.Controls.Add(rightPanel, 1, 0);

        // Status bar
        _statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 25,
            Text = $"Fonts folder: {_fontsFolder}",
            Padding = new Padding(5, 5, 0, 0),
            BorderStyle = BorderStyle.Fixed3D
        };

        // Add controls - order matters for docking!
        // For proper layout: add Fill first, then Top/Bottom
        // WinForms docks in reverse order of adding
        this.Controls.Add(_viewPanel);      // Fill (added first, docks last)
        this.Controls.Add(buttonPanel);     // Top
        this.Controls.Add(_statusLabel);    // Bottom
    }

    private void ListViewButton_Click(object? sender, EventArgs e)
    {
        _fontListView.Visible = true;
        _fontTreeView.Visible = false;
        _jsonTextBox.Visible = false;
        _fontListView.BringToFront();
        UpdateViewButtonStates();
    }

    private void TreeViewButton_Click(object? sender, EventArgs e)
    {
        _fontListView.Visible = false;
        _fontTreeView.Visible = true;
        _jsonTextBox.Visible = false;
        _fontTreeView.BringToFront();
        RefreshTreeView();
        UpdateViewButtonStates();
    }

    private void JsonViewButton_Click(object? sender, EventArgs e)
    {
        _fontListView.Visible = false;
        _fontTreeView.Visible = false;
        _jsonTextBox.Visible = true;
        _jsonTextBox.BringToFront();
        RefreshJsonView();
        UpdateViewButtonStates();
    }

    private void UpdateViewButtonStates()
    {
        // Highlight the active view button
        _listViewButton.BackColor = _fontListView.Visible ? SystemColors.ControlDark : SystemColors.Control;
        _treeViewButton.BackColor = _fontTreeView.Visible ? SystemColors.ControlDark : SystemColors.Control;
        _jsonViewButton.BackColor = _jsonTextBox.Visible ? SystemColors.ControlDark : SystemColors.Control;
    }

    private void RefreshJsonView()
    {
        if (File.Exists(_fontsJsonPath))
        {
            _jsonTextBox.Text = File.ReadAllText(_fontsJsonPath);
        }
        else
        {
            _jsonTextBox.Text = "// fonts.json does not exist yet";
        }
    }

    private void RefreshFontList()
    {
        _fontListView.Items.Clear();

        foreach (var familyName in _fontMap.GetFamilyNames())
        {
            var family = _fontMap.Fonts[familyName];
            var item = new ListViewItem(familyName);
            
            // Build tooltip with all filenames
            var tooltipParts = new List<string>();
            if (family.Regular != null) tooltipParts.Add($"Regular: {family.Regular}");
            if (family.Bold != null) tooltipParts.Add($"Bold: {family.Bold}");
            if (family.Italic != null) tooltipParts.Add($"Italic: {family.Italic}");
            if (family.BoldItalic != null) tooltipParts.Add($"Bold Italic: {family.BoldItalic}");
            item.ToolTipText = string.Join("\n", tooltipParts);
            
            item.SubItems.Add(family.Regular != null ? "\u2713" : "");
            item.SubItems.Add(family.Bold != null ? "\u2713" : "");
            item.SubItems.Add(family.Italic != null ? "\u2713" : "");
            item.SubItems.Add(family.BoldItalic != null ? "\u2713" : "");
            _fontListView.Items.Add(item);
        }

        // Also refresh tree view if visible
        if (_fontTreeView.Visible)
        {
            RefreshTreeView();
        }

        _statusLabel.Text = $"Fonts folder: {_fontsFolder} | {_fontMap.Fonts.Count} font families";
    }

    private void RefreshTreeView()
    {
        _fontTreeView.BeginUpdate();
        _fontTreeView.Nodes.Clear();

        foreach (var familyName in _fontMap.GetFamilyNames())
        {
            var family = _fontMap.Fonts[familyName];
            
            // Create family node
            var familyNode = new TreeNode(familyName)
            {
                Tag = familyName,
                NodeFont = new Font(_fontTreeView.Font, System.Drawing.FontStyle.Bold)
            };

            // Add style children with padded style names for alignment
            if (family.Regular != null)
            {
                familyNode.Nodes.Add(new TreeNode($"{"Regular",-12}  {family.Regular}")
                {
                    Tag = ("Regular", family.Regular)
                });
            }
            if (family.Bold != null)
            {
                familyNode.Nodes.Add(new TreeNode($"{"Bold",-12}  {family.Bold}")
                {
                    Tag = ("Bold", family.Bold)
                });
            }
            if (family.Italic != null)
            {
                familyNode.Nodes.Add(new TreeNode($"{"Italic",-12}  {family.Italic}")
                {
                    Tag = ("Italic", family.Italic)
                });
            }
            if (family.BoldItalic != null)
            {
                familyNode.Nodes.Add(new TreeNode($"{"Bold Italic",-12}  {family.BoldItalic}")
                {
                    Tag = ("BoldItalic", family.BoldItalic)
                });
            }

            _fontTreeView.Nodes.Add(familyNode);
        }

        _fontTreeView.ExpandAll();
        
        // Scroll to top
        if (_fontTreeView.Nodes.Count > 0)
        {
            _fontTreeView.Nodes[0].EnsureVisible();
        }
        
        _fontTreeView.EndUpdate();
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select Font File",
            Filter = "TrueType Fonts (*.ttf)|*.ttf|OpenType Fonts (*.otf)|*.otf|All Files (*.*)|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        foreach (var sourceFile in dialog.FileNames)
        {
            try
            {
                AddFontFile(sourceFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding {Path.GetFileName(sourceFile)}:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        _fontMap.Save(_fontsJsonPath);
        RefreshFontList();
    }

    private void AddFontFile(string sourceFile)
    {
        // Read font metadata
        var (familyName, style) = TtfMetadataReader.ReadFontInfo(sourceFile);

        if (string.IsNullOrEmpty(familyName))
        {
            throw new Exception("Could not read font family name from file.");
        }

        // Generate target filename
        var extension = Path.GetExtension(sourceFile).ToLowerInvariant();
        var styleSuffix = style switch
        {
            MyAlbum.Common.FontStyle.Bold => "-bold",
            MyAlbum.Common.FontStyle.Italic => "-italic",
            MyAlbum.Common.FontStyle.BoldItalic => "-bolditalic",
            _ => ""
        };
        var safeFileName = MakeSafeFileName(familyName) + styleSuffix + extension;
        var targetPath = Path.Combine(_fontsFolder, safeFileName);

        // Check if file already exists and is different
        bool shouldCopy = true;
        if (File.Exists(targetPath) && !FilesAreEqual(sourceFile, targetPath))
        {
            var result = MessageBox.Show(
                $"Font file '{safeFileName}' already exists.\n\nOverwrite with the new file?",
                "Confirm Overwrite",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            shouldCopy = (result == DialogResult.Yes);
        }

        // Copy file if needed
        if (shouldCopy && (!File.Exists(targetPath) || !FilesAreEqual(sourceFile, targetPath)))
        {
            File.Copy(sourceFile, targetPath, overwrite: true);
        }

        // Always update font map (use existing file if not overwritten)
        _fontMap.SetFontFile(familyName, style, safeFileName);
    }

    private void RemoveButton_Click(object? sender, EventArgs e)
    {
        string? familyName = null;

        // Get selected family from current view
        if (_fontListView.Visible)
        {
            if (_fontListView.SelectedItems.Count == 0)
                return;
            familyName = _fontListView.SelectedItems[0].Text;
        }
        else
        {
            if (_fontTreeView.SelectedNode == null)
                return;
            
            // If a child node is selected, get the parent family
            var node = _fontTreeView.SelectedNode;
            if (node.Parent != null)
                node = node.Parent;
            
            familyName = node.Tag as string;
        }

        if (string.IsNullOrEmpty(familyName))
            return;

        // Show custom dialog with "Delete files" checkbox
        using var dialog = new RemoveConfirmDialog(familyName);
        if (dialog.ShowDialog() != DialogResult.Yes)
            return;

        // Delete font files only if user checked "Delete physical files"
        if (dialog.DeleteFiles && _fontMap.Fonts.TryGetValue(familyName, out var family))
        {
            foreach (var file in family.AllFiles)
            {
                var filePath = Path.Combine(_fontsFolder, file);
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); }
                    catch { /* ignore deletion errors */ }
                }
            }
        }

        // Remove from map
        _fontMap.RemoveFamily(familyName);
        _fontMap.Save(_fontsJsonPath);
        RefreshFontList();
    }

    private static string MakeSafeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(name.Where(c => !invalid.Contains(c)).ToArray());
        return safe.Replace(" ", "").ToLowerInvariant();
    }

    private static bool FilesAreEqual(string file1, string file2)
    {
        var info1 = new FileInfo(file1);
        var info2 = new FileInfo(file2);
        return info1.Length == info2.Length;
    }

    /// <summary>
    /// Scan the fonts folder for existing TTF/OTF files and add them to fonts.json
    /// </summary>
    private void ScanExistingFonts()
    {
        var fontFiles = Directory.GetFiles(_fontsFolder, "*.ttf")
            .Concat(Directory.GetFiles(_fontsFolder, "*.otf"))
            .ToList();

        if (fontFiles.Count == 0)
            return;

        // Get all files already in the font map
        var mappedFiles = _fontMap.Fonts.Values
            .SelectMany(f => f.AllFiles)
            .Select(f => f.ToLowerInvariant())
            .ToHashSet();

        int added = 0;
        foreach (var fontFile in fontFiles)
        {
            var filename = Path.GetFileName(fontFile);
            
            // Skip if already mapped
            if (mappedFiles.Contains(filename.ToLowerInvariant()))
                continue;

            try
            {
                var (familyName, style) = TtfMetadataReader.ReadFontInfo(fontFile);
                
                if (!string.IsNullOrEmpty(familyName))
                {
                    _fontMap.SetFontFile(familyName, style, filename);
                    added++;
                }
            }
            catch
            {
                // Skip files that can't be read
            }
        }

        if (added > 0)
        {
            _fontMap.Save(_fontsJsonPath);
        }
    }

    private void ScanButton_Click(object? sender, EventArgs e)
    {
        ScanExistingFonts();
        RefreshFontList();
        MessageBox.Show("Scan complete.", "Scan Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void FontListView_DoubleClick(object? sender, EventArgs e)
    {
        if (_fontListView.SelectedItems.Count == 0)
            return;

        var familyName = _fontListView.SelectedItems[0].Text;
        
        if (!_fontMap.Fonts.TryGetValue(familyName, out var family))
            return;

        // Open preview window (non-modal)
        var previewForm = new FontPreviewForm(familyName, family, _fontsFolder);
        previewForm.Show();
    }

    private void FontTreeView_DoubleClick(object? sender, EventArgs e)
    {
        if (_fontTreeView.SelectedNode == null)
            return;

        // Get family name (from parent if child selected)
        var node = _fontTreeView.SelectedNode;
        string? familyName;
        
        if (node.Parent != null)
        {
            // Child node - get parent family name
            familyName = node.Parent.Tag as string;
        }
        else
        {
            // Family node
            familyName = node.Tag as string;
        }

        if (string.IsNullOrEmpty(familyName) || !_fontMap.Fonts.TryGetValue(familyName, out var family))
            return;

        // Open preview window (non-modal)
        var previewForm = new FontPreviewForm(familyName, family, _fontsFolder);
        previewForm.Show();
    }
}
