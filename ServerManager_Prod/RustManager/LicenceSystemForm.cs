using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Management;
using System.Net.NetworkInformation;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace RustManager
{
    public partial class LicenceSystemForm : Form
    {
        public LicenceSystemForm()
        {
            InitializeComponent();
        }



        string GetUniqueID()
        {
            string cpuID = null;
            string motherboardID = null;
            string pcName = Environment.MachineName;
            string macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault();




            string MotherBoardQuery = "SELECT * FROM Win32_BaseBoard";
            ManagementObjectSearcher MotherBoardsearcher = new ManagementObjectSearcher(MotherBoardQuery);
            foreach (ManagementObject info in MotherBoardsearcher.Get())
            {
                cpuID = info.GetPropertyValue("SerialNumber").ToString();
                break;
            }

            string CPUQuery = "Select * FROM Win32_Processor";
            ManagementObjectSearcher CPUSearcher = new ManagementObjectSearcher(CPUQuery);
            foreach (ManagementObject info in CPUSearcher.Get())
            {
                motherboardID = info.GetPropertyValue("ProcessorId").ToString();
                break;
            }

            string UniqueID = pcName + cpuID + motherboardID + macAddr;
            return UniqueID;
        }


        string Encrypted_Global;
        string Decrypted_Global;
        void Encrypter(string Input, bool Encrypt)
        {
            ButtonsDisabled();
            string Encrypted = Input;
            string Decrypted = Input;
            //Encrypt
            if (Encrypt)
            {
                #region Step 1
                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Input);
                //Encrypt
                Encrypted = Convert.ToBase64String(FunctionClass.Encryption.AES.Encrypt(Encrypted));
                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);
                #endregion

                #region Step 2
                //Binary
                Encrypted = FunctionClass.Encryption.Binary.StringToBinary(Encrypted);
                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);
                //Encrypt
                Encrypted = Convert.ToBase64String(FunctionClass.Encryption.AES.Encrypt(Encrypted));
                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);
                #endregion

                #region Step 3
                //ForwardBackwards
                Encrypted = FunctionClass.Encryption.ForwardBackward.TextForwardBackwards(Encrypted);



                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);



                //Encrypt
                Encrypted = Convert.ToBase64String(FunctionClass.Encryption.AES.Encrypt(Encrypted));



                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);



                //Binary
                Encrypted = FunctionClass.Encryption.Binary.StringToBinary(Encrypted);




                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);



                //Encrypt
                Encrypted = Convert.ToBase64String(FunctionClass.Encryption.AES.Encrypt(Encrypted));


                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);



                //ForwardBackwards
                Encrypted = FunctionClass.Encryption.ForwardBackward.TextForwardBackwards(Encrypted);


                //Shuffle
                Encrypted = FunctionClass.Encryption.ShuffleEncryption.Shuffle(Encrypted);

                #endregion

                //Result
                Encrypted_Global = Encrypted;
            }
            //Decrypt
            if (!Encrypt)
            {
                #region Step 1
                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Input);

                //ForwardBackwards
                Decrypted = FunctionClass.Encryption.ForwardBackward.TextForwardBackwards(Decrypted);

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //Decrypt
                Decrypted = FunctionClass.Encryption.AES.Decrypt(Convert.FromBase64String(Decrypted));

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //DeBinary
                Decrypted = FunctionClass.Encryption.Binary.BinaryToString(Decrypted);

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //Decrypt
                Decrypted = FunctionClass.Encryption.AES.Decrypt(Convert.FromBase64String(Decrypted));

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //ForwardBackwards
                Decrypted = FunctionClass.Encryption.ForwardBackward.TextForwardBackwards(Decrypted);

                #endregion

                #region Step 2
                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //Decrypt
                Decrypted = FunctionClass.Encryption.AES.Decrypt(Convert.FromBase64String(Decrypted));

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //DeBinary
                Decrypted = FunctionClass.Encryption.Binary.BinaryToString(Decrypted);

                #endregion

                #region Step 3
                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                //Decrypt
                Decrypted = FunctionClass.Encryption.AES.Decrypt(Convert.FromBase64String(Decrypted));

                //DeShuffle
                Decrypted = FunctionClass.Encryption.ShuffleEncryption.DeShuffle(Decrypted);

                #endregion

                //Result
                Decrypted_Global = Decrypted;
            }


            ButtonsEnabled();
        }


        private void LicenceSystemForm_Load(object sender, EventArgs e)
        {
            ButtonsDisabled();
            if (EncrypterWorker.IsBusy != true) { EncrypterWorker.RunWorkerAsync();}
            CheckActivateSoftware();
        }

  
        async void CheckActivateSoftware()
        {

            label2.Text = "Validating ID";
            label2.ForeColor = Color.White;

            string pcName = Environment.MachineName;
            try
            {

                WebClient client = new WebClient();
                Stream stream = client.OpenRead($"http://hypergamer/LibKeys/{pcName}.txt");
                StreamReader reader = new StreamReader(stream);
                String WebKey = reader.ReadToEnd();



                Encrypter(WebKey, false);



                //Aproved
                if (Decrypted_Global == GetUniqueID())
                {
                    ButtonsDisabled();
                    label2.Text = "ID Approved";
                    label2.ForeColor = Color.LimeGreen;


                    #region ActivateSoftware

                    byte[] ByteToken = new byte[16];
                    ByteToken = MainForm.ByteToken;

                    MainForm.ByteToken = ByteToken;

                    MainForm NextForm = new MainForm(ByteToken);
                    this.Hide();
                    NextForm.ShowDialog();
                    this.Close();

                    #endregion
                }
 
            }
            catch
            {
                label2.Text = "ID Rejected";
                label2.ForeColor = Color.Tomato;



                for (int i = 5; i >= 0; i--)
                {
                    await Task.Delay(1000);
                    label2.Text = $"Closeing In {i}";
                    label2.ForeColor = Color.White;


                }
                this.Close();

            }


        }

        private void EncrypterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Encrypter(GetUniqueID(), true);
        }




        #region Interface
        private void customButton2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Encrypted_Global);
        }


        void ButtonsDisabled()
        {
            customButton2.Enabled = false;
        }
        void ButtonsEnabled()
        {
            customButton2.Enabled = true;
        }
        #endregion


    }
}

