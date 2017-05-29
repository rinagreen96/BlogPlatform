using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IModeratorService
    {
        void SendingEmailComment(string message_text, int comment_id);
        void SendingEmailArticle(string message_text, int article_id);
    }
}
