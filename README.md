# EventBus
C# 版 EventBus
##### 发送消息：
```C#
EventBus.Default.Post(object message); //参数类型可根据具体发送类型定义
```
##### 接收消息：
```C#
//1、注册消息事件
 EventBus.Default.Register(this);

//2、定义接收消息方法 
 [EventMethod]
public void HandleEvent(object message) //message 类型根据实际 情况自定义
{
     //收到消息处理
}

//3、不需要时 注销事件
EventBus.Default.UnRegister(this); 

```
