using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace InfoProxy.Data
{
    public class Repository : InfoProxy.Data.IRepository
    {

        public void SaveUser(UserData.UserData usrdata, string password)
        {
            var datapath = "data.enc";
            var json = new JavaScriptSerializer().Serialize(usrdata);

            var crypted = Crypto.Crypto.EncryptStringAES(json, password);
            System.IO.File.WriteAllText(datapath, crypted);
        }

        public UserData.UserData GetUser(string password)
        {
            try
            {
                var datapath = "data.enc";
                var crypted = System.IO.File.ReadAllText(datapath);
                var descrypted = Crypto.Crypto.DecryptStringAES(crypted, password);

                var user = new JavaScriptSerializer().Deserialize<UserData.UserData>(descrypted);

                return user;
            }
            catch(Exception exc)
            {
                SingleInstance.Log.Debug(exc.Message);
                return null;
            }
        }
    }
}
