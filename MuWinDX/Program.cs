using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Client.Main;

// Must register code pages BEFORE any access to Constants (which uses EUC-KR encoding 949)
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if DEBUG
Constants.DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Data");
#endif

Application.SetHighDpiMode(HighDpiMode.SystemAware);

using var game = new MuGame();
game.Run();
