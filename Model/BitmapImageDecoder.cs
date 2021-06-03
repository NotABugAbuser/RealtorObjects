using System;
using System.IO;
using System.Windows.Media.Imaging;
using ExifLib;

namespace BitmapImageDecoding
{
    public static class BitmapImageDecoder
    {
        public static Byte[] GetDecodedBytes(String path, Int32 compressionLevel = 95, Int32 newWidth = 0)
        {
            Rotation rotation = GetRotation(path);
            Byte[] data = File.ReadAllBytes(path);
            BitmapImage image = ResizeImageByWidth(data, rotation, newWidth);
            return GetCompressedBytes(image, compressionLevel, rotation);
        }
        public static Byte[] GetDecodedBytes(Byte[] data, Int32 compressionLevel = 95, Int32 newWidth = 0)
        {
            Rotation rotation = Rotation.Rotate0;
            BitmapImage image = ResizeImageByWidth(data, rotation, newWidth);
            return GetCompressedBytes(image, compressionLevel, rotation);
        }
        public static BitmapImage GetDecodedImage(String path, Int32 compressionLevel = 95, Int32 newWidth = 0)
        {
            Rotation rotation = GetRotation(path);
            Byte[] data = GetCompressedBytes(new BitmapImage(new Uri(path)), compressionLevel, rotation);
            return ResizeImageByWidth(data, rotation, newWidth);
        }

        private static Byte[] GetCompressedBytes(BitmapImage image, Int32 compressionLevel, Rotation rotation)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                if (compressionLevel > 0)
                    encoder.QualityLevel = compressionLevel;
                encoder.Rotation = rotation;
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
        private static BitmapImage ResizeImageByWidth(Byte[] data, Rotation rotation, Int32 newWidth)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                if (newWidth > 0)
                    image.DecodePixelWidth = newWidth;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        public static BitmapImage BitmapImageFromBytes(Byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
        public static Byte[] BytesFromBitmapImage(BitmapImage image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Rotation = Rotation.Rotate0;
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static void SaveImage(String path, Byte[] data, Rotation rotation)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 95;
                    encoder.Rotation = rotation;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fs);
                }
            }
        }
        public static Rotation GetRotation(String path)
        {
            try
            {
                UInt16 s;
                ExifReader reader = new ExifReader(path);
                reader.GetTagValue(ExifTags.Orientation, out s);
                if (s == 6)
                    return Rotation.Rotate90;
                else if (s == 3)
                    return Rotation.Rotate180;
                else if (s == 8)
                    return Rotation.Rotate270;
                else
                    return Rotation.Rotate0;
            }
            catch
            {
                return Rotation.Rotate0;
            }
        }
    }
}
