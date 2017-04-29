using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Kursova
{
    public class LogMethod
    {
        public enum Type
        {
            Create,
            Update,
            Delete
        }

        public int CurrentLogMethod(Employees emp, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Employees\" + emp.ID_Employee + emp.SurName + ".txt"))
                {
                    Directory.CreateDirectory(@"Log\Employees");
                    File.Create(@"Log\Employees\" + emp.ID_Employee + emp.SurName + ".txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id=" + emp.ID_Employee);
                log.Add("Name=" + emp.Name);
                log.Add("SurName=" + emp.SurName);
                log.Add("Phone=" + emp.Phone);
                log.Add("Job=" + emp.Job);
                log.Add("Date_in=" + emp.Date_In);
                log.Add("Sallary=" + emp.Salary);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Employees\" + emp.ID_Employee + emp.SurName + ".txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Employees\" + emp.ID_Employee + emp.SurName + ".txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + emp.ID_Employee + emp.SurName);
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Requests req, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Request\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Request");
                    File.Create(@"Log\Request\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id=" + req.ID_Request);
                log.Add("Serial=" + req.Serial);
                log.Add("Type=" + req.Type);
                log.Add("Date_in=" + req.Date_In);
                log.Add("Date_out=" + req.Date_Out);
                log.Add("Price=" + req.Price);
                log.Add("Id_address=" + req.ID_Address);
                log.Add("Id_customer=" + req.ID_Customer);
                log.Add("Id_employee=" + req.ID_Employee);
                log.Add("Id_branch=" + req.ID_Branch);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Request\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Request\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Request\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Spare_parts spr, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Spare_Parts\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Spare_Parts");
                    File.Create(@"Log\Spare_Parts\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id=" + spr.ID_Part);
                log.Add("Serial=" + spr.Serial);
                log.Add("Name=" + spr.Name);
                log.Add("Price=" + spr.Price);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Request\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Spare_Parts\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Spare_Parts\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Addresses adr, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Address\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Address");
                    File.Create(@"Log\Address\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id=" + adr.ID_Address);
                log.Add("City=" + adr.City);
                log.Add("Street=" + adr.Street);
                log.Add("House№=" + adr.House_Num);
                log.Add("Room№=" + adr.Room_Num);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Address\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Address\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Address\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Work_Types WT, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Work_Types\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Work_Types");
                    File.Create(@"Log\Work_Types\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id=" + WT.ID_Type);
                log.Add("Type=" + WT.Type);
                log.Add("Description=" + WT.Description);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Work_Types\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Work_Types\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Work_Types\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Need_Parts NP, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Need_Parts\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Need_Parts");
                    File.Create(@"Log\Need_Parts\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id_Request=" + NP.ID_Request);
                log.Add("Id_Part=" + NP.ID_Part);
                log.Add("Ammount=" + NP.Used_Parts);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Need_Parts\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Need_Parts\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Need_Parts\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Parts_On_Branch PB, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Parts_On_Branch\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Parts_On_Branch");
                    File.Create(@"Log\Parts_On_Branch\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id_Branch=" + PB.ID_Branch);
                log.Add("Id_Part=" + PB.ID_Part);
                log.Add("Ammount=" + PB.Ammount);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Parts_On_Branch\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Parts_On_Branch\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Parts_On_Branch\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Branches PB, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Branches\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\Branches");
                    File.Create(@"Log\Branches\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id_Branch=" + PB.ID_Branch);
                log.Add("ID_Address=" + PB.ID_Address);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Branches\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Branches\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Branches\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(NewEmployeeWorks PB, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\EmployeeWorks\Log.txt"))
                {
                    Directory.CreateDirectory(@"Log\EmployeeWorks");
                    File.Create(@"Log\EmployeeWorks\Log.txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("Id_Employee=" + PB.id);
                var temp = "";
                foreach (var VARIABLE in PB.id_list)
                {
                    temp += VARIABLE;
                    if (VARIABLE != PB.id_list.Last())
                        temp += ", ";
                }
                log.Add("Id_Types=" + temp);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\EmployeeWorks\Log.txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\EmployeeWorks\Log.txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\EmployeeWorks\Log.txt");
                return 0;
            }
            return 1;
        }

        public int CurrentLogMethod(Customer cus, Employees who, Type operation)
        {
            try
            {
                if (!File.Exists(@"Log\Customer\" + cus.ID_Customer + cus.SurName + ".txt"))
                {
                    Directory.CreateDirectory(@"Log\Customer");
                    File.Exists(@"Log\Customer\" + cus.ID_Customer + cus.SurName + ".txt");
                }
                var log = new List<string>();
                log.Add("[" + DateTime.Now + "] ");
                if (operation == Type.Create)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " створив запис";
                else if (operation == Type.Update)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " оновив запис";
                else if (operation == Type.Delete)
                    log[0] += "Користувач " + who.Name + " " + who.SurName + " видалив запис";
                log.Add("ID=" + cus.ID_Customer);
                log.Add("Name=" + cus.Name);
                log.Add("Surname=" + cus.SurName);
                log.Add("Phone=" + cus.Phone);
                log.Add("=====================================================================================");
                if (!File.Exists(@"Log\Customer\" + cus.ID_Customer + cus.SurName + ".txt"))
                    Thread.Sleep(1000);
                File.AppendAllLines(@"Log\Customer\" + cus.ID_Customer + cus.SurName + ".txt", log);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося модифікувати файл " + @"Log\Customer\Log.txt");
                return 0;
            }
            return 1;
        }
    }
}