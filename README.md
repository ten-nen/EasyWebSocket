## 这是一个websocket的简单示例
c#服务端：[Fleck](https://github.com/statianzo/Fleck)
<br>
web客户端：[web-socket-js](https://github.com/gimite/web-socket-js/)
### server
##### [1、继承EasyConnectionHandle](src/EasyWebSocket.Web/SampleHandle.cs)
``` csharp

public class SampleHandle : EasyConnectionHandle
    {
        public override void OnClose()
        {
        }

        public override void OnError(Exception e)
        {
        }

        public override void OnOpen()
        {
        }
    }
    
```

##### [2、启动服务](src/EasyWebSocket.Web/Global.asax.cs)
``` c#

EasyWebSocketServer.CreateServer()
            .MapHandle<SampleHandle>("/simple")
            .WithFlashPoliy()
            .Start();
            
```
### [client](src/EasyWebSocket.Web/sample/sample.html)
``` javascript

<script src="easy.websocket.js"></script>
<script type="text/javascript">
    var websocket = new EasyWebSocket();
    websocket.start("ws://127.0.0.1:12345/simple");

    websocket.onOpen = function () {
    };
    websocket.onClose = function () {
    };
    websocket.onError = function () {
    };
</script>

```
