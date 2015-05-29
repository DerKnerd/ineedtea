module SystemExtensions

open System.IO
open System.Threading.Tasks

module Async =
    let AwaitTaskVoid : Task -> Async<unit> = Async.AwaitIAsyncResult >> Async.Ignore

type StreamWriter with
    member this.AsyncWrite(text : string) = this.WriteAsync(text) |> Async.AwaitTaskVoid