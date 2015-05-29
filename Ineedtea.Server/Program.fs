open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Threading

let App_ID = "I need tea server"

type StreamReader with
    member this.AsyncReadToEnd() = Async.AwaitTask(this.ReadToEndAsync())

type TcpListener with
    member this.AsyncAcceptTcpClient() = Async.AwaitTask(this.AcceptTcpClientAsync())

let gotMessage message = Toaster.Toast("Message incoming", message)

type Server() =

    static member Start(hostname : string, ?port) =
        let ipAddress = Dns.GetHostEntry(hostname).AddressList.[0]
        Server.Start(ipAddress, ?port = port)

    static member Start(?ipAddress, ?port) =
        let ipAddress = defaultArg ipAddress IPAddress.Any
        let port = defaultArg port 80
        let cts = new CancellationTokenSource()
        let listener = new TcpListener(ipAddress, port)
        listener.Start()
        printfn "Started listening on server %A port %d" ipAddress port
        let rec loop() =
            printfn "Waiting for request ..."
            let client = listener.AcceptTcpClient()
            printfn "Received request"
            use streamReader = new StreamReader(client.GetStream())
            let content = streamReader.ReadToEnd()
            gotMessage content
            printfn "%s" content
            client.Close()
            loop()
        loop()

Toaster.PrepareToaster App_ID
Server.Start(port = 8090)
printfn "bye!"