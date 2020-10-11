using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Linq.Dynamic;
using System.Collections;
using System.Configuration;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace SaveMoney
{
    /// <summary>
    /// Interaction logic for WindowForUser.xaml
    /// </summary>
    public partial class WindowForUser : Window
    {
        //public Func<double, string> Formatter { get; set; }
        public UsersInfoDbContext dcs = new UsersInfoDbContext(); // globali baze ???? 

        int gotUserName;

        TimeClass dayToDay = new TimeClass(); // laiko klase ar geriau kas nors kito ? 
        
        private ObservableValue Value1;
        private ObservableValue Value2;
        private ObservableValue Value3;
        private ObservableValue Value4;
       
        public WindowForUser()
        {
            InitializeComponent();

            Value1 = new ObservableValue(0);
            Value2 = new ObservableValue(0);
            Value3 = new ObservableValue(0);
            Value4 = new ObservableValue(0);

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {

                    Title = Convert.ToString(dayToDay.GetYear()), 
                    Values = new ChartValues<ObservableValue> { Value1, Value2, Value3, Value4}
                }
            };
            
            //   Formatter = val=>val.ToString('');
            Labels = new[] { "Fuel", "Food", "Trips", "Gadgets" };
            DataContext = this;
        }
        
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<ObservableValue, string> Formatter { get; set; }
        
        public void getAllData(int userLoginName)
        {// gauti duomenys is pirmos formos. Ar tai geras budas? 
            gotUserName = userLoginName;
            int monthNow = dayToDay.GetTodaysDay();
          //int monthPreviuos = Convert.ToInt32(ConfigurationManager.AppSettings.Get("checkTime"));// blogas sprendimas, visi users tures atnaujinta menesi.
          // Tikrina ar naujas menuo 
            if (dcs.logChecks.Where(a=>a.LogData_id == gotUserName).Select(a=>a.logData).FirstOrDefault() == monthNow)
            {
                AddDataToChartFromDb();
            }
            else
            {
               // Visos lenteles atnaujinamos i nulius. logData verte tampa dabartiniu menesiu. Sekanti menesi ji bus pasenusi   
               ClearAllTablesNewMonth();
               Submit.IsEnabled = false;
               Salary.IsEnabled = true;
                foreach (var item in dcs.logChecks)
                {
                    if (item.LogData_id == gotUserName)
                    {
                        item.logData = monthNow;
                    }
                }
                dcs.SaveChanges();
            }
        }

        private void Salary_Click(object sender, RoutedEventArgs e)
        {// tikrina ar skaicius po teisingo ivedimo, mygtukas uzsirakina

            if (Double.TryParse(Testas.Text, out double number))
            {
                AddSalaryTodb(number);
                Salary.IsEnabled = false;
                Submit.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Wrong input, please write numbers!");
            }
        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        public void AddDataToChartFromDb()
        {// prideda duomenis i grafika. Matomos vartotojo islaidos
            using (var db = new UsersInfoDbContext())
            {
                Value1.Value = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.fuelCosts).FirstOrDefault(); //fuel
                //Value2.Value = db.usersavings.Where(a => a.UserData.id == gotUserName).Select(a => a.foodCosts).FirstOrDefault(); //food // galiu vel susisiekti su lentele per rakta? 
                Value2.Value = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.foodCosts).FirstOrDefault(); //food // galiu vel susisiekti su lentele per rakta? 
                Value3.Value = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.tripsCosts).FirstOrDefault(); //trips
                Value4.Value = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.gadgetsCosts).FirstOrDefault(); //gadgets
                double userSalaryCheck = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.salary).FirstOrDefault(); // salary
                
               // userSalaryCheck = userSalaryCheck - Value1.Value - Value2.Value - Value3.Value - Value4.Value; nereikia
                foreach (var item in db.usersavings)
                {
                    if (item.id_Savings == gotUserName)
                    {
                        item.salary = userSalaryCheck;
                    }
                }
                salaryLeft.Content = Convert.ToString(userSalaryCheck);
                totalSavings.Content = Convert.ToString(db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.savings).FirstOrDefault());
                
                db.SaveChanges();
            }
        }
        // nauja mygtuko vieta 
        private void Submit_Click(object sender, RoutedEventArgs e)
        {// pridedamos vertes i grafika 
            if (Double.TryParse(CostsMinus.Text, out double number))
            {
                if (comboBox.Text == "Fuel")
                {
                    Value1.Value = Value1.Value + number;
                    //string ads = "Food";
                    //string kasnors = ts(ads);
                    UpdateTable(Value1.Value, comboBox.Text);
                    updateSalaryOrSavings(number);
                }
                else if (comboBox.Text == "Food")
                {
                    Value2.Value = Value2.Value + number;
                    UpdateTable(Value2.Value, comboBox.Text);
                    updateSalaryOrSavings(number);
                }
                else if (comboBox.Text == "Trips")
                {
                    Value3.Value = Value3.Value + number;
                    UpdateTable(Value3.Value, comboBox.Text);
                    updateSalaryOrSavings(number);
                }
                else if (comboBox.Text == "Gadgets")
                {
                    Value4.Value = Value4.Value + number;
                    UpdateTable(Value4.Value, comboBox.Text);
                    updateSalaryOrSavings(number);
                }
            }
            else
            {
                MessageBox.Show("Wrong input, please write numbers!");
            }
        }

        public void AddSalaryTodb(double numberFromParse)
        { // prideda alga. Galima tik su viena db  
            using (var db = new UsersInfoDbContext())
            {
                foreach (var item in db.usersavings)
                {
                    if (item.id_Savings == gotUserName)
                    {
                        item.salary = numberFromParse;
                    }
                }
                db.SaveChanges();
            }
        }
        //public IEnumerable ts (string colName) 
        //{
        //    string asd = "Fuel";
        //    List<string> asdas = new List<string>();
        //    var querys = dcs.usersavings.Where($"{asd}");          
        //    return Convert.ToString(querys);
        //}

        public void UpdateTable(double upDateValue, string whichToChoose)
        { // atnaujina grafikus pridedamos islaidos 
            using (var db = new UsersInfoDbContext())
            {
                if (whichToChoose == "Fuel")
                {
                    foreach (var item in db.usersavings)
                    {
                        if (item.id_Savings == gotUserName)
                        {
                            item.fuelCosts = upDateValue;
                        }
                    }
                }
                else if (whichToChoose == "Food")
                {
                    foreach (var item in db.usersavings)
                    {
                        if (item.id_Savings == gotUserName)
                        {
                            item.foodCosts = upDateValue;
                        }
                    }
                }
                else if (whichToChoose == "Trips")
                {
                    foreach (var item in db.usersavings)
                    {
                        if (item.id_Savings == gotUserName)
                        {
                            item.tripsCosts = upDateValue;
                        }
                    }
                }
                else if (whichToChoose == "Gadgets")
                {
                    foreach (var item in db.usersavings)
                    {
                        if (item.id_Savings == gotUserName)
                        {
                            item.gadgetsCosts = upDateValue;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public void updateSalaryOrSavings(double upDateValue)
        {//Atnaujinamos tik salary arba savings lenteles
            using (var db = new UsersInfoDbContext())
            {
                double salaryToChechIfNotEqualsZero = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.salary).FirstOrDefault();
                double userSavings = db.usersavings.Where(a => a.id_Savings == gotUserName).Select(a => a.savings).FirstOrDefault();
                double checkHowMuchLeft = salaryToChechIfNotEqualsZero - upDateValue;

                //MessageBox.Show(Convert.ToString(salaryToChechIfNotEqualsZero));
                foreach (var item in db.usersavings)
                {
                    if (item.id_Savings == gotUserName)
                    {
                        if (checkHowMuchLeft <= 0)
                        {
                            item.salary = 0;
                            salaryLeft.Content = Convert.ToString(item.salary);
                            userSavings = userSavings + checkHowMuchLeft;
                            item.savings = userSavings;
                            totalSavings.Content = Convert.ToString(item.savings);
                        }
                        else
                        {
                            salaryToChechIfNotEqualsZero = salaryToChechIfNotEqualsZero - upDateValue;
                            item.salary = salaryToChechIfNotEqualsZero;
                            salaryLeft.Content = Convert.ToString(item.salary);
                        }
                        //if (salaryToChechIfNotEqualsZero >= 0)
                        //{
                        //    // salaryToChechIfNotEqualsZero = salaryToChechIfNotEqualsZero - upDateValue;
                        //    // item.salary = salaryToChechIfNotEqualsZero;
                        //    salaryToChechIfNotEqualsZero = salaryToChechIfNotEqualsZero - upDateValue;
                        //    item.salary = salaryToChechIfNotEqualsZero;
                        //    salaryLeft.Content = Convert.ToString(item.salary);
                        //}
                        //else
                        //{
                        //    userSavings = userSavings - upDateValue;
                        //    item.savings = userSavings;
                        //    totalSavings.Content = Convert.ToString(item.savings);
                        //}
                    }
                }
                db.SaveChanges();
            }
        }
        public void ClearAllTablesNewMonth()
        {// naujas menuo. i visas lenteles surasomi nuliai. Savings paliekamas, kaip bendras savings sekti bendrus sutaupytus pinigus  
            Value1.Value = 0;
            Value2.Value = 0;
            Value3.Value = 0;
            Value4.Value = 0;
            double showTotalSavings = 0;
            foreach (var item in dcs.usersavings)
            {
                if (item.id_Savings == gotUserName)
                {
                    showTotalSavings = item.salary + item.savings;
                    item.savings = showTotalSavings;
                    totalSavings.Content = item.savings;
                    item.fuelCosts = 0;
                    item.foodCosts = 0;
                    item.gadgetsCosts = 0;
                    item.tripsCosts = 0;
                    item.salary = 0;
                }
            }
            dcs.SaveChanges();
        }

        private void ResetSavingsB_Click(object sender, RoutedEventArgs e)
        {// vartotojas gali nustatyi 0 totalSavings 
            foreach (var item in dcs.usersavings)
            {
                if (gotUserName == item.id_Savings)
                {
                    item.savings = 0;
                }
            }
            totalSavings.Content = 0;
            dcs.SaveChanges();
        }
    }
}
