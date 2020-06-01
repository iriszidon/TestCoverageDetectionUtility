using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExplorationEngine.Automation.Tests.Infra
{
    public class CoverageReportBuilder
    {
        private readonly static string downloadsFolder = $"{Environment.GetEnvironmentVariable("HOMEDRIVE")}{Environment.GetEnvironmentVariable("HOMEPATH")}\\Downloads";
        public readonly string LogFilePath = $"{downloadsFolder}\\ExplorationEngineTestCoverage.csv";
        public readonly string PreviousTestNameFilePath = $"{downloadsFolder}\\LastTestName.csv";
        public readonly string StableFailedFilePath = $"{downloadsFolder}\\StableFailedList.csv";
        List<string> FrequentlyFailedTests;
        Queue<string> FrequentlyFailedQueue;
        string authorPrefix = " Author: ";
        public CoverageReportBuilder()
        {
            FrequentlyFailedTests = new List<string>();
            FrequentlyFailedQueue = new Queue<string>();
            if (File.Exists(StableFailedFilePath))
            {
                using (StreamReader reader = new StreamReader(StableFailedFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FrequentlyFailedQueue.Enqueue(line);
                        int indexOfComma = line.IndexOf(authorPrefix, 0);
                        if (indexOfComma == -1)
                        {
                            FrequentlyFailedTests.Add(line);
                        }
                        else
                        {
                            string testNameOnly = line.Substring(0, indexOfComma -1);
                            FrequentlyFailedTests.Add(testNameOnly);
                        }
                      
                    }
                }
            }
        }

        public void ListCurrentTestIntoReport(TestContext testContext, string lastUpdate, string author)
        {
            if (!File.Exists(LogFilePath) | !File.Exists(PreviousTestNameFilePath))
            {
                Console.WriteLine($"No existing file '{LogFilePath}' under 'Downloads'. Therefore, the coverage is not tracked.");
                return;
            }

            string testName = testContext.Test.Name;
            if (!testName.Contains("_"))
            {
                return;
            }
            if (testName.Contains(","))
            {
                testName = testName.Replace(",", "-");
            }
            FileInfo coverageFileInfo = new FileInfo(LogFilePath);
            if (coverageFileInfo.Length > 1000 * 1000) // 1 MB
            {
                RecreateReport(LogFilePath);
            }

            int firstunderScore = testName.IndexOf("_");
            string subject = testName.Substring(0, firstunderScore);
            subject = InsertSpaceCharBeforeEachUppercase(subject);
            int suffixIndex = 0;
            if (testName.Contains("_"))
            {
                suffixIndex = testName.LastIndexOf("_") + 1;
            }
            string subsubject = testName.Substring(suffixIndex);
            subsubject = InsertSpaceCharBeforeEachUppercase(subsubject);
            using (StreamWriter sw = File.AppendText(LogFilePath))
            {
                bool isMethosExist = false;
                using (StreamReader re = new StreamReader(PreviousTestNameFilePath))
                {
                    isMethosExist = re.ReadLine() == testContext.Test.MethodName;
                }
                if (!isMethosExist)
                {
                    sw.WriteLine($"{testContext.Test.MethodName},{author},{lastUpdate},{subject},{subsubject}");
                    if (File.Exists(StableFailedFilePath))
                    {
                        using (StreamWriter stableFailedWriter = File.AppendText(StableFailedFilePath))
                        {
                            if (testContext.Result.Outcome.Status.ToString() == "Failed")
                            {
                                stableFailedWriter.WriteLine($"{testContext.Test.MethodName},{authorPrefix}{author}");
                            }
                        }
                        FileInfo stableFailedFileInfo = new FileInfo(StableFailedFilePath);
                        if (stableFailedFileInfo.Length >= 70 * 1000)
                        {
                            DequeueOldestTestFromReport(StableFailedFilePath);
                        }
                    }
                }               
            }
            using (StreamWriter sw = new StreamWriter(PreviousTestNameFilePath))
            {
                sw.WriteLine(testContext.Test.MethodName);
            }
        }

        public void DeleteExistingReport(string logFilePath)
        {
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
        }

        public void CreateNewReport(string logFilePath)
        {
            if (!File.Exists(logFilePath))
            {
                using (StreamWriter writer = new StreamWriter(logFilePath))
                {
                    writer.WriteLine($"The file was recreated at {DateTime.Today}");
                }
            }
        }

        private void RecreateReport(string LogFilePath)
        {
            DeleteExistingReport(LogFilePath);
            CreateNewReport(LogFilePath);
        }

        private void DequeueOldestTestFromReport(string logFilePath)
        {
            DeleteExistingReport(logFilePath);         
            int numberOfOldestTests = FrequentlyFailedQueue.Count() / 5; //Delete 20% from the tests
            for (int i = 0; i < numberOfOldestTests; i++)
            {
                FrequentlyFailedQueue.Dequeue();
            }
            CreateNewReport(logFilePath);
            using (StreamWriter writer = new StreamWriter(logFilePath)) //Save the rest of the 80%
            {
                while (FrequentlyFailedQueue.Count > 0)
                {
                    string dequeuedTest = FrequentlyFailedQueue.Dequeue();
                    writer.WriteLine(dequeuedTest);
                }                
            }
        }

        public bool IsTestStableFailed
        {
            get
            {
                if (File.Exists(StableFailedFilePath))
                {                
                    return FrequentlyFailedTests.Where(x => x == TestContext.CurrentContext.Test.MethodName).Count() > 4;
                }
                else
                {
                    return false;
                }
            }         
        }

        private string InsertSpaceCharBeforeEachUppercase(string camelCaseString)
        {
            string splitedText = camelCaseString.Substring(0, 1);
            char[] charArray = camelCaseString.ToCharArray();
            for (int i = 1; i < charArray.Length; i++)
            {
                if (Char.IsUpper(charArray[i]))
                {
                    splitedText += " " + charArray[i];
                }
                else
                {
                    splitedText += charArray[i];
                }               
            }
            return splitedText;
        }
    }
}
