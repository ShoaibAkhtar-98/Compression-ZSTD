using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using ZstdNet;

namespace CompressionZSTD
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string currentFilePath;
        private byte[] originalBytes;
        private byte[] compressedBytes;

        private string originalData;
        public string OriginalData
        {
            get => originalData;
            set { originalData = value; OnPropertyChanged(nameof(OriginalData)); }
        }

        private string compressedData;
        public string CompressedData
        {
            get => compressedData;
            set { compressedData = value; OnPropertyChanged(nameof(CompressedData)); }
        }

        private string originalSize;
        public string OriginalSize
        {
            get => originalSize;
            set { originalSize = value; OnPropertyChanged(nameof(OriginalSize)); }
        }

        private string compressedSize;
        public string CompressedSize
        {
            get => compressedSize;
            set { compressedSize = value; OnPropertyChanged(nameof(CompressedSize)); }
        }

        private string decompressedSize;
        public string DecompressedSize
        {
            get => decompressedSize;
            set { decompressedSize = value; OnPropertyChanged(nameof(DecompressedSize)); }
        }

        private string status;
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        private bool resizeImages = false;
        public bool ResizeImages
        {
            get => resizeImages;
            set { resizeImages = value; OnPropertyChanged(nameof(ResizeImages)); }
        }

        private int compressionLevel = 80;
        public int CompressionLevel
        {
            get => compressionLevel;
            set { compressionLevel = value; OnPropertyChanged(nameof(CompressionLevel)); }
        }

        private readonly string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ZSTD_AppData");
        private readonly string compressedPath;
        private readonly string decompressedPath;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            compressedPath = Path.Combine(basePath, "Compressed");
            decompressedPath = Path.Combine(basePath, "Decompressed");

            Directory.CreateDirectory(compressedPath);
            Directory.CreateDirectory(decompressedPath);
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                await SetBusyAsync(async () =>
                {
                    currentFilePath = dialog.FileName;
                    originalBytes = await File.ReadAllBytesAsync(currentFilePath);

                    OriginalData = BitConverter.ToString(originalBytes).Replace("-", " ");
                    OriginalSize = $"Original Size: {originalBytes.Length / 1024.0:F2} KB";
                    CompressedData = string.Empty;
                    CompressedSize = string.Empty;
                    DecompressedSize = string.Empty;
                    Status = $"Loaded file: {Path.GetFileName(currentFilePath)}";
                });
            }
        }

        private async void CompressFile_Click(object sender, RoutedEventArgs e)
        {
            if (originalBytes == null)
            {
                Status = "No file loaded.";
                return;
            }

            await SetBusyAsync(() =>
            {
                string ext = Path.GetExtension(currentFilePath).ToLower();
                byte[] dataToCompress = originalBytes;

                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".pdf")
                {
                    dataToCompress = CompressJpeg(currentFilePath, CompressionLevel, ResizeImages);
                }

                using var compressor = new Compressor();
                compressedBytes = compressor.Wrap(dataToCompress);

                CompressedData = BitConverter.ToString(compressedBytes).Replace("-", " ");
                CompressedSize = $"Compressed Size: {compressedBytes.Length / 1024.0:F2} KB";

                var outFile = Path.Combine(compressedPath, Path.GetFileName(currentFilePath) + ".zst");
                File.WriteAllBytes(outFile, compressedBytes);

                Status = $"Compressed and saved to: {outFile}";
            });
        }

        private async void DecompressFile_Click(object sender, RoutedEventArgs e)
        {
            if (compressedBytes == null)
            {
                Status = "No compressed file available.";
                return;
            }

            await SetBusyAsync(() =>
            {
                using var decompressor = new Decompressor();
                var decompressedBytes = decompressor.Unwrap(compressedBytes);

                var outPath = Path.Combine(decompressedPath, Path.GetFileNameWithoutExtension(currentFilePath));
                File.WriteAllBytes(outPath, decompressedBytes);

                DecompressedSize = $"Decompressed Size: {decompressedBytes.Length / 1024.0:F2} KB";
                Status = $"Decompressed and saved to: {outPath}";
            });
        }

        private byte[] CompressJpeg(string imagePath, long quality, bool resize)
        {
            using var bitmap = resize ? ResizeImage(imagePath, 800, 600) : new Bitmap(imagePath);
            var encoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            using var ms = new MemoryStream();
            bitmap.Save(ms, encoder, encParams);
            return ms.ToArray();
        }

        private Bitmap ResizeImage(string imagePath, int width, int height)
        {
            using var src = new Bitmap(imagePath);
            var resized = new Bitmap(width, height);
            using var g = Graphics.FromImage(resized);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(src, 0, 0, width, height);
            return resized;
        }

        private async Task SetBusyAsync(Action work)
        {
            IsBusy = true;
            await Task.Run(work);
            IsBusy = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
