using System;
namespace InfoProxy.Data
{
    interface IRepository
    {
        InfoProxy.UserData.UserData GetUser(string password);
        void SaveUser(InfoProxy.UserData.UserData usrdata, string password);
    }
}
