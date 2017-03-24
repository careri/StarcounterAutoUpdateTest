using System;
using Starcounter;

namespace AutoUpdateTest
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/UpdateTest", () =>
            {
                var json = new UpdateTestJson();

                if (Session.Current == null)
                {
                    Session.Current = new Session();
                }
                json.Session = Session.Current;
                return json;
            });
        }
    }
}