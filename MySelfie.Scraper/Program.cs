using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Tweetinvi;
using System.Threading;
using System.Timers;


namespace MySelfie.Scraper
{
    class Program
    {
        public 
        static void Main(string[] foo)
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                exitEvent.Set();
            };

            Logger.Log("Starting up main process");
            List<Wall> wallList;

            using (var db = new MySelfieEntities())
            {
                wallList = db.Walls.Where(x => x.IsActive).ToList();
            }

            var taskList = new List<IPicTask>();
            var threadList = new List<Thread>();

            try
            {
                foreach(var wall in wallList)
                {
                    var thread = new Thread(() =>
                    {
                        IPicTask task = new TweetPicTask(wall.WallId);
                        taskList.Add(task);

                        Logger.Log("Twitter Task starting to watch " + wall.Hashtag);

                        task.Start();                       
                    });

                    threadList.Add(thread);

                    thread.Start();

                    var instagram = new Thread(() =>
                    {
                        IPicTask task = new InstagramPicTask(wall.WallId);
                        
                        taskList.Add(task);

                        Logger.Log("Instagram Task starting to watch " + wall.Hashtag);

                        task.Start();                        
                    });

                    threadList.Add(instagram);

                    instagram.Start();
                }

                //foreach(var task in taskList)
                //{
                //    var thread = new Thread(() =>
                //        {
                //            using (var db = new MySelfieEntities())
                //            {
                //                var commands = db.WorkerCommands
                //                    .Where(x => x.WallId == task.WallId)
                //                    .Where(x => x.TimeStamp > task.LastCommandTime)
                //                    .OrderBy(x => x.TimeStamp)
                //                    .ToList();



                //            }
                //        });
                //}
    
            } 
            catch (Exception ex)
            {
                Logger.Log("Top Level Error: " + ex.ToString());
            }

            exitEvent.WaitOne();

            Logger.Log("Ending main process");
        }       
    }
}
