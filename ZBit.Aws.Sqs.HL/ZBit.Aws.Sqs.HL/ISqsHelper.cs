using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBit.Aws.Sqs.HL {
	interface ISqsSubscriber {
		//static void Send<T>(string sQueueName, T payLoad);
		//static SQSHelper Subscribe<T>(Func<T, bool> handler, string sQueueName, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20);

		void Subscribe<T>(string sQueueName, Func<T, bool> handler, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20);
		void UnSubscribe();
	}
}
