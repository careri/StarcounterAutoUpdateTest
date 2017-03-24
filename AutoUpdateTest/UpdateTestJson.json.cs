using Starcounter;
using System;

namespace AutoUpdateTest
{
    partial class UpdateTestJson : Json
    {
        public string _Time { get { return DateTime.Now.ToLongTimeString(); } } 
    }
}
