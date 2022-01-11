using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;


namespace CRYptLATER
{

    public partial class mainWindow : Form
    {

        //global variables
        int indexDeEncryption;
        int shifftingRounds = 1;
        bool statusPlayPause = true;
        
        //Decryption/Encryption status variables
        enum DeEncryption
        {
            textToText,
            toHex,
            fromHex,
            toBase64,
            fromBase64,
            toShift,
            fromShift,
            decryption,
            encryption
        }



        //Menu initialization and Methods
        private void initializeTreeView()
        {

            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                rootNode.ImageIndex = 0;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }

        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;

            foreach (DirectoryInfo subDir in subDirs)
            {

                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";

                try //to catch the exceptions thrown by accessing read and write protected directories/files
                {
                    subSubDirs = subDir.GetDirectories();

                    if (subSubDirs.Length != 0 && subSubDirs.ToString()[0] != '.')
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);

                    foreach (var file in subDir.GetFiles())
                    {
                        aNode.Nodes.Add(new TreeNode(file.Name) { ImageIndex = 1 });
                    }
                }
                catch (Exception e) { }

            }

        }

        private void setStatusButtonPlayPause()
        {
            if (statusPlayPause) playPauseButton.BackColor = Color.Green;
            else playPauseButton.BackColor = Color.Red;
        }

        private void resetColorDeEncryptionButton()
        {
            toHexButton.BackColor = Color.Beige;
            FromHexButton.BackColor = Color.Beige;
            toBase64Button.BackColor = Color.Beige;
            fromBase64Button.BackColor = Color.Beige;
            toShiftbutton.BackColor = Color.Beige;
            fromShiftButton.BackColor = Color.Beige;
            toTextButton.BackColor = Color.Beige;
            encryptionButton.BackColor = Color.Beige;
            decryptionButton.BackColor = Color.Beige;

        }

        public mainWindow()
        {
            InitializeComponent();
            initializeTreeView();
            setStatusButtonPlayPause();
            resetColorDeEncryptionButton();
            toTextButton.BackColor = Color.Blue;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength != 0)
            {
                if(statusPlayPause)DeEncrypt();
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string filepathTreeNoderelative = e.Node.FullPath;
            string fileExtension = Path.GetExtension(filepathTreeNoderelative);
            string filePathUserDir = "C:\\Users\\";

            string fullPath = filePathUserDir + filepathTreeNoderelative;

            if (fileExtension.Equals(".txt"))
            {
                Console.WriteLine($"Filepath: {filepathTreeNoderelative}");
                Console.WriteLine($"Filepath: {filePathUserDir}");
                Console.WriteLine($"Filepath: {fullPath}");

                try
                {
                    textBox1.Text = File.ReadAllText(fullPath);
                }catch(Exception error)
                {
                    //ErrorMessage
                }
            }

        }

        private void increaseTextSize1(object sender, EventArgs e)
        {
            textBox1.Font = new System.Drawing.Font("Arial", textBox1.Font.SizeInPoints + 1);
            textBox2.Font = new System.Drawing.Font("Arial", textBox2.Font.SizeInPoints + 1);
        }

        private void decreaseTextSize1(object sender, EventArgs e)
        {
            textBox1.Font = new System.Drawing.Font("Arial", textBox1.Font.SizeInPoints - 1);
            textBox2.Font = new System.Drawing.Font("Arial", textBox2.Font.SizeInPoints - 1);
        }

        private void openButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(openFileDialog1.FileName);
                
            }
        }

        private void saveButton1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
                }
            }
        }

        private void saveButton2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName, textBox2.Text);
                }
            }
        }

        private void boldButton1_Click(object sender, EventArgs e)
        {
            if (textBox1.Font.Bold)
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
                textBox2.Font = new Font(textBox2.Font, FontStyle.Regular);
            }
            else
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Bold);
                textBox2.Font = new Font(textBox2.Font, FontStyle.Bold);
            }

        }

        private void Menu_Copy(System.Object sender, System.EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
                textBox1.Copy();
        }

        private void Menu_Cut(System.Object sender, System.EventArgs e)
        {
            if (textBox1.SelectedText != "")
                textBox1.Cut();
        }

        private void Menu_Paste(System.Object sender, System.EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                if (textBox1.SelectionLength > 0)
                {
                    if (MessageBox.Show("Do you want to paste over current selection?", "Cut Example", MessageBoxButtons.YesNo) == DialogResult.No)
                        textBox1.SelectionStart = textBox1.SelectionStart + textBox1.SelectionLength;
                }
                textBox1.Paste();
            }
        }

        private void clipButton1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Copy();
            MessageBox.Show("Content of upper textbox has been copied to clipboard");
        }

        private void Menu_Undo1(System.Object sender, System.EventArgs e)
        {
            if (textBox1.CanUndo == true)
            {
                // Undo the last operation.
                textBox1.Undo();
                // Clear the undo buffer to prevent last action from being redone.
                textBox1.ClearUndo();
            }
        }

        private void Menu_Copy2(System.Object sender, System.EventArgs e)
        {
            if (textBox2.SelectionLength > 0)
                textBox2.Copy();
        }

        private void Menu_Cut2(System.Object sender, System.EventArgs e)
        {
            if (textBox2.SelectedText != "")
                textBox2.Cut();
        }
        private void shifftingRoundsPlusButton_Click(object sender, EventArgs e)
        {

            shifftingRounds++;
            roundCounter.Text = shifftingRounds.ToString();

        }
        private void shifftingRoundsMinusButton_Click(object sender, EventArgs e)
        {
            if (shifftingRounds > 1) shifftingRounds--;
            roundCounter.Text = shifftingRounds.ToString();
        }

        private void playPauseButton_Click(object sender, EventArgs e)
        {
            statusPlayPause = !statusPlayPause;
            setStatusButtonPlayPause();
        }


        private void Menu_Paste2(System.Object sender, System.EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                if (textBox2.SelectionLength > 0)
                {
                    if (MessageBox.Show("Do you want to paste over current selection?", "Cut Example", MessageBoxButtons.YesNo) == DialogResult.No)
                        textBox2.SelectionStart = textBox2.SelectionStart + textBox2.SelectionLength;
                }
                textBox2.Paste();
            }
        }

        private void clipButton2_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
            textBox2.Copy();
            MessageBox.Show("Content of lower textbox has been copied to clipboard");
        }


        private void button_DeEncryption_Click(object sender, EventArgs e)
        {
            bool buttonPushed = sender.Equals(this.toHexButton);
            Console.WriteLine($"Gedrueckter Button: {buttonPushed}");
            resetColorDeEncryptionButton();
            if (sender.Equals(this.toHexButton))
            {
                indexDeEncryption = (int)DeEncryption.toHex;
                toHexButton.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.FromHexButton))
            {
                indexDeEncryption = (int)DeEncryption.fromHex;
                FromHexButton.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.toBase64Button))
            {
                indexDeEncryption = (int)DeEncryption.toBase64;
                toBase64Button.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.fromBase64Button))
            {
                indexDeEncryption = (int)DeEncryption.fromBase64;
                fromBase64Button.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.toShiftbutton))
            {
                indexDeEncryption = (int)DeEncryption.toShift;
                toShiftbutton.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.fromShiftButton))
            {
                indexDeEncryption = (int)DeEncryption.fromShift;
                fromShiftButton.BackColor = Color.Blue;
            }
            else if (sender.Equals(this.toTextButton))
            {
                indexDeEncryption = -1;
                toTextButton.BackColor = Color.Blue;

            }
            else if (sender.Equals(this.decryptionButton))
            {
                indexDeEncryption = (int)DeEncryption.decryption;
                toTextButton.BackColor = Color.Blue;

            }
            else if (sender.Equals(this.encryptionButton))
            {
                indexDeEncryption = (int)DeEncryption.encryption;
                toTextButton.BackColor = Color.Blue;

            }

            DeEncrypt();
        }

        public void DeEncrypt()
        {
            try
            {
                switch (indexDeEncryption)
                {
                    case (int)DeEncryption.toBase64:
                        //ToBase64
                        textBox2.Text = Base64Encode(textBox1.Text);
                        break;

                    case (int)DeEncryption.fromBase64:
                        //FromBase64
                        textBox2.Text = Base64Decode(textBox1.Text);
                        break;

                    case (int)DeEncryption.toHex:
                        //ToHex
                        textBox2.Text = HexEncode(textBox1.Text);
                        break;

                    case (int)DeEncryption.fromHex:
                        //FromHex
                        textBox2.Text = HexDecode(textBox1.Text);
                        break;

                    case (int)DeEncryption.toShift:
                        //FromHex
                        textBox2.Text = ShiftEncode(textBox1.Text, shifftingRounds);
                        break;

                    case (int)DeEncryption.fromShift:
                        //FromHex
                        textBox2.Text = ShiftDecode(textBox1.Text, shifftingRounds);
                        break;

                    case (int)DeEncryption.decryption:
                        if (passwordTextBox.Text == "") break;

                        textBox2.Text = StringCipher.Decrypt(textBox1.Text.ToString(), passwordTextBox.Text.ToString());
                        break;

                    case (int)DeEncryption.encryption:

                        if (passwordTextBox.Text == "") break;
                        string text = textBox1.Text;
                        string password = passwordTextBox.Text;
                        textBox2.Text = StringCipher.Encrypt(text, password);
                        break;

                    default:
                        //no Encoding/Encryption chosen
                        textBox2.Text = textBox1.Text;
                        break;
                }
            }catch(Exception error)
            {
                MessageBox.Show("Choose another algorithm or password");
            }

        }


        //BASE64

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //HEXADECIMAL

        public static string HexEncode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            string hexString = BitConverter.ToString(plainTextBytes);
            return hexString.Replace("-", "");
        }

        public static string HexDecode(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes);
        }

        //Shiffting Methods

        public static string ShiftEncode(string plainText, int rounds)
        {
            string lowerplaintText = plainText.ToLower();
            var sb = new StringBuilder();
            char[] alphabet = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            

            foreach (char Charakter in lowerplaintText)
            {
                for (int i = 0; i < 26; i++)
                {
                    if (Charakter == alphabet[i])
                    {
                        if (i != 25) sb.Append(alphabet[i + 1]);
                        else sb.Append(alphabet[0]);
                    }
                }
            }

            if (rounds == 1)
            {
                return sb.ToString();
            }

            return ShiftEncode(sb.ToString(), rounds - 1);

        }

        public static string ShiftDecode(string shifftedString, int rounds)
        {
            var sb = new StringBuilder();
            char[] alphabet = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            foreach (char Charakter in shifftedString)
            {
                for (int i = 0; i < 26; i++)
                {
                    if (Charakter == alphabet[i])
                    {
                        if (i != 0)
                        {
                            sb.Append(alphabet[i - 1]);
                        }
                        else
                        {
                            sb.Append(alphabet[25]);
                        }    
                    }
                }
            }

            if (rounds == 1)
            {
                return sb.ToString();
            }
            return ShiftDecode(sb.ToString(), rounds - 1);
        }

        //link: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

    }

}
