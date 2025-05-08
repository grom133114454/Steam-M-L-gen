using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace ManifestHubDownloader;

public partial class Form1 : Form
{
    private string steamPath;
    private System.Windows.Forms.Timer imageLoadTimer;

    public Form1()
    {
        InitializeComponent();
        txtGameId.Focus();
        
        // Create M&L folder if it doesn't exist
        if (!Directory.Exists("M&L"))
        {
            Directory.CreateDirectory("M&L");
        }

        // Load saved Steam path
        LoadSteamPath();
        
        // If path not found, try to detect it automatically
        if (string.IsNullOrEmpty(steamPath))
        {
            DetectSteamPath();
        }

        // Initialize timer for delayed image loading
        imageLoadTimer = new System.Windows.Forms.Timer();
        imageLoadTimer.Interval = 500; // 500ms delay
        imageLoadTimer.Tick += ImageLoadTimer_Tick;
    }

    private void ImageLoadTimer_Tick(object sender, EventArgs e)
    {
        imageLoadTimer.Stop();
        LoadGameImage(txtGameId.Text.Trim());
    }

    private void txtGameId_TextChanged(object sender, EventArgs e)
    {
        // Reset timer on each text change
        imageLoadTimer.Stop();
        imageLoadTimer.Start();
    }

