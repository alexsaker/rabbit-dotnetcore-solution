using System;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using WebApi.Models;

namespace WebApi.EventBus
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class;
    }
    public class RabbitManager : IRabbitManager
    {
        private readonly DefaultObjectPool<IModel> _objectPool;
        private readonly ILogger<RabbitManager> _logger;
        public RabbitManager(ILogger<RabbitManager> logger, IPooledObjectPolicy<IModel> objectPolicy)
        {
            _logger = logger;
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
            registerWeatherForecastConsumers();
        }

        private void registerWeatherForecastConsumers()
        {
            var channel = _objectPool.Get();
            channel.QueueBind(queue: "web-api",
                      exchange: "web-api.service",
                      routingKey: "webapi.weatherforecast.get");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                WeatherForecast wf = JsonConvert.DeserializeObject<WeatherForecast>(Encoding.UTF8.GetString(body.ToArray()));
                _logger.LogInformation($"############# Received {wf.ToString()}");
            };
            channel.BasicConsume(queue: "web-api",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);
                _logger.LogInformation("###### Publishing {0}", message.ToString());

                var sendBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
    }
}