
bool state = false;

Console.WriteLine("Time is " + System.DateTime.Now.Hour);

string[] set1 = { "9", "8" }; 
string[] set2 = { "9", "8" };
//ChangeLightColor(new string[] { "9" }, "ffffff");
ChangeLightColor(set1, "ffffff");
Sleep(1000);
ChangeLightColor(set2, "ff00ff");

