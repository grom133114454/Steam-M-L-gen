using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ManifestHubDownloader;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        txtGameId.Focus();
        
        // Создаем папку M&L если её нет
        if (!Directory.Exists("M&L"))
        {
            Directory.CreateDirectory("M&L");
        }
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
        picGame.Image = null; // Очищаем картинку перед новым поиском
        string userInput = txtGameId.Text.Trim();
        if (string.IsNullOrEmpty(userInput))
        {
            MessageBox.Show("Введите ID игры!");
            txtGameId.Focus();
            return;
        }

        string branch = userInput; // Используем ID как название ветки
        string url = $"https://github.com/SteamAutoCracks/ManifestHub/archive/refs/heads/{branch}.zip";
        string zipPath = Path.Combine("M&L", $"{branch}.zip");
        string extractPath = Path.Combine("M&L", $"{branch}_unzipped");
        string finalPath = Path.Combine("M&L", userInput);

        try
        {
            // Удаляем старые папки если они есть
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

            // Находим распакованную папку и переименовываем её
            string[] extractedDirs = Directory.GetDirectories(extractPath);
            if (extractedDirs.Length > 0)
            {
                string originalDir = extractedDirs[0];
                Directory.Move(originalDir, finalPath);

                // Удаляем ненужные файлы
                string keyVdfPath = Path.Combine(finalPath, "key.vdf");
                if (File.Exists(keyVdfPath))
                {
                    File.Delete(keyVdfPath);
                }

                // Удаляем все .json файлы
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

            MessageBox.Show($"Игра {userInput} успешно загружена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string imgUrl = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{userInput}/header.jpg";
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
                MessageBox.Show("Изображение не найдено для этой игры.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Игра не найдена в базе ManifestHub.");
        }
        finally
        {
            // Удаляем временные папки и ZIP файл
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
}
