using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Net.Sockets;
using Raylib_cs;

while (true)
{
    var IN = new Thread(Incomer);
    var OUT = new Thread(Outgoer);

    IN.Start();
    //OUT.Start();
}

void Incomer()
{
    while (true)
    {
        IPAddress addr = new IPAddress(new byte[] { 10, 151, 172, 161 });
        TcpListener listener = new TcpListener(addr, 1302);
        listener.Start();

        TcpClient client = listener.AcceptTcpClient();
        StreamReader sr = new StreamReader(client.GetStream());

        try
        {
            string message = sr.ReadLine();
            Console.WriteLine(message);
        }
        catch (Exception e)
        {
            Console.WriteLine("failed");
        }

        client.Close();
    }
}

void Outgoer()
{
    while (true)
    {
        string message = Console.ReadLine();
        TcpClient client = new TcpClient("10.151.168.166", 585);
        StreamReader sr = new StreamReader(client.GetStream());
        StreamWriter sw = new StreamWriter(client.GetStream());
        sw.WriteLine(message);
        sw.Flush();

    }
}



//Raylib.InitWindow(800, 800, "Game");
//GameLoop();

void GameLoop()
{
    Game game = new Game("Samme");
    NetworkingDeluxe network = new NetworkingDeluxe();

    while (!Raylib.WindowShouldClose())
    {
        var v = network.Incoming();
        var ourPos = game.Run(v);
        network.Outgoing(ourPos);
    }
}

class NetworkingDeluxe
{
    TcpClient client;
    StreamReader sr;
    StreamWriter sw;

    public NetworkingDeluxe()
    {
        client = new TcpClient("10.151.168.166", 1302);
        sr = new StreamReader(client.GetStream());
        sw = new StreamWriter(client.GetStream());
    }

    public Vector2 Incoming()
    {
        Vector2 v = new Vector2();

        try
        {
            string data = sr.ReadLine();
            string[] positions = data.Split(" ");
            string x = positions[0];
            string y = positions[1];

            v = new Vector2(Convert.ToInt16(x), Convert.ToInt16(y));

        }
        catch (System.Exception)
        {
            Console.WriteLine("skipped");
        }

        return v;
    }

    async public void Outgoing(Vector2 cords)
    {
        sw.WriteLine($"{cords.X} {cords.Y}");
        sw.Flush();
    }
}

class Game
{
    string name;
    const int size = 10;
    Vector2 myPos = new Vector2();

    public Game(string name_)
    {
        name = name_;
        myPos = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
    }

    public Vector2 Run(Vector2 theirPos)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);
        Raylib.DrawRectangle((int)myPos.X, (int)myPos.Y, size, size, Color.BROWN);

        if (theirPos != null)
        {
            Raylib.DrawRectangle((int)theirPos.X, (int)theirPos.Y, size, size, Color.BROWN);
        }

        Raylib.EndDrawing();

        return myPos;
    }
}