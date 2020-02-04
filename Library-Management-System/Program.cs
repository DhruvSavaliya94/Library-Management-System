using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System
{
    class Program
    {
        static string s = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Semester-6\DNT\Mini Project\Library-Management-System\Library-Management-System\LibraryData.mdf;Integrated Security=True";
        static SqlDataAdapter ad;
        static SqlConnection sc = new SqlConnection(s);
        static SqlCommand cmd;
        static DataTable dt = new DataTable();
        static string query;
        static bool executeDMLQuery(string query)
        {
            cmd = new SqlCommand(query, sc);
            try
            {
                sc.Open();
                cmd.ExecuteNonQuery();
                sc.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message);
                sc.Close();
            }
            return false;
        }
        static DataTable getData(string query)
        {
            dt = new DataTable();
            ad = new SqlDataAdapter(query, sc);
            ad.Fill(dt);
            return dt;
        }
        static void getBookData()
        {
            query = "select * from Books";
            dt = getData(query);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine("ID:{0}", dt.Rows[i][0]);
                    Console.WriteLine("Name:{0}", dt.Rows[i][1]);
                    Console.WriteLine("No of Books:{0}", dt.Rows[i][2]);
                    Console.WriteLine("Available Books:{0}", dt.Rows[i][3]);
                    Console.WriteLine("------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("Record not found");
            }
        }
        static void getStudentData()
        {
            query = "select * from Student";
            dt = getData(query);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine("ID:{0}", dt.Rows[i][0]);
                    Console.WriteLine("Name:{0}", dt.Rows[i][1]);
                    Console.WriteLine("Email:{0}", dt.Rows[i][2]);
                    Console.WriteLine("Contact:{0}", dt.Rows[i][3]);
                    Console.WriteLine("Issued Books:{0}", dt.Rows[i][4]);
                    Console.WriteLine("------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("Record not found");
            }
        }
        static void updateIssuedBookTable(int sid, int bid)
        {
            if (checkStudent(sid) && checkBook(bid))
            {
                query = "insert into IssuedBook values(" + sid + "," + bid + ",'" + DateTime.Now + "')";
                if (executeDMLQuery(query))
                {
                    Console.WriteLine("Record Inserted..");
                }
                else
                {
                    Console.WriteLine("Record not Inserted..");
                }
            }
            else
            {
                Console.WriteLine("Student or Book does not exist.");
            }
        }
        static void issueBook(int sid, int bid)
        {
            query = "select * from IssuedBook where StudentId=" + sid + " and BookId=" + bid;
            dt = getData(query);
            int match = dt.Rows.Count;
            if (checkStudent(sid) && checkBook(bid) && match == 0)
            {
                //Issue book
                query = "select noOfBook from Student where ID=" + sid;
                dt = getData(query);
                if (Convert.ToInt32(dt.Rows[0][0]) < 3)
                {
                    updateIssuedBookTable(sid, bid);
                    query = "update Student set noOfBook=noOfBook+1 where Id=" + sid;
                    executeDMLQuery(query);
                    query = "update Books set availableBooks=availableBooks-1 where Id=" + bid;
                    executeDMLQuery(query);
                }
                else
                {
                    Console.WriteLine("You Can't Issue more than 3 Book or You are trying to issue same book again.");
                }
            }
            else
            {
                Console.WriteLine("Student or Book does not exist or Student already issued that book.");
            }
        }
        static void getIssuedData()
        {
            query = "select * from IssuedBook";
            dt = getData(query);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine("ID:{0}", dt.Rows[i][0]);
                    Console.WriteLine("Student Id:{0}", dt.Rows[i][1]);
                    Console.WriteLine("Book Id:{0}", dt.Rows[i][2]);
                    Console.WriteLine("Date:{0}", dt.Rows[i][3]);
                    Console.WriteLine("------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("Record not found");
            }
        }
        public static void returnBook(int sid, int bid)
        {
            if (checkStudent(sid) && checkBook(bid))
            {
                query = "select * from IssuedBook where StudentId=" + sid + " and " + "BookId=" + bid;
                dt = getData(query);
                if (dt.Rows.Count > 0)
                {
                    query = "delete from IssuedBook where StudentId=" + sid + " and " + "BookId=" + bid;
                    executeDMLQuery(query);
                    query = "update Student set noOfBook=noOfBook-1 where Id=" + sid;
                    executeDMLQuery(query);
                    query = "update Books set availableBooks=availableBooks+1 where Id=" + bid;
                    executeDMLQuery(query);
                }
                else
                {
                    Console.WriteLine("Record not found");
                }
            }
            else
            {
                Console.WriteLine("Student or Book does not exist.");
            }
        }
        public static bool getIssueBookData(int sid)
        {
            if (checkStudent(sid))
            {
                query = "select * from IssuedBook where StudentId=" + sid;
                dt = getData(query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Console.WriteLine("ID:{0}", dt.Rows[i][0]);
                        Console.WriteLine("Student Id:{0}", dt.Rows[i][1]);
                        Console.WriteLine("Book Id:{0}", dt.Rows[i][2]);
                        Console.WriteLine("Date:{0}", dt.Rows[i][3]);
                        Console.WriteLine("------------------------------------------------------------");
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("You have not issued any book.");
                }
            }
            else
            {
                Console.WriteLine("Student not found...");
            }
            return false;
        }
        public static bool checkStudent(int sid)
        {
            query = "select Id from Student where Id=" + sid;
            dt = getData(query);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public static bool checkBook(int bid)
        {
            query = "select Id from Books where Id=" + bid;
            dt = getData(query);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            int choice;
            do
            {
                Console.WriteLine("\nEnter Your Choice:\n1.Issue Book\n2.Book Data\n3.Student Data\n4.Issued Data\n5.Return Book\n6.Exit");
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        int sid, bid;
                        getStudentData();
                        Console.WriteLine("Enter Student ID:");
                        sid = Convert.ToInt32(Console.ReadLine());
                        getBookData();
                        Console.WriteLine("Enter Book ID:");
                        bid = Convert.ToInt32(Console.ReadLine());
                        issueBook(sid, bid);
                        break;
                    case 2:
                        getBookData();
                        break;
                    case 3:
                        getStudentData();
                        break;
                    case 4:
                        getIssuedData();
                        break;
                    case 5:
                        int _sid, _bid;
                        getStudentData();
                        Console.WriteLine("Enter Student ID:");
                        _sid = Convert.ToInt32(Console.ReadLine());
                        if (getIssueBookData(_sid))
                        {
                            Console.WriteLine("Enter Book ID:");
                            _bid = Convert.ToInt32(Console.ReadLine());
                            returnBook(_sid, _bid);
                        }
                        break;
                    case 6:
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please enter valid choice...");
                        break;
                }
            } while (true);
        }
    }
}
