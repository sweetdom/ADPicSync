using System;
using System.IO;
using System.Drawing;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace UserTIle
{
    class Usertile
    {
        [DllImport("shell32.dll", EntryPoint = "#262", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SetUserTile(string username, int whatever, string picpath);
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string machine = Environment.MachineName;
                string user = Environment.UserName;
                string domain = Environment.UserDomainName;
                string temp = System.IO.Path.GetTempPath();

                if (domain.ToUpperInvariant() != machine.ToUpperInvariant())
                {
                    DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain);
                    DirectorySearcher Dsearch = new DirectorySearcher(entry);
                    Dsearch.Filter = "(SAMAccountName=" + user + ")";
                    Dsearch.PropertiesToLoad.Add("thumbnailPhoto");
                    SearchResult result = Dsearch.FindOne();
                    if (result.Properties["thumbnailPhoto"].Count > 0)
                    {
                        byte[] thumb = (byte[])result.Properties["thumbnailPhoto"][0];
                        MemoryStream ms = new MemoryStream(thumb);
                        string foto = temp + domain + "+" + user + ".jpg";
                        Image.FromStream(ms).Save(foto,
                                                  System.Drawing.Imaging.ImageFormat.Jpeg);
                        SetUserTile(domain + "\\" + user, 0, foto);
                    }

                }
                else //arguments
                {
                    if (args.Length == 0)
                        if (System.IO.File.Exists(args.ToString()))
                        {
                            SetUserTile(domain + "\\" + user, 0, args.ToString());
                        }
                } //end arguments
            }
            catch (Exception) { }
        }
    }
}
