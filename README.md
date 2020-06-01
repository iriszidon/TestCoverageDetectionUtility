1.	In order to prompt the document line by line please press 'Enter' key.
2.	In order to prompt the entire document please type 'Space' key, and then hit 'Enter' key.
3.	This application is useful for test automation developers, who use C# and Nunit.
4.	The program is a c# console application that functions as an external utility for a test project.
5.	The Application Performs 3 functions:
6.	1 - Create Coverage report
7.	2 - Prompt StableFailed test list
8.	3 - Prompt ReadMe file and get help.
9.	In order to see the application in action, create a new c# Console application project, and run Program.cs.
10.	The file Coverage Report Builder is an object class, that may be included in your test project, and collect data on the tests run.
11.	The coverage feature gives us the ability to produce updated coverage during the run of the automation.
12.	The feature may be turned off and on.
13.	You can turn the features on by adding an empty csv file named 'TestCoverage.csv' for example, and place it under downloads folder.
14.	in addition, you must create 2 more empry csv files, StableFailedList.csv and LastTestName.csv under downloads folder.
15.	You can produce the coverage csv using a batch file.
16.	When the feature is turned off then the coverage is not tracked.
17.	In order to turn off simply delete all csv files you have created form downloads folder, you may use a batch file.
18.	The Coverage Report Builder outputs the test's data (Name, Subject etc.) during clean up.
19.	Only tests that run are logged, therefore we do not need to filter out `stable failed' tests.
20.	Important!!! The feature must not be turned on in the QA LAB machine. It will cause errors in parallel run.
21.	The log file accumulates data, and is saved in the machine you use under `Downloads' folder.
22.	The log can accumulate data of more than one run, until it reaches 1000 k size.
23.	When the log reaches 1000 k size it will be automatically recreated.
24.	The log keeps the last result of the test.
25.	In order to see the author and last update time of the test in the log, the test setup should contain the lines
26.	(the test class should have two global variables , string Author, and String LastUpdate)
27.	Author = "Benjamin";
28.	LastUpdate = "5.5.2020";
29.	The The Coverage Report Builder detecs frequntly failed test and prodices a black list of those tests.
30.	I hope this read me file was helpful for you <3 ....
31.	Enjoy :) <3 <3 <3

