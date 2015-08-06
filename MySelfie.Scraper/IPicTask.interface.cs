using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    public interface IPicTask
    {
        void Start();
        void Pause();
        void Resume();
        void Stop();

        int WallId { get; }
    }
}
