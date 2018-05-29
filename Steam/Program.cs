using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using SteamKit2;

namespace Steam
{
    class Program
    {
        static string username, password;

        static SteamClient SteamClient;
        static CallbackManager manager;
        static SteamUser SteamUser;
        static SteamFriends SteamFriends;

        static bool isRunning;

        private static string authCode;
        private static string twoFactorAuth;

        static void Main(string[] args)
        {
            Console.Title = "Steam bot";
            Console.WriteLine("Ctrl + C quits the program");


            Console.Write("Username: ");
            username = Console.ReadLine();
            Console.Write("Password: ");
            password = Console.ReadLine();

            //username = "rishav394";
            //password = "p9845097056";

            Steam_login();
        }

        private static void Steam_login()
        {
            SteamClient = new SteamClient();

            manager = new CallbackManager(SteamClient);

            SteamUser = SteamClient.GetHandler<SteamUser>();

            SteamFriends = SteamClient.GetHandler<SteamFriends>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.AccountInfoCallback>(onAccountInfo);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

            manager.Subscribe<SteamFriends.FriendMsgCallback>(OnChatMessage);
            manager.Subscribe<SteamFriends.FriendsListCallback>(onFriendList);

            
            Console.WriteLine("Connecting to steam in 3s");
            SteamClient.Connect();

            isRunning = true;
            while (isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }

        }

        private static void onFriendList(SteamFriends.FriendsListCallback obj)
        {
            System.Threading.Thread.Sleep(2500);

            foreach(var friend in obj.FriendList)
            {
                if (friend.Relationship == EFriendRelationship.RequestRecipient)
                {

                    Console.Write("\t\tFriend added ");
                    Console.ForegroundColor = ConsoleColor.Blue; ;
                    Console.WriteLine(SteamFriends.GetFriendPersonaName(friend.SteamID));
                    Console.ResetColor();


                    SteamFriends.AddFriend(friend.SteamID);
                    System.Threading.Thread.Sleep(1000);
                    SteamFriends.SendChatMessage(friend.SteamID, EChatEntryType.ChatMsg, "Hi. I am a HourBoosting , Reporting and commending Bot. Please unfriend me when u are done. You can use !help for help.");
                }
            }
        }

        public static void Recogmess(SteamFriends.FriendMsgCallback obj)
        {

            Console.Write("\t\tCommand recognised ");
            Console.ForegroundColor = ConsoleColor.Blue; ;
            Console.WriteLine(obj.Message);
            Console.ResetColor();

        }

        private static void OnChatMessage(SteamFriends.FriendMsgCallback obj)
        {
            if (obj.EntryType == EChatEntryType.ChatMsg)
            {
                Console.Write("New message from ");
                Console.ForegroundColor = ConsoleColor.Green;;
                Console.WriteLine(SteamFriends.GetFriendPersonaName(obj.Sender));
                Console.ResetColor();
                
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HourBoostr\\HourBoostr.exe");
                var titan_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Titan\\titan.exe");


                switch (obj.Message.Split(' ')[0])
                {
                    case "!report":
                        try
                        {
                            var test = obj.Message.Split(' ')[1];
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\t\tInvalid Syntax. Sending help");
                            Console.ResetColor();
                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Please use !report [steamID]");
                            break;

                        }

                        var rparam = "report -t" + obj.Message.Split(' ')[1] + " -i 1 --secure";
                        Recogmess(obj);
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Reporting...");
                        try
                        {
                            Process.Start(titan_path,rparam);
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Please put the Titan folder under ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                            Console.ResetColor();

                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Error reporting. Titan folder not found.");
                        }


                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(8));
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "The Target was PROBABLY reported provoded the bots were free.");

                        break;

                    case "!commend":

                        try
                        {
                            var test = obj.Message.Split(' ')[1];
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\t\tInvalid Syntax. Sending help");
                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Please use !commend [steamID]");
                            Console.ResetColor();
                            break;

                        }

                        var cparam = "commend -t" + obj.Message.Split(' ')[1] + " -i 1 --secure";
                        Recogmess(obj);
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Commending...");
                        try
                        {
                            Process.Start(titan_path, cparam);
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Please put the Titan folder under ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                            Console.ResetColor();

                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Error commending. Titan folder not found.");
                        }
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(8));
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "The Target was PROBABLY commended if the bots were free.");


