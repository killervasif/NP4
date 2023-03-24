using System.Net.Sockets;
using System.Net;
using System.Drawing;

UdpClient server = new UdpClient(45678);

while (true)
{
    var result = await server.ReceiveAsync();

    new Task(async () =>
    {

        var remoteEP = result.RemoteEndPoint;
        while (true)
        {
            await Task.Delay(20);
            var img = ScreenShot();

            var bytesImg = ImageToByte(img);


            var myArray = bytesImg.Chunk(ushort.MaxValue - 29);

            foreach (var array in myArray)
                await server.SendAsync(array, array.Length, remoteEP);
        }

    }).Start();
}

byte[] ImageToByte(Image img)
{
    using(var stream = new MemoryStream())
    {
        img.Save(stream,System.Drawing.Imaging.ImageFormat.Jpeg);
        return stream.ToArray();
    }
}

Image ScreenShot()
{
    Bitmap memo;
    var width = 1920;
    var height = 1080;
    memo= new Bitmap(width, height); 
    Graphics g = Graphics.FromImage(memo);
    g.CopyFromScreen(0, 0, 0, 0, memo.Size);
    return memo;
}