using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketera.Providers
{
    public interface IConfig
    {
        string CurrentDomain { get; }
        bool IsDebug { get; }
        string MapPath(string path);
        string Get(string key);
        string GetConnetionString();
    }
}
