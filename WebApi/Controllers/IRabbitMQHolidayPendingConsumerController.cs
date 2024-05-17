using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public interface IRabbitMQHolidayPendingConsumerController
{
    void StartConsuming();
    void ConfigQueue(string queueName);
}