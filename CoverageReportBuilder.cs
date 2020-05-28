﻿using NUnit.Framework;
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
        string authorPrefix = " Author: ";
        public CoverageReportBuilder()
        {
            FrequentlyFailedTests = new List<string>();
            if (File.Exists(StableFailedFilePath))
            {
                using (StreamReader reader = new StreamReader(StableFailedFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
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
            if (coverageFileInfo.Length > 1000*1000) // 1 MB
            {
                RecreateReport(LogFilePath);
            }

            int firstunderScore = testName.IndexOf("_");
            string subject = testName.Substring(0, firstunderScore);
            int suffixIndex = 0;
            if (testName.Contains("_"))
            {
                suffixIndex = testName.LastIndexOf("_") + 1;
            }
            string subsubject = testName.Substring(suffixIndex);
            foreach (char item in subsubject)
            {
                if (Char.IsUpper(item))
                {
                    if(subsubject.IndexOf(item) > 0)
                    {
                        subsubject = subsubject.Insert(subsubject.IndexOf(item), " ");
                    }
                }
            }
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
                        if (stableFailedFileInfo.Length > 2000 * 1000)
                        {
                            RecreateReport(StableFailedFilePath);
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
                    writer.WriteLine($"{DateTime.Today}");
                }
            }
        }

        private void RecreateReport(string LogFilePath)
        {
            DeleteExistingReport(LogFilePath);
            CreateNewReport(LogFilePath);
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
    }
}