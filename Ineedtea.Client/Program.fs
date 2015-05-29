namespace Ineedtea.Client

open SystemExtensions
open System
open System.Resources
open System.Windows.Forms
open System.Drawing
open System.Net.Sockets
open System.IO
open System.Threading.Tasks

module UI =
    open System.Reflection

    let extendedResourceManager = new ResourceManager("Ineedtea.Client", Assembly.GetExecutingAssembly())
    let Icon = extendedResourceManager.GetObject "$this.Icon"

module Client =
    let init() =
        let servernamelabel = new Label()
        servernamelabel.Location <- Point(5, 5)
        servernamelabel.Text <- "Servername"
        let servername = new TextBox()
        servername.Width <- 195
        servername.Location <- Point(servernamelabel.Width + 10, 5)
        let sendMessage server (sender : Object) =
            async {
                use tcpClient = new TcpClient(server, 8090)
                use streamwriter = new StreamWriter(tcpClient.GetStream())
                streamwriter.AutoFlush <- true
                do! streamwriter.AsyncWrite (sender :?> Button).Text
            }

        let makeButton text location =
            use font = new Font("Stencil", 18 |> float32)
            let button = new Button()
            button.Text <- text
            button.Click.AddHandler(fun sender _ -> Async.Start(sendMessage servername.Text sender))
            button.Location <- location
            button.Font <- font
            button.Height <- 60
            button.Width <- 300
            button

        let ineedtea = makeButton "I NEED TEA" (Point(5, 30))
        let ineedacuddle = makeButton "I NEED A CUDDLE" (Point(5, 100))
        let ineedfood = makeButton "I NEED FOOD" (Point(5, 170))
        let ineedakiss = makeButton "I NEED A KISS" (Point(305, 30))
        let emergency = makeButton "EMERGENCY :(" (Point(305, 100))
        let form = new Form()
        let resources = new System.ComponentModel.ComponentResourceManager(form.GetType())
        form.Height <- 275
        form.Width <- 630
        form.Icon <- UI.Icon :?> System.Drawing.Icon
        form.Text <- "I need tea"
        form.Controls.Add servernamelabel
        form.Controls.Add servername
        form.Controls.Add ineedtea
        form.Controls.Add ineedacuddle
        form.Controls.Add ineedfood
        form.Controls.Add ineedakiss
        form.Controls.Add emergency
        form

    [<EntryPoint>]
    [<STAThread>]
    let main argv =
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault false
        Application.Run(init())
        0