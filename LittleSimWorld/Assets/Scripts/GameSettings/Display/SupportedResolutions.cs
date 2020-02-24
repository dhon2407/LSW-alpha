using System.Collections.Generic;
using UnityEngine;

public partial class DisplayHandler
{
    private readonly Dictionary<(int, int), List<Resolution>> AllowAspectRatios =
        new Dictionary<(int, int), List<Resolution>>
    {
        { (31,9),
            new List<Resolution>()
            {
                new Resolution { width = 3840, height = 1080 },
                new Resolution { width = 5120, height = 1440 },
            }
        },

        { (21,9),
            new List<Resolution>()
            {
                new Resolution { width = 2560, height = 1080 },
                new Resolution { width = 3440, height = 1440 },
                new Resolution { width = 5120, height = 2160 },
            }
        },

        { (16,9),
            new List<Resolution>()
            {
                new Resolution { width = 1280, height = 720 },
                new Resolution { width = 1366, height = 768 },
                new Resolution { width = 1600, height = 900 },
                new Resolution { width = 1920, height = 1080 },
                new Resolution { width = 2560, height = 1440 },
                new Resolution { width = 3840, height = 2160 },
                new Resolution { width = 5120, height = 2880 },
                new Resolution { width = 7680, height = 4320 },
            }
        },

        { (16,10),
            new List<Resolution>()
            {
                new Resolution { width = 1280, height = 800 },
                new Resolution { width = 1920, height = 1200 },
                new Resolution { width = 2560, height = 1600 },
            }
        },

        { (4,3),
            new List<Resolution>()
            {
                new Resolution { width = 1400, height = 1050 },
                new Resolution { width = 1440, height = 1080 },
                new Resolution { width = 1600, height = 1200 },
                new Resolution { width = 1920, height = 1440 },
                new Resolution { width = 2048, height = 1536 },
            }
        },
    };
}
