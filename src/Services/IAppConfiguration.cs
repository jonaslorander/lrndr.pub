using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using lrndrpub.Models;

namespace lrndrpub.Services
{
    public interface ILPConfiguration
    {
        SettingsValue this[string key] { get; set; }
    }
}
