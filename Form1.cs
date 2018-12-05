using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bitmapfy
{
    public partial class Form1 : Form
    {
        string newFilePath = "";

        private const int BITS_PER_PIXEL = 24;
        private const int CUSTOM_BYTES = 12; // Margin(4) + Extension(8)
        private const int HEADER_SIZE = 54;

        public Form1()
        {
            InitializeComponent();
        }

        private void FileSelectButton_Click(object sender, EventArgs e)
        {
            Status.Text = "Processing...";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string extention = Path.GetExtension(openFileDialog.FileName).ToUpper();
                if (extention.Equals(".BMP"))
                {
                    convertBmpToFile(openFileDialog.FileName);
                }
                else
                {
                    convertFileToBmp(openFileDialog.FileName);
                }
            }

            Status.Text = "Process Completed";
        }

        private bool convertBmpToFile(string fileName)
        {
            try
            {
                FileStream reader = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                // File
                byte[] buffer = new byte[reader.Length];
                reader.Read(buffer, 0, buffer.Length);

                // Margin
                byte[] marginByteArray = new byte[4];
                Array.Copy(buffer, HEADER_SIZE, marginByteArray, 0, 4);
                int margin = BitConverter.ToInt32(marginByteArray, 0);

                // Extesion
                byte[] extesionByteArray = new byte[8];
                Array.Copy(buffer, HEADER_SIZE + 4, extesionByteArray, 0, 8);
                string extension = Encoding.Default.GetString(extesionByteArray);

                // Cutting bmp file
                byte[] newFileByteArray = new byte[buffer.Length - HEADER_SIZE - CUSTOM_BYTES - margin];
                Array.Copy(buffer, HEADER_SIZE + CUSTOM_BYTES, newFileByteArray, 0, buffer.Length - HEADER_SIZE - CUSTOM_BYTES - margin);

                // Writing new file
                newFilePath = Path.Combine(Path.GetDirectoryName(fileName), "bmpInFile" + new Random().Next(10000, 99999) + extension);
                writeBinaryInFile(newFileByteArray, 0);
                reader.Close();
            }
            catch
            {
                Status.Text = "ERROR";
                return false;
            }
            return true;
        }

        private bool convertFileToBmp(string filePath)
        {
            newFilePath = Path.Combine(Path.GetDirectoryName(filePath), "fileInBmp" + new Random().Next(10000, 99999) + ".bmp");

            int sizeImageChunk = Convert.ToInt32(new FileInfo(filePath).Length) + CUSTOM_BYTES,
            step_margin = calculateWidth(sizeImageChunk),
            marginImageChunk = sizeImageChunk % step_margin,
            totalImageSize = sizeImageChunk + marginImageChunk,
            sizeNewFile = HEADER_SIZE + totalImageSize,
            width = step_margin,
            height = (totalImageSize / (BITS_PER_PIXEL / 8)) / width; // (TotalImageSize / bytesPerPixel) / width

            #region [ Header ]

            writeBinaryInFile(new byte[] { 0x42, 0x4D }, 2); // .bmp Mark
            writeBinaryInFile(BitConverter.GetBytes(sizeNewFile), 4); // New file size
            writeBinaryInFile(new byte[] { }, 4); // Space Reserved
            writeBinaryInFile(BitConverter.GetBytes(0x36), 4); // Start of Image Data
            writeBinaryInFile(BitConverter.GetBytes(0x28), 4); // Size of Header after this slot of memory

            // Dimensions
            writeBinaryInFile(BitConverter.GetBytes(width), 4); // Width
            writeBinaryInFile(BitConverter.GetBytes(height), 4); // Height

            // Others
            writeBinaryInFile(new byte[] { 0x01 }, 2); // Qty of Planes
            writeBinaryInFile(BitConverter.GetBytes(BITS_PER_PIXEL), 2); // Bits Per Pixel // 24 bits // 3 bytes
            writeBinaryInFile(new byte[] { }, 4); // compression // 0 = None, 1 = RLE-8, 2 = RLE-4

            writeBinaryInFile(BitConverter.GetBytes(sizeImageChunk), 4); // sizeImageData

            // Optionals
            writeBinaryInFile(new byte[] { }, 4); // Horizontal Pixel Per Meter
            writeBinaryInFile(new byte[] { }, 4); // Vertical Pixel Per Meter
            writeBinaryInFile(new byte[] { }, 4); // Qty of Colours or Zero
            writeBinaryInFile(new byte[] { }, 4); // Qty of Important Colours or Zero

            // Not native from Bitmap format specification
            writeBinaryInFile(BitConverter.GetBytes(marginImageChunk), 0); // Custom: Margin

            byte[] extesionByteArray = checkByteArraySizeFiller(
                Encoding.ASCII.GetBytes(Path.GetExtension(openFileDialog.FileName)),
                8,
                0x20);
            writeBinaryInFile(extesionByteArray, 0); // Custom: Extension

            #endregion

            // Write file
            FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[reader.Length];
            reader.Read(buffer, 0, buffer.Length);
            writeBinaryInFile(buffer, 0);
            reader.Close();

            // Write margin
            for (int i = 0; i < marginImageChunk; i++)
            {
                writeBinaryInFile(new byte[] { 0x00 }, 1);
            }
            return true;
        }

        private int calculateWidth(int sizeImageChunk)
        {
            // Try to make the image more square as possible
            int bytesPerPixel = BITS_PER_PIXEL / 8;
            return (int)Math.Ceiling(Math.Sqrt(sizeImageChunk / bytesPerPixel));
        }

        private int writeBinaryInFile(byte[] byteArray, int size)
        {
            byte[] data = byteArray;
            data = checkByteArraySize(byteArray, size);

            try
            {
                FileStream fileStream = new FileStream(newFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                BinaryWriter Writer = new BinaryWriter(fileStream);
                Writer.Write(data);
                Writer.Close();
            }
            catch
            {
                Status.Text = "ERROR";
                return 0;
            }

            return data.Length;
        }

        private byte[] checkByteArraySize(byte[] byteArray, int size)
        {
            return checkByteArraySizeFiller(byteArray, size, 0x00);
        }

        private byte[] checkByteArraySizeFiller(byte[] byteArray, int size, byte filler)
        {
            List<byte> byteList = byteArray.ToList();
            if (size > 0)
            {
                byteList.ToArray();
                while (byteList.Count < size)
                {
                    byteList.Add(filler);
                }

                while (byteList.Count > size)
                {
                    byteList.RemoveAt(byteList.Count - 1);
                }
            }
            return byteList.ToArray();
        }
    }
}
