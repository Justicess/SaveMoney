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
using System.Windows.Shapes;

namespace SaveMoney
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public UsersInfoDbContext dcs = new UsersInfoDbContext(); // globali baze  
        List<User> users = new List<User>();
        int userIdForchanges;
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsersData();
        }

        private void Unlock_Click(object sender, RoutedEventArgs e)
        {//update user mygtukas 
            if ("False" == LockStatusBox.Text  || "True" == LockStatusBox.Text )
            {
                if (long.TryParse(UserPhoneBox.Text, out long number))
                {
                    if ( number > 999999999)
                    { //Convert.ToInt64(UserPhoneBox.Text)
                        MessageBox.Show("Phone number can't be longer than 9 digits");
                    }
                    else
                    {
                        foreach (var item in dcs.userDatas)
                        {
                            if (item.id == userIdForchanges)
                            {
                                item.fakeNameLog = LogNameBox.Text;
                                item.firstName = FirstNameBox.Text;
                                item.lastName = LastNameBox.Text;
                                item.userPassword = PasswordBox.Text;
                                item.lockInfo = Convert.ToBoolean(LockStatusBox.Text);
                                item.number = UserPhoneBox.Text;
                            }
                        }
                        dcs.SaveChanges();
                        Unlock.IsEnabled = false;
                        ClearForUpdates();
                        users.Clear();
                        showUsersData.ItemsSource = null;
                        LoadUsersData();
                    }
                }
                else
                {
                    MessageBox.Show("Wrong input, please write numbers!");
                }
            }
            else
            {
                MessageBox.Show(LockStatusBox.Text);
                MessageBox.Show("Wrong input! lock status must be True or False");
            }    
        }
        public void LoadUsersData()
        {// duomenys sukeliami i lentele
            foreach (var item in dcs.userDatas)
            {
                users.Add(new User() { User_ID = item.id, logname = item.fakeNameLog, First_Name = item.firstName, Last_Name = item.lastName, LockStatus = item.lockInfo, UserPassword = item.userPassword, User_Phone = item.number});
            }
            showUsersData.ItemsSource = users;
        }

        private void showUsersData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {// gaunami vieno vartotojo duomenys 
            User st = showUsersData.SelectedItem as User;
            Unlock.IsEnabled = true;
  
            userIdForchanges = st.User_ID;
            LogNameBox.Text = st.logname;
            FirstNameBox.Text = st.First_Name;
            LastNameBox.Text = st.Last_Name;
            PasswordBox.Text = st.UserPassword;
            LockStatusBox.Text = Convert.ToString(st.LockStatus);
            UserPhoneBox.Text = st.User_Phone;
        }

        public void ClearForUpdates()
        {// langai isvalomi
            LogNameBox.Text = string.Empty;
            FirstNameBox.Text = string.Empty;
            LastNameBox.Text = string.Empty;
            PasswordBox.Text = string.Empty;
            LockStatusBox.Text = string.Empty;
            UserPhoneBox.Text = string.Empty;
        }

        private void createUser_Click(object sender, RoutedEventArgs e)
        { // pridedamas naujas vartotojas 
            int lastUserID = dcs.userDatas.Max(a=>a.id);
            MessageBox.Show(Convert.ToString(lastUserID));
            dcs.userDatas.Add(new UserData { fakeNameLog = "NewFake", firstName = "NewName", lastName = "NewLast", lockInfo = false, number = "0", userPassword = "0", id = (lastUserID + 1) });
            dcs.usersavings.Add(new UserSavings { salary = 0, foodCosts = 0, fuelCosts = 0, gadgetsCosts = 0, savings = 0, tripsCosts = 0, id_Savings = (lastUserID +1) });
            dcs.logChecks.Add(new LogCheck { logData = 0, LogData_id = (lastUserID+1)}); ;
            dcs.SaveChanges();
            users.Clear();
            showUsersData.ItemsSource = null;
            LoadUsersData();
        }

        private void deleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(deleteUserID.Text, out int number))
            {
                bool userNotFound = false;
                foreach (var item in dcs.userDatas)
                {
                    if (item.id == number )
                    {
                        userNotFound = true;
                    }
                   
                }
                if (userNotFound == false)
                {
                    MessageBox.Show($"User with ID: {deleteUserID.Text} was not found!");
                }
                else
                {
                    if (number == 6)
                    {
                        MessageBox.Show("You can't delete Admin!");
                    }
                    else
                    {
                        //trina vartotoja
                        DeleteUserFromDB(number);
                    }
                }
            }
            else
            {
                MessageBox.Show("Wrong input, please write numbers!");
            }
        }
        public void DeleteUserFromDB(int number)
        {
            UserData userDatasdelete = dcs.userDatas.Find(number);
            UserSavings userSavingsdelete = dcs.usersavings.Find(number);
            LogCheck logCheckdelete = dcs.logChecks.Find(number);
            dcs.userDatas.Remove(userDatasdelete);
            dcs.usersavings.Remove(userSavingsdelete);
            dcs.logChecks.Remove(logCheckdelete);
            dcs.SaveChanges();
            users.Clear();
            showUsersData.ItemsSource = null;
            LoadUsersData();
        }
    }
}
