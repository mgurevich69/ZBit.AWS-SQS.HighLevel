using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace ZBit.Aws.Sqs.HL {
	public class SQSHelper {
		protected static ConcurrentDictionary<string, string> g_dict = new ConcurrentDictionary<string, string>();

		protected bool m_bShouldListen;

		public static void Send<T>(string sQueueName, T payLoad) {
			AmazonSQSClient sqs = new AmazonSQSClient();
			string sPayload = JsonConvert.SerializeObject(payLoad);
			sqs.SendMessage(GetQueue(sqs, sQueueName), sPayload);
		}

		public static SQSHelper Subscribe<T>(Func<T, bool> handler, string sQueueName, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20) {
			SQSHelper queueHelperOut = new SQSHelper();
			queueHelperOut.Subscribe(sQueueName, handler, bDeleteMsgOnSuccess, iMaxMessagesAtaTime, iWaitTimeSeconds);
			return queueHelperOut;
		}

		public void Subscribe<T>(string sQueueName, Func<T, bool> handler, bool bDeleteMsgOnSuccess = true, int iMaxMessagesAtaTime = 10, int iWaitTimeSeconds = 20) {
			if (null == handler) {
				throw new ArgumentException("required parameter", "handler");
			}

			m_bShouldListen = true;
			Task.Run(() => {
				bool bHandlerRes;
				AmazonSQSClient sqs = new AmazonSQSClient();
				string sQueueUrl = GetQueue(sqs, sQueueName);
				while (m_bShouldListen) {
					ReceiveMessageResponse resp = sqs.ReceiveMessage(
						new ReceiveMessageRequest(sQueueUrl) {
							WaitTimeSeconds = iWaitTimeSeconds,
							MaxNumberOfMessages = iMaxMessagesAtaTime
						}
					);
					if (HttpStatusCode.OK != resp.HttpStatusCode) {
						//todo log error
						Thread.Sleep(1000);
						continue;
					}
					foreach (var msg in resp.Messages) {
						T obj = JsonConvert.DeserializeObject<T>(msg.Body);
						bHandlerRes = false;
						try {
							bHandlerRes = handler(obj);
						} catch (Exception) {
							//todo log error
						}
						if (bHandlerRes) {
							if (bDeleteMsgOnSuccess) {
								sqs.DeleteMessage(sQueueUrl, msg.ReceiptHandle);
							}
						}
					}
				}
			});
		}

		public void UnSubscribe() {
			m_bShouldListen = false;
		}

		public static string GetQueue(AmazonSQSClient sqs, string sQueueName) {
			string sQueueUrl;
			if (null == sqs) throw new ArgumentException("required parameter", "sqs");
			if (!g_dict.TryGetValue(sQueueName, out sQueueUrl)) {
				var respQueCreate = sqs.CreateQueue(sQueueName);
				if (HttpStatusCode.OK != respQueCreate.HttpStatusCode) {
					throw new ApplicationException("Unexpected result creating SQS: " + respQueCreate.HttpStatusCode);
				}
				sQueueUrl = respQueCreate.QueueUrl;
				g_dict[sQueueName] = sQueueUrl;
			}
			return sQueueUrl;
		}
	}
}
