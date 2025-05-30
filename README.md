# 📦 CompressionZSTD

**CompressionZSTD** is a C# WPF application that provides efficient file compression and decompression using the powerful [Zstandard (Zstd)](https://facebook.github.io/zstd/) algorithm. Designed with a clean and intuitive UI, the app supports general-purpose file compression as well as image-specific optimizations such as JPEG quality reduction and image resizing.

---

## 🚀 Features

- ⚡ **Fast compression & decompression** using Zstandard
- 📁 Supports any file type (binary, text, media, etc.)
- 🖼️ **Image-specific compression**:
  - JPEG: adjustable quality reduction
  - PNG & other formats: image resizing
- 📊 View raw binary data before and after compression
- 🎚️ Adjustable compression level via UI slider
- 📂 Organized storage in app-specific folders:
  - `ZSTD AppData/Compressed/`
  - `ZSTD AppData/Decompressed/`
- 🔧 Built entirely in WPF (no XAML used)

---

## 🖥️ Screenshots

screenshots are in the folders

### Main Window
![Main UI](screenshots/main_ui.png)

### Compression in Action
![Compression](screenshots/compression_progress.png)

---

## 🛠️ Technologies Used

- 💻 **Language**: C# (.NET 9.0)
- 🖼️ **UI**: WPF (Windows Presentation Foundation)
- 📦 **Compression Library**: [Zstd.Net](https://github.com/therocode/zstd.net)

---

## 📂 Folder Structure

ZSTD AppData/
├── Compressed/
│ └── [Your compressed files]
├── Decompressed/
│ └── [Your decompressed files]


---

## 📝 How to Use

1. **Launch the application**
2. **Select a file** using the file picker
3. Choose compression level and (if applicable) image options
4. Click **Compress** or **Decompress**
5. Files will be saved automatically in respective folders

---

## 🧩 Future Improvements (Optional Ideas)

- Add drag-and-drop support
- Batch compression
- Preview image after compression
- Export compression logs

---

## 📄 License

This project is licensed under the **MIT License** – feel free to use, modify, and distribute it.

---

## 🙌 Acknowledgments

- [Zstandard Algorithm by Meta](https://facebook.github.io/zstd/)
- [Zstd.Net – .NET bindings](https://github.com/therocode/zstd.net)
