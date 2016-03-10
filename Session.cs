using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloudBot
{
    abstract class Session
    {
        protected SoundCloud sc;
        protected List<string> res;
        protected string accepted;
        public Session(SoundCloud sc, string accepted)
        {
            this.sc = sc;
            this.accepted = accepted;
            Count = 0;
        }

        public int Count { get; private set; }
        private int last;
        public void Start(int session_limit, int key_limit)
        {
            new Thread(() =>
            {
                last = 0;
                Program.GUI.log("Session started");
                while (Count < session_limit)
                {
                    int index = -1;
                    Program.GUI.Invoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            if (Count % key_limit == 0 || !res.Any()) res = sc.Search(Program.GUI.GetKey(last++)).Where(x => x.Item2 == accepted).Select(x => x.Item1).ToList();
                            index = Program.GUI.IsRandomID() ? new Random().Next(res.Count - 1) : 0;
                        }
                        catch { index = 0; }
                    });
                    try
                    {
                        while (index == -1) ;
                        make(res[index]);
                        res.RemoveAt(index);
                    }
                    catch { }
                    Count++;
                }
                Program.GUI.log("Session terminated");
            }).Start();
        }

        protected abstract void make(string curr);
    }

    class LikeSession : Session
    {
        public LikeSession(SoundCloud sc) : base(sc,"track") {}

        protected override void make(string curr)
        {
            if (!Directory.Exists(".\\liked\\"))
                Directory.CreateDirectory(".\\liked\\");
            File.AppendAllText(".\\liked\\" + sc.GetOAuth() + ".txt", Environment.NewLine + curr);
            sc.LikeTrack(curr);
            Program.GUI.log("Liked " + curr);
        }
    }

    class FollowSession : Session
    {
        public FollowSession(SoundCloud sc) : base(sc, "user") { }

        protected override void make(string curr)
        {
            if (!Directory.Exists(".\\followed\\"))
                Directory.CreateDirectory(".\\followed\\");
            File.AppendAllText(".\\followed\\" + sc.GetOAuth() + ".txt", Environment.NewLine + curr);
            sc.Follow(curr);
            Program.GUI.log("Followed " + curr);
        }
    }

    class CommentSession : Session
    {
        public CommentSession(SoundCloud sc) : base(sc, "track") { }

        private int lastc = 0;
        protected override void make(string curr)
        {
            try
            {
                string comment = null;
                Program.GUI.Invoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            comment = Program.GUI.GetComment(lastc);
                        }
                        catch { comment = "X"; }
                    });
                while (comment == null) ;
                if (comment == "X") throw new Exception();
                sc.CommentTrack(curr, comment);
                Program.GUI.log("Commmented " + curr);
            }
            catch(Exception)
            {
                lastc = 0;
                make(curr);
            }
        }
    }
}
