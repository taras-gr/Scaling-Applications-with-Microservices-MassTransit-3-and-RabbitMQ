// See https://aka.ms/new-console-template for more information
using FireOnWheels.Registration.Service;

Console.Title = "Registration service";
using (var rabbitMqManager = new RabbitMqManager())
{
    rabbitMqManager.ListenForRegisterOrderCommand();
    Console.WriteLine("Listening for RegisterOrderCommand..");
    Console.ReadKey();
}
