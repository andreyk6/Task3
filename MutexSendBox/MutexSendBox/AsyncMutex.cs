using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task3
{
    public class AsyncMutex : IDisposable
    {
        Queue<Task> coldTasks = new Queue<Task>();

        private Mutex mutex = new Mutex();
        private object lockobj = new object();

        public AsyncMutex()
        {
            //Add finished task (for first .Lock() entry)
            coldTasks.Enqueue(Task.Factory.StartNew(() => { }));
        }
        public Task Lock()
        {
            lock (lockobj)
            {
                coldTasks.Enqueue(new Task(() => { })); //Add new task for the future .Lock()
                return coldTasks.Peek(); // Return top task
            }
        }

        public void Release()
        {
            lock (lockobj)
            {
                if (coldTasks.Count > 0)
                {
                    coldTasks.Dequeue(); //Remove current task
                    if (coldTasks.Peek().IsCompleted == false) coldTasks.Peek().Start(); //Unlock access for next thread
                }
            }
        }

        //Interface implementation for "using" operator
        public void Dispose()
        {
            Release();
        }
    }
}
