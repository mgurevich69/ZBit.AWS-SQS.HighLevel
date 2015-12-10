# ZBit.AWS-SQS.HighLevel
Higher Level Library for Amazon SQS.

Allows you to Subscribe to a queue instead of polling it in a busy loop. That is, you provide a delegate function in “Subscribe” method and this callback will be invoked every time a new message is added to the queue. This library will also handle serialization and deserialization for your objects that you put in the queue.

Also, optionally, would automatically delete messages from the queue once they processed successfully, which is not done by the AWS SDK library.

It creates a new queue either in Send or in Subscribe if the queue with the given name doesn’t exist yet under your account on AWS.

Small and Simple. Requires AWSSDK.SQS, Newtonsof.Json and log4net.
