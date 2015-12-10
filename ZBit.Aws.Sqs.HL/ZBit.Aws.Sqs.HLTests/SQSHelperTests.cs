using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZBit.Aws.Sqs.HL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
namespace ZBit.Aws.Sqs.HL.Tests {
	public class MyType {
		public int prop1 { get; set; }
		public string prop2 { get; set; }
	}

	[TestClass()]
	public class SQSHelperTests {
		[TestMethod()]
		public void SendTest() {
			string sSuperString = "my strIng;some (5) special symbols: &%_@' and a quote [\"] and a backslash <\\>!";
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = sSuperString });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = sSuperString });
		}

		int m_iTotalCallCount, m_iCallCountFor2;

		[TestMethod()]
		public void SubscribeTest() {
			SQSHelper q = new SQSHelper();
			m_iTotalCallCount = 0;
			m_iCallCountFor2 = 0;
			q.Subscribe<MyType>("Utest1Q", QueueMsgProcessor);
			SQSHelper.Send("Utest1Q", new { prop1 = 11, prop2 = "test2" });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test3" });
			SQSHelper.Send("Utest1Q", new { prop1 = 22, prop2 = "test2" });
			Thread.Sleep(1000);
			Assert.IsTrue(m_iTotalCallCount > 2, "total: " + m_iTotalCallCount);
			Assert.AreEqual(2, m_iCallCountFor2);
			q.UnSubscribe();
		}

		private bool QueueMsgProcessor(MyType obj) {
			m_iTotalCallCount++;
			if (obj.prop2 == "test2") m_iCallCountFor2++;
			return true;
		}
	}
}
