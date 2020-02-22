using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



/// <summary>
/// This class is used to control the number of currently running tasks
/// </summary>
[System.Serializable]
public class TaskExecutionManager
{
    private Queue<Action> actionsToRun = new Queue<Action>();
    public int ranTaskCount = 4;
    private Task[] currentRanTasks;
    public NoiseGroup[] noiseGroup;
    public TaskStatus[] taskStatuses;

    public void Initialize()
    {
        ranTaskCount = Environment.ProcessorCount;
        this.currentRanTasks = new Task[ranTaskCount];
        this.noiseGroup = new NoiseGroup[ranTaskCount];
        this.taskStatuses = new TaskStatus[ranTaskCount];
    }

    public void AddAction(Action a)
    {
        actionsToRun.Enqueue(a);
    }

    public int GetIndexByTaskID(int? id)
    {
        if (id == null)
            return -1;
        for(int i = 0; i < ranTaskCount; i++)
        {
            if (currentRanTasks[i].Id == id)
                return i;
        }
        return -1;
    }

    public async void UpdateManager()
    {
        for (int i = 0; i < ranTaskCount; i++)
        {
            if (currentRanTasks[i] != null)
                taskStatuses[i] = currentRanTasks[i].Status;
        }
        if (actionsToRun.Count > 0)
        {
            for (int i = 0; i < ranTaskCount; i++)
            {
                Task t = currentRanTasks[i];
                if (t == null || t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Faulted)
                {
                    try
                    {
                        Task task = currentRanTasks[i] = Task.Run(actionsToRun.Dequeue());
                        
                        await task;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Task faulted with error: {e.Message} \n {e.StackTrace}");
                    }
                    break;
                }
            }
        }
    }
}
