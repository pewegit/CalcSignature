using System;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow()
        {
            InitializeComponent();

            // Fill ComboBoxSignatureType with items
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
            string cbiText, bytesAsString = "";
            
            // Get selected item as string
            ComboBoxItem cbi = ((sender as ComboBox).SelectedItem as ComboBoxItem);
            cbiText = cbi.Content.ToString();
            
            if (TextBoxFilePath.Text == "") return;

            using (var stream = new BufferedStream(File.OpenRead(TextBoxFilePath.Text), 1200000))
            {
                bytesAsString = checksum(stream, cbiText);
            }
            TextBoxSignature.Text = bytesAsString;
            System.Windows.Clipboard.SetDataObject(bytesAsString);
        }

        public string checksum(BufferedStream stream, string signatureName)
        {
            byte[] checksum = { 0 };

            switch (signatureName)
            {
                case "MD5":
                    MD5 md5;
                    md5 = new MD5CryptoServiceProvider();
                    checksum = md5.ComputeHash(stream);
                    break;
                case "SHA-1":
                    SHA1 sha1;
                    sha1 = new SHA1CryptoServiceProvider();
                    checksum = sha1.ComputeHash(stream);
                    break;
                case "SHA-256":
                    SHA256 sha256;
                    sha256 = new SHA256Managed();
                    checksum = sha256.ComputeHash(stream);
                    break;
                case "SHA-384":
                    SHA384 sha384;
                    sha384 = new SHA384Managed();
                    checksum = sha384.ComputeHash(stream);
                    break;
                case "SHA-512":
                    SHA512 sha512;
                    sha512 = new SHA512Managed();
                    checksum = sha512.ComputeHash(stream);
                    break;
                default:
                    ; // invalid
                    break;
            }

            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);
            return bytesAsString;
        }
    }
}
