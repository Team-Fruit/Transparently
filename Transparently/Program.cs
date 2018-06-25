using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Transparently {
    class Program {
        private static readonly Color TransparentColor = Color.FromArgb(0, 0, 0, 0);

        static void Main(string[] args) {
            string[] files = System.Environment.GetCommandLineArgs();
            if (files.Length > 1) {
                for (int i = 1; i < files.Length; i++) {
                    Process(files[i]);
                }
            } else {
                while (true) {
                    Console.WriteLine("画像かディレクトリのパスを入力してください");
                    string inputPath = Console.ReadLine();
                    Process(inputPath);
                }
            }
        }

        static void Process(string path) {
            FileAttributes attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory)) {
                foreach (FileInfo f in new DirectoryInfo(path).GetFiles()) {
                    try {
                        Console.WriteLine(f.Name);
                        Transparently(Path.Combine(path, f.Name));
                    } catch (Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                }
            } else {
                try {
                    Transparently(path);
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        static void Transparently(string path) {
            try {
                Color[,] pixelData;
                int width, height;
                ImageFormat format;
                using (Bitmap tmpImg = new Bitmap(path)) {
                    using (Bitmap img = new Bitmap(tmpImg)) {
                        width = img.Width;
                        height = img.Height;
                        format = img.RawFormat;
                        pixelData = new Color[img.Width, img.Height];
                        for (int y = 0; y < height; y++) {
                            for (int x = 0; x < width; x++) {
                                Color color = img.GetPixel(x, y);
                                if (color.R == 0 && color.G == 0 && color.B == 0) {
                                    pixelData[x, y] = TransparentColor;
                                } else {
                                    pixelData[x, y] = color;
                                }
                            }
                        }
                    }
                }
                using (Bitmap saveImg = new Bitmap(width, height)) {
                    for (int y = 0; y < height; y++) {
                        for (int x = 0; x < width; x++) {
                            saveImg.SetPixel(x, y, pixelData[x, y]);
                        }
                    }
                    saveImg.Save(path, format);
                }
                Console.WriteLine("success!");
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}