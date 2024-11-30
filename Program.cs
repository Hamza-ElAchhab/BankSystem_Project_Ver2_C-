using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Bank_Project_Ver2
{
    internal class Program
    {
        public const string ClientsFile = "Clients.txt";
        public const string UsersFile = "Users.txt";
    
        struct st_ClientInfo
        {
            public string AcountNumber;
            public string PinCode;
            public string Name;
            public string Phone;
            public int Balance;
            public bool isMark;
        }
        enum en_MainMenu_Options
        {
            E_ClientsList = 1, E_Add = 2, E_Delete = 3, E_UpDate = 4, E_Find = 5, E_Transations_Menu = 6, E_ManageUsers_Menu = 7, E_LogOut = 8
        }
        static en_MainMenu_Options Read_Main_Menu_Option()
        {
            int num = 0;
            Console.Write("\n\tEnter Your Choice [1 -> 8] : ");
            num = Convert.ToInt16(Console.ReadLine());

            while (num > 8 || num < 1)
            {
                Console.Write("\n\n\tEnter Your Choice Between [1 -> 8] : ");
                num = Convert.ToInt16(Console.ReadLine());
            }

            return (en_MainMenu_Options)num;
        }
        static st_ClientInfo Fill_Struct_Data_From_Record(string record)
        {
            st_ClientInfo info = new st_ClientInfo();

            string[] arr = new string[5];
            arr = record.Split('#');

            info.AcountNumber = arr[0];
            info.PinCode = arr[1];
            info.Name = arr[2];
            info.Phone = arr[3];
            info.Balance = Convert.ToInt32(arr[4]);

            return info;
        }
        static List<st_ClientInfo> LoadData_FromFile_ToList()
        {
            List<st_ClientInfo> lData = new List<st_ClientInfo>();

            if (File.Exists(ClientsFile))
            {
                StreamReader MyFile = new StreamReader(ClientsFile);
                st_ClientInfo info;

                string Buffer;
                while ((Buffer = MyFile.ReadLine()) != null)
                {
                    info = Fill_Struct_Data_From_Record(Buffer);
                    info.isMark = false;
                    lData.Add(info);
                }

                MyFile.Close();
                return lData;
            }
            else
            {
                Console.WriteLine("\n");
            }
            return lData;
        }
        static void Print_One_Record_Info(st_ClientInfo info)
        {
            Console.WriteLine($"\t\t{info.AcountNumber}    | {info.PinCode}         | {info.Name}  |       {info.Phone}  |     {info.Balance}");
        }
        static void ClientsList()
        {
            if (!Check_Permission(en_Permissisons.E_List))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();

            Console.WriteLine($"\n\t\t\t\t\t   {lData.Count} Client(s).");
            Console.WriteLine("\t-----------------------------------------------------------------------------------");
            Console.WriteLine("\t Acount Number :|  PinCode :   |  Name :          |     Phone :         |  Balance ");
            Console.WriteLine("\t-----------------------------------------------------------------------------------");

            foreach (st_ClientInfo info in lData)
            {
                Print_One_Record_Info(info);
            }

            Console.WriteLine("\t-----------------------------------------------------------------------------------");
        }
        static void system_pause()
        {
            Console.Write("\t\n\nPress Any Key To Continue...");
            Console.ReadKey();
        }
        static string Join_Struct_Client_Info(st_ClientInfo info, char sep = '#')
        {
            string str = info.AcountNumber + sep;

            str += info.PinCode + sep;
            str += info.Name + sep;
            str += info.Phone + sep;
            str += (info.Balance).ToString();

            return str;
        }
        static bool isCleintExist_GetIt(string AcountNumber, st_ClientInfo[] info)
        {
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].AcountNumber == AcountNumber)
                {
                    info[0] = lData[i];
                    return true;
                }
            }
            return false;
        }
        static void SaveData_FromList_ToFile(List<st_ClientInfo> lData)
        {
            StreamWriter MyFile = new StreamWriter(ClientsFile);
            string Line;

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].isMark == false)
                {
                    Line = Join_Struct_Client_Info(lData[i]);
                    MyFile.WriteLine(Line);
                }
            }

            MyFile.Close();
        }
        static void Add_RecordLine_ToFile(string RecLine)
        {
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();

            lData.Add((Fill_Struct_Data_From_Record(RecLine)));

            SaveData_FromList_ToFile(lData);
        }
        static st_ClientInfo Read_StructCleint_Info(string AcountNumber)
        {
            st_ClientInfo info = new st_ClientInfo();

            Console.WriteLine("\n\n\t________ Read Info __________");

            info.AcountNumber = AcountNumber;
            Console.Write("\t  Enter Pin Code      : ");
            info.PinCode = Console.ReadLine();
            Console.Write("\t  Enter Your Name     : ");
            info.Name = Console.ReadLine();
            Console.Write("\t  Enter Your Phone    : ");
            info.Phone = Console.ReadLine();
            Console.Write("\t  Enter Your Balance  : ");
            info.Balance = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\t______________________________");

            return info;
        }
        static string Read_AcountNumber()
        {
            string AcountN;
            Console.Write("\n\n\tEnter an Acount Number : ");
            AcountN = Console.ReadLine();

            return AcountN;
        }
        static void AddClient()
        {
            if (!Check_Permission(en_Permissisons.E_Add))
            {
                AccessErrorMessage();
                return;
            }

            char ans = 'n';
            do
            {
                Console.Clear();
                Console.WriteLine("\t________________________");
                Console.WriteLine("\t   Add Client Screen");
                Console.WriteLine("\t________________________");

                string acountnumber = Read_AcountNumber();
                st_ClientInfo[] info = new st_ClientInfo[1];

                while (isCleintExist_GetIt(acountnumber, info))
                {
                    Console.Write("\nAcount Number Already Exists, Enter Another one : ");
                    acountnumber = Console.ReadLine();
                }

                Add_RecordLine_ToFile(Join_Struct_Client_Info(Read_StructCleint_Info(acountnumber)));
                Console.WriteLine("\nAdded Succesfully.\n");

                Console.Write("\nDo You Want To Add More [Y/N] : ");
                ans = Convert.ToChar(Console.ReadLine());

            } while (char.ToUpper(ans) == 'Y');
        }
        static void EndProject()
        {
            Console.Clear();
            Console.WriteLine("\n\t_________________________");
            Console.WriteLine("\t     End Program");
            Console.WriteLine("\t-------------------------\n");
        }
        static void Print_Client_Card(st_ClientInfo info)
        {
            Console.WriteLine("\n\n________ Client :________");
            Console.WriteLine("Acount Num : {0}", info.AcountNumber);
            Console.WriteLine("Pin Code   : {0}", info.PinCode);
            Console.WriteLine("Name       : {0}", info.Name);
            Console.WriteLine("Phone      : {0}", info.Phone);
            Console.WriteLine("Balance    : {0}", info.Balance);
            Console.WriteLine("_________________________");
        }
        static st_ClientInfo get_mark_true(st_ClientInfo info)
        {
            info.isMark = true;
            return info;
        }
        static List<st_ClientInfo> Mark_Client_For_Delete(string acountnumber)
        {
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].AcountNumber == acountnumber)
                {
                    lData[i] = get_mark_true(lData[i]);
                    break;
                }
            }
            return lData;
        }
        static void DeleteClient()
        {
            if (!Check_Permission(en_Permissisons.E_Delete))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            Console.WriteLine("\t________________________");
            Console.WriteLine("\t Delete Client Screen");
            Console.WriteLine("\t________________________");

            string AcountNum = Read_AcountNumber();
            st_ClientInfo[] info = new st_ClientInfo[1];
            char ans = 'n';

            if (isCleintExist_GetIt(AcountNum, info))
            {
                Print_Client_Card(info[0]);

                Console.Write("\nAre You Sure [Y/N] : ");
                ans = Convert.ToChar(Console.ReadLine());

                if (char.ToUpper(ans) == 'Y')
                {
                    List<st_ClientInfo> lData = Mark_Client_For_Delete(AcountNum);
                    SaveData_FromList_ToFile(lData);
                    Console.WriteLine("\n\nDeleted Succesfully.\n");
                }
            }
            else
            {
                Console.WriteLine($"\nClient With {AcountNum} Does Not Exists.\n");
            }
        }
        static void UpDate_ByAcountNumber(string AcountNumber)
        {
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].AcountNumber == AcountNumber)
                {
                    lData[i] = Read_StructCleint_Info(AcountNumber);
                    break;
                }
            }
            SaveData_FromList_ToFile(lData);
        }
        static void UpDateClient()
        {
            if (!Check_Permission(en_Permissisons.E_UpDate))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            Console.WriteLine("\t________________________");
            Console.WriteLine("\t UpDate Client Screen");
            Console.WriteLine("\t________________________");

            string AcountNum = Read_AcountNumber();
            st_ClientInfo[] info = new st_ClientInfo[1];
            char ans = 'n';

            if (isCleintExist_GetIt(AcountNum, info))
            {
                Print_Client_Card(info[0]);

                Console.Write("\nAre You Sure [Y/N] : ");
                ans = Convert.ToChar(Console.ReadLine());

                if (char.ToUpper(ans) == 'Y')
                {
                    UpDate_ByAcountNumber(AcountNum);
                    Console.WriteLine("\nUpDated Successfully.\n");
                }
            }
            else
            {
                Console.WriteLine($"\nClient With {AcountNum} Does Not Exists.\n");
            }
        }
        static void FindClient()
        {
            if (!Check_Permission(en_Permissisons.E_Find))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            Console.WriteLine("\t________________________");
            Console.WriteLine("\t Find Client Screen");
            Console.WriteLine("\t________________________");

            string AcountNum = Read_AcountNumber();
            st_ClientInfo[] info = new st_ClientInfo[1];

            if (isCleintExist_GetIt(AcountNum, info))
            {
                Print_Client_Card(info[0]);
            }
            else
            {
                Console.WriteLine($"\nClient With {AcountNum} Does Not Exists.\n");
            }
        }
        static void Woking_Main_Menu(en_MainMenu_Options option)
        {
            switch (option)
            {

                case en_MainMenu_Options.E_ClientsList:
                    ClientsList();
                    system_pause();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_Add:
                    AddClient();
                    system_pause();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_LogOut:
                    LogIn();
                    break;


                case en_MainMenu_Options.E_Delete:
                    DeleteClient();
                    system_pause();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_UpDate:
                    UpDateClient();
                    system_pause();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_Find:
                    FindClient();
                    system_pause();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_Transations_Menu:
                    Transactions_Menu();
                    MainMenu();
                    break;


                case en_MainMenu_Options.E_ManageUsers_Menu:
                    ManageUser_MainMenu();
                    MainMenu();
                    break;



                default:
                    Console.WriteLine("\nWrong Choice....\n");
                    break;
            }
        }
        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("\t\t\t  Version 2 C#\n\n");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t\tMain Menu Screen :");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t      [1] : Show Clients List.");
            Console.WriteLine("\t\t      [2] : Add New Client.");
            Console.WriteLine("\t\t      [3] : Delete Client.");
            Console.WriteLine("\t\t      [4] : UpDate Client.");
            Console.WriteLine("\t\t      [5] : Find Client.");
            Console.WriteLine("\t\t      [6] : Transations Menu.");
            Console.WriteLine("\t\t      [7] : Manage Users.");
            Console.WriteLine("\t\t      [8] : LogOut.");
            Console.WriteLine("\t=================================================");

            Woking_Main_Menu(Read_Main_Menu_Option());
        }
        /////
        enum en_TransationsMenu_Options
        {
            e_Deposite = 1, e_WithDraw = 2, e_TotalBalances = 3, e_GoBack = 4
        }
        static en_TransationsMenu_Options Read_TransationsMenu_Option()
        {
            int num = 0;
            Console.Write("\n\t\tEnter Your Choice [1 -> 4] : ");
            num = Convert.ToInt32(Console.ReadLine());

            while (num > 4 || num < 1)
            {
                Console.Write("\n\n\tEnter Your Choice Between [1 -> 4] : ");
                num = Convert.ToInt16(Console.ReadLine());
            }

            return (en_TransationsMenu_Options)num;
        }
        static st_ClientInfo Increase_Decrease_By_StructInfo(st_ClientInfo info, int amount)
        {
            info.Balance += amount;
            return info;
        }
        static void Increase_And_Decrease_BalanceOfClient(String Acount, int Amount)
        {
            List<st_ClientInfo> lData = LoadData_FromFile_ToList();
            int NewBalance = 0;

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].AcountNumber == Acount)
                {
                    lData[i] = Increase_Decrease_By_StructInfo(lData[i], Amount);
                    NewBalance = lData[i].Balance;
                    break;
                }
            }

            Console.WriteLine($"\nNew Balance is {NewBalance}");
            SaveData_FromList_ToFile(lData);
        }
        static void Deposite()
        {
            Console.Clear();
            Console.WriteLine("\t-------------------------------");
            Console.WriteLine("\t\tDeposite Screen : ");
            Console.WriteLine("\t-------------------------------\n");

            string AcountNum;
            Console.Write("\n\tEnter An Acount Number : ");
            AcountNum = Console.ReadLine();
            st_ClientInfo[] info = new st_ClientInfo[1];

            while (!isCleintExist_GetIt(AcountNum, info))
            {
                Console.Write($"\nAcount With {AcountNum} Does not Exists\nEnter An Acount Number Again : ");
                AcountNum = Console.ReadLine();
            }

            Print_Client_Card(info[0]);

            int Amount;
            Console.Write("\n\n\tEnter positive Deposite Amount : ");
            Amount = Convert.ToInt32(Console.ReadLine());

            while (Amount < 0)
            {
                Console.Write("\n\tEnter positive Deposite Amount : ");
                Amount = Convert.ToInt32(Console.ReadLine());
            }

            char ans = 'n';
            Console.Write("\nAre You Sure [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());

            if (char.ToUpper(ans) == 'Y')
            {
                Increase_And_Decrease_BalanceOfClient(AcountNum, Amount);
                Console.WriteLine("Done.");
            }
        }
        static void WithDraw()
        {
            Console.Clear();
            Console.WriteLine("\t-------------------------------");
            Console.WriteLine("\t\tWithDraw Screen : ");
            Console.WriteLine("\t-------------------------------\n");

            string AcountNum;
            Console.Write("\n\tEnter An Acount Number : ");
            AcountNum = Console.ReadLine();
            st_ClientInfo[] info = new st_ClientInfo[1];

            while (!isCleintExist_GetIt(AcountNum, info))
            {
                Console.Write($"\nAcount With {AcountNum} Does not Exists\nEnter An Acount Number Again : ");
                AcountNum = Console.ReadLine();
            }
            Print_Client_Card(info[0]);

            int Amount;
            Console.Write("\n\n\tEnter positive WithDraw Amount : ");
            Amount = Convert.ToInt32(Console.ReadLine());

            while (Amount < 0)
            {
                Console.Write("\n\tEnter positive WithDraw Amount : ");
                Amount = Convert.ToInt32(Console.ReadLine());
            }

            while (Amount > info[0].Balance || Amount < 0)
            {
                Console.Write("\n\tAcount Balance is Less Than Amount,\n\tEnter Again positive WithDraw : ");
                Amount = Convert.ToInt32(Console.ReadLine());
            }

            char ans = 'n';
            Console.Write("\nAre You Sure [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());

            if (char.ToUpper(ans) == 'Y')
            {
                Increase_And_Decrease_BalanceOfClient(AcountNum, Amount * (-1));
                Console.WriteLine("Done.");
            }
        }
        static void Print_One_Record_Info_TotalBalances(st_ClientInfo info)
        {
            Console.WriteLine($"\t\t\t{info.AcountNumber}           | {info.Name}            |        {info.Balance}");
        }
        static void TotalBalancesList()
        {
            Console.Clear();

            List<st_ClientInfo> lData = LoadData_FromFile_ToList();
            int TotalBalances = 0;
            for (int i = 0; i < lData.Count; i++)
            {
                TotalBalances += lData[i].Balance;
            }

            Console.WriteLine($"\n\t\t\t\t\t   {lData.Count} Client(s).");
            Console.WriteLine("\t---------------------------------------------------------------------------------");
            Console.WriteLine("\t\tAcount Number :        |  Name :                    |  Balance : ");
            Console.WriteLine("\t---------------------------------------------------------------------------------");

            foreach (st_ClientInfo info in lData)
            {
                Print_One_Record_Info_TotalBalances(info);
            }

            Console.WriteLine("\t---------------------------------------------------------------------------------");
            Console.WriteLine($"\n\t\t\t\tTotal Balances : {TotalBalances}");
            Console.WriteLine("\t---------------------------------------------------------------------------------");
        }
        static void Working_Transactions_Menu(en_TransationsMenu_Options option)
        {
            switch (option)
            {

                case en_TransationsMenu_Options.e_GoBack:
                    MainMenu();
                    break;


                case en_TransationsMenu_Options.e_Deposite:
                    Deposite();
                    system_pause();
                    Transactions_Menu();
                    break;


                case en_TransationsMenu_Options.e_WithDraw:
                    WithDraw();
                    system_pause();
                    Transactions_Menu();
                    break;


                case en_TransationsMenu_Options.e_TotalBalances:
                    TotalBalancesList();
                    system_pause();
                    Transactions_Menu();
                    break;


                default:
                    Console.WriteLine("\nWrong Choice.\n");
                    break;
            }
        }
        static void Transactions_Menu()
        {
            if (!Check_Permission(en_Permissisons.E_Transations_Menu))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            Console.WriteLine("\t\t\t  Version 2 C#\n\n");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t   Transactions Menu Screen :");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t      [1] : Deposite.");
            Console.WriteLine("\t\t      [2] : WithDarw.");
            Console.WriteLine("\t\t      [3] : Total Balances.");
            Console.WriteLine("\t\t      [4] : Go Back To Main Menu.");
            Console.WriteLine("\t=================================================");

            Working_Transactions_Menu(Read_TransationsMenu_Option());
        }
        ////
        struct st_User_Info
        {
            public string UserName;
            public string PassWord;
            public int Permssion;
            public bool isMark;
        }

        static st_User_Info CurrentUser;
        enum en_ManageUsers_Options
        {
            e_UsersList = 1, e_AddUser =2, e_DeleteUser = 3, e_UpDateUser = 4, e_FindUser = 5, e_BackToMainMenu = 6
        }
        static en_ManageUsers_Options Read_ManageUsers_Choice()
        {
            int num = 0;
            Console.Write("\n\tEnter Your Choice [1 -> 6] : ");
            num = Convert.ToInt32(Console.ReadLine());

            while (num > 6 || num < 1)
            {
                Console.Write("\n\n\tEnter Your Choice Between [1 -> 6] : ");
                num = Convert.ToInt16(Console.ReadLine());
            }

            return (en_ManageUsers_Options)num;
        }
        static string Read_UserName()
        {
            string name;
            Console.Write("\n\tEnter User Name : ");
            name = Console.ReadLine();
            return name;
        }
        static st_User_Info Fill_UserStruct_By_Line(string Line)
        {
            st_User_Info info = new st_User_Info();
            string[] arr = new string[3];

            arr = Line.Split('#');

            info.UserName = arr[0];
            info.PassWord = arr[1];
            info.Permssion = Convert.ToInt32(arr[2]);

            return info;
        }
        static List<st_User_Info> Load_Users_Data_FromFile()
        {
            List<st_User_Info> lData = new List<st_User_Info>();
            st_User_Info info;  
           
            if (File.Exists(UsersFile))
            {
                StreamReader MyFile = new StreamReader(UsersFile);
                string RecLine;

                while ((RecLine = MyFile.ReadLine()) != null)
                {
                    info = Fill_UserStruct_By_Line(RecLine);
                    info.isMark = false;

                    lData.Add(info);
                }

                MyFile.Close();
                return lData;
            }

            return lData;
        }
        static bool isUserExists_GetIt(string UserName, st_User_Info[] arr)
        {
            List<st_User_Info> lData = Load_Users_Data_FromFile();

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].UserName == UserName)
                {
                    arr[0] = lData[i];
                    return true;
                }
            }
            return false;
        }
        static st_User_Info Read_User_StructInfo()
        {
            st_User_Info info = new st_User_Info();

            Console.WriteLine("\nReading Info : ");
            Console.WriteLine("---------------------------------");

            Console.Write("Enter User Name : ");
            info.UserName = Console.ReadLine();

            st_User_Info[] arr = new st_User_Info[1];
            while (isUserExists_GetIt(info.UserName, arr))
            {
                Console.Write("User Name Already Exists, Enter Again : ");
                info.UserName = Console.ReadLine();
            }

            Console.Write("Enter Pass Word : ");
            info.PassWord = Console.ReadLine();

            info.Permssion = Read_Permissions();

            Console.WriteLine("---------------------------------\n");

            return info;
        }
        static string Join_Struct_UserInfo_ToString(st_User_Info info, string sep = "#")
        {
            string str = info.UserName + sep;
            str += info.PassWord + sep;
            str += (info.Permssion).ToString();

            return str;
        }
        static void Add_RecordUserLine_ToFile(string RecLine)
        {
            List<st_User_Info> lData = Load_Users_Data_FromFile();
            lData.Add(Fill_UserStruct_By_Line(RecLine));

            StreamWriter MyFile = new StreamWriter(UsersFile);
            for (int i = 0; i < lData.Count; i++)
            {
                MyFile.WriteLine(Join_Struct_UserInfo_ToString(lData[i]));
            }   
            MyFile.Close();
        }
        static void AddUser()
        {
            char ans = 'n';

            do
            {
                Console.Clear();
                Console.WriteLine("\n\t\t=================================");
                Console.WriteLine("\t\t      Add User Screen : ");
                Console.WriteLine("\t\t=================================\n");

                Add_RecordUserLine_ToFile(Join_Struct_UserInfo_ToString(Read_User_StructInfo()));

                Console.Write("\n\nDo You Want To Add More [Y/N] : ");
                ans = Convert.ToChar(Console.ReadLine());

            } while (char.ToUpper(ans) == 'Y');
        }
        static void Print_User_Card(st_User_Info info)
        {
            Console.WriteLine("\n\t------User Card:--------");
            Console.WriteLine($"\tUser Name : {info.UserName}");
            Console.WriteLine($"\tPass Word : {info.PassWord}");
            Console.WriteLine($"\tPermission: {info.Permssion}");
            Console.WriteLine("\t------------------------\n\n");
        }
        static st_User_Info Mark_User_Info(st_User_Info info)
        {
            info.isMark = true;
            return info;
        }
        static List<st_User_Info> Mark_User_For_Delete(string UserName)
        {
            List<st_User_Info> lData = Load_Users_Data_FromFile();

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].UserName == UserName)
                {
                    lData[i] = Mark_User_Info(lData[i]);
                    break;
                }
            }
            return lData;
        }
        static void Sava_ListData_ToFile(List<st_User_Info> lData)
        {
            StreamWriter MyFile = new StreamWriter(UsersFile);
            string RecLine = "";

            for (int i = 0; i < lData.Count; i++)
            {
                if (lData[i].isMark == false)
                {
                    RecLine = Join_Struct_UserInfo_ToString(lData[i]);
                    MyFile.WriteLine(RecLine);
                }
            }
            MyFile.Close();
        }
        static void DeleteUser()
        {
            Console.Clear();
            Console.WriteLine("\n\t\t=================================");
            Console.WriteLine("\t\t    Delete User Screen : ");
            Console.WriteLine("\t\t=================================\n");

            string UserName = Read_UserName();
            st_User_Info[] info = new st_User_Info[1];
            List<st_User_Info> lData;

            char ans = 'n';

            if (isUserExists_GetIt(UserName, info))
            {
                Print_User_Card(info[0]);
                Console.Write("\tAre You Sure [Y/N] : ");

                ans = Convert.ToChar(Console.ReadLine());

                if (char.ToUpper(ans) == 'Y')
                {
                    if (UserName == "Admin")
                    {
                        AccessErrorMessage();
                        return;
                    }
                    lData = Mark_User_For_Delete(UserName);
                    Sava_ListData_ToFile(lData);
                    Console.WriteLine("\n\tDeleted Successfully.\n");
                }
            }
            else
            {
                Console.WriteLine($"\nUser With {UserName} Does not Exists.\n");
            }
        }
        static st_User_Info Read_NewUpdate_UserInfo(string UserName)
        {
            st_User_Info info = new st_User_Info();
            info.UserName = UserName;

            Console.WriteLine("\n-------------------------");
            Console.Write("Enter PassWord    : ");
            info.PassWord = Console.ReadLine();
            Console.Write("Enter Permissions : ");
            info.Permssion = Read_Permissions();
            Console.WriteLine("-------------------------\n");

            return info;
        }
        static void UpDateUser()
        {
            Console.Clear();
            Console.WriteLine("\n\t\t=================================");
            Console.WriteLine("\t\t    UpDate User Screen : ");
            Console.WriteLine("\t\t=================================\n");

            string UserName = Read_UserName();
            st_User_Info[] info = new st_User_Info[1];
            char ans = 'n';


            if (isUserExists_GetIt(UserName, info))
            {
                Print_User_Card(info[0]);
                Console.Write("\tAre You Sure [Y/N] : ");
                ans = Convert.ToChar(Console.ReadLine());

                if (char.ToUpper(ans) == 'Y')
                {
                    List<st_User_Info> lData = Load_Users_Data_FromFile();

                    for (int i = 0; i < lData.Count; i++)
                    {
                        if (lData[i].UserName == UserName)
                        {
                            lData[i] = Read_NewUpdate_UserInfo(UserName);
                            break;
                        }
                    }
                    Sava_ListData_ToFile(lData);
                    Console.WriteLine("\nUser UpDated Successfully.\n");
                }
            }
            else
            {
                Console.WriteLine($"\nUser Name with {UserName} Does not exists.\n");
            }
        }
        static void FindUser()
        {
            Console.Clear();
            Console.WriteLine("\n\t\t=================================");
            Console.WriteLine("\t\t      Find User Screen : ");
            Console.WriteLine("\t\t=================================\n");

            string UserName = Read_UserName();
            st_User_Info[] info = new st_User_Info[1];


            while (!(isUserExists_GetIt(UserName, info)))
            {
                Console.WriteLine($"\nUser Name with {UserName} Does not exists.");
                Console.Write("Enter Another One : ");
                UserName = Console.ReadLine();
            }

            Console.WriteLine("\n\n\t   User Information : ");
            Print_User_Card(info[0]);
        }
        static void Print_One_Record_UserInfo(st_User_Info info)
        {
            Console.WriteLine($"\t\t\t  {info.UserName}            |           {info.PassWord}         |         {info.Permssion} ");
        }
        static void UsersList()
        {
            Console.Clear();
            List<st_User_Info> lData = Load_Users_Data_FromFile();

            if (lData.Count == 0)
            {
                Console.WriteLine($"\n\t\t\t\t\t   There is no User.");
            }
            else
            {
                Console.WriteLine($"\n\t\t\t\t\t\t{lData.Count} User(s).");
            }

            Console.WriteLine("\t\t----------------------------------------------------------------------------");
            Console.WriteLine("\t\t\tUser Name :        |         Pass Word :    |      Permissions :     ");
            Console.WriteLine("\t\t----------------------------------------------------------------------------");

            foreach (st_User_Info info in lData)
            {
                Print_One_Record_UserInfo(info);
            }

            Console.WriteLine("\t\t----------------------------------------------------------------------------");
        }
        static void Working_ManageUser(en_ManageUsers_Options option)
        {
            switch(option)
            {

                case en_ManageUsers_Options.e_BackToMainMenu:
                    MainMenu();
                    break;


                case en_ManageUsers_Options.e_AddUser:
                    AddUser();
                    system_pause();
                    ManageUser_MainMenu();
                    break;


                case en_ManageUsers_Options.e_DeleteUser:
                    DeleteUser();
                    system_pause();
                    ManageUser_MainMenu();
                    break;


                case en_ManageUsers_Options.e_UpDateUser:
                    UpDateUser();
                    system_pause();
                    ManageUser_MainMenu();
                    break;


                case en_ManageUsers_Options.e_FindUser:
                    FindUser();
                    system_pause();
                    ManageUser_MainMenu();
                    break;


                case en_ManageUsers_Options.e_UsersList:
                    UsersList();
                    system_pause();
                    ManageUser_MainMenu();
                    break;


                default:
                    Console.WriteLine("\nWrong Choice\n");
                    break;

            }
        }
        static void ManageUser_MainMenu()
        {
            if (!Check_Permission(en_Permissisons.E_ManageUsers_Menu))
            {
                AccessErrorMessage();
                return;
            }

            Console.Clear();
            Console.WriteLine("\t\t\t  Version 2 C#\n\n");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t    Manage Users Menu Screen :");
            Console.WriteLine("\t=================================================");
            Console.WriteLine("\t\t      [1] : Show Users List.");
            Console.WriteLine("\t\t      [2] : Add New User.");
            Console.WriteLine("\t\t      [3] : Delete User.");
            Console.WriteLine("\t\t      [4] : UpDate User.");
            Console.WriteLine("\t\t      [5] : Find User.");
            Console.WriteLine("\t\t      [6] : Back To Main Menu.");
            Console.WriteLine("\t=================================================");

            Working_ManageUser(Read_ManageUsers_Choice());
        }

        enum en_Permissisons
        {
            e_FullAccess = -1, E_List = 1, E_Add = 2, E_Delete = 4, E_UpDate = 8, E_Find = 16, E_Transations_Menu = 32, E_ManageUsers_Menu = 64
        }
        static int Read_Permissions()
        {
            int total_permissions = 0;

            char ans = 'n';
            Console.Write("\nDo You Want To Give FULL ACCESS [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                return (int)en_Permissisons.e_FullAccess;
            }

            Console.WriteLine("\n\nReding Permissions : ");
            Console.WriteLine("---------------------------");

            Console.Write("Give Access To Clients List [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_List;
            }

            Console.Write("Give Access To Add Clinets [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_Add;
            }

            Console.Write("Give Access To Delete Clinets [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_Delete;
            }

            Console.Write("Give Access To UpDate Clinets [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_UpDate;
            }

            Console.Write("Give Access To Find Clinets [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_Find;
            }

            Console.Write("Give Access To Transactions Menu [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_Transations_Menu;
            }

            Console.Write("Give Access To Manage Users [Y/N] : ");
            ans = Convert.ToChar(Console.ReadLine());
            if (char.ToUpper(ans) == 'Y')
            {
                total_permissions += (int)en_Permissisons.E_ManageUsers_Menu;
            }

            return total_permissions;
        }
        static void AccessErrorMessage()
        {
            Console.Clear();
            Console.WriteLine("\n\t\t----------------------------");
            Console.WriteLine("\t\t You Don't Have Permission.");
            Console.WriteLine("\t\t----------------------------\n");
            Console.Write("OK ...");
            Console.ReadKey();
        }
        static bool Check_Permission(en_Permissisons PerNumber)
        {
            int NUmber_Permission_ToEnter_casting = (int)PerNumber;

            if (CurrentUser.Permssion == (int)en_Permissisons.e_FullAccess)
            {
                return true;
            }

            if ((NUmber_Permission_ToEnter_casting & CurrentUser.Permssion) == NUmber_Permission_ToEnter_casting)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool FindUser_By_UserName_PassWord(string UserName, string PassWord)
        {
            List<st_User_Info> ListOfData = Load_Users_Data_FromFile();

            for (int i = 0; i < ListOfData.Count; i++)
            {
                if (ListOfData[i].UserName == UserName && ListOfData[i].PassWord == PassWord)
                {
                    CurrentUser = ListOfData[i];
                    return true;
                }
            }
            return false;
        }

        static void LogIn()
        {
            Console.Clear();
            Console.WriteLine("\t---------------------------");
            Console.WriteLine("\t\tLonin Screen : ");
            Console.WriteLine("\t---------------------------\n");

            string UserName, PassWord;

            Console.Write("\n\tEnter User Name : ");
            UserName = Console.ReadLine();

            Console.Write("\tEnter Pass Word : ");
            PassWord = Console.ReadLine();

            if (FindUser_By_UserName_PassWord(UserName, PassWord))
            {
                MainMenu();
            }
            else
            {
                Console.Write("\nWrong UserName or PassWord, Enter To Read Again ...");
                Console.ReadKey();
                LogIn();
            }      
        }


        static void Main(string[] args)
        {


            LogIn();

           

            Console.ReadKey();
        }
    }
}