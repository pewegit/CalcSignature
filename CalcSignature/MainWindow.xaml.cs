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
                        
            // Fill ComboBoxSignatureType with items
            for (int i = 0; i < signatureNames.Length; i++)
            {
                ComboBoxItem cboxitem = new ComboBoxItem();
                cboxitem.Content = signatureNames[i];
                ComboBoxSignatureType.Items.Add(cboxitem);
            }

            // Prepare two different background images
            myBrush1 = new ImageBrush();
            myBrush2 = new ImageBrush();

            // If it exists, load background picture 1 from sub-folder of "current process directory"\pic\Background1.jpg
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "pic", "Background1.jpg");
            if ( File.Exists(path) )
            {
                var uri = new Uri(path);
                myBrush1.ImageSource = new BitmapImage(uri);
                // Following, if picture is a resource in the program (added to project):
                // = new BitmapImage(new Uri("pack://application:,,,/DSCN3955_25.jpg", UriKind.Absolute));
                useBackground1 = true;
            }

            // If it exists, load background picture 2 from sub-folder of "current process directory"\pic\Background2.jpg
            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "pic", "Background2.jpg");
            if ( File.Exists(path) )
            {
                var uri = new Uri(path);
                myBrush2.ImageSource = new BitmapImage(uri);
                // Following, if picture is a resource in the program (added to project):    
                // = new BitmapImage(new Uri("pack://application:,,,/DSCN3958_25.jpg", UriKind.Absolute));
                useBackground2 = true;
            }

            if ( useBackground1 )
            {
                this.Background = myBrush1;
            }
            else
            {
                this.Background = myBrush2;
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
            string cbiText, bytesAsString = "";

            iSelectedBrush ^= 1; // On each type change, toggle variable between 0 and 1
            if (iSelectedBrush == 1)
            {
                if ( useBackground1 )
                {
                    this.Background = myBrush1;
                }
            }
            else
            {
                if ( useBackground2 )
                {
                    this.Background = myBrush2;
                }
            }

            // Get selected item as string
            ComboBoxItem cbi = ((sender as ComboBox).SelectedItem as ComboBoxItem);
            cbiText = cbi.Content.ToString();
            
            if (TextBoxFilePath.Text == "") return;

            using (var stream = new BufferedStream(File.OpenRead(TextBoxFilePath.Text), 1200000))
            {
                //SHA256Managed sha = new SHA256Managed();
                //byte[] checksum = sha.ComputeHash(stream);
                //bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

                // Depending on selected item (string), decide signature to be calculated
                if (cbiText.Equals(signatureNames[(int)signatureIndex.MD5]))
                {
                    bytesAsString = computeMd5(stream);
                }
                else if (cbiText.Equals(signatureNames[(int)signatureIndex.SHA_1]))
                {
                    bytesAsString = computeSha1(stream);
                }
                else if (cbiText.Equals(signatureNames[(int)signatureIndex.SHA_256]))
                {
                    bytesAsString = computeSha256(stream);
                }
                else if (cbiText.Equals(signatureNames[(int)signatureIndex.SHA_384]))
                {
                    bytesAsString = computeSha384(stream);
                }
                else if (cbiText.Equals(signatureNames[(int)signatureIndex.SHA_512]))
                {
                    bytesAsString = computeSha512(stream);
                }
                else
                {
                    ; // selection not implemented
                }
            }
            TextBoxSignature.Text = bytesAsString;
            System.Windows.Clipboard.SetText(bytesAsString);
        }

        public string computeMd5(BufferedStream stream)
        {
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            byte[] checksum = md5.ComputeHash(stream);
            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

            return bytesAsString;

            // can be written in one line like this
            //new MD5CryptoServiceProvider().ComputeHash(dataBytes);
        }

        public string computeSha1(BufferedStream stream)
        {
            SHA1 sha1;
            sha1 = new SHA1CryptoServiceProvider();
            byte[] checksum = sha1.ComputeHash(stream);
            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

            return bytesAsString;
        }

        public string computeSha256(BufferedStream stream)
        {
            SHA256 sha256;
            sha256 = new SHA256Managed();
            byte[] checksum = sha256.ComputeHash(stream);
            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

            return bytesAsString;
        }

        public string computeSha384(BufferedStream stream)
        {
            SHA384 sha384;
            sha384 = new SHA384Managed();
            byte[] checksum = sha384.ComputeHash(stream);
            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

            return bytesAsString;
        }

        public string computeSha512(BufferedStream stream)
        {
            SHA512 sha512;
            sha512 = new SHA512Managed();
            byte[] checksum = sha512.ComputeHash(stream);
            string bytesAsString = BitConverter.ToString(checksum).Replace("-", String.Empty);

            return bytesAsString;
        }
    }
}
