using System;
using System.Linq;
using System.Reflection;
using Autofac;
using GreenPipes;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using MassTransit;
using MassTransit.ActiveMqTransport;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.BusClient.MassTransit.DiExtensions
{
    public static class BusExtension
    {
        public static void AddMassTransit(this IServiceCollection services, ServiceBusConfigModel messagingConfig, ServiceBusConsumerConfigModel consumerConfig, ServiceBusProducerConfigModel producerConfig)
        {
            _ = services.AddMassTransit(x =>
              {
                  if (messagingConfig.Uri.StartsWith("activemq"))
                  {
                      x.AddBus(context => Bus.Factory.CreateUsingActiveMq(cfg =>
                      {
                          cfg.Host(new Uri(messagingConfig.Uri), hst =>
                          {
                              hst.Username(messagingConfig.UserName);
                              hst.Password(messagingConfig.Password);
                              hst.UseSsl();
                          });

                          switch (messagingConfig.SerializerType)
                          {
                              case SerializerType.Json:
                                  cfg.UseJsonSerializer();
                                  break;

                              case SerializerType.Bson:
                                  cfg.UseBsonSerializer();
                                  break;

                              case SerializerType.Xml:
                                  cfg.UseXmlSerializer();
                                  break;
                          }

                          var retryConfig = messagingConfig.RetryConfig;
                          switch (messagingConfig.RetryConfig.RetryType)
                          {
                              case RetryType.Interval:
                                  cfg.UseMessageRetry(r => r.Interval(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                                  break;

                              case RetryType.Exponential:
                                  cfg.UseMessageRetry(r => r.Exponential(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.MinInterval.Value),
                                      TimeSpan.FromSeconds(retryConfig.MaxInterval.Value),
                                      TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                                  break;

                              case RetryType.Immediate:
                                  cfg.UseMessageRetry(r => r.Immediate(retryConfig.RetryCount.Value));
                                  break;

                              case RetryType.Incremental:
                                  cfg.UseMessageRetry(r => r.Incremental(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.IntervalSecond.Value),
                                      TimeSpan.FromSeconds(retryConfig.IntervalIncrement.Value)));
                                  break;

                              case RetryType.Intervals:
                                  var intervals = retryConfig.Intervals.Split(',').Select(i => Convert.ToInt32(i))
                                      .ToArray();
                                  cfg.UseMessageRetry(r => r.Intervals(intervals));
                                  break;

                              default:
                                  cfg.UseMessageRetry(r => r.None());
                                  break;
                          }

                          if (consumerConfig?.Consumers != null)
                          {
                              var buseExtensionType = typeof(BusExtension);
                              var conventionMapMethodInfo = buseExtensionType.GetMethod("Consumer",
                                  BindingFlags.Static | BindingFlags.NonPublic);
                              foreach (var cons in consumerConfig.Consumers)
                              {
                                  x.AddConsumer(cons.ConsumerType);
                                  if (cons.ErrorConsumerType != default)
                                  {
                                      x.AddConsumer(cons.ErrorConsumerType);
                                  }
                              }

                              foreach (var cons in consumerConfig.Consumers)
                              {
                                  cfg.ReceiveEndpoint(cons.QueueName, ep =>
                                  {
                                      ep.ConfigureConsumer(context, cons.ConsumerType);
                                      ep.UseConcurrencyLimit(1);
                                      if (cons.ErrorConsumerType != default)
                                      {
                                          cfg.ReceiveEndpoint($"{cons.QueueName}_error", c =>
                                          {
                                              c.ConfigureConsumer(context, cons.ErrorConsumerType);
                                          });
                                      }
                                  });
                              }
                          }

                          if (producerConfig?.Producers != null)
                          {
                              var testType = typeof(BusExtension);
                              var conventionMapMethodInfo = testType.GetMethod("ConventionMap",
                                  BindingFlags.Static | BindingFlags.NonPublic);

                              foreach (var prod in producerConfig.Producers)
                              {
                                  if (prod.EntityType.GetInterfaces().Contains(typeof(ICommand)))
                                  {
                                      var toInvoke = conventionMapMethodInfo?.MakeGenericMethod(prod.EntityType);
                                      var uriString = messagingConfig.Uri + "/" + prod.ExchangeName;
                                      toInvoke?.Invoke(null, new object[] { uriString });
                                  }
                              }
                          }
                      }));
                  }
                  else
                  {
                      x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                      {
                          cfg.Host(new Uri(messagingConfig.Uri), hst =>
                          {
                              hst.Username(messagingConfig.UserName);
                              hst.Password(messagingConfig.Password);
                          });

                          switch (messagingConfig.SerializerType)
                          {
                              case SerializerType.Json:
                                  cfg.UseJsonSerializer();
                                  break;

                              case SerializerType.Bson:
                                  cfg.UseBsonSerializer();
                                  break;

                              case SerializerType.Xml:
                                  cfg.UseXmlSerializer();
                                  break;
                          }

                          var retryConfig = messagingConfig.RetryConfig;
                          switch (messagingConfig.RetryConfig.RetryType)
                          {
                              case RetryType.Interval:
                                  cfg.UseMessageRetry(r => r.Interval(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                                  break;

                              case RetryType.Exponential:
                                  cfg.UseMessageRetry(r => r.Exponential(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.MinInterval.Value),
                                      TimeSpan.FromSeconds(retryConfig.MaxInterval.Value),
                                      TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                                  break;

                              case RetryType.Immediate:
                                  cfg.UseMessageRetry(r => r.Immediate(retryConfig.RetryCount.Value));
                                  break;

                              case RetryType.Incremental:
                                  cfg.UseMessageRetry(r => r.Incremental(retryConfig.RetryCount.Value,
                                      TimeSpan.FromSeconds(retryConfig.IntervalSecond.Value),
                                      TimeSpan.FromSeconds(retryConfig.IntervalIncrement.Value)));
                                  break;

                              case RetryType.Intervals:
                                  var intervals = retryConfig.Intervals.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                                  cfg.UseMessageRetry(r => r.Intervals(intervals));
                                  break;

                              default:
                                  cfg.UseMessageRetry(r => r.None());
                                  break;
                          }

                          if (consumerConfig?.Consumers != null)
                          {
                              foreach (var cons in consumerConfig.Consumers)
                              {
                                  x.AddConsumer(cons.ConsumerType);
                                  if (cons.ErrorConsumerType != default)
                                  {
                                      x.AddConsumer(cons.ErrorConsumerType);
                                  }
                              }

                              foreach (var cons in consumerConfig.Consumers)
                              {
                                  cfg.ReceiveEndpoint(cons.QueueName, ep =>
                                  {
                                      ep.Bind(cons.ExchangeName);
                                      ep.ConfigureConsumer(context, cons.ConsumerType);
                                      ep.UseConcurrencyLimit(1);
                                      if (cons.ErrorConsumerType != default)
                                      {
                                          cfg.ReceiveEndpoint($"{cons.QueueName}_error", c =>
                                          {
                                              c.Bind($"{cons.ExchangeName}_error");
                                              c.ConfigureConsumer(context, cons.ErrorConsumerType);
                                          });
                                      }
                                  });
                              }
                          }

                          if (producerConfig?.Producers != null)
                          {
                              var testType = typeof(BusExtension);
                              var conventionMapMethodInfo = testType.GetMethod("ConventionMap", BindingFlags.Static | BindingFlags.NonPublic);

                              foreach (var prod in producerConfig.Producers)
                              {
                                  if (prod.EntityType.GetInterfaces().Contains(typeof(ICommand)))
                                  {
                                      var toInvoke = conventionMapMethodInfo?.MakeGenericMethod(prod.EntityType);
                                      var uriString = messagingConfig.Uri + "/" + prod.ExchangeName;
                                      toInvoke?.Invoke(null, new object[] { uriString });
                                  }
                              }
                          }
                      }));
                  }
              });
        }

        public static void AddMassTransit(this ContainerBuilder services, ServiceBusConfigModel messagingConfig, ServiceBusConsumerConfigModel consumerConfig, ServiceBusProducerConfigModel producerConfig)
        {
            _ = services.AddMassTransit(x =>
              {
                  x.AddBus(context => Bus.Factory.CreateUsingActiveMq(cfg =>
                  {
                      cfg.Host(new Uri(messagingConfig.Uri), hst =>
                      {
                          hst.Username(messagingConfig.UserName);
                          hst.Password(messagingConfig.Password);
                          hst.UseSsl();
                      });

                      switch (messagingConfig.SerializerType)
                      {
                          case SerializerType.Json:
                              cfg.UseJsonSerializer();
                              break;

                          case SerializerType.Bson:
                              cfg.UseBsonSerializer();
                              break;

                          case SerializerType.Xml:
                              cfg.UseXmlSerializer();
                              break;
                      }

                      var retryConfig = messagingConfig.RetryConfig;
                      switch (messagingConfig.RetryConfig.RetryType)
                      {
                          case RetryType.Interval:
                              cfg.UseMessageRetry(r => r.Interval(retryConfig.RetryCount.Value,
                                  TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                              break;

                          case RetryType.Exponential:
                              cfg.UseMessageRetry(r => r.Exponential(retryConfig.RetryCount.Value,
                                  TimeSpan.FromSeconds(retryConfig.MinInterval.Value),
                                  TimeSpan.FromSeconds(retryConfig.MaxInterval.Value),
                                  TimeSpan.FromSeconds(retryConfig.IntervalDelta.Value)));
                              break;

                          case RetryType.Immediate:
                              cfg.UseMessageRetry(r => r.Immediate(retryConfig.RetryCount.Value));
                              break;

                          case RetryType.Incremental:
                              cfg.UseMessageRetry(r => r.Incremental(retryConfig.RetryCount.Value,
                                  TimeSpan.FromSeconds(retryConfig.IntervalSecond.Value),
                                  TimeSpan.FromSeconds(retryConfig.IntervalIncrement.Value)));
                              break;

                          case RetryType.Intervals:
                              var intervals = retryConfig.Intervals.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                              cfg.UseMessageRetry(r => r.Intervals(intervals));
                              break;

                          default:
                              cfg.UseMessageRetry(r => r.None());
                              break;
                      }

                      if (consumerConfig?.Consumers != null)
                      {
                          var buseExtensionType = typeof(BusExtension);
                          var conventionMapMethodInfo = buseExtensionType.GetMethod("Consumer", BindingFlags.Static | BindingFlags.NonPublic);
                          foreach (var cons in consumerConfig.Consumers)
                          {
                              x.AddConsumer(cons.ConsumerType);
                              if (cons.ErrorConsumerType != default)
                              {
                                  x.AddConsumer(cons.ErrorConsumerType);
                              }
                          }

                          foreach (var cons in consumerConfig.Consumers)
                          {
                              cfg.ReceiveEndpoint(cons.QueueName, ep =>
                              {
                                  var consumerMethodToInvoke = conventionMapMethodInfo?.MakeGenericMethod(cons.ConsumerType);
                                  consumerMethodToInvoke?.Invoke(null, new object[] { ep, context });
                                  ep.UseConcurrencyLimit(1);
                                  if (cons.ErrorConsumerType != default)
                                  {
                                      cfg.ReceiveEndpoint($"{cons.QueueName}_error", c =>
                                      {
                                          c.ConfigureConsumer(context, cons.ErrorConsumerType);
                                      });
                                  }
                              });
                          }
                      }

                      if (producerConfig?.Producers != null)
                      {
                          var testType = typeof(BusExtension);
                          var conventionMapMethodInfo = testType.GetMethod("ConventionMap", BindingFlags.Static | BindingFlags.NonPublic);

                          foreach (var prod in producerConfig.Producers)
                          {
                              if (prod.EntityType.GetInterfaces().Contains(typeof(ICommand)))
                              {
                                  var toInvoke = conventionMapMethodInfo?.MakeGenericMethod(prod.EntityType);
                                  var uriString = messagingConfig.Uri + "/" + prod.ExchangeName;
                                  toInvoke?.Invoke(null, new object[] { uriString });
                              }
                          }
                      }
                  }));
              });
        }

        private static void ConventionMap<T>(string uri) where T : class
        {
            EndpointConvention.Map<T>(new Uri(uri));
        }

        private static void Consumer<T>(IActiveMqReceiveEndpointConfigurator conf, IServiceProvider context) where T : class, IConsumer, new()
        {
            conf.Consumer<T>(context);
        }
    }
}
