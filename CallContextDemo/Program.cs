using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace CallContextDemo
{
    class Program
    {
        private const String c_LCCDataName = "Logical";
        private const String c_CCDataName = "Local";

        private static void Main()
        {
            // Set an item in the thread’s call context
            CallContext.LogicalSetData(c_LCCDataName, "Logical CallContext Data");
            CallContext.SetData(c_CCDataName, "Local CallContext Data");

            // Get the item in the thread’s call context
            GetThreadCallContext();

            // Show that call context flows to another thread
            WaitCallback wc = na => GetThreadCallContext();
            wc.EndInvoke(wc.BeginInvoke(null, null, null));

            // Show that call context flows to another AppDomain
            AppDomain ad = AppDomain.CreateDomain("Different AppDomain");
            ad.DoCallBack(GetThreadCallContext);
            AppDomain.Unload(ad);

            // verify call context data is still available
            Console.WriteLine("\n--- Verify call context data is still available ---");
            GetThreadCallContext();

            // Remove the key 
            CallContext.FreeNamedDataSlot(c_LCCDataName);
            CallContext.FreeNamedDataSlot(c_CCDataName);

            // verify value for the was removed 
            Console.WriteLine("\n--- Remove Context key via FreeNamedDataSlot() ---");
            GetThreadCallContext();

            Console.WriteLine("\n\nHit any key to exit");
            Console.ReadKey();
        }

        private static void GetThreadCallContext()
        {
            // Get the item in the thread’s LOGICAL call context
            Console.WriteLine("\nLogical Call Context: AppDomain = {0}, Thread ID = {1}, Name = {2}, Data = {3}",
                AppDomain.CurrentDomain.FriendlyName,
                Thread.CurrentThread.ManagedThreadId,
                c_LCCDataName,
                CallContext.LogicalGetData(c_LCCDataName));

            // Get the item in the thread’s LOCAL call context
            Console.WriteLine("Local Call Context: AppDomain = {0}, Thread ID = {1}, Name = {2}, Data = {3}",
            AppDomain.CurrentDomain.FriendlyName,
            Thread.CurrentThread.ManagedThreadId,
            c_CCDataName,
            CallContext.GetData(c_CCDataName));
        }
    }
}