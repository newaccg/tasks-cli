# .NET 10 CLI Task Tracker
Project idea is taken from [Roadmap.sh](https://roadmap.sh/projects/task-tracker).

## Functions
* Display a **help** message 
* **add** task with specified description and status
* **list(ls)** tasks with specified statuses
* **remove(rm)** tasks with specified IDs
* **update(upd)** task to specified description
* **mark** task to specified status

## Building
```bash
# 1. Clone this repository:
git clone https://github.com/newaccg/tasks-cli.git
# 2. Go to the cloned repository:
cd tasks-cli
# 3. Build this:
dotnet build
```

## Usage
Tracker can be used via interactive mode (q or quit to exit):
```bash
./tasks-cli 
(tasks-cli) ls
# no tasks yet
(tasks-cli) q
```
Or via non-interactive mode:
```bash
./tasks-cli ls
# no tasks yet
```
