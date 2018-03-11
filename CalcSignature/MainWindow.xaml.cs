using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.IO;                    // File stream
using System.Security.Cryptography;	// MD5, SHA-1, ... hash

namespace CalcSignature
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String[] signatureNames = new string[5] { "MD5", "SHA-1", "SHA-256", "SHA-384", "SHA-512" }; // as string
        private enum signatureIndex { MD5, SHA_1, SHA_256, SHA_384, SHA_512 }; // as index
        private int iSelectedBrush = 0; // Flag to toggle between two different background images
        private ImageBrush myBrush1;
        private ImageBrush myBrush2;
        private bool useBackground1 = false;
        private bool useBackground2 = false;

        public MainWindow()
        {
            InitializeComponent();
                        
            for (int i = 0; i < signatureNames.Length; i++)
            {
                ComboBoxItem cboxitem = new ComboBoxItem();
                cboxitem.Content = signatureNames[i];
                ComboBoxSignatureType.Items.Add(cboxitem);
            }
        }

        private void onBtnFileSelect(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Show open file dialog box, on which user can select a file
            Nullable<bool> result = dlg.ShowDialog();

            // If valid file was chosen
            if (result == true)
            {
                // Show selected path and file name in corresponding TextBox
                string filename = dlg.FileName;
                TextBoxFilePath.Text = filename;
                ComboBoxSignatureType.SelectedIndex = 0; // MD5 as initial signature type selection
            }
        }

        private void onCBSignatureType(object sender, SelectionChangedEventArgs e)
        {
            byte[] bytes, chkSum;
            string cbiText, bytesAsString = "";

            // Get selected item as string
            ComboBoxItem cbi = ((sender as ComboBox).SelectedItem as ComboBoxItem);
            cbiText = cbi.Content.ToString();
            
            if (TextBoxFilePath.Text == "") return;

            bytes = readFile(TextBoxFilePath.Text);
            if ( bytes != null )
            {
                // Depending on selected item (string), decide signature to be calculated
                if ( cbiText.Equals(signatureNames[(int) signatureIndex.MD5]) )
                {
                    chkSum = computeMd5(bytes);
                    bytesAsString = byte2hexString(chkSum, 16);
                }
                else if ( cbiText.Equals(signatureNames[(int) signatureIndex.SHA_1]) )
                {
                    chkSum = computeSha1(bytes);
                    bytesAsString = byte2hexString(chkSum, 20); // 20 Bytes
                }
                else if ( cbiText.Equals(signatureNames[(int) signatureIndex.SHA_256]) )
                {
                    chkSum = computeSha256(bytes);
                    bytesAsString = byte2hexString(chkSum, 32); // 32 Bytes
                }
                else if ( cbiText.Equals(signatureNames[(int) signatureIndex.SHA_384]) )
                {
                    chkSum = computeSha384(bytes);
                    bytesAsString = byte2hexString(chkSum, 48); // 48 Bytes
                }
                else if ( cbiText.Equals(signatureNames[(int)signatureIndex.SHA_512]) )
                {
                    chkSum = computeSha512(bytes);
                    bytesAsString = byte2hexString(chkSum, 64); // 64 Bytes
                }
                else
                {
                    ; // selection not implemented
                }
                TextBoxSignature.Text = bytesAsString;
                System.Windows.Clipboard.SetText(bytesAsString);
            }
        }

        ////////////////////////////////////////////////////////////
        // Returns all the bytes in file
        ////////////////////////////////////////////////////////////
        public byte[] readFile(string file)
        {
            byte[] bytes = null;

            try
            {
                using (FileStream fsSource = new FileStream(file,
                    FileMode.Open, FileAccess.Read))
                {
                    // Read the source file into a byte array.
                    bytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Console.WriteLine(ioEx.Message);
            }

            return bytes;
        }

        ////////////////////////////////////////////////////////////
        // Returns the MD5 checksum of the dataBytes
        ////////////////////////////////////////////////////////////
        public byte[] computeMd5(byte[] dataBytes)
        {
            byte[] result;
            MD5 md5;

            md5 = new MD5CryptoServiceProvider();
            result = md5.ComputeHash(dataBytes);

            return result;

            // can be written in one line like this
            //return new MD5CryptoServiceProvider().ComputeHash(dataBytes);
        }
        ////////////////////////////////////////////////////////////
        // Returns the SHA-1 checksum of the dataBytes
        ////////////////////////////////////////////////////////////
        byte[] computeSha1(byte[] dataBytes)
        {
            byte[] result;
            SHA1 sha1;

            sha1 = new SHA1CryptoServiceProvider();
            result = sha1.ComputeHash(dataBytes);

            return result;
        }

        ////////////////////////////////////////////////////////////
        // Returns the SHA-256 checksum of the dataBytes
        ////////////////////////////////////////////////////////////
        byte[] computeSha256(byte[] dataBytes)
        {
            byte[] result;
            SHA256 sha256;

            sha256 = new SHA256Managed();
            result = sha256.ComputeHash(dataBytes);

            return result;
        }

        ////////////////////////////////////////////////////////////
        // Returns the SHA-384 checksum of the dataBytes
        ////////////////////////////////////////////////////////////
        byte[] computeSha384(byte[] dataBytes)
        {
            byte[] result;
            SHA384 sha384;

            sha384 = new SHA384Managed();
            result = sha384.ComputeHash(dataBytes);

            return result;
        }

        ////////////////////////////////////////////////////////////
        // Returns the SHA-512 checksum of the dataBytes
        ////////////////////////////////////////////////////////////
        byte[] computeSha512(byte[] dataBytes)
        {
            byte[] result;
            SHA512 sha512;

            sha512 = new SHA512Managed();
            result = sha512.ComputeHash(dataBytes);

            return result;
        }

        ////////////////////////////////////////////////////////////
        // Returns the 'num' bytes as string buffer
        ////////////////////////////////////////////////////////////
        public string byte2hexString(byte[] bytes, uint num)
        {
            string bytesAsString = "";
            for (int i = 0; i < num; i++)
            {
                string str = String.Format("{0:X2}", bytes[i]); ;
                bytesAsString += str;
            }
            return bytesAsString;
        }
    }
}
