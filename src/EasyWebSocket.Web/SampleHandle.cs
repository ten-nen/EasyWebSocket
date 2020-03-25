using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyWebSocket.Web
{
    public class SampleHandle : EasyConnectionHandle
    {
        public void Test(string name, int age, string msg)
        {
            Current.SendAsync("test", "server", $"欢迎 {name},{age} “{msg}”").Wait();
            AllConnections.SendAsync("test", name, msg).Wait();
        }
        public void TestJson(Person p, string msg)
        {
            Current.SendAsync("testJson", new { Name = "server" }, $"欢迎 {p.Name},{p.Age} “{msg}”").Wait();
            AllConnections.SendAsync("testJson", new { p.Name }, msg).Wait();
        }
        public override void OnClose()
        {
            System.Diagnostics.Debug.WriteLine("OnClose");
        }

        public override void OnError(Exception e)
        {
            System.Diagnostics.Debug.WriteLine("OnError");
        }

        public override void OnOpen()
        {
            System.Diagnostics.Debug.WriteLine("OnOpen");
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}