﻿using Newtonsoft.Json;
using NHibernate;
using NHibernate.Proxy;
using System;

namespace Breeze.Nhibernate.WebApi
{
    /// <summary>
    /// JsonConverter for handling NHibernate proxies.  
    /// Only serializes the object if it is initialized, i.e. the proxied object has been loaded.
    /// </summary>
    /// <see cref="http://james.newtonking.com/projects/json/help/html/T_Newtonsoft_Json_JsonConverter.htm"/>
    public class NHibernateProxyJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                var proxy = value as INHibernateProxy;
                if (proxy != null)
                {
                    value = proxy.HibernateLazyInitializer.GetImplementation();
                }

                if (NHibernateUtil.IsInitialized(value))
                {
                    serializer.Serialize(writer, value);
                }
                else
                {
                    serializer.Serialize(writer, null);
                }
            }
            catch (LazyInitializationException)
            {
                // ignore it
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            bool b = typeof(INHibernateProxy).IsAssignableFrom(objectType);
            return b;
        }
    }
}
