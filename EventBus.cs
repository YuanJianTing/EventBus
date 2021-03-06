﻿using MT4SignalRServer.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MT4SignalRServer.Events
{
    public class EventBus
    {
       private static object obj = new object();

        private static EventBus eventBus;
        public static EventBus Default
        {
            get
            {
                if (eventBus == null)
                {
                    lock (obj)
                        eventBus = new EventBus();
                }
                return eventBus;
            }
        }



        /// <summary>
        /// 定义线程安全集合
        /// </summary>
        private Dictionary<string, object> _eventAndHandlerMapping;
        private TaskFactory taskFactory;
        private SynchronizationContext m_context;

        public EventBus()
        {
            _eventAndHandlerMapping = new Dictionary<string, object>();
            taskFactory = new TaskFactory();
        }
        public EventBus(EventBusBuilder builder)
        {
            _eventAndHandlerMapping = new Dictionary<string, object>();
            taskFactory = new TaskFactory();
            m_context = builder.context;

        }


        public void Register(object cla)
        {
            string key = cla.GetType().Name;
            if (!_eventAndHandlerMapping.ContainsKey(key))
            {
                _eventAndHandlerMapping.Add(key, cla);
            }
        }


        public void UnRegister(object cla)
        {
            string key = cla.GetType().Name;
            if (_eventAndHandlerMapping.ContainsKey(key))
            {
                _eventAndHandlerMapping.Remove(key);
            }
        }

        /// <summary>
        /// 根据事件源触发绑定的事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        public void Post(object eventData)
        {
            try
            {
                if (_eventAndHandlerMapping.Count > 0)
                {
                    foreach (object item in _eventAndHandlerMapping.Values)
                    {
                        //
                        List<MethodInfo> result = IsEventBusMethod(item);
                        if (result.Count > 0)
                        {
                            foreach (MethodInfo method in result)
                            {
                                if (IsEventBusMethodParam(method, eventData.GetType()))
                                {
                                    /**
                                     * 1、获取方法 EventMethod 特性 
                                     * 2、根据ThreadMode 属性判断执行线程 如果 m_context 为空则忽略
                                     * 
                                     * **/
                                    object[] objs = method.GetCustomAttributes(typeof(EventMethodAttribute),true);
                                    foreach (var attr in objs)
                                    {
                                        if(attr is EventMethodAttribute)
                                        {
                                            EventMethodAttribute attribute = attr as EventMethodAttribute;
                                            if (attribute.ThreadMode == ThreadMode.MAIN)
                                            {
                                                if (m_context != null)
                                                {
                                                    m_context.Post((o) =>
                                                    {
                                                        method.Invoke(item, new object[] { eventData });
                                                    }, null);
                                                }else
                                                    method.Invoke(item, new object[] { eventData });
                                            }
                                            else if (attribute.ThreadMode == ThreadMode.ASYNC)
                                            {
                                                taskFactory.StartNew(() => method.Invoke(item, new object[] { eventData }));
                                            }
                                            else
                                            {
                                                method.Invoke(item, new object[] { eventData });
                                            }
                                            break;
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MyLog4Net.LogInfo("EventBus.Trigger()",ex);
            }
        }

        /// <summary>
        /// 判断方法参数类型是否 为 param
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool IsEventBusMethodParam(MethodInfo methodInfo, Type param)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
                return false;
            return parameterInfos[0].ParameterType == param;
        }



        /// <summary>
        /// 获取类中 包含EventMethod 特性发 方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<MethodInfo> IsEventBusMethod(object obj)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            MethodInfo[] methodInfos = obj.GetType().GetMethods();
            foreach (MethodInfo item in methodInfos)
            {
                object[] attrs = item.GetCustomAttributes(typeof(EventMethodAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    result.Add(item);
            }
            return result;
        }

        public static EventBusBuilder Builder => new EventBusBuilder();


    }
}