    private void LoadGameImage(string gameId)
    {
        if (string.IsNullOrEmpty(gameId))
        {
            picGame.Image = null;
            return;
        }

        string imgUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameId}/header.jpg";
        try
        {
            using (WebClient client = new WebClient())
            {
                var imgStream = client.OpenRead(imgUrl);
                picGame.Image = System.Drawing.Image.FromStream(imgStream);
            }
        }
        catch
        {
            picGame.Image = null;
        }
    }

    private void LoadSteamPath()
    {
        try
        {
            if (File.Exists("steam_path.txt"))
            {
                steamPath = File.ReadAllText("steam_path.txt").Trim();
                if (!Directory.Exists(steamPath))
                {
                    steamPath = null;
                }
            }
        }
        catch
        {
            steamPath = null;
        }
    }

    private void SaveSteamPath(string path)
    {
        try
        {
            File.WriteAllText("steam_path.txt", path);
            steamPath = path;
        }
        catch
        {
            MessageBox.Show("Failed to save Steam folder path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DetectSteamPath()
    {
        // Try to find path in registry
        try
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam"))
            {
                if (key != null)
                {
                    string path = key.GetValue("InstallPath") as string;
                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    {
                        SaveSteamPath(path);
                        return;
                    }
                }
            }
        }
        catch { }

        // If not found in registry, try standard paths
        string[] possiblePaths = new[]
        {
            @"C:\Program Files (x86)\Steam",
            @"C:\Program Files\Steam",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam")
        };

        foreach (string path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                SaveSteamPath(path);
                return;
            }
        }

        // If not found automatically, ask user
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            dialog.Description = "Select Steam folder";
            dialog.ShowNewFolderButton = false;
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveSteamPath(dialog.SelectedPath);
            }
        }
    }

    private void ProcessGameFiles(string gameId, string sourcePath)
    {
        try
        {
            // Find .lua file
            string[] luaFiles = Directory.GetFiles(sourcePath, "*.lua", SearchOption.AllDirectories);
            if (luaFiles.Length == 0)
            {
                MessageBox.Show("Lua file not found in game folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string luaFile = luaFiles[0];
            string luapackaPath = Path.Combine(Application.StartupPath, "luapacka.exe");

            if (!File.Exists(luapackaPath))
            {
                MessageBox.Show("luapacka.exe not found in program folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create necessary folders if they don't exist
            string stpluginPath = Path.Combine(steamPath, "config", "stplug-in");
            string depotcachePath = Path.Combine(steamPath, "config", "depotcache");

            Directory.CreateDirectory(stpluginPath);
            Directory.CreateDirectory(depotcachePath);

            // Run luapacka.exe to process .lua file
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = luapackaPath,
                Arguments = $"\"{luaFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    MessageBox.Show("Error processing Lua file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Copy processed file to stplug-in
            string processedFile = Path.ChangeExtension(luaFile, ".bin");
            if (File.Exists(processedFile))
            {
                string targetFile = Path.Combine(stpluginPath, Path.GetFileName(processedFile));
                File.Copy(processedFile, targetFile, true);
            }

            // Copy files to appropriate folders
            foreach (string file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (file == luaFile || file == processedFile)
                    continue;

                string extension = Path.GetExtension(file).ToLower();
                string targetPath;

                if (extension == ".st")
                {
                    // Copy .st files to stplug-in
                    targetPath = Path.Combine(stpluginPath, Path.GetFileName(file));
                    File.Copy(file, targetPath, true);
                }
                else if (extension == ".manifest")
                {
                    // Copy .manifest files to depotcache
                    targetPath = Path.Combine(depotcachePath, Path.GetFileName(file));
                    File.Copy(file, targetPath, true);
                }
                else
                {
                    // Copy other files to depotcache
                    string relativePath = file.Substring(sourcePath.Length + 1);
                    targetPath = Path.Combine(depotcachePath, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    File.Copy(file, targetPath, true);
                }
            }

            MessageBox.Show("Files successfully processed and copied to Steam folders.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
        string userInput = txtGameId.Text.Trim();
        if (string.IsNullOrEmpty(userInput))
        {
            MessageBox.Show("Enter game ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtGameId.Focus();
            return;
        }

        if (string.IsNullOrEmpty(steamPath))
        {
            MessageBox.Show("Steam folder path not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        string branch = userInput; // Use ID as branch name
        string url = $"https://github.com/SteamAutoCracks/ManifestHub/archive/refs/heads/{branch}.zip";
        string zipPath = Path.Combine("M&L", $"{branch}.zip");
        string extractPath = Path.Combine("M&L", $"{branch}_unzipped");
        string finalPath = Path.Combine("M&L", userInput);

        try
        {
            // Delete old folders if they exist
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            if (Directory.Exists(finalPath))
            {
                Directory.Delete(finalPath, true);
            }

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, zipPath);
            }

            ZipFile.ExtractToDirectory(zipPath, extractPath);

            // Find extracted folder and rename it
            string[] extractedDirs = Directory.GetDirectories(extractPath);
            if (extractedDirs.Length > 0)
            {
                string originalDir = extractedDirs[0];
                Directory.Move(originalDir, finalPath);

                // Delete unnecessary files
                string keyVdfPath = Path.Combine(finalPath, "key.vdf");
                if (File.Exists(keyVdfPath))
                {
                    File.Delete(keyVdfPath);
                }

                // Delete all .json files
                string[] jsonFiles = Directory.GetFiles(finalPath, "*.json", SearchOption.AllDirectories);
                foreach (string jsonFile in jsonFiles)
                {
                    try
                    {
                        File.Delete(jsonFile);
                    }
                    catch { }
                }
            }

            // Process files after successful extraction
            ProcessGameFiles(userInput, finalPath);

            MessageBox.Show($"Game {userInput} successfully downloaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Game not found in ManifestHub database.");
        }
        finally
        {
            // Delete temporary folders and ZIP file
            if (Directory.Exists(extractPath))
            {
                try
                {
                    Directory.Delete(extractPath, true);
                }
                catch { }
            }
            if (File.Exists(zipPath))
            {
                try
                {
                    File.Delete(zipPath);
                }
                catch { }
            }
        }
        txtGameId.Focus();
    }

    private void btnDownloadSteamTools_Click(object sender, EventArgs e)
    {
        try
        {
            string steamToolsPath = Path.Combine(Application.StartupPath, "Steamtools.exe");
            if (File.Exists(steamToolsPath))
            {
                MessageBox.Show("SteamTools is already downloaded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a temporary file for the download
            string tempFile = Path.Combine(Path.GetTempPath(), "Steamtools.exe");
            
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://github.com/SteamTools-Team/SteamTools/releases/latest/download/Steamtools.exe", tempFile);
            }

            // Move the downloaded file to the application directory
            File.Move(tempFile, steamToolsPath, true);
            MessageBox.Show("SteamTools has been successfully downloaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error downloading SteamTools: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
