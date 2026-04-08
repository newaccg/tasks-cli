using System.Text.Json;
// TODO: add interacrive mode 
namespace TasksApp{
    public class Task{
        public int id {get; set;}
        public string? task {get; set;}
        public string? status {get; set;}
        public string? updatedAt {get; set;}
        public string? createdAt {get; set;}

        // for json desrializer
        public Task(){}

        public Task(int i, string t, string s, string u, string c){
            id = i;
            task = t;
            status = s;
            updatedAt = u;
            createdAt = c;
        }
    }

    class MyTasksCli{
        const string FILE_PATH = "tasks.json";

        static void printHelp(string oneMore)
        {
            Console.WriteLine(oneMore);
            Console.WriteLine(
        """
        I have these comands:

        ls STATUS - list the tasks that have the status STATUS. If STATUS is not specified, list all tasks.

        add TASK STATUS - add a task with the specified TASK description and with specified STATUS status. If STATUS is not specified, the default status is "todo".

        remove ID - remove a task with the specified ID.

        update ID NEWTASK - change the task description from the ID to the NEWTASK description. 

        mark ID NEWSTATUS - change the task status from the ID to the NEWSTATUS status.
        """
        );
        }

        static List<Task> getTasks(string fileContent){
            List<Task> tasks = new();
            string curJson = "";

            bool bad = true;

            // -2, i = 1 because it has useless "}\n" and "{" at the end and at the begin
            for (int i = 1; i < fileContent.Length - 2; i++){
                curJson += fileContent[i];

                if (fileContent[i] == '}')
                {
                    // if file contains at least one "}" it can be not bad
                    bad = false;

                    Task? task;
                    try{
                        task = JsonSerializer.Deserialize<Task>(curJson);
                    }
                    catch{
                        bad = true;
                        break;
                    }

#pragma warning disable CS8604 // disable possible null reference argument warning
                    tasks.Add(task);
#pragma warning restore CS8604 

                    curJson = "";
                    // for skipping the next ','
                    i++;
                }
            }

            if (bad && curJson.Length != 0){
                Console.WriteLine("!!!!WARNING!!!!");
                Console.WriteLine("CORRUPTED PART OF JSON FILE:");
                Console.WriteLine(curJson);
                Console.WriteLine("WRITING NEW EMPTY JSON FILE");

                File.WriteAllText(FILE_PATH, "{}\n");
            }

            return tasks;
        }

        static void giveTasks(List<Task> tasks){
            string json = "{";

            if (tasks.Count != 0)
                json += JsonSerializer.Serialize(tasks[0]);

            for (int i = 1; i < tasks.Count; i++){
                json += ',' + JsonSerializer.Serialize(tasks[i]);
            }

            File.WriteAllText(FILE_PATH, json + "}\n");
        }

        static int getIndex(List<Task> tasks, string strId, bool help=true){
            int left = 0, right = tasks.Count - 1;
            int id;

            try
            {
                id = int.Parse(strId);
            }
            catch
            {
                Console.WriteLine(strId + ": bad id value");
                if (help) printHelp("");
                return -2;
            }

            while (left <= right){
                int mid = left + (right - left) / 2;

                if (tasks[mid].id == id) return mid;
                if (tasks[mid].id < id) left = mid + 1;
                else right = mid - 1;
            }

            Console.WriteLine(strId + ": task not found");
            return -1;
        }

        static void Main(string[] args){
            if (!File.Exists(FILE_PATH)){
                File.Create(FILE_PATH).Close();
                File.WriteAllText(FILE_PATH, "{}\n");
            }

            if (args.Length == 0){
                printHelp("no command specified");
                return;
            }

            string fileContent = File.ReadAllText(FILE_PATH), command = args[0];
            string[] INDEX_REQUIRES = ["remove", "update", "mark"];
            List<Task> tasks = getTasks(fileContent);
            int len = tasks.Count;

            switch(command){
                case "help":{
                    printHelp("");
                    break;
                }
                case "add":{
                    if (args.Length == 1)
                    {
                        printHelp("no task description specified");
                        return;
                    }

                    int i = 0;
                    Task task;
                    
                    string status = "todo";
                    if (args.Length > 2) status = args[2];
                    
                    if (len == 0){
                        task = new(1, args[1], status, DateTime.Now.ToString(), DateTime.Now.ToString());
                        File.WriteAllText(FILE_PATH, '{' + JsonSerializer.Serialize(task) + "}\n");
                    }
                    else{
                        // go while there's the "1, 2, 3.." sequence
                        while (i < len && i + 1 == tasks[i].id){
                            i++;
                        }

                        task = new(i + 1, args[1], status, DateTime.Now.ToString(), DateTime.Now.ToString());
                        tasks.Insert(i, task);

                        giveTasks(tasks);
                    }

                    Console.WriteLine("added task with id " + (i + 1).ToString() + " and status " + status);

                    break;
                }

                case "ls":{
                    if (len == 0)
                    {
                        Console.WriteLine("no tasks yet");
                    }
                    else foreach (Task task in tasks){
                        bool contains = false;

                        for (int i = 1; i < args.Length; i++)
                        {
                            if (args[i] == task.status)
                            {
                                contains = true;
                                break;
                            }
                        }
                        if (args.Length < 2 || contains){
                            Console.WriteLine("Id: " + task.id);
                            Console.WriteLine("Task: " + task.task);
                            Console.WriteLine("Status: " + task.status);
                            Console.WriteLine("Updated at: " + task.updatedAt);
                            Console.WriteLine("Created at: " + task.createdAt);
                            Console.WriteLine();
                        }
                    }
                    break;
                }

                default:{
                    if (!INDEX_REQUIRES.Contains(command))
                    {
                        printHelp(command + ": I don't know this command");
                        return;
                    }

                    if (command == "remove"){
                        for (int i = 1; i < args.Length; i++)
                        {
                            tasks.RemoveAt(getIndex(tasks, args[i], false));
                            giveTasks(tasks);
                        }
                    }
                    else if (args.Length > 2){
                        int ind = getIndex(tasks, args[1]);

                        if (ind < 0) return;

                        if (command == "update"){
                            tasks[ind].task = args[2];      
                        }
                        else if (command == "mark"){
                            tasks[ind].status = args[2];
                        }

                        tasks[ind].updatedAt = DateTime.Now.ToString();
                        giveTasks(tasks);
                    }
                    else{
                        printHelp("I don't know this command usage");
                    }
                        
                    break;
                }
            }
        }
    }
}

