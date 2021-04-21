using ReadFillesAsDatatable.Controller.LoadFileController;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadFillesAsDatatable
{
    class Program
    {  
        static void Main(string[] args)
        {
             
            var myList = new List<(string, string, int)>() { ("specifica_name_turbine.csv", ",", 1), ("name_sensor.csv", ",", 2), ("name_error_sensor.csv", ",", 3), ("Vestas Error Code List.csv", ";", 4) };
             var task = Directory.GetFiles("../ReadFillesAsDatatable/files/").ToList().Select((x) =>
             {
                 if (myList.Exists(element => x.Contains(element.Item1)))
                 {
                     var myitem = myList.First(name => x.Contains(name.Item1));
                     var res = Task.Run(() => {
                         //reading all the lines(rows) from the file.
                         Console.WriteLine("Task {0} running on thread {1}",
                                                   Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                         string[] rows = File.ReadAllLines(x);

                         DataTable rs = new();
                         string[] rowValues = null;
                         DataRow dr = rs.NewRow();

                         //Creating columns
                         if (rows.Length > 0)
                         {
                             foreach (string columnName in rows[0].Split(myitem.Item2))
                                 rs.Columns.Add(columnName);
                         }

                         //Creating row for each line.(except the first line, which contain column names)
                         for (int row = 1; row < rows.Length; row++)
                         {
                             rowValues = rows[row].Split(myitem.Item2);
                             dr = rs.NewRow();
                             dr.ItemArray = rowValues;
                             rs.Rows.Add(dr);
                         }


                         return (x, rs, myitem.Item3);
                     });
                     Console.WriteLine("ok2");
                     return res;
                 }
                 return default;
             }).Where(x => x != null).ToArray();

             Task.WaitAll(task);
             var final = task.ToList().Select(async (x) =>
             {
                 ILoadFileController loadFile = new LoadFileController();
                 FileInfo fi = new(x.Result.x);
                 await loadFile.ReadBasicFiles(x.Result.rs, fi.Name, fi.Extension, "", x.Result.Item3);
             }).ToArray();
             Console.WriteLine("ok");
             Task.WaitAll(final); 
            var task2= Directory.GetFiles("../ReadFillesAsDatatable/files/").ToList().Select((x) =>
            {
                if (!myList.Exists(element => x.Contains(element.Item1)))
                {
                    return Task.Run(() =>
                    {
                        DataTable rs = new();
                        Console.WriteLine("Task {0} running on thread {1}",
                                                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
                        using var odConnection = new OleDbConnection($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={x};Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");

                        odConnection.Open();
                        using OleDbCommand cmd = new();
                        cmd.Connection = odConnection;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Data Export$]";
                        using OleDbDataAdapter oleda = new(cmd);
                        {
                            oleda.Fill(rs);
                        }
                        odConnection.Close();
                        Console.WriteLine("ok2");
                        return (x, rs);
                    });
                }
                return default;
            }).Where(x=>x!=null).ToList().Select(task=> 
                            task.ContinueWith(async value => {
                                ILoadFileController loadFile = new LoadFileController();
                                FileInfo fi = new(value.Result.x);
                                await loadFile.ReadSensorTurbine(value.Result.rs, fi.Name, fi.Extension, "", false);
                            }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToArray();
            Console.WriteLine("ok"); 
            Task.WaitAll(task2);
            Console.ReadLine();
        }
    }
}
