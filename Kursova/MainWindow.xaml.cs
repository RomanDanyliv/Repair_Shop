using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursova.Reports;

namespace Kursova
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly RepairEntities Repair = new RepairEntities();

        /// ////////////////////////////////////////////////////
        private List<Addresses> addresses;

        private readonly Autorize Autorization;
        private List<NewAddress> br_adr;
        public Employees Chosen_one;
        private List<Customer> customers;
        private List<Employees> employees;
        private List<NewEmployeeWorks> emplworks;
        private readonly List<string> jobs = new List<string>();
        private readonly LogMethod method = new LogMethod();
        private List<NewPartsOnBranch> prts_brnch;
        private List<NewPartsOnBranch> prts_need;
        private List<Requests> requests;
        private readonly List<Spare_parts> spare;
        private readonly List<Work_Types> works;

        public MainWindow(Employees employee, Autorize form)
        {
            InitializeComponent();
            Autorization = form;
            Chosen_one = employee;
            label1.Content = "Ви зайшли як " + Chosen_one.Name + " " + Chosen_one.SurName;
            if (Chosen_one.Job != "Супер-Адмін")
            {
                if (Chosen_one.Job == "Майстер")
                    requests = Repair.Requests.ToList().Where(r => r.ID_Employee == Chosen_one.ID_Employee).ToList();
                else
                    requests = Repair.Requests.ToList().Where(r => r.ID_Branch == Chosen_one.ID_Branch).ToList();
                addresses = Repair.Addresses.ToList();
                employees = Repair.Employees.ToList().Where(e => e.ID_Branch == Chosen_one.ID_Branch).ToList();
                customers = Repair.Customer.ToList();
                spare = Repair.Spare_parts.ToList();
                works = Repair.Work_Types.ToList();
            }
            else
            {
                addresses = Repair.Addresses.ToList();
                requests = Repair.Requests.ToList();
                employees = Repair.Employees.ToList();
                customers = Repair.Customer.ToList();
                spare = Repair.Spare_parts.ToList();
                works = Repair.Work_Types.ToList();
            }
            SelectedEmployee = null;
            SelectedAddress = null;
            SelectedCustomer = null;
            SelectedRequest = null;
            SelectedSpareParts = null;
            GridAddresses.DataContext = addresses;
            GridRequests.DataContext = requests;
            GridEmployees.DataContext = employees;
            GridCustomers.DataContext = customers;
            GridSpareParts.DataContext = spare;
            GridWorkTypes.DataContext = works;
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            br_adr = new List<NewAddress>();
            foreach (var VARIABLE in Repair.Branches)
            {
                var temp = new NewAddress();
                temp.id = VARIABLE.ID_Branch;
                var adr = Repair.Addresses.ToList().Where(b => b.ID_Address == VARIABLE.ID_Address).First();
                temp.adr = adr.City + ", " + adr.Street + ", буд. " + adr.House_Num;
                if (adr.Room_Num != null)
                    temp.adr += ", квартира " + adr.Room_Num;
                temp.adr_id = adr.ID_Address;
                br_adr.Add(temp);
            }
            GridBranches.DataContext = br_adr;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            prts_need = new List<NewPartsOnBranch>();
            for (var i = 0; i < requests.Count(); i++)
            {
                var nd = requests[i].Need_Parts.ToList();
                foreach (var VARIABLE in nd)
                {
                    var temp = new NewPartsOnBranch();
                    temp.id = VARIABLE.ID_Request;
                    temp.part_Id = VARIABLE.ID_Part;
                    temp.part_Ammount = VARIABLE.Used_Parts.Value;
                    var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                    temp.part_Name = part.Name;
                    temp.part_Serial = part.Serial;
                    prts_need.Add(temp);
                }
            }
            GridNeedParts.DataContext = prts_need;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            prts_brnch = new List<NewPartsOnBranch>();
            foreach (var VARIABLE in Repair.Parts_On_Branch.OrderBy(o => o.ID_Part).OrderBy(p => p.ID_Branch))
            {
                var temp = new NewPartsOnBranch();
                if (VARIABLE.ID_Branch != Chosen_one.ID_Branch && (Chosen_one.Job != "Супер-Адмін"))
                    continue;
                temp.id = VARIABLE.ID_Branch;
                temp.part_Id = VARIABLE.ID_Part;
                var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                temp.part_Ammount = VARIABLE.Ammount.Value;
                temp.part_Name = part.Name;
                temp.part_Serial = part.Serial;
                prts_brnch.Add(temp);
            }
            GridPartsOnBranch.DataContext = prts_brnch;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            emplworks = new List<NewEmployeeWorks>();
            foreach (var VARIABLE in employees)
            {
                var temp = new NewEmployeeWorks();
                temp.id = VARIABLE.ID_Employee;
                var emp = VARIABLE;
                for (var i = 0; i < emp.Work_Types.Count(); i++)
                {
                    temp.name += emp.Work_Types.ToList()[i].Type;
                    temp.name_id += emp.Work_Types.ToList()[i].ID_Type.ToString();
                    temp.id_list.Add(emp.Work_Types.ToList()[i].ID_Type);
                    if (i != emp.Work_Types.Count() - 1)
                    {
                        temp.name += ", ";
                        temp.name_id += ", ";
                    }
                }
                emplworks.Add(temp);
            }
            GridEmployeeWorks.DataContext = emplworks;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var EmpTypes = new List<CheckListBox>();
            foreach (var VARIABLE in Repair.Work_Types)
            {
                var temp = new CheckListBox();
                temp.Checked = false;
                temp.Label = VARIABLE.Type;
                temp.id = VARIABLE.ID_Type;
                EmpTypes.Add(temp);
            }
            EmpWrkListBox1.ItemsSource = EmpTypes;
            var types = Repair.Work_Types.Select(b => b.Type).ToList();
            reqcomboBox.ItemsSource = types;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            foreach (var VARIABLE in Repair.Employees.ToList())
            {
                if (!jobs.Contains(VARIABLE.Job))
                    jobs.Add(VARIABLE.Job);
            }
            if (Chosen_one.Job != "Супер-Адмін")
                jobs.Remove("Супер-Адмін");
            empCombo.ItemsSource = jobs;
            empCombo2.ItemsSource = Repair.Branches.ToList().Select(i => i.ID_Branch).OrderBy(m => m).ToList();
            reqcomboBox2.ItemsSource = Repair.Branches.ToList().Select(i => i.ID_Branch).OrderBy(m => m).ToList();
            FillComboBoxes();
            PrepareForm();
            UpdateComboBoxLists();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PrepareForm()
        {
            if (Chosen_one.Job == "Майстер")
            {
                EmpWrkbutton.IsEnabled = EmpWrkbutton2.IsEnabled = false;
                button1.IsEnabled = false;
                Cusbutton.IsEnabled = Cusbutton2.IsEnabled = false;
                Sparebutton.IsEnabled = Sparebutton2.IsEnabled = false;
                adrbutton.IsEnabled = adrbutton2.IsEnabled = false;
                empbutton2.IsEnabled = button.IsEnabled = false;
                PartsBrnchButton.IsEnabled = PartsBrnchButton2.IsEnabled = false;
                workTbutton.IsEnabled = workTbutton2.IsEnabled = false;
                Branchbutton.IsEnabled = Branchbutton2.IsEnabled = false;
                reqbutton2.IsEnabled = false;
            }
            if (Chosen_one.Job == "Касир")
            {
                EmpWrkbutton.IsEnabled = EmpWrkbutton2.IsEnabled = false;
                Sparebutton.IsEnabled = Sparebutton2.IsEnabled = false;
                empbutton2.IsEnabled = button.IsEnabled = false;
                PartsBrnchButton.IsEnabled = PartsBrnchButton2.IsEnabled = false;
                Branchbutton.IsEnabled = Branchbutton2.IsEnabled = false;
                reqcomboBox2.IsEnabled = false;
                reqcomboBox2.Text = Chosen_one.ID_Branch.ToString();
            }
            if (Chosen_one.Job == "Адміністратор")
            {
                empCombo2.IsEnabled = false;
                reqcomboBox2.IsEnabled = false;
                PartsBrnchTextBox1.IsEnabled = false;
                Branchbutton.IsEnabled = Branchbutton2.IsEnabled = false;
                PartsBrnchTextBox1.Text = Chosen_one.ID_Branch.ToString();
                reqcomboBox2.Text = Chosen_one.ID_Branch.ToString();
                empCombo2.Text = Chosen_one.ID_Branch.ToString();
                reqcomboBox2.Text = Chosen_one.ID_Branch.ToString();
            }
        }

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void emp_Click(object sender, RoutedEventArgs e)
        {
            if (empCombo2.Text == "" || empCombo.Text == "" || empText1.Text == "" || empText2.Text == "" ||
                empText3.Text == "" ||
                empText4.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            if (empText4.Text.Length > 10)
            {
                MessageBox.Show("Номер телефону - 10 цифр");
                return;
            }
            var Update = false;
            if (SelectedEmployee != null && button.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_Employee = new Employees();
                new_Employee.ID_Employee = Repair.Employees.Count();
                if (Update)
                {
                    new_Employee = SelectedEmployee;
                    if (method.CurrentLogMethod(new_Employee, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                else
                {
                    new_Employee.Date_In = DateTime.Today;
                }
                new_Employee.Name = empText1.Text.Replace(" ", "");
                new_Employee.SurName = empText2.Text.Replace(" ", "");
                new_Employee.Job = empCombo.Text;
                new_Employee.Branches = Repair.Branches.ToList().First(b => b.ID_Branch == int.Parse(empCombo2.Text));
                new_Employee.Salary = decimal.Parse(empText3.Text.Replace(" ", ""));
                new_Employee.Phone = empText4.Text.Replace(" ", "");
                var error =
                    Repair.Employees.ToList()
                        .Where(
                            b =>
                                b.Name == new_Employee.Name && b.SurName == new_Employee.SurName &&
                                b.Phone == new_Employee.Phone && b.Job == new_Employee.Job);
                if (error.Count() > 0 + Convert.ToInt32(Update))
                {
                    MessageBox.Show("Такий працівник вже є. Його ID=" +
                                    error.ToList()[0 + Convert.ToInt32(Update)].ID_Employee);
                    return;
                }
                if (Update != true)
                {
                    Repair.Employees.Add(new_Employee);
                    if (method.CurrentLogMethod(new_Employee, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                Repair.SaveChanges();
                if (Update)
                {
                    empbutton2_Click(sender, e);
                }
                empText1.Text = empText2.Text = empText3.Text = empText4.Text = "";
                if (Chosen_one.Job != "Супер-Адмін")
                    employees = Repair.Employees.ToList().Where(em => em.ID_Branch == Chosen_one.ID_Branch).ToList();
                else employees = Repair.Employees.ToList();
                GridEmployees.ItemsSource = employees;
                UpdateComboBoxLists();
            }
            catch (Exception)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void adr_Click(object sender, RoutedEventArgs e)
        {
            if (adrText1.Text == "" || adrText1.Text == "" || adrText1.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedAddress != null && adrbutton.Content == "Оновити дані")
                Update = true;
            var new_Address = new Addresses();
            new_Address.ID_Address = Repair.Addresses.Count();
            if (Update)
            {
                new_Address = SelectedAddress;
                if (method.CurrentLogMethod(new_Address, Chosen_one, LogMethod.Type.Update) == 0)
                    return;
            }
            new_Address.City = adrText1.Text.Replace(" ", "");
            ;
            new_Address.Street = adrText2.Text.TrimStart().TrimEnd();
            new_Address.House_Num = adrText3.Text.Replace(" ", "");
            ;
            new_Address.Room_Num = adrText4.Text.Replace(" ", "");
            ;
            var error =
                Repair.Addresses.ToList()
                    .Where(
                        b =>
                            b.City == new_Address.City && b.Street == new_Address.Street &&
                            b.House_Num == new_Address.House_Num && b.Room_Num == new_Address.Room_Num);
            if (error.Count() > 0 + Convert.ToInt32(Update))
            {
                MessageBox.Show("Така адреса вже є. Її ID=" + error.ToList()[0 + Convert.ToInt32(Update)].ID_Address);
                return;
            }
            if (Update == false)
            {
                Repair.Addresses.Add(new_Address);
                if (method.CurrentLogMethod(new_Address, Chosen_one, LogMethod.Type.Create) == 0)
                    return;
            }
            else
                adrbutton2_Click(sender, e);
            Repair.SaveChanges();
            adrText1.Text = adrText2.Text = adrText3.Text = adrText4.Text = "";
            if (Chosen_one.Job != "Супер-Адмін")
            {
                foreach (var VARIABLE in requests)
                {
                    if (VARIABLE.Addresses != null)
                        addresses.Add(VARIABLE.Addresses);
                }
            }
            else addresses = Repair.Addresses.ToList();
            GridAddresses.ItemsSource = addresses;
            UpdateComboBoxLists();
        }

        private void workTbutton_Click(object sender, RoutedEventArgs e)
        {
            if (workTText1.Text == "" || workTText2.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedCustomer != null && Cusbutton.Content == "Оновити дані")
                Update = true;
            var new_WorkType = new Work_Types();
            new_WorkType.ID_Type = Repair.Work_Types.Count();
            if (Update)
            {
                new_WorkType = SelectedWorkTypes;
                if (method.CurrentLogMethod(new_WorkType, Chosen_one, LogMethod.Type.Update) == 0)
                    return;
            }
            new_WorkType.Type = workTText1.Text;
            new_WorkType.Description = workTText2.Text;
            var error =
                Repair.Work_Types.ToList()
                    .Where(
                        b =>
                            b.Type == new_WorkType.Type);
            if (error.Count() > 0 + Convert.ToInt32(Update))
            {
                MessageBox.Show("Такий тип вже є. Його ID=" + error.ToList()[0 + Convert.ToInt32(Update)].ID_Type);
                return;
            }
            if (Update == false)
            {
                Repair.Work_Types.Add(new_WorkType);
                if (method.CurrentLogMethod(new_WorkType, Chosen_one, LogMethod.Type.Create) == 0)
                    return;
            }
            Repair.SaveChanges();
            if (Update)
                workTbutton2_Click(sender, e);
            workTText1.Text = workTText2.Text = "";
            GridWorkTypes.ItemsSource = Repair.Work_Types.ToList();
            UpdateComboBoxLists();
        }

        private void Cusbutton_Click(object sender, RoutedEventArgs e)
        {
            if (CusText1.Text == "" || CusText2.Text == "" || CusText3.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            if (CusText3.Text.Length > 10)
            {
                MessageBox.Show("Номер телефону - 10 цифр");
                return;
            }
            var Update = false;
            if (SelectedCustomer != null && Cusbutton.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_Customer = new Customer();
                new_Customer.ID_Customer = Repair.Customer.Count();
                if (Update)
                {
                    new_Customer = SelectedCustomer;
                    if (method.CurrentLogMethod(new_Customer, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                new_Customer.Name = CusText1.Text.Replace(" ", "");
                new_Customer.SurName = CusText2.Text.Replace(" ", "");
                ;
                new_Customer.Phone = CusText3.Text;
                var error =
                    Repair.Customer.ToList()
                        .Where(
                            b =>
                                b.Name == new_Customer.Name && b.SurName == new_Customer.SurName &&
                                b.Phone == new_Customer.Phone);
                if (error.Count() > 0 + Convert.ToInt32(Update))
                {
                    MessageBox.Show("Такий замовник вже є. Його ID=" +
                                    error.ToList()[0 + Convert.ToInt32(Update)].ID_Customer);
                    return;
                }
                if (Update != true)
                {
                    Repair.Customer.Add(new_Customer);
                    if (method.CurrentLogMethod(new_Customer, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                Repair.SaveChanges();
                if (Update)
                    Cusbutton2_Click(sender, e);
                CusText1.Text = CusText2.Text = CusText3.Text = "";
                if (Chosen_one.Job != "Супер-Адмін")
                {
                    customers = new List<Customer>();
                    foreach (var VARIABLE in requests)
                    {
                        customers.Add(VARIABLE.Customer);
                    }
                }
                else customers = Repair.Customer.ToList();
                GridCustomers.ItemsSource = customers;
                UpdateComboBoxLists();
            }
            catch (Exception)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void Sparebutton_Click(object sender, RoutedEventArgs e)
        {
            if (SpareText1.Text == "" || SpareText2.Text == "" || SpareText3.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedSpareParts != null && Sparebutton.Content == "Оновити дані")
                Update = true;
            var new_Part = new Spare_parts();
            new_Part.ID_Part = Repair.Spare_parts.Count();
            if (Update)
            {
                new_Part = SelectedSpareParts;
                if (method.CurrentLogMethod(new_Part, Chosen_one, LogMethod.Type.Update) == 0)
                    return;
            }
            new_Part.Serial = SpareText1.Text;
            new_Part.Name = SpareText2.Text;
            new_Part.Price = decimal.Parse(SpareText3.Text);
            var error =
                Repair.Spare_parts.ToList()
                    .Where(
                        b =>
                            b.Serial == new_Part.Serial && b.Name == new_Part.Name);
            if (error.Count() > 0 + Convert.ToInt32(Update))
            {
                MessageBox.Show("Така деталь вже є. Її ID=" + error.ToList()[0 + Convert.ToInt32(Update)].ID_Part);
                return;
            }
            if (Update == false)
            {
                Repair.Spare_parts.Add(new_Part);
                if (method.CurrentLogMethod(new_Part, Chosen_one, LogMethod.Type.Create) == 0)
                    return;
            }
            Repair.SaveChanges();
            if (Update)
                Cusbutton2_Click(sender, e);
            SpareText1.Text = SpareText2.Text = SpareText3.Text = "";
            GridSpareParts.ItemsSource = Repair.Spare_parts.ToList();
            UpdateComboBoxLists();
        }

        private void Branchbutton_Click(object sender, RoutedEventArgs e)
        {
            if (BranchText1.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedBranch != null && Branchbutton.Content == "Оновити дані")
                Update = true;
            var new_Branch = new Branches();
            new_Branch.ID_Branch = Repair.Branches.Count();
            if (Update)
            {
                new_Branch = SelectedBranch;
                if (method.CurrentLogMethod(new_Branch, Chosen_one, LogMethod.Type.Update) == 0)
                    return;
            }
            new_Branch.Addresses = Repair.Addresses.ToList().First(b => b.ID_Address == int.Parse(BranchText1.Text));
            var error =
                Repair.Branches.ToList()
                    .Where(
                        b =>
                            b.ID_Address == new_Branch.ID_Address);
            if (error.Count() > 0 + Convert.ToInt32(Update))
            {
                MessageBox.Show("Така філія за такою адресою вже є. Його ID=" +
                                error.ToList()[0 + Convert.ToInt32(Update)].ID_Address);
                return;
            }
            if (Update == false)
            {
                Repair.Branches.Add(new_Branch);
                if (method.CurrentLogMethod(new_Branch, Chosen_one, LogMethod.Type.Create) == 0)
                    return;
            }
            Repair.SaveChanges();
            if (Update)
                Branchbutton2_Click(sender, e);
            BranchText1.Text = "";
            br_adr = new List<NewAddress>();
            foreach (var VARIABLE in Repair.Branches)
            {
                var temp = new NewAddress();
                temp.id = VARIABLE.ID_Branch;
                var adr = Repair.Addresses.ToList().Where(b => b.ID_Address == VARIABLE.ID_Address).First();
                temp.adr = adr.City + ", " + adr.Street + ", буд. " + adr.House_Num;
                if (adr.Room_Num != null)
                    temp.adr += ", квартира " + adr.Room_Num;
                temp.adr_id = adr.ID_Address;
                br_adr.Add(temp);
            }
            GridBranches.ItemsSource = br_adr;
            UpdateComboBoxLists();
        }

        private void EmpWrkbutton_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<Work_Types>();
            foreach (var VARIABLE in EmpWrkListBox1.Items)
            {
                if ((VARIABLE as CheckListBox).Checked)
                    list.Add(Repair.Work_Types.ToList().First(l => l.ID_Type == (VARIABLE as CheckListBox).id));
            }
            if (EmpWrkText1.Text == "" || list.Count() == 0)
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedEmployeeType != null && EmpWrkbutton.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_WorkType =
                    Repair.Employees.ToList().First(em => em.ID_Employee == int.Parse(EmpWrkText1.Text));
                if (Update)
                {
                    new_WorkType = SelectedEmployeeType;
                    var temp = new NewEmployeeWorks();
                    temp.id = new_WorkType.ID_Employee;
                    var emp = new_WorkType;
                    for (var i = 0; i < emp.Work_Types.Count(); i++)
                    {
                        temp.name += emp.Work_Types.ToList()[i].Type;
                        temp.name_id += emp.Work_Types.ToList()[i].ID_Type.ToString();
                        temp.id_list.Add(emp.Work_Types.ToList()[i].ID_Type);
                        if (i != emp.Work_Types.Count() - 1)
                        {
                            temp.name += ", ";
                            temp.name_id += ", ";
                        }
                    }
                    if (method.CurrentLogMethod(temp, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                if (Update == false)
                {
                    var error = "";
                    foreach (var VARIABLE in list)
                    {
                        if (new_WorkType.Work_Types.ToList().Contains(VARIABLE))
                        {
                            error = VARIABLE.Type;
                            break;
                        }
                    }
                    if (error != "")
                    {
                        MessageBox.Show("Працівник вже має такий тип робіт '" + error + "'");
                        return;
                    }
                }
                if (Update)
                    new_WorkType.Work_Types.Clear();
                foreach (var VARIABLE in list)
                {
                    new_WorkType.Work_Types.Add(VARIABLE);
                }
                Repair.SaveChanges();
                EmpWrkbutton2_Click(sender, e);
                emplworks = new List<NewEmployeeWorks>();
                foreach (var VARIABLE in Repair.Employees)
                {
                    var temp = new NewEmployeeWorks();
                    temp.id = VARIABLE.ID_Employee;
                    var emp = VARIABLE;
                    for (var i = 0; i < emp.Work_Types.Count(); i++)
                    {
                        temp.name += emp.Work_Types.ToList()[i].Type;
                        temp.name_id += emp.Work_Types.ToList()[i].ID_Type.ToString();
                        if (i != emp.Work_Types.Count() - 1)
                        {
                            temp.name += ", ";
                            temp.name_id += ", ";
                        }
                    }
                    emplworks.Add(temp);
                }
                var EmpTypes = new List<CheckListBox>();
                foreach (var VARIABLE in Repair.Work_Types)
                {
                    var temp = new CheckListBox();
                    temp.Checked = false;
                    temp.Label = VARIABLE.Type;
                    temp.id = VARIABLE.ID_Type;
                    EmpTypes.Add(temp);
                }
                EmpWrkListBox1.ItemsSource = EmpTypes;
                EmpWrkText1.Text = "";
                GridEmployeeWorks.DataContext = emplworks;
                UpdateComboBoxLists();
            }
            catch (Exception)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (reqTextBox1.Text == "" || reqcomboBox2.Text == "" || reqTextBox3.Text == "" || reqTextBox4.Text == "" ||
                reqcomboBox.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedRequest != null && button1.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_Request = new Requests();
                new_Request.ID_Request = Repair.Requests.Count();
                if (Update)
                {
                    new_Request = SelectedRequest;
                    if (method.CurrentLogMethod(new_Request, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                else
                {
                    new_Request.Date_In = DateTime.Today;
                }
                new_Request.Serial = reqTextBox1.Text;
                new_Request.Type = reqcomboBox.Text;
                new_Request.Branches = Repair.Branches.ToList().First(b => b.ID_Branch == int.Parse(reqcomboBox2.Text));
                new_Request.Employees =
                    Repair.Employees.ToList().First(em => em.ID_Employee == int.Parse(reqTextBox3.Text));
                new_Request.Customer =
                    Repair.Customer.ToList().First(em => em.ID_Customer == int.Parse(reqTextBox4.Text));
                if (reqTextBox2.Text.Replace(" ", "") != "")
                    new_Request.Addresses =
                        Repair.Addresses.ToList().First(ad => ad.ID_Address == int.Parse(reqTextBox2.Text));
                else
                    new_Request.Addresses = null;
                var error =
                    Repair.Requests.ToList()
                        .Where(
                            b =>
                                b.Serial == new_Request.Serial && b.Date_In == new_Request.Date_In &&
                                b.Customer == new_Request.Customer && b.Employees == new_Request.Employees &&
                                b.Addresses == new_Request.Addresses);
                if (error.Count() > 0 + Convert.ToInt32(Update))
                {
                    MessageBox.Show("Таке замовлення вже є. Його ID=" +
                                    error.ToList()[0 + Convert.ToInt32(Update)].ID_Request);
                    return;
                }
                if (Update == false)
                {
                    Repair.Requests.Add(new_Request);
                    if (method.CurrentLogMethod(new_Request, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                Repair.SaveChanges();
                if (Update)
                    reqbutton2_Click(sender, e);
                reqTextBox1.Text = reqTextBox2.Text = reqTextBox3.Text = reqTextBox4.Text = "";
                if (Chosen_one.Job != "Супер-Адмін")
                {
                    if (Chosen_one.Job == "Майстер")
                        requests = Repair.Requests.ToList().Where(r => r.ID_Employee == Chosen_one.ID_Employee).ToList();
                    else
                        requests = Repair.Requests.ToList().Where(r => r.ID_Branch == Chosen_one.ID_Branch).ToList();
                }
                else requests = Repair.Requests.ToList();
                GridRequests.ItemsSource = requests;
                UpdateComboBoxLists();
            }
            catch (Exception)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void PartsBrnchButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartsBrnchTextBox1.Text == "" || PartsBrnchTextBox2.Text == "" || PartsBrnchTextBox3.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var Update = false;
            if (SelectedPartBranch != null && PartsBrnchButton.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_PartsOnBranch = new Parts_On_Branch();
                if (Update)
                {
                    new_PartsOnBranch = SelectedPartBranch;
                    if (method.CurrentLogMethod(new_PartsOnBranch, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                new_PartsOnBranch.Branches =
                    Repair.Branches.ToList().First(b => b.ID_Branch == int.Parse(PartsBrnchTextBox1.Text));
                new_PartsOnBranch.Spare_parts =
                    Repair.Spare_parts.ToList().First(b => b.ID_Part == int.Parse(PartsBrnchTextBox2.Text));
                new_PartsOnBranch.Ammount = int.Parse(PartsBrnchTextBox3.Text);
                var error =
                    Repair.Parts_On_Branch.ToList()
                        .Where(
                            p =>
                                p.Branches.ID_Branch == new_PartsOnBranch.Branches.ID_Branch &&
                                p.Spare_parts.ID_Part == new_PartsOnBranch.Spare_parts.ID_Part);
                if (error.Count() > 0 + Convert.ToInt32(Update))
                {
                    MessageBox.Show("Така деталь на такій філії вже є!");
                    return;
                }
                if (Update == false)
                {
                    Repair.Parts_On_Branch.Add(new_PartsOnBranch);
                    if (method.CurrentLogMethod(new_PartsOnBranch, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                Repair.SaveChanges();
                if (Update)
                    PartsBrnchButton2_Click(sender, e);
                prts_brnch = new List<NewPartsOnBranch>();
                foreach (var VARIABLE in Repair.Parts_On_Branch.OrderBy(o => o.ID_Part).OrderBy(p => p.ID_Branch))
                {
                    var temp = new NewPartsOnBranch();
                    if (VARIABLE.ID_Branch != Chosen_one.ID_Branch && (Chosen_one.Job != "Супер-Адмін"))
                        continue;
                    temp.id = VARIABLE.ID_Branch;
                    temp.part_Id = VARIABLE.ID_Part;
                    var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                    temp.part_Ammount = VARIABLE.Ammount.Value;
                    temp.part_Name = part.Name;
                    temp.part_Serial = part.Serial;
                    prts_brnch.Add(temp);
                }
                PartsBrnchTextBox1.Text = PartsBrnchTextBox2.Text = PartsBrnchTextBox3.Text = "";
                GridPartsOnBranch.DataContext = prts_brnch;
                UpdateComboBoxLists();
            }
            catch (Exception)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void NeedPartsButton_Click(object sender, RoutedEventArgs e)
        {
            if (NeedPartsTextBox1.Text == "" || NeedPartsTextBox2.Text == "" || NeedPartsTextBox3.Text == "")
            {
                MessageBox.Show("Заповність усі поля");
                return;
            }
            var diff = 0;
            var Update = false;
            if (SelectedNeedPart != null && NeedPartsButton.Content == "Оновити дані")
                Update = true;
            try
            {
                var new_NeedPart = new Need_Parts();
                if (Update)
                {
                    new_NeedPart = SelectedNeedPart;
                    diff = new_NeedPart.Used_Parts.Value - int.Parse(NeedPartsTextBox3.Text);
                    if (method.CurrentLogMethod(new_NeedPart, Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                }
                new_NeedPart.Requests =
                    Repair.Requests.ToList().First(b => b.ID_Request == int.Parse(NeedPartsTextBox1.Text));
                new_NeedPart.Spare_parts =
                    Repair.Spare_parts.ToList().First(b => b.ID_Part == int.Parse(NeedPartsTextBox2.Text));
                new_NeedPart.Used_Parts = int.Parse(NeedPartsTextBox3.Text);
                var error =
                    Repair.Need_Parts.ToList()
                        .Where(
                            p =>
                                p.Requests == new_NeedPart.Requests &&
                                p.Spare_parts == new_NeedPart.Spare_parts);
                if (error.Count() > 0 + Convert.ToInt32(Update))
                {
                    MessageBox.Show("Така деталь для такого замовлення вже є!");
                    return;
                }
                if (Update == false)
                {
                    Repair.Need_Parts.Add(new_NeedPart);
                    if (method.CurrentLogMethod(new_NeedPart, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                if (Update)
                    NeedPartsButton2_Click(sender, e);
                Repair.SaveChanges();
                var prt =
                    Repair.Parts_On_Branch.ToList()
                        .Where(p => p.ID_Part == new_NeedPart.ID_Part && p.ID_Branch == new_NeedPart.Requests.ID_Branch)
                        .ToList();
                if (prt.Count() > 0)
                {
                    if (method.CurrentLogMethod(prt[0], Chosen_one, LogMethod.Type.Update) == 0)
                        return;
                    if (Update == false)
                        prt[0].Ammount -= new_NeedPart.Used_Parts.Value;
                    else
                    {
                        if (diff >= 0)
                            prt[0].Ammount -= -diff;
                        else prt[0].Ammount += diff;
                    }
                }
                else
                {
                    var temp = new Parts_On_Branch();
                    temp.Branches = new_NeedPart.Requests.Branches;
                    temp.Spare_parts = new_NeedPart.Spare_parts;
                    temp.Ammount = -new_NeedPart.Used_Parts.Value;
                    Repair.Parts_On_Branch.Add(temp);
                    if (method.CurrentLogMethod(temp, Chosen_one, LogMethod.Type.Create) == 0)
                        return;
                }
                Repair.SaveChanges();
                prts_need = new List<NewPartsOnBranch>();
                for (var i = 0; i < requests.Count(); i++)
                {
                    var nd = requests[i].Need_Parts.ToList();
                    foreach (var VARIABLE in nd)
                    {
                        var temp = new NewPartsOnBranch();
                        temp.id = VARIABLE.ID_Request;
                        temp.part_Id = VARIABLE.ID_Part;
                        temp.part_Ammount = VARIABLE.Used_Parts.Value;
                        var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                        temp.part_Name = part.Name;
                        temp.part_Serial = part.Serial;
                        prts_need.Add(temp);
                    }
                }
                GridNeedParts.ItemsSource = prts_need;
                prts_brnch = new List<NewPartsOnBranch>();
                foreach (var VARIABLE in Repair.Parts_On_Branch.OrderBy(o => o.ID_Part).OrderBy(p => p.ID_Branch))
                {
                    var temp = new NewPartsOnBranch();
                    if (VARIABLE.ID_Branch != Chosen_one.ID_Branch && (Chosen_one.Job != "Супер-Адмін"))
                        continue;
                    temp.id = VARIABLE.ID_Branch;
                    temp.part_Id = VARIABLE.ID_Part;
                    var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                    temp.part_Ammount = VARIABLE.Ammount.Value;
                    temp.part_Name = part.Name;
                    temp.part_Serial = part.Serial;
                    prts_brnch.Add(temp);
                }
                GridPartsOnBranch.DataContext = prts_brnch;
                NeedPartsTextBox1.Text = NeedPartsTextBox2.Text = NeedPartsTextBox3.Text = "";
                UpdateComboBoxLists();
            }
            catch (Exception er)
            {
                MessageBox.Show("Введені не коректні дані");
            }
        }

        private void reqbutton3_Click(object sender, RoutedEventArgs e)
        {
            var current = SelectedRequest;
            var price = new decimal(8);
            price = current.Employees.Salary.Value*(decimal) 0.3;
            foreach (var VARIABLE in current.Need_Parts)
            {
                price += VARIABLE.Used_Parts.Value*VARIABLE.Spare_parts.Price.Value*(decimal) 0.3 +
                         VARIABLE.Spare_parts.Price.Value;
            }
            current.Date_Out = DateTime.Today;
            current.Price = Math.Round(price, 2);
            Repair.SaveChanges();
            if (Chosen_one.Job != "Супер-Адмін")
            {
                if (Chosen_one.Job == "Майстер")
                    requests = Repair.Requests.ToList().Where(r => r.ID_Employee == Chosen_one.ID_Employee).ToList();
                else
                    requests = Repair.Requests.ToList().Where(r => r.ID_Branch == Chosen_one.ID_Branch).ToList();
            }
            else requests = Repair.Requests.ToList();
            GridRequests.ItemsSource = requests;
            UpdateComboBoxLists();
            reqbutton3.IsEnabled = false;
        }

        public void FillComboBoxes()
        {
            {
                var s = new List<string>();
                s.Add("За ID");
                s.Add("За серійним кодом");
                s.Add("За типом");
                s.Add("За датою подання");
                s.Add("За датою здачі");
                s.Add("За ціною");
                s.Add("За ID адреси");
                s.Add("За ID замовника");
                s.Add("За ID працівника");
                if (Chosen_one.Job == "Супер-Адмін")
                    s.Add("За ID філії");
                SearchcomboBox1.ItemsSource = s;
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            {
                var s = new List<string>();
                s.Add("За ID працівника");
                s.Add("За ID типу");
                s.Add("За типом");
                SearchcomboBox2.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                s.Add("За ID замовлення");
                s.Add("За ID деталі");
                s.Add("За кількістю");
                s.Add("За назвою");
                s.Add("За кодом деталі");
                SearchcomboBox3.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                s.Add("За ID замовника");
                s.Add("За ім'ям");
                s.Add("За прізвищем");
                s.Add("За телефоном");
                SearchcomboBox4.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                s.Add("За ID деталі");
                s.Add("За кодом");
                s.Add("За назвою");
                s.Add("За ціною");
                SearchcomboBox5.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                s.Add("За ID адреси");
                s.Add("За містом");
                s.Add("За вулицею");
                s.Add("За номером будинку");
                s.Add("За номером квартири");
                SearchcomboBox6.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                s.Add("За ID працівника");
                s.Add("За ім'ям");
                s.Add("За прізвищем");
                s.Add("За номером телефону");
                s.Add("За зарплатою");
                if (Chosen_one.Job == "Супер-Адмін")
                    s.Add("За ID філії");
                SearchcomboBox7.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Clear();
                if (Chosen_one.Job == "Супер-Адмін")
                    s.Add("За ID філії");
                s.Add("За ID деталі");
                s.Add("За кількістю");
                s.Add("За назвою");
                s.Add("За кодом");
                SearchcomboBox8.ItemsSource = s;
            }
            {
                var s = new List<string>();
                if (Chosen_one.Job == "Супер-Адмін")
                    s.Add("За ID філії");
                s.Add("За ID адреси");
                s.Add("За повною назвою");
                SearchcomboBox9.ItemsSource = s;
            }
            {
                var s = new List<string>();
                s.Add("За ID типу");
                s.Add("За типом");
                SearchcomboBox10.ItemsSource = s;
            }

            /////////////////////////////////////
        }


        public void UpdateComboBoxLists()
        {
            foreach (var VARIABLE in employees)
            {
                if (VARIABLE.Job == "Майстер")
                {
                    if (reqcomboBox2.Text != "")
                    {
                        if (VARIABLE.ID_Branch != int.Parse(reqcomboBox2.Text))
                            continue;
                    }
                    reqTextBox3.Items.Add(VARIABLE.ID_Employee);
                    EmpWrkText1.Items.Add(VARIABLE.ID_Employee);
                }
            }
            foreach (var VARIABLE in customers)
            {
                reqTextBox4.Items.Add(VARIABLE.ID_Customer);
            }
            foreach (var VARIABLE in addresses)
            {
                reqTextBox2.Items.Add(VARIABLE.ID_Address);
                BranchText1.Items.Add(VARIABLE.ID_Address);
            }
            foreach (var VARIABLE in requests)
            {
                NeedPartsTextBox1.Items.Add(VARIABLE.ID_Request);
            }
            foreach (var VARIABLE in spare)
            {
                NeedPartsTextBox2.Items.Add(VARIABLE.ID_Part);
                PartsBrnchTextBox2.Items.Add(VARIABLE.ID_Part);
            }
            foreach (var VARIABLE in br_adr)
            {
                PartsBrnchTextBox1.Items.Add(VARIABLE.id);
            }
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender == SearchTextBox1)
                {
                    if (SearchcomboBox1.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (Chosen_one.Job != "Супер-Адмін")
                    {
                        if (Chosen_one.Job == "Майстер")
                            requests =
                                Repair.Requests.ToList().Where(r => r.ID_Employee == Chosen_one.ID_Employee).ToList();
                        else
                            requests = Repair.Requests.ToList().Where(r => r.ID_Branch == Chosen_one.ID_Branch).ToList();
                    }
                    else requests = Repair.Requests.ToList();
                    if (SearchcomboBox1.SelectedIndex == 0)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.ID_Request == int.Parse(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 1)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.Serial.ToString().Contains(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 2)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.Type.ToString().Contains(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 3)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.Date_In.ToString().Contains(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 4)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.Date_Out.ToString().Contains(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 5)
                    {
                        GridRequests.ItemsSource = requests.Where(l => l.Price.ToString().Contains(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 6)
                    {
                        GridRequests.ItemsSource = requests.Where(l => l.ID_Address == int.Parse(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 7)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.ID_Customer == int.Parse(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 8)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.ID_Employee == int.Parse(SearchTextBox1.Text));
                    }
                    if (SearchcomboBox1.SelectedIndex == 9)
                    {
                        GridRequests.ItemsSource =
                            requests.Where(l => l.ID_Branch == int.Parse(SearchTextBox1.Text));
                    }
                }
                if (sender == SearchTextBox2)
                {
                    if (SearchcomboBox2.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    emplworks = new List<NewEmployeeWorks>();
                    foreach (var VARIABLE in employees)
                    {
                        var temp = new NewEmployeeWorks();
                        temp.id = VARIABLE.ID_Employee;
                        var emp = VARIABLE;
                        for (var i = 0; i < emp.Work_Types.Count(); i++)
                        {
                            temp.name += emp.Work_Types.ToList()[i].Type;
                            temp.name_id += emp.Work_Types.ToList()[i].ID_Type.ToString();
                            temp.id_list.Add(emp.Work_Types.ToList()[i].ID_Type);
                            if (i != emp.Work_Types.Count() - 1)
                            {
                                temp.name += ", ";
                                temp.name_id += ", ";
                            }
                        }
                        emplworks.Add(temp);
                    }
                    if (SearchcomboBox2.SelectedIndex == 0)
                    {
                        GridEmployeeWorks.ItemsSource = emplworks.Where(l => l.id == int.Parse(SearchTextBox2.Text));
                    }
                    if (SearchcomboBox2.SelectedIndex == 1)
                    {
                        GridEmployeeWorks.ItemsSource =
                            emplworks.Where(l => l.name_id.ToString().Contains(SearchTextBox2.Text));
                    }
                    if (SearchcomboBox2.SelectedIndex == 2)
                    {
                        GridEmployeeWorks.ItemsSource =
                            emplworks.Where(l => l.name.ToString().Contains(SearchTextBox2.Text));
                    }
                }
                if (sender == SearchTextBox3)
                {
                    if (SearchcomboBox3.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    prts_need = new List<NewPartsOnBranch>();
                    for (var i = 0; i < requests.Count(); i++)
                    {
                        var nd = requests[i].Need_Parts.ToList();
                        foreach (var VARIABLE in nd)
                        {
                            var temp = new NewPartsOnBranch();
                            temp.id = VARIABLE.ID_Request;
                            temp.part_Id = VARIABLE.ID_Part;
                            temp.part_Ammount = VARIABLE.Used_Parts.Value;
                            var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                            temp.part_Name = part.Name;
                            temp.part_Serial = part.Serial;
                            prts_need.Add(temp);
                        }
                    }
                    if (SearchcomboBox3.SelectedIndex == 0)
                    {
                        GridNeedParts.ItemsSource = prts_need.Where(l => l.id == int.Parse(SearchTextBox3.Text));
                    }
                    if (SearchcomboBox3.SelectedIndex == 1)
                    {
                        GridNeedParts.ItemsSource = prts_need.Where(l => l.part_Id == int.Parse(SearchTextBox3.Text));
                    }
                    if (SearchcomboBox3.SelectedIndex == 2)
                    {
                        GridNeedParts.ItemsSource =
                            prts_need.Where(l => l.part_Ammount == int.Parse(SearchTextBox3.Text));
                    }
                    if (SearchcomboBox3.SelectedIndex == 3)
                    {
                        GridNeedParts.ItemsSource =
                            prts_need.Where(l => l.part_Name.ToString().Contains(SearchTextBox3.Text));
                    }
                    if (SearchcomboBox3.SelectedIndex == 4)
                    {
                        GridNeedParts.ItemsSource =
                            prts_need.Where(l => l.part_Serial.ToString().Contains(SearchTextBox3.Text));
                    }
                }
                if (sender == SearchTextBox4)
                {
                    if (SearchcomboBox4.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (Chosen_one.Job != "Супер-Адмін")
                    {
                        customers = new List<Customer>();
                        foreach (var VARIABLE in requests)
                        {
                            customers.Add(VARIABLE.Customer);
                        }
                    }
                    else customers = Repair.Customer.ToList();
                    if (SearchcomboBox4.SelectedIndex == 0)
                    {
                        GridCustomers.ItemsSource = customers.Where(l => l.ID_Customer == int.Parse(SearchTextBox4.Text));
                    }
                    if (SearchcomboBox4.SelectedIndex == 1)
                    {
                        GridCustomers.ItemsSource = customers.Where(l => l.Name.ToString().Contains(SearchTextBox4.Text));
                    }
                    if (SearchcomboBox4.SelectedIndex == 2)
                    {
                        GridCustomers.ItemsSource =
                            customers.Where(l => l.SurName.ToString().Contains(SearchTextBox4.Text));
                    }
                    if (SearchcomboBox4.SelectedIndex == 3)
                    {
                        GridCustomers.ItemsSource =
                            customers.Where(l => l.Phone.ToString().Contains(SearchTextBox4.Text));
                    }
                }
                if (sender == SearchTextBox5)
                {
                    if (SearchcomboBox5.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (SearchcomboBox5.SelectedIndex == 0)
                    {
                        GridSpareParts.ItemsSource =
                            Repair.Spare_parts.ToList().Where(l => l.ID_Part == int.Parse(SearchTextBox5.Text));
                    }
                    if (SearchcomboBox5.SelectedIndex == 1)
                    {
                        GridSpareParts.ItemsSource =
                            Repair.Spare_parts.ToList().Where(l => l.Serial.ToString().Contains(SearchTextBox5.Text));
                    }
                    if (SearchcomboBox5.SelectedIndex == 2)
                    {
                        GridSpareParts.ItemsSource =
                            Repair.Spare_parts.ToList().Where(l => l.Name.ToString().Contains(SearchTextBox5.Text));
                    }
                    if (SearchcomboBox5.SelectedIndex == 3)
                    {
                        GridSpareParts.ItemsSource =
                            Repair.Spare_parts.ToList().Where(l => l.Price.ToString().Contains(SearchTextBox5.Text));
                    }
                }
                if (sender == SearchTextBox6)
                {
                    if (SearchcomboBox6.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (Chosen_one.Job != "Супер-Адмін")
                    {
                        foreach (var VARIABLE in requests)
                        {
                            if (VARIABLE.Addresses != null)
                                addresses.Add(VARIABLE.Addresses);
                        }
                    }
                    else addresses = Repair.Addresses.ToList();
                    if (SearchcomboBox6.SelectedIndex == 0)
                    {
                        GridAddresses.ItemsSource = addresses.Where(l => l.ID_Address == int.Parse(SearchTextBox6.Text));
                    }
                    if (SearchcomboBox6.SelectedIndex == 1)
                    {
                        GridAddresses.ItemsSource = addresses.Where(l => l.City.ToString().Contains(SearchTextBox6.Text));
                    }
                    if (SearchcomboBox6.SelectedIndex == 2)
                    {
                        GridAddresses.ItemsSource =
                            addresses.Where(l => l.Street.ToString().Contains(SearchTextBox6.Text));
                    }
                    if (SearchcomboBox6.SelectedIndex == 3)
                    {
                        GridAddresses.ItemsSource =
                            addresses.Where(l => l.House_Num.ToString().Contains(SearchTextBox6.Text));
                    }
                    if (SearchcomboBox6.SelectedIndex == 4)
                    {
                        if (SearchTextBox6.Text != "")
                        {
                            var temp = addresses.Where(l => l.Room_Num != null);
                            GridAddresses.ItemsSource =
                                temp.Where(l => l.Room_Num.ToString().Contains(SearchTextBox6.Text));
                        }
                        else GridAddresses.ItemsSource = addresses;
                    }
                }
                if (sender == SearchTextBox7)
                {
                    if (SearchcomboBox7.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (Chosen_one.Job != "Супер-Адмін")
                        employees = Repair.Employees.ToList().Where(em => em.ID_Branch == Chosen_one.ID_Branch).ToList();
                    else employees = Repair.Employees.ToList();
                    if (SearchcomboBox7.SelectedIndex == 0)
                    {
                        GridEmployees.ItemsSource = employees.Where(l => l.ID_Employee == int.Parse(SearchTextBox7.Text));
                    }
                    if (SearchcomboBox7.SelectedIndex == 1)
                    {
                        GridEmployees.ItemsSource = employees.Where(l => l.Name.ToString().Contains(SearchTextBox7.Text));
                    }
                    if (SearchcomboBox7.SelectedIndex == 2)
                    {
                        GridEmployees.ItemsSource =
                            employees.Where(l => l.SurName.ToString().Contains(SearchTextBox7.Text));
                    }
                    if (SearchcomboBox7.SelectedIndex == 3)
                    {
                        GridEmployees.ItemsSource =
                            employees.Where(l => l.Phone.ToString().Contains(SearchTextBox7.Text));
                    }
                    if (SearchcomboBox7.SelectedIndex == 4)
                    {
                        GridEmployees.ItemsSource =
                            employees.Where(l => l.Salary.ToString().Contains(SearchTextBox7.Text));
                    }
                    if (SearchcomboBox7.SelectedIndex == 5)
                    {
                        GridEmployees.ItemsSource = employees.Where(l => l.ID_Branch == int.Parse(SearchTextBox7.Text));
                    }
                }
                if (sender == SearchTextBox8)
                {
                    if (SearchcomboBox8.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    prts_brnch = new List<NewPartsOnBranch>();
                    foreach (var VARIABLE in Repair.Parts_On_Branch.OrderBy(o => o.ID_Part).OrderBy(p => p.ID_Branch))
                    {
                        var temp = new NewPartsOnBranch();
                        if (VARIABLE.ID_Branch != Chosen_one.ID_Branch && (Chosen_one.Job != "Супер-Адмін"))
                            continue;
                        temp.id = VARIABLE.ID_Branch;
                        temp.part_Id = VARIABLE.ID_Part;
                        var part = Repair.Spare_parts.ToList().First(p => p.ID_Part == VARIABLE.ID_Part);
                        temp.part_Ammount = VARIABLE.Ammount.Value;
                        temp.part_Name = part.Name;
                        temp.part_Serial = part.Serial;
                        prts_brnch.Add(temp);
                    }
                    if (SearchcomboBox8.SelectedIndex == 0)
                    {
                        GridPartsOnBranch.ItemsSource = prts_brnch.Where(l => l.id == int.Parse(SearchTextBox8.Text));
                    }
                    if (SearchcomboBox8.SelectedIndex == 1)
                    {
                        GridPartsOnBranch.ItemsSource =
                            prts_brnch.Where(l => l.part_Id == int.Parse(SearchTextBox8.Text));
                    }
                    if (SearchcomboBox8.SelectedIndex == 2)
                    {
                        GridPartsOnBranch.ItemsSource =
                            prts_brnch.Where(l => l.part_Ammount == int.Parse(SearchTextBox8.Text));
                    }
                    if (SearchcomboBox8.SelectedIndex == 3)
                    {
                        GridPartsOnBranch.ItemsSource =
                            prts_brnch.Where(l => l.part_Name.ToString().Contains(SearchTextBox8.Text));
                    }
                    if (SearchcomboBox8.SelectedIndex == 4)
                    {
                        GridPartsOnBranch.ItemsSource =
                            prts_brnch.Where(l => l.part_Serial.ToString().Contains(SearchTextBox8.Text));
                    }
                }
                if (sender == SearchTextBox9)
                {
                    if (SearchcomboBox9.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    br_adr = new List<NewAddress>();
                    foreach (var VARIABLE in Repair.Branches)
                    {
                        var temp = new NewAddress();
                        temp.id = VARIABLE.ID_Branch;
                        var adr =
                            Repair.Addresses.ToList().Where(b => b.ID_Address == VARIABLE.ID_Address).First();
                        temp.adr = adr.City + ", " + adr.Street + ", буд. " + adr.House_Num;
                        if (adr.Room_Num != null)
                            temp.adr += ", квартира " + adr.Room_Num;
                        temp.adr_id = adr.ID_Address;
                        br_adr.Add(temp);
                    }
                    if (SearchcomboBox9.SelectedIndex == 0)
                    {
                        GridBranches.ItemsSource = br_adr.Where(l => l.id == int.Parse(SearchTextBox9.Text));
                    }
                    if (SearchcomboBox9.SelectedIndex == 1)
                    {
                        GridBranches.ItemsSource = br_adr.Where(l => l.adr_id == int.Parse(SearchTextBox9.Text));
                    }
                }
                if (sender == SearchTextBox10)
                {
                    if (SearchcomboBox10.SelectedIndex == -1)
                    {
                        MessageBox.Show("Виберіть критерій пошуку");
                        return;
                    }
                    if (SearchcomboBox10.SelectedIndex == 0)
                    {
                        GridWorkTypes.ItemsSource =
                            Repair.Work_Types.ToList().Where(l => l.ID_Type == int.Parse(SearchTextBox10.Text));
                    }
                    if (SearchcomboBox10.SelectedIndex == 1)
                    {
                        GridWorkTypes.ItemsSource =
                            Repair.Work_Types.ToList().Where(l => l.Type.ToString().Contains(SearchTextBox10.Text));
                    }
                }
            }
            catch (Exception ex)
            {
                Status.Text = "НЕПРАВИЛЬНО ВВЕДЕНІ ДАНІ";
                return;
            }
            Status.Text = "";
        }

        private void SearchSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == SearchcomboBox1)
            {
                SearchTextChanged(SearchTextBox1, null);
            }
            if (sender == SearchcomboBox2)
            {
                SearchTextChanged(SearchTextBox2, null);
            }
            if (sender == SearchcomboBox3)
            {
                SearchTextChanged(SearchTextBox3, null);
            }
            if (sender == SearchcomboBox4)
            {
                SearchTextChanged(SearchTextBox4, null);
            }
            if (sender == SearchcomboBox5)
            {
                SearchTextChanged(SearchTextBox5, null);
            }
            if (sender == SearchcomboBox6)
            {
                SearchTextChanged(SearchTextBox6, null);
            }
            if (sender == SearchcomboBox7)
            {
                SearchTextChanged(SearchTextBox7, null);
            }
            if (sender == SearchcomboBox8)
            {
                SearchTextChanged(SearchTextBox8, null);
            }
            if (sender == SearchcomboBox9)
            {
                SearchTextChanged(SearchTextBox9, null);
            }
            if (sender == SearchcomboBox10)
            {
                SearchTextChanged(SearchTextBox10, null);
            }
        }

        private void UI_Closing(object sender, CancelEventArgs e)
        {
            Autorization.IsEnabled = true;
        }

        private void CreateRepo_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ReqRepo)
            {
                var obj = new CrystalReportRequests();
                var newRequests = new List<NewRequest>();
                foreach (var VARIABLE in requests)
                {
                    var temp = new NewRequest();
                    temp.ID_Request = VARIABLE.ID_Request.ToString();
                    temp.Serial = VARIABLE.Serial;
                    temp.Type = VARIABLE.Type;
                    temp.Price = VARIABLE.Price.ToString();
                    temp.Date_In = VARIABLE.Date_In.ToString();
                    temp.Date_Out = VARIABLE.Date_Out.ToString();
                    temp.ID_Address = VARIABLE.ID_Address.ToString();
                    temp.ID_Branch = VARIABLE.ID_Branch.ToString();
                    temp.ID_Customer = VARIABLE.ID_Customer.ToString();
                    temp.ID_Employee = VARIABLE.ID_Employee.ToString();
                    newRequests.Add(temp);
                }
                obj.SetDataSource(newRequests);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == EmpWrkRepo)
            {
                var obj = new CrystalReportEmpWorks();
                obj.SetDataSource(emplworks);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == CusRepo)
            {
                var obj = new CrystalReportCustomer();
                obj.SetDataSource(customers);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == NeedPartsRepo)
            {
                var obj = new CrystalReportNeedParts();
                obj.SetDataSource(prts_need);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == SparePartsRepo)
            {
                var obj = new CrystalReportSparePart();
                var newSpare = new List<NewSpareParts>();
                foreach (var VARIABLE in spare)
                {
                    var temp = new NewSpareParts();
                    temp.ID_Part = VARIABLE.ID_Part.ToString();
                    temp.Serial = VARIABLE.Serial;
                    temp.Name = VARIABLE.Name;
                    temp.Price = VARIABLE.Price.ToString();
                    newSpare.Add(temp);
                }
                obj.SetDataSource(newSpare);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == AddrRepo)
            {
                var obj = new CrystalReportAddress();
                obj.SetDataSource(addresses);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == PartsBranchRepo)
            {
                var obj = new CrystalReportPartsOnBranch();
                obj.SetDataSource(prts_brnch);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == EmployeeRepo)
            {
                var obj = new CrystalReportEmplyee();
                var newEmployees = new List<NewEmployees>();
                foreach (var VARIABLE in employees)
                {
                    var temp = new NewEmployees();
                    temp.ID_Employee = VARIABLE.ID_Employee.ToString();
                    temp.Name = VARIABLE.Name;
                    temp.SurName = VARIABLE.SurName;
                    temp.Phone = VARIABLE.Phone;
                    temp.Date_In = VARIABLE.Date_In.ToString();
                    temp.Job = VARIABLE.Job;
                    temp.ID_Branch = VARIABLE.ID_Branch.ToString();
                    temp.Salary = VARIABLE.Salary.ToString();
                    newEmployees.Add(temp);
                }
                obj.SetDataSource(newEmployees);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == BranchRepo)
            {
                var obj = new CrystalReportBranch();
                obj.SetDataSource(br_adr);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
            if (sender == WrkTypesRepo)
            {
                var obj = new CrystalReportWorkTypes();
                obj.SetDataSource(works);
                var rw = new ReportWindow(this);
                IsEnabled = false;
                rw.Show();
                rw.Viewer.ViewerCore.ReportSource = obj;
            }
        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == reqRenew)
            {
                reqTextBox1.Text =
                    reqTextBox2.Text = reqTextBox3.Text = reqTextBox4.Text = reqcomboBox.Text = reqcomboBox2.Text = "";
            }
            if (sender == empWorksRenew)
            {
                EmpWrkText1.Text = "";
                var EmpTypes = new List<CheckListBox>();
                foreach (var VARIABLE in Repair.Work_Types)
                {
                    var temp = new CheckListBox();
                    temp.Checked = false;
                    temp.Label = VARIABLE.Type;
                    temp.id = VARIABLE.ID_Type;
                    EmpTypes.Add(temp);
                }
                EmpWrkListBox1.ItemsSource = EmpTypes;
            }
            if (sender == CusRenew)
            {
                CusText1.Text = CusText2.Text = CusText3.Text = "";
            }
            if (sender == needPartsRenew)
            {
                NeedPartsTextBox1.Text = NeedPartsTextBox2.Text = NeedPartsTextBox3.Text = "";
            }
            if (sender == AddrRenew)
            {
                adrText1.Text = adrText2.Text = adrText3.Text = adrText4.Text = "";
            }
            if (sender == SparePartsRenew)
            {
                SpareText1.Text = SpareText2.Text = SpareText3.Text = "";
            }
            if (sender == BranchPartsRenew)
            {
                PartsBrnchTextBox1.Text = PartsBrnchTextBox2.Text = PartsBrnchTextBox3.Text = "";
            }
            if (sender == EmpRenew)
            {
                empText1.Text = empText2.Text = empText3.Text = empText4.Text = empCombo.Text = empCombo2.Text = "";
            }
            if (sender == WorksRenew)
            {
                workTText1.Text = workTText2.Text = "";
            }
            if (sender == BranchRenew)
            {
                BranchText1.Text = "";
            }
        }

        private void RelatedSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == reqcomboBox2)
            {
                RenewButton_Click(sender, e);
                reqTextBox3.Items.Clear();
                foreach (var VARIABLE in employees)
                {
                    if (VARIABLE.Job == "Майстер")
                    {
                        if (reqcomboBox2.SelectedValue.ToString() != "")
                        {
                            if (VARIABLE.ID_Branch != int.Parse(reqcomboBox2.SelectedValue.ToString()))
                                continue;
                        }
                        reqTextBox3.Items.Add(VARIABLE.ID_Employee);
                    }
                }
            }
        }

        /// 

        #region SetItem
        private Employees _selectedEmployeeType;

        public Employees SelectedEmployeeType
        {
            get { return _selectedEmployeeType; }
            set
            {
                _selectedEmployeeType = value;
                NotifyPropertyChanged("SelectedEmployeeType");
            }
        }

        public NewEmployeeWorks SelectedNewEmpWorks
        {
            set
            {
                _selectedEmployeeType = Repair.Employees.ToList().First(l => l.ID_Employee == value.id);
                NotifyPropertyChanged("SelectedNewEmpWorks");
            }
        }

        private Need_Parts _selectedNeedPart;

        public Need_Parts SelectedNeedPart
        {
            get { return _selectedNeedPart; }
            set
            {
                _selectedNeedPart = value;
                NotifyPropertyChanged("SelectedNeedPart");
            }
        }

        public NewPartsOnBranch SelectedNewNeedPart
        {
            set
            {
                _selectedNeedPart =
                    Repair.Need_Parts.ToList().First(l => l.ID_Request == value.id && l.ID_Part == value.part_Id);
                NotifyPropertyChanged("SelectedNewNeedPart");
            }
        }


        private Work_Types _selectedWorkTypes;

        public Work_Types SelectedWorkTypes
        {
            get { return _selectedWorkTypes; }
            set
            {
                _selectedWorkTypes = value;
                NotifyPropertyChanged("SelectedWorkTypes");
            }
        }

        private Parts_On_Branch _selectedPartBranch;

        public Parts_On_Branch SelectedPartBranch
        {
            get { return _selectedPartBranch; }
            set
            {
                _selectedPartBranch = value;
                NotifyPropertyChanged("SelectedPartBranch");
            }
        }

        public NewPartsOnBranch SelectedNewPartBranch
        {
            set
            {
                _selectedPartBranch =
                    Repair.Parts_On_Branch.ToList().First(l => l.ID_Branch == value.id && l.ID_Part == value.part_Id);
                NotifyPropertyChanged("SelectedNewPartBranch");
            }
        }

        private Branches _selectedBranch;

        public Branches SelectedBranch
        {
            get { return _selectedBranch; }
            set
            {
                _selectedBranch = value;
                NotifyPropertyChanged("SelectedBranch");
            }
        }

        private Employees _selectedEmployee;

        public Employees SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                NotifyPropertyChanged("SelectedEmployee");
            }
        }

        private Addresses _selectedAddress;

        public Addresses SelectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                _selectedAddress = value;
                NotifyPropertyChanged("SelectedAddress");
            }
        }

        private Requests _selectedRequest;

        public Requests SelectedRequest
        {
            get { return _selectedRequest; }
            set
            {
                _selectedRequest = value;
                if (value != null)
                    reqbutton3.IsEnabled = true;
                else
                    reqbutton3.IsEnabled = false;
                NotifyPropertyChanged("SelectedRequest");
            }
        }

        private Spare_parts _selectedSpareParts;

        public Spare_parts SelectedSpareParts
        {
            get { return _selectedSpareParts; }
            set
            {
                _selectedSpareParts = value;
                NotifyPropertyChanged("SelectedSpareParts");
            }
        }

        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                NotifyPropertyChanged("SelectedCustomer");
            }
        }

        #endregion

        #region SetUpdate

        private void empbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Виберіть працівника для редагування");
                return;
            }
            if (SelectedEmployee != null && button.Content == "Оновити дані")
            {
                SelectedEmployee = null;
                empText1.Text = empText2.Text = empText3.Text = empText4.Text = "";
                empCombo.SelectedIndex = empCombo2.SelectedIndex = -1;
                button.Content = "Додати деталь";
                empbutton2.Content = "Оновити дані";
                return;
            }
            button.Content = "Оновити дані";
            empbutton2.Content = "Відмінити оновлення";
            var update = SelectedEmployee;
            empText1.Text = update.Name;
            empText2.Text = update.SurName;
            empText3.Text = update.Salary.ToString();
            empText4.Text = update.Phone;
            empCombo.SelectedValue = update.Job;
            empCombo2.SelectedValue = update.ID_Branch;
        }

        private void reqbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRequest == null)
            {
                MessageBox.Show("Виберіть замовлення для редагування");
                return;
            }
            if (SelectedRequest != null && button1.Content == "Оновити дані")
            {
                SelectedRequest = null;
                reqTextBox1.Text = reqTextBox2.Text = reqTextBox3.Text = reqTextBox4.Text = "";
                reqcomboBox.SelectedIndex = reqcomboBox2.SelectedIndex = -1;
                button1.Content = "Додати замовлення";
                reqbutton2.Content = "Оновити дані";
                return;
            }
            button1.Content = "Оновити дані";
            reqbutton2.Content = "Відмінити оновлення";
            var update = SelectedRequest;
            reqTextBox1.Text = update.Serial;
            reqTextBox2.Text = update.ID_Address.ToString();
            reqTextBox3.Text = update.ID_Employee.ToString();
            reqTextBox4.Text = update.ID_Customer.ToString();
            reqcomboBox.SelectedValue = update.Type;
            reqcomboBox2.SelectedValue = update.ID_Branch;
        }

        private void adrbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAddress == null)
            {
                MessageBox.Show("Виберіть адресу для редагування");
                return;
            }
            if (SelectedAddress != null && adrbutton.Content == "Оновити дані")
            {
                SelectedAddress = null;
                adrText1.Text = adrText2.Text = adrText3.Text = adrText4.Text = "";
                adrbutton.Content = "Додати адресу";
                adrbutton2.Content = "Оновити дані";
                return;
            }
            adrbutton.Content = "Оновити дані";
            adrbutton2.Content = "Відмінити оновлення";
            var update = SelectedAddress;
            adrText1.Text = update.City;
            adrText2.Text = update.Street;
            adrText3.Text = update.House_Num;
            adrText4.Text = update.Room_Num;
        }

        private void Cusbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Виберіть замовника для редагування");
                return;
            }
            if (SelectedCustomer != null && Cusbutton.Content == "Оновити дані")
            {
                SelectedCustomer = null;
                CusText1.Text = CusText2.Text = CusText3.Text = "";
                Cusbutton.Content = "Додати замовника";
                Cusbutton2.Content = "Оновити дані";
                return;
            }
            Cusbutton.Content = "Оновити дані";
            Cusbutton2.Content = "Відмінити оновлення";
            var update = SelectedCustomer;
            CusText1.Text = update.Name;
            CusText2.Text = update.SurName;
            CusText3.Text = update.Phone;
        }

        private void Sparebutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpareParts == null)
            {
                MessageBox.Show("Виберіть деталь для редагування");
                return;
            }
            if (SelectedSpareParts != null && Sparebutton.Content == "Оновити дані")
            {
                SelectedSpareParts = null;
                SpareText1.Text = SpareText2.Text = SpareText3.Text = "";
                Sparebutton.Content = "Додати деталь";
                Sparebutton2.Content = "Оновити дані";
                return;
            }
            Sparebutton.Content = "Оновити дані";
            Sparebutton2.Content = "Відмінити оновлення";
            var update = SelectedSpareParts;
            SpareText1.Text = update.Serial;
            SpareText2.Text = update.Name;
            SpareText3.Text = update.Price.ToString();
        }

        private void Branchbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBranch == null)
            {
                MessageBox.Show("Виберіть філію для редагування");
                return;
            }
            if (SelectedBranch != null && Branchbutton.Content == "Оновити дані")
            {
                SelectedBranch = null;
                BranchText1.Text = "";
                Branchbutton.Content = "Додати філію";
                Branchbutton2.Content = "Оновити дані";
                return;
            }
            Branchbutton.Content = "Оновити дані";
            Branchbutton2.Content = "Відмінити оновлення";
            var update = SelectedBranch;
            BranchText1.Text = update.ID_Address.ToString();
        }

        private void PartsBrnchButton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPartBranch == null)
            {
                MessageBox.Show("Виберіть деталь на філії для редагування");
                return;
            }
            if (SelectedPartBranch != null && PartsBrnchButton.Content == "Оновити дані")
            {
                SelectedPartBranch = null;
                PartsBrnchTextBox1.Text = PartsBrnchTextBox2.Text = PartsBrnchTextBox3.Text = "";
                PartsBrnchButton.Content = "Додати деталь";
                PartsBrnchButton2.Content = "Оновити дані";
                return;
            }
            PartsBrnchButton.Content = "Оновити дані";
            PartsBrnchButton2.Content = "Відмінити оновлення";
            var update = SelectedPartBranch;
            PartsBrnchTextBox1.Text = update.ID_Branch.ToString();
            PartsBrnchTextBox2.Text = update.ID_Part.ToString();
            PartsBrnchTextBox3.Text = update.Ammount.ToString();
        }

        private void workTbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedWorkTypes == null)
            {
                MessageBox.Show("Виберіть тип робіт для редагування");
                return;
            }
            if (SelectedWorkTypes != null && workTbutton.Content == "Оновити дані")
            {
                SelectedPartBranch = null;
                workTText1.Text = workTText2.Text = "";
                workTbutton.Content = "Додати тип робіт";
                workTbutton2.Content = "Оновити дані";
                return;
            }
            workTbutton.Content = "Оновити дані";
            workTbutton2.Content = "Відмінити оновлення";
            var update = SelectedWorkTypes;
            workTText1.Text = update.Type;
            workTText2.Text = update.Description;
        }

        private void NeedPartsButton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNeedPart == null)
            {
                MessageBox.Show("Виберіть деталь з замовлення для редагування");
                return;
            }
            if (SelectedNeedPart != null && NeedPartsButton.Content == "Оновити дані")
            {
                SelectedNeedPart = null;
                NeedPartsTextBox1.Text = NeedPartsTextBox2.Text = NeedPartsTextBox3.Text = "";
                NeedPartsButton.Content = "Додати деталь";
                NeedPartsButton2.Content = "Оновити дані";
                return;
            }
            NeedPartsButton.Content = "Оновити дані";
            NeedPartsButton2.Content = "Відмінити оновлення";
            var update = SelectedNeedPart;
            NeedPartsTextBox1.Text = update.ID_Request.ToString();
            NeedPartsTextBox2.Text = update.ID_Part.ToString();
            NeedPartsTextBox3.Text = update.Used_Parts.ToString();
        }

        private void EmpWrkbutton2_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployeeType == null)
            {
                MessageBox.Show("Виберіть тип робіт працівника для редагування");
                return;
            }
            if (SelectedEmployeeType != null && EmpWrkbutton.Content == "Оновити дані")
            {
                SelectedEmployeeType = null;
                EmpWrkText1.Text = "";
                EmpWrkbutton.Content = "Додати тип робіт";
                EmpWrkbutton2.Content = "Оновити дані";
                return;
            }
            EmpWrkbutton.Content = "Оновити дані";
            EmpWrkbutton2.Content = "Відмінити оновлення";
            var update = SelectedEmployeeType;
            EmpWrkText1.Text = update.ID_Employee.ToString();
            var EmpTypes = new List<CheckListBox>();
            foreach (var VARIABLE in Repair.Work_Types)
            {
                var temp = new CheckListBox();
                if (update.Work_Types.ToList().Contains(VARIABLE))
                    temp.Checked = true;
                else
                    temp.Checked = false;
                temp.Label = VARIABLE.Type;
                temp.id = VARIABLE.ID_Type;
                EmpTypes.Add(temp);
            }
            EmpWrkListBox1.ItemsSource = EmpTypes;
        }

        #endregion
    }

    public class NewSpareParts
    {
        public string ID_Part, Serial, Name, Price;

        public NewSpareParts()
        {
            ID_Part = Serial = Name = Price = null;
        }
    }

    public class NewEmployees
    {
        public string Date_In, Job, ID_Branch, Salary;
        public string ID_Employee, Name, SurName, Phone;

        public NewEmployees()
        {
            ID_Employee = Name = SurName = Phone = null;
            Date_In = Job = ID_Branch = Salary = null;
        }
    }

    public class CheckListBox
    {
        public int id { get; set; }
        public bool Checked { get; set; }
        public string Label { get; set; }
    }

    public class NewRequest
    {
        public string Date_In, Date_Out, Type, Serial, Price;
        public string ID_Request, ID_Address, ID_Branch, ID_Customer, ID_Employee;

        public NewRequest()
        {
            ID_Request = ID_Address = ID_Branch = ID_Customer = ID_Employee = "";
            Date_In = Date_Out = Type = Serial = Price = "";
        }
    }

    public class NewAddress
    {
        public int id { get; set; }
        public string adr { get; set; }
        public int adr_id { get; set; }
    }


    public class NewEmployeeWorks
    {
        public NewEmployeeWorks()
        {
            id_list = new List<int>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string name_id { get; set; }
        public List<int> id_list { get; set; }
    }

    public class NewPartsOnBranch
    {
        public int id { get; set; }
        public string part_Name { get; set; }
        public int part_Id { get; set; }
        public int part_Ammount { get; set; }
        public string part_Serial { get; set; }
    }
}