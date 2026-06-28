using Client.Main;

#if DEBUG
Constants.DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Data");
#endif

using var game = new MuGame();
game.Run();
