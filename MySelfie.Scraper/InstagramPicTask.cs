using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Extensions;
using System.Net;
using Newtonsoft.Json;

namespace MySelfie.Scraper
{
    class InstagramPicTask : IPicTask
    {
        private Thread _pullThread;
        private WallModel _model;
        private InstagramPicRepository _repo;

        private DateTime _startTime;

        private int _totalPulls;
        private int _pullNotificationFrequency;
        private int _totalRecievedPull;
        private int _totalNewPulled;
        private int _totalSkippedPull;

        private int _pullDelayMilliseconds;
        private int _pullMaxAmount;
        private long _pullLastID;

        private bool _keepPulling;

        private IList<Thread> _processingThreadList;
        private int _currentProcesses;
        private int _finishedProcesses;
        private int _processNotificationFrequency;

        private int _errorCount;

        public int WallId { 
            get
            {
                return this._model.WallId;
            }
            
        }
        public DateTime LastCommandTime { get; set; }

        public InstagramPicTask(int wallId)
        {
            using (var db = new MySelfieEntities())
            {
                var entity = db.Walls.Single(x => x.WallId == wallId);

                this._model = new WallModel(entity);

                this._pullLastID = db.Photos
                    .Where(x => x.WallId == wallId)
                    .OrderByDescending(x => x.SocialID)
                    .Select(x => x.SocialID)
                    .FirstOrDefault()
                    .IfNotNull(x => x, 0);
            }

            if (this._model.Instagram_AccessToken.IsEmptyOrNull())
            {
                this._model.Instagram_AccessToken = "1545103628.1fb234f.e6f242fd81fe48c1a156444871e803b3";  // TESTING
            }

            this.LastCommandTime = DateTime.Now;

            this._startTime = DateTime.Now;

            this._pullDelayMilliseconds = 10000;
            this._pullMaxAmount = 100;
            this._keepPulling = true;

            this._processingThreadList = new List<Thread>();
            this._currentProcesses = 0;
            this._finishedProcesses = 0;
            this._processNotificationFrequency = 100;   // how often system will log activity

            this._errorCount = 0;
            this._totalSkippedPull = 0;
            this._totalNewPulled = 0;
            this._totalPulls = 0;
            this._totalRecievedPull = 0;
            this._pullNotificationFrequency = 10;   // how often system will log activity

            this._repo = new InstagramPicRepository(this.WallId);
        }

        public void Start()
        {
            //Logger.Log("InstagramPicTask: Starting pull loop: " + this._model.Hashtag);

            this._pullThread = new Thread(() =>
            {
                Pull(this._model.Hashtag);
            });

            this._pullThread.Start();
            
        }

        public void Pause()
        {
            Logger.Log("InstagramPicTask: Pausing pull loop: " + this._model.Hashtag);

            this._keepPulling = false;
        }

        public void Resume()
        {
            Logger.Log("InstagramPicTask: Resuming pull loop: " + this._model.Hashtag);

            this._keepPulling = true;
        }

        public void Stop()
        {
            Logger.Log("InstagramPicTask: Stopping pull loop: " + this._model.Hashtag);

            this._pullThread.Abort();
        }

        private void Pull(string hashtag)
        {
            if (this._keepPulling)
            {
                this._totalPulls++;

                try
                {
                    hashtag = hashtag.Replace("#", "");
                    //hashtag = "selfie";     // TESTING
                    var url = "https://api.instagram.com/v1/tags/";
                    var url_end = "/media/recent";
                    var url_token = "?access_token=" + this._model.Instagram_AccessToken;

                    url = url + hashtag + url_end + url_token;

                    WebResponse response = processWebRequest(url);

                    if (response.IsNotNull())
                    {
                        using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                        {
                            var _instagram = JsonConvert.DeserializeObject<InstagramObject>(sr.ReadToEnd());
                            foreach (var data in _instagram.data)
                            {
                                this.Receive(data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //this._errorCount++;

                    Logger.Log("Instagram Pull exception: " + ex.ToString());
                }
            }

            Thread.Sleep(this._pullDelayMilliseconds);

            //this.WriteState();

            this.Pull(this._model.Hashtag);
        }

        private void WriteState()
        {
            if (this._totalPulls % this._pullNotificationFrequency == 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("INSTAGRAM Status Report!");
                Console.WriteLine("Thread ID: " + this._pullThread.ManagedThreadId);
                Console.WriteLine("Wall ID: " + this._model.WallId);
                Console.WriteLine("Hashtag: " + this._model.Hashtag);
                Console.WriteLine("Started: " + this._startTime.ToShortTimeString() + " on " + this._startTime.ToShortDateString());
                Console.WriteLine("Time Elapsed: " + this._startTime.DifferenceInHours(DateTime.Now) + " hours");
                Console.WriteLine("Error Count: " + this._errorCount);
                Console.WriteLine("Finished Processes: " + this._finishedProcesses);
                Console.WriteLine("Current Processes: " + this._currentProcesses);
                Console.WriteLine("Total Recieved Pulling: " + this._totalRecievedPull);
                Console.WriteLine("Total New Pulling: " + this._totalNewPulled);
                Console.WriteLine("Total Skipped Pulling: " + this._totalSkippedPull);
                Console.WriteLine("Last ID Pulled: " + this._pullLastID);
                Console.WriteLine("Max Pull Amount: " + this._pullMaxAmount);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void Receive(InstagramObject.Datum data)
        {
            this._totalRecievedPull++;

            if (this._repo.IsNew(long.Parse(data.caption.id)))
            {
                //this._totalNewPulled++;
                Console.Write("x");
                
                this._repo.Add(data);
            }
            else
            {
                //this._totalSkippedPull++;
                Console.Write("=");
            }
        }

        private WebResponse processWebRequest(string url, bool continueOnError = true)
        {
            try
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();

                return response;
            }
            catch(Exception ex)
            {
                //this._errorCount++;

                //Logger.Log("Instagram processWebRequest exception: " + ex.ToString());

                if (continueOnError)
                {
                    return processWebRequest(url, false);
                } else
                {
                    return null;
                }
            }
        }
    }
}
