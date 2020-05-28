using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoverageReportBuilder
{
    public class ExplorationEngineTestCoverageBuilder
    {
        public static void Main(string[] args)
        {
            int numberOfFailures = 4;
            string downloadsFolder = $"{Environment.GetEnvironmentVariable("HOMEDRIVE")}{Environment.GetEnvironmentVariable("HOMEPATH")}\\Downloads";
            string csvCoverageFilePath = $"{downloadsFolder}\\ExplorationEngineTestCoverage.csv";
            string excelFilePath = $"{downloadsFolder}\\ExplorationEngineTestCoverage.xlsx";
            string stabeFailedFilePath = $"{downloadsFolder}\\StableFailedList.csv";
            Console.WriteLine($"\nHello {Environment.GetEnvironmentVariable("USERNAME")}, Which action would you like to perform?\n");
            Console.WriteLine(@"1 - Create Coverage report
2 - Prompt StableFailed test list
Other - Quit the program");
            Console.WriteLine();
            string action = Console.ReadLine();
            switch (action)
            {
                case "1":
                    {
                        if (!File.Exists(csvCoverageFilePath))
                        {
                            Console.WriteLine($"No file '{csvCoverageFilePath}' found under this location.");
                            return;
                        }
                        if (File.Exists(excelFilePath))
                        {
                            File.Delete(excelFilePath);
                            Console.WriteLine($"Deleting previous file '{excelFilePath}'");
                        }
                        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                        var worKbooK = excel.Workbooks.Add(Type.Missing);
                        var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                        worKsheeT.EnableAutoFilter = true;
                        try
                        {
                            excel.Visible = false;
                            excel.DisplayAlerts = false;
                            worKsheeT.Name = "RawData";
                            int rowCount = 2;
                            int colCount = 2;
                            worKsheeT.Cells[1, 1] = "Number";
                            worKsheeT.Columns["A"].AutoFit();
                            worKsheeT.Cells[1, 2] = "Name";
                            worKsheeT.Columns["B"].AutoFit();
                            worKsheeT.Cells[1, 3] = "Autor";
                            worKsheeT.Cells[1, 4] = "LastUpdate";
                            worKsheeT.Cells[1, 5] = "Subject";
                            worKsheeT.Cells[1, 6] = "SubSubject";
                            worKsheeT.Columns["F"].ColumnWidth = 50;
                            worKsheeT.Range["A1", "F1"].Interior.Color = 63850;
                            worKsheeT.Cells[1, 1].EntireRow.Font.Bold = true;
                            using (StreamReader reader = File.OpenText(csvCoverageFilePath))
                            {
                                string line = reader.ReadLine();// The first line is always "Coverage"
                                while ((line = reader.ReadLine()) != null)
                                {
                                    worKsheeT.Cells[rowCount, 1] = rowCount - 1;
                                    string[] spilltedLine = line.Split(',');
                                    foreach (var item in spilltedLine)
                                    {
                                        worKsheeT.Cells[rowCount, colCount] = item;
                                        colCount++;
                                    }
                                    colCount = 2;
                                    rowCount++;
                                }
                            }
                            worKsheeT.Columns["B"].ColumnWidth = 75;
                            worKsheeT.Columns["E"].AutoFit();
                            worKsheeT.Columns["C"].AutoFit();
                            worKsheeT.Columns["D"].AutoFit();
                            worKsheeT.Columns["G"].AutoFit();
                            worKbooK.SaveAs(excelFilePath);
                            worKbooK.Close();
                            excel.Quit();
                            Console.WriteLine($"Creating updated file '{excelFilePath}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            worKsheeT = null;
                            worKbooK = null;
                            Console.WriteLine("Work is Done :)");
                        }
                        break;
                    }
                case "2":
                    {
                        if (File.Exists(stabeFailedFilePath))
                        {
                            Console.WriteLine("What is the number of failurs that you want to have as upper bound?");
                            string upperBound =  Console.ReadLine();
                            if (upperBound.All(x => Char.IsDigit(x)) & upperBound != string.Empty)
                            {
                                numberOfFailures = Int32.Parse(upperBound);
                            }
                            Console.WriteLine("Stable Failed Test are: ...");
                            using (StreamReader reader = new StreamReader(stabeFailedFilePath))
                            {
                                List<string> failedTests = new List<string>();
                                string line = "Bibi";
                                while ((line = reader.ReadLine()) != null)
                                {
                                    failedTests.Add(line);
                                }
                                failedTests.Sort();
                                var distinctFailedTests = failedTests.Distinct();
                                foreach (var testName in distinctFailedTests)
                                {
                                    int failuresCount = failedTests.Where(x => x == testName).Count();
                                    if (failuresCount >= numberOfFailures)
                                    {
                                        Console.WriteLine($"{testName} - Failed {failuresCount} times");
                                    }
                                }
                            }
                            Console.WriteLine($"\nThat's it, {Environment.GetEnvironmentVariable("USERNAME")} ;)");
                            //Console.WriteLine($"Click any Key to exit");
                            //Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine($"No '{stabeFailedFilePath}' File to analize. Please supply a file.");
                            //Console.WriteLine($"Click any Key to exit");
                            //Console.ReadLine();
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Thank you {Environment.GetEnvironmentVariable("USERNAME")}, bye bye!");
                        break;
                    }                   
            }
            //
        }
    }
}
