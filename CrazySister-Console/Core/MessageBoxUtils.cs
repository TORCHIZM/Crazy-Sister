using System.Collections.Generic;
using System.Windows.Forms;

namespace Server.Core
{
    public static class MessageBoxUtils
    {
        public static Dictionary<int, MessageBoxButtons> BoxButtons = new Dictionary<int, MessageBoxButtons>() {
            { 1, MessageBoxButtons.OK },
            { 2, MessageBoxButtons.YesNo },
            { 3, MessageBoxButtons.OKCancel },
            { 4, MessageBoxButtons.AbortRetryIgnore }
        };

        public static Dictionary<int, MessageBoxIcon> BoxIcons = new Dictionary<int, MessageBoxIcon>() {
            { 1, MessageBoxIcon.Error },
            { 2, MessageBoxIcon.Information },
            { 3, MessageBoxIcon.Question },
            { 4, MessageBoxIcon.Warning }
        };
    }
}
