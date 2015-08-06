using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Events.EventArguments;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Models;
using Microsoft.WindowsAzure;
using Tweetinvi.Core.Interfaces.oAuth;
using System.Web;
using System.Security.Cryptography;
using Shared.Extensions;
using System.Threading;

namespace MySelfie.Scraper
{
    class TweetPicTask : IPicTask
    {
        private Thread _pullThread;
        private Thread _commandThread;
        private IFilteredStream _stream;
        private WallModel _model;
        private TweetPicRepository _tweetRepo;

        private DateTime _startTime;

        private int _totalPulls;
        private int _pullNotificationFrequency;
        private int _totalRecievedStream;
        private int _totalRecievedPull;
        private int _totalNewPulled;
        private int _totalSkippedPull;

        private int _pullDelayMilliseconds;
        private int _pullMaxAmount;
        private long _pullLastID;

        private bool _isStreaming;
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

        public TweetPicTask(int wallId)
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

            this.LastCommandTime = DateTime.Now;

            this.setTwitterCredentials(this._model);

            this._startTime = DateTime.Now;

            this._pullDelayMilliseconds = 10000;
            this._pullMaxAmount = 100;

            this._isStreaming = false;
            this._keepPulling = true;

            if (this._isStreaming)
            {
                this.configureStream();
            }

            this._processingThreadList = new List<Thread>();
            this._currentProcesses = 0;
            this._finishedProcesses = 0;
            this._processNotificationFrequency = 500;   // how often system will log activity
            
            this._errorCount = 0;
            this._totalSkippedPull = 0;
            this._totalNewPulled = 0;
            this._totalPulls = 0;
            this._totalRecievedPull = 0;
            this._pullNotificationFrequency = 10;   // how often system will log activity

            this._tweetRepo = new TweetPicRepository(this._model.WallId);
        }

        private bool configureStream()
        {
            bool result = false;

            try
            {
                this._stream = Tweetinvi.Stream.CreateFilteredStream();
                this._stream.AddTrack(this._model.Hashtag);
                this._stream.MatchingTweetReceived += (sender, args) =>
                {
                    this.Receive(sender, args);
                };

                result = true;
            }
            catch (Exception ex)
            {
                this._errorCount++;
                Logger.Log("TweetPicTask: Stream (" + this._model.Hashtag + ") configureStream error: " + ex.ToString());
            }

            return result;
        }

        private void setTwitterCredentials(WallModel model)
        {
            TwitterCredentials.SetCredentials(this.getTwitterCredentials(this._model));
        }
        private IOAuthCredentials getTwitterCredentials(WallModel model)
        {
            string consumerKey = model.Twitter_ConsumerKey.Trim(); 
            string consumerSecret = model.Twitter_ConsumerSecret.Trim();
            string userTokenKey = model.Twitter_UserTokenKey.Trim();
            string userTokenSecret = model.Twitter_UserTokenSecret.Trim();
            
            return TwitterCredentials.CreateCredentials(userTokenKey, userTokenSecret, consumerKey, consumerSecret); 
        }

        public void Start()
        {
            if (this._isStreaming)
            {
                //Logger.Log("TweetPicTask: Starting stream: " + this._model.Hashtag);

                this._stream.StartStreamMatchingAllConditions();
            }
            else
            {
                //Logger.Log("TweetPicTask: Starting pull loop: " + this._model.Hashtag);

                this._pullThread = new Thread(() =>
                {
                    Pull(this._model.Hashtag);
                });

                this._pullThread.Start();
            }            
        }

        public void Pause()
        {
            Logger.Log("TweetPicTask: Pausing: " + this._model.Hashtag);

            this._stream.PauseStream();

            this._keepPulling = false;
        }
        public void Resume()
        {
            Logger.Log("TweetPicTask: Resuming: " + this._model.Hashtag);

            this._keepPulling = true;
        }

        public void Stop()
        {
            Logger.Log("TweetPicTask: Stopping: " + this._model.Hashtag);

            this._stream.StopStream();

            this._pullThread.Abort();
        }

        public void Status(int id)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }

        public void Pull(string hashtag)
        {
            if (this._keepPulling)
            {
                try
                {
                    //this._totalPulls++;

                    var searchParameter = Search.GenerateSearchTweetParameter(hashtag);
                    searchParameter.SearchType = Tweetinvi.Core.Enum.SearchResultType.Recent;
                    searchParameter.MaximumNumberOfResults = this._pullMaxAmount;
                    searchParameter.SinceId = this._pullLastID;

                    IList<ITweet> tweets = null;

                    try
                    {
                        tweets = Search.SearchTweets(searchParameter);
                    }
                    catch (Exception e)
                    {
                        //this._errorCount++;
                        Console.WriteLine("TweetPicTask.Pull: Search.SearchTweets fail. Message: " + e.Message);
                    }

                    if (tweets.IsNotNull())
                    {
                        foreach (var tweet in tweets)
                        {
                            Receive(tweet);
                        }

                        this._pullLastID = tweets
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.Id)
                            .FirstOrDefault()
                            .IfNotNull(x => x, 0);
                    }                    
                }
                catch (Exception ex)
                {
                    //this._errorCount++;
                    Logger.Log("TweetPicTask.Pull exception: " + ex.Message);

                    this.setTwitterCredentials(this._model);
                }
            }

            Thread.Sleep(this._pullDelayMilliseconds);
            
            this.WriteState();

            this.Pull(this._model.Hashtag);       
        }

        private void WriteState()
        {
            if (this._totalPulls % this._pullNotificationFrequency == 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("TWITTER Status Report!");
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
                Console.WriteLine("Total Recieved Streaming: " + this._totalRecievedStream);
                Console.WriteLine("Last ID Pulled: " + this._pullLastID);
                Console.WriteLine("Max Pull Amount: " + this._pullMaxAmount);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void Receive(ITweet tweet)
        {
            //this._totalRecievedPull++;

            if (this._tweetRepo.IsNew(tweet.Id))
            {
                //this._totalNewPulled++;
                Console.Write("+");

                this._tweetRepo.Add(tweet);
            }
            else
            {
                //this._totalSkippedPull++;
                Console.Write("-");
            }

            //if (this._totalRecievedPull % this._pullMaxAmount == 0)
                //Logger.Log("TweetPicTask: Pull (" + this._model.Hashtag + ") total recieved: " + this._totalRecievedPull + ", last ID: " + this._pullLastID);
        }

        private void Receive(object sender, MatchedTweetReceivedEventArgs args)
        {
            Console.Write(".");
            //this._totalRecievedStream++;

            //if (this._totalRecievedStream % 10 == 0)
                //Logger.Log("Stream (" + this._model.Hashtag + ") total recieved: " + this._totalRecievedStream);

            this._tweetRepo.Add(args.Tweet);
        }
    }
}
