// See https://aka.ms/new-console-template for more information
using FireOnWheels.Notification.Service;

Console.Title = "Notification service";
using (var rabbitMqManager = new RabbitMqManager())
{
    rabbitMqManager.ListenForOrderRegisteredEvent();
    Console.WriteLine("Listening for OrderRegisteredEvent..");
    Console.ReadKey();
}