                        break;

                    case "!start":

                        Recogmess(obj);
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Starting HourBoostr");
                        try
                        {
                            Process.Start(path);
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Please put the HourBoostr folder under ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                            Console.ResetColor();

                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Error Starting HourBoostr. HourBoostr folder not found.");
                        }
                        break;
                    case "!status":

                        Recogmess(obj);

                        Process[] pname = Process.GetProcessesByName("HourBoostr");
                        if (pname.Length == 0)
                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "HourBoostr is not running.");
                        else
                            SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "HourBoostr is running.");
                        break;
                    case "!help":

                        Recogmess(obj);

                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Use !start to start HourBoostr");
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Use !stop to stop HourBoostr");
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Use !status to check running status");
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Use !report http://steamcommunity.com/id/zzzzzzzzzz to report a mofo");
                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Use !commend http://steamcommunity.com/id/zzzzzzzzzzzz to commend");
                        break;
                    case "!stop":

                        Recogmess(obj);

                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Stopping HourBoostr if running.");
                        Process.Start("cmd.exe", "/c taskkill /IM HourBoostr.exe");
                        break;

                    default:

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\t\tCommand not recognised ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(obj.Message);
                        Console.ResetColor();

                        SteamFriends.SendChatMessage(obj.Sender, EChatEntryType.ChatMsg, "Sorry command not recognised. Press !help for help.");
                        break;
                }

            }
        }

        private static void onAccountInfo(SteamUser.AccountInfoCallback obj)
        {
            SteamFriends.SetPersonaState(EPersonaState.Online);
        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Console.WriteLine("Updating sentryfile...");

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"

            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = SHA1.Create())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

            // inform the steam servers that we're accepting this sentry file
            SteamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            Console.WriteLine("Done!");
        }

        private static void OnLoggedOff(SteamUser.LoggedOffCallback obj)
        {
            Console.WriteLine("Logged out of steam {0}", obj.Result);
        }

        private static void OnLoggedOn(SteamUser.LoggedOnCallback obj)
        {

            bool isSteamGuard = obj.Result == EResult.AccountLogonDenied;
            bool is2FA = obj.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (isSteamGuard || is2FA)
            {
                Console.WriteLine("This account is SteamGuard protected!");

                if (is2FA)
                {
                    Console.Write("Please enter your 2 factor auth code from your authenticator app: ");
                    twoFactorAuth = Console.ReadLine();
                }
                else
                {
                    Console.Write("Please enter the auth code sent to the email at {0}: ", obj.EmailDomain);
                    authCode = Console.ReadLine();
                }

                return;
            }

            if (obj.Result != EResult.OK)
            { 
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", obj.Result, obj.ExtendedResult);

                isRunning = false;
                return;
            }


            Console.WriteLine("Successfully logged into steam with id {0}", username);
        
            /// For the test
            /// 
            //SteamUser.LogOff();
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback obj)
        {
            
            Console.WriteLine("Disconnected from steam. Reconnecting... ");

            //isRunning = false; //we dont need it 

            //We gotta wait here to steam to completely disconnect
            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));


            //RECONNECT TO STEAM now with the correct 2fa
            SteamClient.Connect();
        }

        private static void OnConnected(SteamClient.ConnectedCallback obj)
        {
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", username);

            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))
            {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }
            //Console.WriteLine(sentryHash);

            SteamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = username,
                Password = password,

                // in this sample, we pass in an additional authcode
                // this value will be null (which is the default) for our first logon attempt
                AuthCode = authCode,

                // if the account is using 2-factor auth, we'll provide the two factor code instead
                // this will also be null on our first logon attempt
                TwoFactorCode = twoFactorAuth,

                // our subsequent logons use the hash of the sentry file as proof of ownership of the file
                // this will also be null for our first (no authcode) and second (authcode only) logon attempts
                SentryFileHash = sentryHash,
            });

        }

    }
}
