using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using Microsoft.Win32;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.IO;


namespace SaveMoney
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int lockCounter = 3;
        string previousName = "";
        SendMessage sendMessage = new SendMessage();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Username = UserName.Text;

            // Duomenu baze 

            //using (var db = new UsersInfoDbContext())
            //{
            //    db.userDatas.Add(new UserData { fakeNameLog = "JonJon", firstName = "Jonas", lastName = "Jonik", lockInfo = false, number = "864844038", userPassword = "100", id = 1 });
            //    db.SaveChanges();
            //}
        }
        // Tikrina ar kas nors buvo ivesta i slaptazodzio langeli
        private void UserPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string Userpassword = UserPassword.Password;
            Login.IsEnabled = true;
            if (Userpassword == string.Empty)
            {
                Login.IsEnabled = false;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new UsersInfoDbContext())
            {
              bool noUser = true;
              int UsersCounter = db.userDatas.Max(a=>a.id);
              int UsersCounterControl = 0;
              foreach (var item in db.userDatas)
              {
                if (item.firstName == UserName.Text && item.firstName == "Admin")
                {
                        // admino patikra ir jungimasis
                    string checkPassword = ReadFromUSB();
                    if (checkPassword == "No")
                    {
                        MessageBox.Show("Key was not found!");
                    }
                    else
                    {
                        if (checkPassword != "898989891919" || UserPassword.Password != db.userDatas.Where(a=>a.firstName == "Admin").Select(a=>a.userPassword).FirstOrDefault() ) // reikes linq gauti admino slaptazodi
                        {
                            MessageBox.Show("Wrong password or key!");
                        }
                        else
                        {
                                //MessageBox.Show("Log"); // naujas langas pas admin
                                AdminWindow adminWindow = new AdminWindow();
                                //this.Visibility = Visibility.Hidden;
                                adminWindow.Show();
                                this.Close();
                        }
                    }
                }
                else if (item.fakeNameLog == UserName.Text && item.userPassword == UserPassword.Password && item.lockInfo == false)
                {
                        //prisijungimas prie sistemos vartotojo
                        // MainWindowForAll.Navigate(new UserPage());
                        //UserData userDataWindow = new UserData();

                        WindowForUser windowForUser = new WindowForUser();
                        windowForUser.getAllData(item.id);
                        //this.Visibility = Visibility.Hidden;
                        windowForUser.Show();
                        this.Close();
                    }
                else if (item.fakeNameLog == UserName.Text && item.lockInfo == true)
                {
                        // tikrina ar vartotojas uzrakintas
                        MessageBox.Show("Account locked!");
                }
                else if (item.fakeNameLog == UserName.Text && UserName.Text == previousName && item.userPassword != UserPassword.Password)
                {
                        lockCounter--;
                        MessageBox.Show($"Wrong password");
                        // kai lockCounter tampa 0 tada vartotojas tampa uzrakintas
                        if (lockCounter == 0)
                        {
                            item.lockInfo = true;
                            MessageBox.Show("Your account is locked Please wait administration call!");
                            sendMessage.NotificationsForAdmin();
                            //db.SaveChanges();
                            lockCounter = 0;
                            // taip pat butu gerai sms zinute admin del vartotojo uzrakinimo
                        }   
                }
                    else if (item.fakeNameLog == UserName.Text && item.fakeNameLog != previousName && item.userPassword != UserPassword.Password)
                    {
                        // tikrina ar atsirado naujas vartotojo vardas, jei taip, tada galima kartoti vel tris kartus
                        lockCounter = 3;
                    }
                else if (item.fakeNameLog != UserName.Text)
                {
                    UsersCounterControl++;
                    if (UsersCounterControl == UsersCounter)
                    {
                        noUser = false;
                        UsersCounterControl = 0;
                    }
                }
                 if (noUser == false)
                 {
                    MessageBox.Show($"There is no user with this name:{UserName.Text}");
                    lockCounter = 3; // pridetas kad kartoti 
                    noUser = true;
                 }
              }// foreach
                db.SaveChanges();
                previousName = UserName.Text;
            }
            
        }
        // Tikrina ar yra raktas, tam kad prisijungti prie Admin paskyros
        public string ReadFromUSB()
        {
            string valueToReturn = " ";
            string codeKey = " ";
            string path = @"E:\KeyToConnect.txt";
            try
            {
                codeKey = File.ReadAllText(path);
                valueToReturn = codeKey;
            }
            catch (Exception e)
            {
                valueToReturn = "No";
            }
            //if (!System.IO.File.Exists(path))
            //{
            //    MessageBox.Show("Admin key not found!");
            //    return;
            //}
            return valueToReturn;
        }
    }
}
