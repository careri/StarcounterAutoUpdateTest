using System;
using Starcounter;
using System.Threading;
using Starcounter.Internal;

namespace AutoUpdateTest
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/UpdateTest/client", () =>
            {
                var json = new UpdateTestJson
                {
                    IsAutoUpdate = true,
                    Message = "Client auto update test"
                };

                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }
                json.Session = Session.Current;
                return json;
            });

            Handle.GET("/UpdateTest/server", () =>
            {
                var json = new UpdateTestJson
                {
                    IsAutoUpdate = false,
                    Message = "Server auto update test"
                };

                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }
                json.Session = Session.Current;
                TimeUpdater.Start(json, TimeSpan.FromSeconds(1));
                return json;
            });

            
        }
        private class TimeUpdater
        {
            private readonly Session m_session;
            private readonly UpdateTestJson m_json;
            private readonly TimeSpan m_interval;
            private readonly Timer m_timer;
            private readonly byte m_schedulerID;

            public TimeUpdater(Session session, UpdateTestJson json, TimeSpan interval)
            {
                m_schedulerID = StarcounterEnvironment.CurrentSchedulerId;
                m_session = session;
                m_json = json;
                m_interval = interval;
                m_timer = new Timer(OnTimer, null, interval, Timeout.InfiniteTimeSpan);
            }

            private void OnTimer(object state)
            {
                if (m_session.IsAlive())
                {
                    Scheduling.ScheduleTask(
                        m_session.CalculatePatchAndPushOnWebSocket, 
                        true, m_schedulerID);

                    if (m_session.IsAlive())
                    {
                        m_timer.Change(m_interval, Timeout.InfiniteTimeSpan);
                    }
                }
                else
                {
                    m_timer.Dispose();
                }
            }

            internal static void Start(UpdateTestJson json, TimeSpan interval)
            {
                var session = json.Session;

                if (session != null)
                {
                    new TimeUpdater(session, json, interval);
                }
            }
        }

    }
}